//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Moq;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class GroupDefinitionBuilderTest
    {
        private static IEnumerable<PartDefinition> CreatePluginTypes()
        {
            var plugins = new List<PartDefinition> 
                {
                    new PartDefinition
                        {
                            Identity = TypeIdentity.CreateDefinition(typeof(ActionOnMethod)),
                            Exports = new List<SerializableExportDefinition> 
                                {
                                    TypeBasedExportDefinition.CreateDefinition("ActionExport", typeof(ActionOnMethod))
                                },
                            Imports = new List<SerializableImportDefinition>(),
                            Actions = new List<ScheduleActionDefinition>
                                {
                                    ScheduleActionDefinition.CreateDefinition(
                                        "ActionMethod", 
                                        typeof(ActionOnMethod).GetMethod("ActionMethod"))
                                },
                            Conditions = new List<ScheduleConditionDefinition>(),
                        },
                    new PartDefinition
                        {
                            Identity = TypeIdentity.CreateDefinition(typeof(ConditionOnMethod)),
                            Exports = new List<SerializableExportDefinition> 
                                {
                                    TypeBasedExportDefinition.CreateDefinition("ConditionOnMethodExport", typeof(ConditionOnMethod))
                                },
                            Imports = new List<SerializableImportDefinition>(),
                            Actions = new List<ScheduleActionDefinition>(),
                            Conditions = new List<ScheduleConditionDefinition>
                                {
                                    MethodBasedScheduleConditionDefinition.CreateDefinition(
                                        "OnMethod",
                                        typeof(ConditionOnMethod).GetMethod("ConditionMethod"))
                                },
                        },
                    new PartDefinition
                        {
                            Identity = TypeIdentity.CreateDefinition(typeof(ConditionOnProperty)),
                            Exports = new List<SerializableExportDefinition> 
                                {
                                    TypeBasedExportDefinition.CreateDefinition("ConditionOnPropertyExport", typeof(ConditionOnProperty))
                                },
                            Imports = new List<SerializableImportDefinition>(),
                            Actions = new List<ScheduleActionDefinition>(),
                            Conditions = new List<ScheduleConditionDefinition>
                                {
                                    PropertyBasedScheduleConditionDefinition.CreateDefinition(
                                        "OnProperty",
                                        typeof(ConditionOnProperty).GetProperty("ConditionProperty"))
                                },
                        },
                    new PartDefinition
                        {
                            Identity = TypeIdentity.CreateDefinition(typeof(ExportOnProperty)),
                            Exports = new List<SerializableExportDefinition> 
                                {
                                    PropertyBasedExportDefinition.CreateDefinition(
                                        typeof(IExportingInterface).FullName, 
                                        typeof(ExportOnProperty).GetProperty("ExportingProperty"))
                                },
                            Imports = new List<SerializableImportDefinition>(),
                            Actions = new List<ScheduleActionDefinition>(),
                            Conditions = new List<ScheduleConditionDefinition>(),
                        },
                    new PartDefinition
                        {
                            Identity = TypeIdentity.CreateDefinition(typeof(ImportOnProperty)),
                            Exports = new List<SerializableExportDefinition> 
                                {
                                    TypeBasedExportDefinition.CreateDefinition(typeof(ImportOnProperty).FullName, typeof(ImportOnProperty))
                                },
                            Imports = new List<SerializableImportDefinition>
                                {
                                    PropertyBasedImportDefinition.CreateDefinition(
                                        typeof(IExportingInterface).FullName,
                                        TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                        ImportCardinality.ExactlyOne,
                                        false,
                                        CreationPolicy.Shared,
                                        typeof(ImportOnProperty).GetProperty("ImportingProperty"))
                                },
                            Actions = new List<ScheduleActionDefinition>(),
                            Conditions = new List<ScheduleConditionDefinition>(),
                        }
                };

            return plugins;
        }

        [Test]
        public void RegisterObjectWithUnknownType()
        {
            var repository = new Mock<IPluginRepository>();
            var importEngine = new Mock<IConnectParts>();
            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder, 
                new PluginFileInfo("a", DateTimeOffset.Now));
            Assert.Throws<UnknownPluginTypeException>(() => builder.RegisterObject(typeof(ExportOnPropertyWithName)));
        }

        [Test]
        public void RegisterObject()
        {
            var plugins = CreatePluginTypes();
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
            }

            var importEngine = new Mock<IConnectParts>();
            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var info = builder.RegisterObject(typeof(ActionOnMethod));

            Assert.IsFalse(info.RegisteredConditions.Any());
            Assert.IsFalse(info.RegisteredImports.Any());

            Assert.AreEqual(1, info.RegisteredExports.Count());
            Assert.AreEqual("ActionExport", info.RegisteredExports.First().ContractName);

            Assert.AreEqual(1, info.RegisteredActions.Count());
        }

        [Test]
        public void RegisterObjectWithMultipleSameType()
        {
            var plugins = CreatePluginTypes();
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
            }

            var importEngine = new Mock<IConnectParts>();
            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var firstInfo = builder.RegisterObject(typeof(ActionOnMethod));
            var secondInfo = builder.RegisterObject(typeof(ActionOnMethod));

            Assert.AreNotEqual(firstInfo.Id, secondInfo.Id);
        }

        [Test]
        public void ConnectWithNonmatchingExport()
        {
            var plugins = CreatePluginTypes();
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
            }

            var importEngine = new Mock<IConnectParts>();
            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ActionOnMethod));
            Assert.Throws<CannotMapExportToImportException>(
                () => builder.Connect(firstInfo.RegisteredImports.First(), secondInfo.RegisteredExports.First()));
        }

        [Test]
        public void Connect()
        {
            var plugins = CreatePluginTypes();
            GroupDefinition groupInfo = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
                repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                    .Callback<GroupDefinition, PluginFileInfo>((g, f) => groupInfo = g);
            }

            var importEngine = new Mock<IConnectParts>();
            {
                importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();
            
            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ExportOnProperty));
            builder.Connect(firstInfo.RegisteredImports.First(), secondInfo.RegisteredExports.First());

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(firstInfo.RegisteredImports.First(), groupInfo.InternalConnections.First().Import);
            Assert.AreEqual(1, groupInfo.InternalConnections.First().Exports.Count());
            Assert.AreEqual(secondInfo.RegisteredExports.First(), groupInfo.InternalConnections.First().Exports.First());
        }

        [Test]
        public void ConnectOverridingCurrentConnection()
        {
            var plugins = CreatePluginTypes();
            GroupDefinition groupInfo = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
                repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                    .Callback<GroupDefinition, PluginFileInfo>((g, f) => groupInfo = g);
            }

            var importEngine = new Mock<IConnectParts>();
            {
                importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ExportOnProperty));
            builder.Connect(firstInfo.RegisteredImports.First(), secondInfo.RegisteredExports.First());

            var thirdInfo = builder.RegisterObject(typeof(ExportOnProperty));
            builder.Connect(firstInfo.RegisteredImports.First(), thirdInfo.RegisteredExports.First());

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(firstInfo.RegisteredImports.First(), groupInfo.InternalConnections.First().Import);
            Assert.AreEqual(1, groupInfo.InternalConnections.First().Exports.Count());
            Assert.AreEqual(thirdInfo.RegisteredExports.First(), groupInfo.InternalConnections.First().Exports.First());
        }

        [Test]
        public void DefineSchedule()
        {
            var plugins = CreatePluginTypes();
            GroupDefinition groupInfo = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
                repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                    .Callback<GroupDefinition, PluginFileInfo>((g, f) => groupInfo = g);
            }

            var importEngine = new Mock<IConnectParts>();
            {
                importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var actionInfo = builder.RegisterObject(typeof(ActionOnMethod));
            var conditionInfo = builder.RegisterObject(typeof(ConditionOnProperty));

            var registrator = builder.RegisterSchedule();
            {
                var vertex = registrator.AddExecutingAction(actionInfo.RegisteredActions.First());
                registrator.LinkFromStart(vertex, conditionInfo.RegisteredConditions.First());
                registrator.LinkToEnd(vertex);
                registrator.Register();
            }

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.AreEqual(actionInfo.RegisteredActions.First(), groupInfo.Schedule.Actions.First().Value);
            Assert.AreEqual(conditionInfo.RegisteredConditions.First(), groupInfo.Schedule.Conditions.First().Value);
            Assert.AreEqual(3, groupInfo.Schedule.Schedule.Vertices.Count());
        }

        [Test]
        public void DefineExport()
        {
            var plugins = CreatePluginTypes();
            GroupDefinition groupInfo = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
                repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                    .Callback<GroupDefinition, PluginFileInfo>((g, f) => groupInfo = g);
            }

            var importEngine = new Mock<IConnectParts>();
            {
                importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));
            var secondInfo = builder.RegisterObject(typeof(ExportOnProperty));
            var thirdInfo = builder.RegisterObject(typeof(ActionOnMethod));
            var fourthInfo = builder.RegisterObject(typeof(ConditionOnProperty));
            builder.Connect(firstInfo.RegisteredImports.First(), secondInfo.RegisteredExports.First());

            var registrator = builder.RegisterSchedule();
            {
                var vertex = registrator.AddExecutingAction(thirdInfo.RegisteredActions.First());
                registrator.LinkFromStart(vertex, fourthInfo.RegisteredConditions.First());
                registrator.LinkToEnd(vertex);
                registrator.Register();
            }

            var groupExportName = "groupExport";
            builder.DefineExport(groupExportName);

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(new GroupRegistrationId(groupName), groupInfo.GroupExport.ContainingGroup);
            Assert.AreEqual(groupExportName, groupInfo.GroupExport.ContractName);

            Assert.That(
                groupInfo.GroupExport.ProvidedExports,
                Is.EquivalentTo(
                    new List<ExportRegistrationId> 
                    { 
                        firstInfo.RegisteredExports.First(),
                        thirdInfo.RegisteredExports.First(),
                        fourthInfo.RegisteredExports.First(),
                    }));
        }

        [Test]
        public void DefineImportWithScheduleElement()
        {
            var plugins = CreatePluginTypes();
            GroupDefinition groupInfo = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
                repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                    .Callback<GroupDefinition, PluginFileInfo>((g, f) => groupInfo = g);
            }

            var importEngine = new Mock<IConnectParts>();
            {
                importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            builder.RegisterObject(typeof(ImportOnProperty));

            var registrator = builder.RegisterSchedule();
            var vertex = registrator.AddInsertPoint();
            registrator.LinkFromStart(vertex);
            registrator.LinkToEnd(vertex);
            registrator.Register();

            var groupImportName = "groupImport";
            builder.DefineImport(groupImportName, vertex);

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(new GroupRegistrationId(groupName), groupInfo.GroupImports.First().ContainingGroup);
            Assert.AreEqual(vertex, groupInfo.GroupImports.First().ScheduleInsertPosition);

            Assert.IsFalse(groupInfo.GroupImports.First().ImportsToMatch.Any());
        }

        [Test]
        public void DefineImportWithObjectImports()
        {
            var plugins = CreatePluginTypes();
            GroupDefinition groupInfo = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(plugins);
                repository.Setup(r => r.AddGroup(It.IsAny<GroupDefinition>(), It.IsAny<PluginFileInfo>()))
                    .Callback<GroupDefinition, PluginFileInfo>((g, f) => groupInfo = g);
            }

            var importEngine = new Mock<IConnectParts>();
            {
                importEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            Func<IBuildFixedSchedules> scheduleBuilder = () => new FixedScheduleBuilder();

            var builder = new GroupDefinitionBuilder(
                repository.Object, 
                importEngine.Object, 
                identityGenerator, 
                scheduleBuilder,
                new PluginFileInfo("a", DateTimeOffset.Now));
            var firstInfo = builder.RegisterObject(typeof(ImportOnProperty));

            var groupImportName = "groupImport";
            builder.DefineImport(groupImportName, new List<ImportRegistrationId> { firstInfo.RegisteredImports.First() });

            var groupName = "MyGroup";
            builder.Register(groupName);

            Assert.IsNotNull(groupInfo);
            Assert.AreEqual(new GroupRegistrationId(groupName), groupInfo.GroupImports.First().ContainingGroup);
            Assert.IsNull(groupInfo.GroupImports.First().ScheduleInsertPosition);

            Assert.That(
                groupInfo.GroupImports.First().ImportsToMatch,
                Is.EquivalentTo(
                    new List<ImportRegistrationId> 
                    { 
                        firstInfo.RegisteredImports.First(),
                    }));
        }
    }
}
