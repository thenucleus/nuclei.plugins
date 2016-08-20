//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities;
using Moq;
using Nuclei;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteAssemblyScannerTest
    {
        private static IEnumerable<TypeDefinition> s_Types;
        private static IEnumerable<PartDefinition> s_Parts;
        private static IEnumerable<GroupDefinition> s_Groups;

        private static bool AreVerticesEqual(IScheduleVertex first, IScheduleVertex second)
        {
            if (first.GetType() != second.GetType())
            {
                return false;
            }

            var executingActionVertex = first as ExecutingActionVertex;
            if (executingActionVertex != null)
            {
                return executingActionVertex.ActionToExecute == ((ExecutingActionVertex)second).ActionToExecute;
            }

            var subScheduleVertex = first as SubScheduleVertex;
            if (subScheduleVertex != null)
            {
                return subScheduleVertex.ScheduleToExecute == ((SubScheduleVertex)second).ScheduleToExecute;
            }

            return true;
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            try
            {
                var types = new List<TypeDefinition>();
                var parts = new List<PartDefinition>();
                var groups = new List<GroupDefinition>();
                var repository = new Mock<IPluginRepository>();
                {
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<string>()))
                        .Returns<string>(n => types.Any(t => t.Identity.AssemblyQualifiedName.Equals(n)));
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<TypeIdentity>()))
                        .Returns<TypeIdentity>(n => types.Any(t => t.Identity.Equals(n)));
                    repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                        .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).First());
                    repository.Setup(r => r.Parts())
                        .Returns(parts);
                    repository.Setup(r => r.AddType(It.IsAny<TypeDefinition>()))
                        .Callback<TypeDefinition>(types.Add);
                    repository.Setup(r => r.AddPart(It.IsAny<PartDefinition>(), It.IsAny<PluginFileInfo>()))
                        .Callback<PartDefinition, PluginFileInfo>((p, f) => parts.Add(p));
                    repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                        .Callback<GroupDefinition, PluginFileInfo>((g, f) => groups.Add(g));
                }

                var importEngine = new Mock<IConnectParts>();
                {
                    importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                        .Returns(true);
                }

                var scanner = new RemoteAssemblyScanner(
                    repository.Object,
                    importEngine.Object,
                    new Mock<ILogMessagesFromRemoteAppDomains>().Object,
                    () => new FixedScheduleBuilder());

                var localPath = Assembly.GetExecutingAssembly().LocalFilePath();
                scanner.Scan(new List<string> { localPath });

                s_Types = types;
                s_Parts = parts;
                s_Groups = groups;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception in RemoteAssemblyScannerTest.Setup: {0}",
                        e));

                throw;
            }
        }

        [Test]
        public void ExportOnTypeWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnTypeWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnTypeWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(ExportOnType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnPropertyWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnPropertyWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithName).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnPropertyWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithType).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnProperty).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnMethodWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnMethodWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithName).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethodWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithType).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethod()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);

            // for some unknown reason MEF adds () to the exported type on a method. No clue why what so ever....!!!
            Assert.AreEqual(typeof(IExportingInterface).FullName + "()", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethod).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ImportOnConstructorWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnConstructor", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(IExportingInterface) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(IExportingInterface) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(IExportingInterface) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(IExportingInterface) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructor()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructor));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(IExportingInterface) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(IExportingInterface) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithEnumerable()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithEnumerable));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ContractName", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithEnumerable).GetConstructor(new[] { typeof(IEnumerable<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithEnumerable).GetConstructor(
                        new[] 
                        { 
                            typeof(IEnumerable<IExportingInterface>) 
                        }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithLazy()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithLazy));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ContractName", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Lazy<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithLazy).GetConstructor(new[] { typeof(Lazy<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithLazy).GetConstructor(new[] { typeof(Lazy<IExportingInterface>) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithFunc()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithFunc));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ContractName", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Func<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithFunc).GetConstructor(new[] { typeof(Func<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithFunc).GetConstructor(new[] { typeof(Func<IExportingInterface>) }).GetParameters().First()),
                import.Parameter);
        }

        [Test]
        public void ImportOnPropertyWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithName));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnProperty", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithName).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithType));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithType).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnProperty).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithEnumerable()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithEnumerable));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ContractName", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithEnumerable).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithLazy()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithLazy));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ContractName", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Lazy<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithLazy).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithFunc()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithFunc));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ContractName", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Func<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithFunc).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ActionOnMethod()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ActionOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Actions.Count());

            var action = plugin.Actions.First();
            Assert.IsNotNull(action);
            Assert.AreEqual("ActionMethod", action.ContractName);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ActionOnMethod).GetMethod("ActionMethod")), 
                action.Method);
        }

        [Test]
        public void ConditionOnMethod()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ConditionOnMethod));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Conditions.Count());

            var condition = plugin.Conditions.First() as MethodBasedScheduleConditionDefinition;
            Assert.IsNotNull(condition);
            Assert.AreEqual("OnMethod", condition.ContractName);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ConditionOnMethod).GetMethod("ConditionMethod")),
                condition.Method);
        }

        [Test]
        public void ConditionOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ConditionOnProperty));
            Assert.IsTrue(s_Types.Exists(s => s.Identity.Equals(id)));

            var plugins = s_Parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Conditions.Count());

            var condition = plugin.Conditions.First() as PropertyBasedScheduleConditionDefinition;
            Assert.IsNotNull(condition);
            Assert.AreEqual("OnProperty", condition.ContractName);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ConditionOnProperty).GetProperty("ConditionProperty")),
                condition.Property);
        }

        [Test]
        public void GroupWithExport()
        {
            Assert.AreEqual(3, s_Groups.Count());

            var group = s_Groups.First();

            Assert.AreEqual(new GroupRegistrationId(GroupExporter.GroupName1), group.GroupExport.ContainingGroup);
            Assert.AreEqual(GroupExporter.GroupExportName, group.GroupExport.ContractName);

            Assert.AreEqual(4, group.GroupExport.ProvidedExports.Count());
        }

        [Test]
        public void GroupWithImport()
        {
            Assert.AreEqual(3, s_Groups.Count());

            var group = s_Groups.First();
            Assert.AreEqual(new GroupRegistrationId(GroupExporter.GroupName1), group.GroupImports.First().ContainingGroup);
            Assert.IsNotNull(group.GroupImports.First().ScheduleInsertPosition);
            
            Assert.AreEqual(4, group.Schedule.Schedule.Vertices.Count());
            Assert.IsTrue(
                AreVerticesEqual(
                    group.Schedule.Schedule.Start,
                    group.Schedule.Schedule.Vertices.ElementAt(0)));
            Assert.IsTrue(
                AreVerticesEqual(
                    new ExecutingActionVertex(2, group.Schedule.Actions.First().Key),
                    group.Schedule.Schedule.Vertices.ElementAt(1)));
            Assert.IsTrue(
                AreVerticesEqual(
                    new InsertVertex(3),
                    group.Schedule.Schedule.Vertices.ElementAt(2)));
            Assert.IsTrue(
                AreVerticesEqual(
                    group.Schedule.Schedule.End,
                    group.Schedule.Schedule.Vertices.ElementAt(3)));

            Assert.AreEqual(1, group.GroupImports.First().ImportsToMatch.Count());
        }
    }
}
