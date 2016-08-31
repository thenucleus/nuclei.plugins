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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
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
    public sealed class GroupImportEngineTest
    {
        private static List<PartDefinition> s_Parts;
        private static List<GroupDefinition> s_Groups;

        private static GroupDefinition CreateExportingGroup()
        {
            return new GroupDefinition("a")
                {
                    InternalConnections = Enumerable.Empty<PartImportToPartExportMap>(),
                    Parts = new List<GroupPartDefinition>
                                {
                                    new GroupPartDefinition(
                                        TypeIdentity.CreateDefinition(typeof(int)),
                                        0,
                                        new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                            {
                                                { 
                                                    new ExportRegistrationId(typeof(int), 0, "PartContract1"),
                                                    TypeBasedExportDefinition.CreateDefinition("PartContract1", typeof(int))
                                                }
                                            },
                                        new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                        new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                        new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                    new GroupPartDefinition(
                                        TypeIdentity.CreateDefinition(typeof(string)),
                                        1,
                                        new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                            {
                                                { 
                                                    new ExportRegistrationId(typeof(string), 1, "PartContract2"),
                                                    TypeBasedExportDefinition.CreateDefinition("PartContract2", typeof(string))
                                                }
                                            },
                                        new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                        new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                        new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                    new GroupPartDefinition(
                                        TypeIdentity.CreateDefinition(typeof(Version)),
                                        2,
                                        new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                            {
                                                { 
                                                    new ExportRegistrationId(typeof(string), 2, "PartContract2"),
                                                    TypeBasedExportDefinition.CreateDefinition("PartContract2", typeof(string))
                                                }
                                            },
                                        new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                        new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                        new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                },
                    GroupExport = GroupExportDefinition.CreateDefinition(
                        "ContractName",
                        new GroupRegistrationId("a"),
                        new List<ExportRegistrationId>
                        {
                            new ExportRegistrationId(typeof(int), 0, "PartContract1"),
                            new ExportRegistrationId(typeof(string), 1, "PartContract2"),
                            new ExportRegistrationId(typeof(string), 2, "PartContract2"),
                        }),
                };
        }

        private static GroupDefinition CreateImportingGroup()
        {
            return new GroupDefinition("b")
                {
                    InternalConnections = Enumerable.Empty<PartImportToPartExportMap>(),
                    Parts = new List<GroupPartDefinition>
                                {
                                    new GroupPartDefinition(
                                        TypeIdentity.CreateDefinition(typeof(string)),
                                        0,
                                        new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                                        new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                                            {
                                                { 
                                                    new ImportRegistrationId(typeof(string), 0, "PartContract1"),
                                                    PropertyBasedImportDefinition.CreateDefinition(
                                                        "PartContract1", 
                                                        TypeIdentity.CreateDefinition(typeof(int)),
                                                        ImportCardinality.ExactlyOne,
                                                        false,
                                                        CreationPolicy.Any,
                                                        typeof(ImportOnPropertyWithType).GetProperty("ImportingProperty"))
                                                }
                                            },
                                        new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                        new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                    new GroupPartDefinition(
                                        TypeIdentity.CreateDefinition(typeof(string)),
                                        1,
                                        new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                                        new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                                            {
                                                { 
                                                    new ImportRegistrationId(typeof(string), 1, "PartContract2"),
                                                    PropertyBasedImportDefinition.CreateDefinition(
                                                        "PartContract2", 
                                                        TypeIdentity.CreateDefinition(typeof(string)),
                                                        ImportCardinality.ExactlyOne,
                                                        false,
                                                        CreationPolicy.Any,
                                                        typeof(ImportOnPropertyWithEnumerable).GetProperty("ImportingProperty"))
                                                }
                                            },
                                        new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                        new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                },
                    GroupImports = new List<GroupImportDefinition> 
                            {
                                GroupImportDefinition.CreateDefinition(
                                    "ContractName",
                                    new GroupRegistrationId("b"),
                                    null,
                                    new List<ImportRegistrationId>
                                        {
                                            new ImportRegistrationId(typeof(string), 0, "PartContract1"),
                                            new ImportRegistrationId(typeof(string), 1, "PartContract2"),
                                        })
                            },
                };
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
        public void AcceptsWithNonmatchingContractName()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(importEngine.Accepts(s_Groups[1].GroupImports.First(), s_Groups[2].GroupExport));
        }

        [Test]
        public void AcceptsWithNoPlaceForSchedule()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(importEngine.Accepts(s_Groups[1].GroupImports.First(), s_Groups[0].GroupExport));
        }

        [Test]
        public void AcceptsWithUnmatchedPartImport()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(false);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(importEngine.Accepts(s_Groups[0].GroupImports.First(), s_Groups[1].GroupExport));
        }

        [Test]
        public void Accepts()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsTrue(importEngine.Accepts(s_Groups[0].GroupImports.First(), s_Groups[2].GroupExport));
        }

        [Test]
        public void ExportMatchesSelectionCriteriaWithoutMatch()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsFalse(
                importEngine.ExportPassesSelectionCriteria(
                    s_Groups.First().GroupExport,
                    new Dictionary<string, object> { { "a", new object() } }));
        }

        [Test]
        public void ExportMatchesSelectionCriteriaWithMatch()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            Assert.IsTrue(
                importEngine.ExportPassesSelectionCriteria(
                    s_Groups.First().GroupExport,
                    new Dictionary<string, object>()));
        }

        [Test]
        public void MatchingGroupsWithSelectionCriteria()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            var groups = importEngine.MatchingGroups(new Dictionary<string, object>());

            Assert.AreEqual(3, groups.Count());
        }

        [Test]
        public void MatchingGroupsWithGroupImport()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.Parts())
                    .Returns(s_Parts);
                repository.Setup(r => r.Part(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(t => s_Parts.FirstOrDefault(p => p.Identity.Equals(t)));
                repository.Setup(r => r.Groups())
                    .Returns(s_Groups);
                repository.Setup(r => r.Group(It.IsAny<GroupRegistrationId>()))
                    .Returns<GroupRegistrationId>(id => s_Groups.FirstOrDefault(g => g.Id.Equals(id)));
            }

            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns(true);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            var groups = importEngine.MatchingGroups(s_Groups[0].GroupImports.First());

            Assert.AreEqual(2, groups.Count());
            Assert.AreEqual(s_Groups[1].Id, groups.First().Id);
            Assert.AreEqual(s_Groups[2].Id, groups.Last().Id);
        }

        [Test]
        public void GenerateConnectionForWithSingleConnection()
        {
            var repository = new Mock<IPluginRepository>();
            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns<SerializableImportDefinition, SerializableExportDefinition>(
                        (import, export) => import.ContractName == export.ContractName);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            var connections = importEngine.GenerateConnectionFor(s_Groups[0], s_Groups[0].GroupImports.First(), s_Groups[2]);

            Assert.AreEqual(1, connections.Count());

            var connection = connections.First();
            Assert.AreEqual(s_Groups[0].GroupImports.First().ImportsToMatch.First(), connection.Import);
            Assert.AreEqual(1, connection.Exports.Count());
            Assert.AreEqual(s_Groups[2].GroupExport.ProvidedExports.First(), connection.Exports.First());
        }

        [Test]
        public void GenerateConnectionForWithMultipleConnections()
        {
            var group1 = CreateExportingGroup();
            var group2 = CreateImportingGroup();

            var repository = new Mock<IPluginRepository>();
            var partImportEngine = new Mock<IConnectParts>();
            {
                partImportEngine.Setup(i => i.Accepts(It.IsAny<SerializableImportDefinition>(), It.IsAny<SerializableExportDefinition>()))
                    .Returns<SerializableImportDefinition, SerializableExportDefinition>(
                        (import, export) => import.ContractName == export.ContractName);
            }

            var importEngine = new GroupImportEngine(repository.Object, partImportEngine.Object);
            var connections = importEngine.GenerateConnectionFor(group2, group2.GroupImports.First(), group1);

            Assert.AreEqual(2, connections.Count());

            var connection = connections.ElementAt(0);
            Assert.AreEqual(group2.GroupImports.First().ImportsToMatch.ElementAt(0), connection.Import);
            Assert.AreEqual(1, connection.Exports.Count());
            Assert.AreEqual(group1.GroupExport.ProvidedExports.ElementAt(0), connection.Exports.ElementAt(0));

            connection = connections.ElementAt(1);
            Assert.AreEqual(group2.GroupImports.First().ImportsToMatch.ElementAt(1), connection.Import);
            Assert.AreEqual(2, connection.Exports.Count());
            Assert.That(
                connection.Exports,
                Is.EquivalentTo(
                    new List<ExportRegistrationId> 
                    {
                        group1.GroupExport.ProvidedExports.ElementAt(1),
                        group1.GroupExport.ProvidedExports.ElementAt(2),
                    }));
        }
    }
}
