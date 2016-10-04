//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery.Properties;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Performs assembly scanning in search for plugin information.
    /// </summary>
    internal sealed class RemoteAssemblyScanner : MarshalByRefObject, IAssemblyScanner
    {
        private static SerializableImportDefinition CreateConstructorParameterImport(
            ContractBasedImportDefinition import,
            Func<Type, TypeIdentity> identityGenerator)
        {
            var parameterInfo = ReflectionModelServices.GetImportingParameter(import);
            var requiredType = ExtractRequiredType(parameterInfo.Value.GetCustomAttributes(), parameterInfo.Value.ParameterType);
            if (requiredType == null)
            {
                return null;
            }

            return ConstructorBasedImportDefinition.CreateDefinition(
                import.ContractName,
                TypeIdentity.CreateDefinition(requiredType),
                import.RequiredTypeIdentity,
                import.Cardinality,
                import.RequiredCreationPolicy,
                parameterInfo.Value,
                identityGenerator);
        }

        private static SerializableExportDefinition CreateMethodExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, TypeIdentity> identityGenerator)
        {
            if (memberInfo.GetAccessors().Count() != 1)
            {
                throw new InvalidExportMethodException();
            }

            if (!(memberInfo.GetAccessors().First() is MethodInfo))
            {
                throw new InvalidExportMethodException();
            }

            var methodInfo = memberInfo.GetAccessors().First() as MethodInfo;

            var exportedMefType = ExtractExportedMefType(export);
            if (string.IsNullOrEmpty(exportedMefType))
            {
                exportedMefType = methodInfo.ReturnType.FullName;
            }

            return MethodBasedExportDefinition.CreateDefinition(
                export.ContractName,
                exportedMefType,
                methodInfo,
                identityGenerator);
        }

        private static SerializableExportDefinition CreatePropertyExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, TypeIdentity> identityGenerator)
        {
            // this is really ugly because we assume that the underlying methods for a property are named as:
            // get_PROPERTYNAME and set_PROPERTYNAME. In this case we assume that exports always
            // have a get method.
            var getMember = memberInfo.GetAccessors().First(m => m.Name.Contains("get_"));
            var name = getMember.Name.Substring("get_".Length);
            var property = getMember.DeclaringType.GetProperty(name);

            var exportedMefType = ExtractExportedMefType(export);
            if (string.IsNullOrEmpty(exportedMefType))
            {
                exportedMefType = property.PropertyType.FullName;
            }

            return PropertyBasedExportDefinition.CreateDefinition(
                export.ContractName,
                exportedMefType,
                property,
                identityGenerator);
        }

        private static SerializableImportDefinition CreatePropertyImport(
            ContractBasedImportDefinition import,
            Func<Type, TypeIdentity> identityGenerator)
        {
            var memberInfo = ReflectionModelServices.GetImportingMember(import);
            if (memberInfo.MemberType != MemberTypes.Property)
            {
                throw new ArgumentOutOfRangeException("import");
            }

            // this is really ugly because we assume that the underlying methods for a property are named as:
            // get_PROPERTYNAME and set_PROPERTYNAME. In this case we assume that imports always
            // have a set method.
            var getMember = memberInfo.GetAccessors().First(m => m.Name.Contains("set_"));
            var name = getMember.Name.Substring("set_".Length);
            var property = getMember.DeclaringType.GetProperty(name);

            var requiredType = ExtractRequiredType(property.GetCustomAttributes(), property.PropertyType);
            if (requiredType == null)
            {
                return null;
            }

            return PropertyBasedImportDefinition.CreateDefinition(
                import.ContractName,
                TypeIdentity.CreateDefinition(requiredType),
                import.RequiredTypeIdentity,
                import.Cardinality,
                import.IsRecomposable,
                import.RequiredCreationPolicy,
                property,
                identityGenerator);
        }

        private static SerializableExportDefinition CreateTypeExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, TypeIdentity> identityGenerator)
        {
            {
                Debug.Assert(memberInfo.GetAccessors().Count() == 1, "Only expecting one accessor for a type export.");
                Debug.Assert(memberInfo.GetAccessors().First() is Type, "Expecting the export to be a Type.");
            }

            var type = memberInfo.GetAccessors().First() as Type;

            var exportedMefType = ExtractExportedMefType(export);
            if (string.IsNullOrEmpty(exportedMefType))
            {
                exportedMefType = type.FullName;
            }

            return TypeBasedExportDefinition.CreateDefinition(
                export.ContractName,
                exportedMefType,
                type,
                identityGenerator);
        }

        private static string ExtractExportedMefType(ExportDefinition export)
        {
            if (export.Metadata.ContainsKey(MefConstants.ExportTypeIdentity))
            {
                return (string)export.Metadata[MefConstants.ExportTypeIdentity];
            }

            return null;
        }

        private static Type ExtractRequiredType(IEnumerable<Attribute> memberAttributes, Type memberType)
        {
            // This is really rather ugly but we can't get the RequiredTypeIdentity straight from the import
            // (eventhough it has a property named as such) because we want an actual type and the
            // MEF ImportDefinition.RequiredTypeIdentity is a string that has been mangled, e.g. for
            // delegates the type name is <RETURNTYPE>(<PARAMETERTYPES>), which means we don't know
            // exactly what the type is. Also MEF strips the Lazy<T> type and replaces it with T. Hence
            // we go straight to the actual import attribute and get it from there.
            var importAttribute = memberAttributes.OfType<ImportAttribute>().FirstOrDefault();
            if (importAttribute != null)
            {
                var requiredType = importAttribute.ContractType ?? memberType;
                return requiredType;
            }

            var importManyAttribute = memberAttributes.OfType<ImportManyAttribute>().FirstOrDefault();
            if (importManyAttribute != null)
            {
                var requiredType = importManyAttribute.ContractType ?? memberType;
                return requiredType;
            }

            throw new MissingImportAttributeException();
        }

        /// <summary>
        /// The object that will pass through the log messages.
        /// </summary>
        private readonly ILogMessagesFromRemoteAppDomains _logger;

        /// <summary>
        /// The object that stores all the information about the parts and the part groups.
        /// </summary>
        private readonly IPluginRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAssemblyScanner"/> class.
        /// </summary>
        /// <param name="repository">The object that stores all the information about the parts and the part groups.</param>
        /// <param name="logger">The object that passes through the log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        public RemoteAssemblyScanner(
            IPluginRepository repository,
            ILogMessagesFromRemoteAppDomains logger)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;
            _repository = repository;
        }

        private IEnumerable<PartDefinition> ExtractImportsAndExports(Assembly assembly, Func<Type, TypeIdentity> createTypeIdentity)
        {
            var catalog = new AssemblyCatalog(assembly);
            foreach (var part in catalog.Parts)
            {
                var type = ReflectionModelServices.GetPartType(part).Value;

                var exports = new List<SerializableExportDefinition>();
                foreach (var export in part.ExportDefinitions)
                {
                    try
                    {
                        var memberInfo = ReflectionModelServices.GetExportingMember(export);
                        SerializableExportDefinition exportDefinition = null;
                        switch (memberInfo.MemberType)
                        {
                            case MemberTypes.Method:
                                exportDefinition = CreateMethodExport(export, memberInfo, createTypeIdentity);
                                break;
                            case MemberTypes.Property:
                                exportDefinition = CreatePropertyExport(export, memberInfo, createTypeIdentity);
                                break;
                            case MemberTypes.NestedType:
                            case MemberTypes.TypeInfo:
                                exportDefinition = CreateTypeExport(export, memberInfo, createTypeIdentity);
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        if (exportDefinition != null)
                        {
                            exports.Add(exportDefinition);
                            _logger.Log(
                                LevelToLog.Info,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Discovered export: {0}",
                                    exportDefinition));
                        }
                        else
                        {
                            _logger.Log(
                                LevelToLog.Warn,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Unable to process export: {0} on a {1}",
                                    export.ContractName,
                                    memberInfo.MemberType));
                        }
                    }
                    catch (ArgumentException e)
                    {
                        _logger.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Plugins_LogMessage_Scanner_InvalidExport_WithContractNameAndTypeAndException,
                                export.ContractName,
                                type,
                                e));
                    }
                    catch (InvalidExportMethodException e)
                    {
                        _logger.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Plugins_LogMessage_Scanner_InvalidExport_WithContractNameAndTypeAndException,
                                export.ContractName,
                                type,
                                e));
                    }
                }

                var imports = new List<SerializableImportDefinition>();
                foreach (var import in part.ImportDefinitions)
                {
                    try
                    {
                        Debug.Assert(import is ContractBasedImportDefinition, "All import objects should be ContractBasedImportDefinition objects.");
                        var contractImport = import as ContractBasedImportDefinition;

                        SerializableImportDefinition importDefinition = !ReflectionModelServices.IsImportingParameter(contractImport)
                            ? CreatePropertyImport(contractImport, createTypeIdentity)
                            : CreateConstructorParameterImport(contractImport, createTypeIdentity);

                        if (importDefinition != null)
                        {
                            imports.Add(importDefinition);
                            _logger.Log(
                                LevelToLog.Info,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Discovered import: {0}",
                                    importDefinition));
                        }
                        else
                        {
                            _logger.Log(
                                LevelToLog.Warn,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Unable to process import: {0}",
                                    import.ContractName));
                        }
                    }
                    catch (ArgumentException e)
                    {
                        _logger.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Plugins_LogMessage_Scanner_InvalidImport_WithContractNameAndTypeAndException,
                                import.ContractName,
                                type,
                                e));
                    }
                    catch (MissingImportAttributeException e)
                    {
                        _logger.Log(
                            LevelToLog.Error,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Plugins_LogMessage_Scanner_InvalidImport_WithContractNameAndTypeAndException,
                                import.ContractName,
                                type,
                                e));
                    }
                }

                yield return new PartDefinition
                    {
                        Identity = createTypeIdentity(type),
                        Exports = exports,
                        Imports = imports,
                    };
            }
        }

        private Assembly LoadAssembly(string file)
        {
            if (file == null)
            {
                return null;
            }

            if (file.Length == 0)
            {
                return null;
            }

            if (!File.Exists(file))
            {
                return null;
            }

            string fileName = Path.GetFileNameWithoutExtension(file);
            try
            {
                return Assembly.Load(fileName);
            }
            catch (FileNotFoundException e)
            {
                // The file does not exist. Only possible if somebody removes the file
                // between the check and the loading.
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (FileLoadException e)
            {
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (BadImageFormatException e)
            {
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }

            return null;
        }

        /// <summary>
        /// Scans the assemblies for which the given file paths have been provided and
        /// returns the plugin description information.
        /// </summary>
        /// <param name="assemblyFilesToScan">
        /// The collection that contains the file paths to all the assemblies to be scanned.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assemblyFilesToScan"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Will catch an log here because we don't actually know what exceptions can happen due to the LoadAssembly() call.")]
        public void Scan(IEnumerable<string> assemblyFilesToScan)
        {
            if (assemblyFilesToScan == null)
            {
                throw new ArgumentNullException("assemblyFilesToScan");
            }

            foreach (var a in assemblyFilesToScan)
            {
                try
                {
                    var assembly = LoadAssembly(a);
                    ScanAssembly(assembly);
                }
                catch (Exception e)
                {
                    _logger.Log(
                        LevelToLog.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Plugins_LogMessage_Scanner_TypeScanFailed_WithAssemblyAndException,
                            a,
                            e));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Will catch an log here because we don't actually know what exceptions can happen due to the ExtractGroups() call.")]
        private void ScanAssembly(Assembly assembly)
        {
            try
            {
                _logger.Log(
                    LevelToLog.Trace,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.RemoteAssemblyScanner_LogMessage_ScanningAssembly_WithName,
                        assembly.FullName));

                var file = new FileInfo(assembly.LocalFilePath());
                var fileInfo = new PluginFileOrigin(file.FullName, file.LastWriteTimeUtc);

                var createTypeIdentity = TypeIdentityBuilder.IdentityFactory(_repository, new Dictionary<Type, TypeIdentity>());
                var mefParts = ExtractImportsAndExports(assembly, createTypeIdentity);
                foreach (var pair in mefParts)
                {
                    _logger.Log(
                        LevelToLog.Trace,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.RemoteAssemblyScanner_LogMessage_AddingPartToRepository_WithPartInformation,
                            pair.Identity));

                    _repository.AddPart(pair, fileInfo);
                }
            }
            catch (Exception e)
            {
                _logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_TypeScanFailed_WithAssemblyAndException,
                        assembly.GetName().FullName,
                        e));
            }
        }
    }
}
