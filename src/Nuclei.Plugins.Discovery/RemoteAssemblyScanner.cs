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
using System.Threading.Tasks;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Nuclei.Plugins;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Nuclei;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Performs assembly scanning in search for plugin information.
    /// </summary>
    internal sealed class RemoteAssemblyScanner : MarshalByRefObject, IAssemblyScanner
    {
        private static Type ExtractRequiredType(IEnumerable<Attribute> memberAttributes, Type memberType)
        {
            // This is really rather ugly but we can't get the RequiredTypeIdentity straight from the import
            // (eventhough it has a property named as such) because we want an actual type and the 
            // MEF ImportDefinition.RequiredTypeIdentity is a string that has been mangled, e.g. for 
            // delegates the type name is <RETURNTYPE>(<PARAMETERTYPES>), which means we don't know
            // exactly what the type is. Also MEF strips the Lazy<T> type and replaces it with T. Hence
            // we go straight to the actual import attribute and get it from there.
            var attribute = memberAttributes.OfType<ImportAttribute>().FirstOrDefault();
            Debug.Assert(attribute != null, "There should be an import attribute.");

            var requiredType = attribute.ContractType ?? memberType;
            return requiredType;
        }

        private static SerializableExportDefinition CreateMethodExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, TypeIdentity> identityGenerator)
        {
            Debug.Assert(memberInfo.GetAccessors().Count() == 1, "Only expecting one accessor for a method export.");
            Debug.Assert(memberInfo.GetAccessors().First() is MethodInfo, "Expecting the method export to be an MethodInfo object.");
            return MethodBasedExportDefinition.CreateDefinition(
                export.ContractName,
                memberInfo.GetAccessors().First() as MethodInfo,
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
            return PropertyBasedExportDefinition.CreateDefinition(
                export.ContractName,
                property,
                identityGenerator);
        }

        private static SerializableExportDefinition CreateTypeExport(
            ExportDefinition export,
            LazyMemberInfo memberInfo,
            Func<Type, TypeIdentity> identityGenerator)
        {
            Debug.Assert(memberInfo.GetAccessors().Count() == 1, "Only expecting one accessor for a type export.");
            Debug.Assert(memberInfo.GetAccessors().First() is Type, "Expecting the export to be a Type.");
            return TypeBasedExportDefinition.CreateDefinition(
                export.ContractName,
                memberInfo.GetAccessors().First() as Type,
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
                import.Cardinality,
                import.IsRecomposable,
                import.RequiredCreationPolicy,
                property,
                identityGenerator);
        }

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
                import.Cardinality,
                import.RequiredCreationPolicy,
                parameterInfo.Value,
                identityGenerator);
        }

        /// <summary>
        /// The object that stores all the information about the parts and the part groups.
        /// </summary>
        private readonly IPluginRepository m_Repository;

        /// <summary>
        /// The object that provides methods for part import matching.
        /// </summary>
        private readonly IConnectParts m_ImportEngine;

        /// <summary>
        /// The object that will pass through the log messages.
        /// </summary>
        private readonly ILogMessagesFromRemoteAppDomains m_Logger;

        /// <summary>
        /// The function that creates schedule building objects.
        /// </summary>
        private readonly Func<IBuildFixedSchedules> m_ScheduleBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAssemblyScanner"/> class.
        /// </summary>
        /// <param name="repository">The object that stores all the information about the parts and the part groups.</param>
        /// <param name="importEngine">The object that provides methods for part import matching.</param>
        /// <param name="logger">The object that passes through the log messages.</param>
        /// <param name="scheduleBuilder">The function that returns a schedule building object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="importEngine"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="logger"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scheduleBuilder"/> is <see langword="null" />.
        /// </exception>
        public RemoteAssemblyScanner(
            IPluginRepository repository,
            IConnectParts importEngine,
            ILogMessagesFromRemoteAppDomains logger, 
            Func<IBuildFixedSchedules> scheduleBuilder)
        {
            {
                Lokad.Enforce.Argument(() => repository);
                Lokad.Enforce.Argument(() => importEngine);
                Lokad.Enforce.Argument(() => logger);
                Lokad.Enforce.Argument(() => scheduleBuilder);
            }

            m_Repository = repository;
            m_ImportEngine = importEngine;
            m_Logger = logger;
            m_ScheduleBuilder = scheduleBuilder;
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
        public void Scan(IEnumerable<string> assemblyFilesToScan)
        {
            {
                Lokad.Enforce.Argument(() => assemblyFilesToScan);
            }

            // It is expected that the loading of an assembly will take more
            // time than the scanning of that assembly. 
            // Because we're dealing with disk IO we want to optimize the load
            // process so we load the assemblies one-by-one (thus reducing disk
            // search times etc.)
            Parallel.ForEach(
                assemblyFilesToScan,
                a => 
                { 
                    var assembly = LoadAssembly(a);
                    ScanAssembly(assembly);
                });
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

            // Check if the file exists.
            if (!File.Exists(file))
            {
                return null;
            }

            // Try to load the assembly. If we can't load the assembly
            // we log the exception / problem and return a null reference
            // for the assembly.
            string fileName = Path.GetFileNameWithoutExtension(file);
            try
            {
                // Only use the file name of the assembly
                return Assembly.Load(fileName);
            }
            catch (FileNotFoundException e)
            {
                // The file does not exist. Only possible if somebody removes the file
                // between the check and the loading.
                m_Logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (FileLoadException e)
            {
                // Could not load the assembly.
                m_Logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }
            catch (BadImageFormatException e)
            {
                // incorrectly formatted assembly.
                m_Logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_AssemblyLoadFailed_WithNameAndException,
                        fileName,
                        e));
            }

            return null;
        }
        
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Will catch an log here because we don't actually know what exceptions can happen due to the ExtractGroups() call.")]
        private void ScanAssembly(Assembly assembly)
        {
            try
            {
                m_Logger.Log(
                    LevelToLog.Trace,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.RemoteAssemblyScanner_LogMessage_ScanningAssembly_WithName,
                        assembly.FullName));

                var file = new FileInfo(assembly.LocalFilePath());
                var fileInfo = new PluginFileInfo(file.FullName, file.LastWriteTimeUtc);

                var createTypeIdentity = TypeIdentityBuilder.IdentityFactory(m_Repository, new Dictionary<Type, TypeIdentity>());
                var mefParts = ExtractImportsAndExports(assembly, createTypeIdentity);
                var parts = mefParts.Select(p => ExtractActionsAndConditions(p.Item1, p.Item2, createTypeIdentity));
                foreach (var part in parts)
                {
                    m_Logger.Log(
                        LevelToLog.Trace,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.RemoteAssemblyScanner_LogMessage_AddingPartToRepository_WithPartInformation,
                            part.Identity));

                    m_Repository.AddPart(part, fileInfo);
                }

                var groupExporters = assembly.GetTypes().Where(t => typeof(IExportGroupDefinitions).IsAssignableFrom(t));
                foreach (var t in groupExporters)
                {
                    try
                    {
                        m_Logger.Log(
                            LevelToLog.Trace,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.RemoteAssemblyScanner_LogMessage_RegisteringGroupsViaExporter_WithExporterType,
                                t.AssemblyQualifiedName));

                        var builder = new GroupDefinitionBuilder(
                            m_Repository,
                            m_ImportEngine, 
                            createTypeIdentity,
                            m_ScheduleBuilder,
                            fileInfo);

                        var exporter = Activator.CreateInstance(t) as IExportGroupDefinitions;
                        exporter.RegisterGroups(builder);
                    }
                    catch (Exception e)
                    {
                        m_Logger.Log(LevelToLog.Warn, e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                m_Logger.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Scanner_TypeScanFailed_WithAssemblyAndException,
                        assembly.GetName().FullName,
                        e));
            }
        }

        private IEnumerable<Tuple<Type, PartDefinition>> ExtractImportsAndExports(Assembly assembly, Func<Type, TypeIdentity> createTypeIdentity)
        {
            var catalog = new AssemblyCatalog(assembly);
            foreach (var part in catalog.Parts)
            {
                var exports = new List<SerializableExportDefinition>();
                foreach (var export in part.ExportDefinitions)
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
                        m_Logger.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered export: {0}",
                                exportDefinition));
                    }
                    else 
                    {
                        m_Logger.Log(
                            LevelToLog.Warn,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Unable to process export: {0} on a {1}",
                                export.ContractName,
                                memberInfo.MemberType));
                    }
                }

                var imports = new List<SerializableImportDefinition>();
                foreach (var import in part.ImportDefinitions)
                {
                    Debug.Assert(import is ContractBasedImportDefinition, "All import objects should be ContractBasedImportDefinition objects.");
                    var contractImport = import as ContractBasedImportDefinition;

                    SerializableImportDefinition importDefinition = !ReflectionModelServices.IsImportingParameter(contractImport)
                        ? CreatePropertyImport(contractImport, createTypeIdentity)
                        : CreateConstructorParameterImport(contractImport, createTypeIdentity);

                    if (importDefinition != null)
                    {
                        imports.Add(importDefinition);
                        m_Logger.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered import: {0}",
                                importDefinition));
                    }
                    else
                    {
                        m_Logger.Log(
                            LevelToLog.Warn,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Unable to process import: {0}",
                                import.ContractName));
                    }
                }

                var type = ReflectionModelServices.GetPartType(part).Value;
                yield return new Tuple<Type, PartDefinition>( 
                    type,
                    new PartDefinition
                        {
                            Identity = createTypeIdentity(type),
                            Exports = exports,
                            Imports = imports,
                            Actions = Enumerable.Empty<ScheduleActionDefinition>(),
                            Conditions = Enumerable.Empty<ScheduleConditionDefinition>(),
                        });
            }
        }

        private PartDefinition ExtractActionsAndConditions(Type type, PartDefinition part, Func<Type, TypeIdentity> createTypeIdentity)
        {
            var actions = new List<ScheduleActionDefinition>();
            var conditions = new List<ScheduleConditionDefinition>();
            foreach (var method in type.GetMethods())
            {
                if (method.ReturnType == typeof(void) && !method.GetParameters().Any())
                {
                    var actionAttribute = method.GetCustomAttribute<ScheduleActionAttribute>(true);
                    if (actionAttribute != null)
                    {
                        var actionDefinition = ScheduleActionDefinition.CreateDefinition(
                            actionAttribute.Name, 
                            method, 
                            createTypeIdentity);
                        actions.Add(actionDefinition);
                            
                        m_Logger.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered action: {0}",
                                actionDefinition));
                    }

                    continue;
                }

                if (method.ReturnType == typeof(bool) && !method.GetParameters().Any())
                {
                    var conditionAttribute = method.GetCustomAttribute<ScheduleConditionAttribute>(true);
                    if (conditionAttribute != null)
                    {
                        var conditionDefinition = MethodBasedScheduleConditionDefinition.CreateDefinition(
                            conditionAttribute.Name,
                            method,
                            createTypeIdentity);
                        conditions.Add(conditionDefinition);

                        m_Logger.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered condition: {0}",
                                conditionDefinition));
                    }
                }
            }

            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType == typeof(bool))
                {
                    var conditionAttribute = property.GetCustomAttribute<ScheduleConditionAttribute>(true);
                    if (conditionAttribute != null)
                    {
                        var conditionDefinition = PropertyBasedScheduleConditionDefinition.CreateDefinition(
                            conditionAttribute.Name,
                            property,
                            createTypeIdentity);
                        conditions.Add(conditionDefinition);

                        m_Logger.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Discovered condition: {0}",
                                conditionDefinition));
                    }
                }
            }

            if (actions.Count > 0 || conditions.Count > 0)
            {
                part.Actions = actions;
                part.Conditions = conditions;
            }

            return part;
        }
    }
}
