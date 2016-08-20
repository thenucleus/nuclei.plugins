//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Moq;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Instantiation
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CompositionLayerTest
    {
        private static GroupDefinition CreateExportingDefinition()
        {
            var groupName = "Export";
            return new GroupDefinition(groupName)
                {
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

                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(DateTime)),
                                            2,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                                                {
                                                    { 
                                                        new ExportRegistrationId(typeof(string), 3, "PartContract3"),
                                                        TypeBasedExportDefinition.CreateDefinition("PartContract3", typeof(string))
                                                    }
                                                },
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(List<int>)),
                                            2,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                                                {
                                                    { 
                                                        new ImportRegistrationId(typeof(string), 1, "PartContract3"),
                                                        PropertyBasedImportDefinition.CreateDefinition(
                                                            "PartContract3", 
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
                    InternalConnections = new List<PartImportToPartExportMap>
                        {
                            new PartImportToPartExportMap(
                                new ImportRegistrationId(typeof(string), 1, "PartContract3"),
                                new List<ExportRegistrationId>
                                    {   
                                        new ExportRegistrationId(typeof(string), 3, "PartContract3")
                                    }),
                        },
                    GroupExport = GroupExportDefinition.CreateDefinition(
                        "ContractName",
                        new GroupRegistrationId(groupName),
                        new List<ExportRegistrationId>
                            {
                                new ExportRegistrationId(typeof(int), 0, "PartContract1"),
                                new ExportRegistrationId(typeof(string), 1, "PartContract2"),
                                new ExportRegistrationId(typeof(string), 2, "PartContract2"),
                            }),
                };
        }

        private static GroupDefinition CreateImportingDefinition()
        {
            var groupName = "Import";
            return new GroupDefinition(groupName)
                {
                    InternalConnections = Enumerable.Empty<PartImportToPartExportMap>(),
                    Parts = new List<GroupPartDefinition>
                                    {
                                        new GroupPartDefinition(
                                            TypeIdentity.CreateDefinition(typeof(List<string>)),
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
                                            TypeIdentity.CreateDefinition(typeof(List<double>)),
                                            1,
                                            new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                                            new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                                                {
                                                    { 
                                                        new ImportRegistrationId(typeof(string), 1, "PartContract2"),
                                                        ConstructorBasedImportDefinition.CreateDefinition(
                                                            "PartContract2", 
                                                            TypeIdentity.CreateDefinition(typeof(string)),
                                                            ImportCardinality.ExactlyOne,
                                                            CreationPolicy.Any,
                                                            typeof(Version).GetConstructor(
                                                                new[] 
                                                                { 
                                                                    typeof(string) 
                                                                }).GetParameters().First())
                                                    }
                                                },
                                            new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                                            new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>()),
                                    },
                    GroupImports = new List<GroupImportDefinition> 
                                {
                                    GroupImportDefinition.CreateDefinition(
                                        "ContractName",
                                        new GroupRegistrationId(groupName),
                                        null,
                                        new List<ImportRegistrationId>
                                            {
                                                new ImportRegistrationId(typeof(string), 0, "PartContract1"),
                                                new ImportRegistrationId(typeof(string), 1, "PartContract2"),
                                            })
                                },
                };
        }

        [Test]
        public void Add()
        {
            var definition = CreateExportingDefinition();

            int index = -1;
            var instanceIds = new List<PartInstanceId> 
                {
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                };
            var storage = new Mock<IStoreInstances>();
            {
                storage.Setup(
                        s => s.Construct(
                            It.IsAny<GroupPartDefinition>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Callback<GroupPartDefinition, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (d, i) =>
                        {
                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(int))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(string))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(Version))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(DateTime))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(List<int>))))
                            {
                                Assert.AreEqual(1, i.Count());
                                Assert.AreEqual(definition.Parts.ElementAt(4).RegisteredImports.First(), i.First().Item1);
                                Assert.AreEqual(instanceIds[index], i.First().Item2);
                                Assert.AreEqual(definition.Parts.ElementAt(3).RegisteredExports.First(), i.First().Item3);
                                return;
                            }

                            Assert.Fail();
                        })
                    .Returns<GroupPartDefinition, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (d, i) => 
                        {
                            index++;
                            return instanceIds[index];
                        })
                    .Verifiable();
            }

            var layer = CompositionLayer.CreateInstanceWithoutTimeline(storage.Object);
            
            var id = new GroupCompositionId();
            layer.Add(id, definition);

            Assert.AreEqual(1, layer.Groups().Count());
            Assert.AreSame(id, layer.Groups().First());
            Assert.AreSame(definition, layer.Group(id));

            storage.Verify(
                s => s.Construct(
                    It.IsAny<GroupPartDefinition>(), 
                    It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()), 
                Times.Exactly(5));
        }

        [Test]
        public void AddMultipleInstanceWithSameDefinition()
        {
            var firstDefinition = CreateExportingDefinition();

            int index = -1;
            var instanceIds = new List<PartInstanceId> 
                {
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                };
            var storage = new Mock<IStoreInstances>();
            {
                storage.Setup(
                        s => s.Construct(
                            It.IsAny<GroupPartDefinition>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Callback<GroupPartDefinition, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (d, i) =>
                        {
                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(int))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(string))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(Version))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(DateTime))))
                            {
                                Assert.IsFalse(i.Any());
                                return;
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(List<int>))))
                            {
                                Assert.AreEqual(1, i.Count());
                                Assert.AreEqual(instanceIds[index], i.First().Item2);
                                return;
                            }

                            Assert.Fail();
                        })
                    .Returns<GroupPartDefinition, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (d, i) =>
                        {
                            index++;
                            return instanceIds[index];
                        })
                    .Verifiable();
            }

            var layer = CompositionLayer.CreateInstanceWithoutTimeline(storage.Object);

            var firstId = new GroupCompositionId();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateExportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.AreEqual(2, layer.Groups().Count());
            Assert.AreSame(firstDefinition, layer.Group(firstId));
            Assert.AreSame(firstDefinition, layer.Group(secondId));

            storage.Verify(
                s => s.Construct(
                    It.IsAny<GroupPartDefinition>(),
                    It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()),
                Times.Exactly(10));
        }

        [Test]
        public void Remove()
        {
            int index = -1;
            var instanceIds = new List<PartInstanceId> 
                {
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                };
            var storage = new Mock<IStoreInstances>();
            {
                storage.Setup(
                        s => s.Construct(
                            It.IsAny<GroupPartDefinition>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Returns(
                        () =>
                        {
                            index++;
                            return instanceIds[index];
                        });

                storage.Setup(s => s.Release(It.IsAny<PartInstanceId>()))
                    .Returns<PartInstanceId>(
                        i =>
                        {
                            return new List<InstanceUpdate> 
                                { 
                                    new InstanceUpdate 
                                        {
                                            Instance = i,
                                            Change = InstanceChange.Removed,
                                        }
                                };
                        })
                    .Verifiable();
            }

            var layer = CompositionLayer.CreateInstanceWithoutTimeline(storage.Object);

            var groupId = new GroupCompositionId();
            var definition = CreateExportingDefinition();

            layer.Add(groupId, definition);

            Assert.AreEqual(1, layer.Groups().Count());
            Assert.AreSame(groupId, layer.Groups().First());
            Assert.AreSame(definition, layer.Group(groupId));

            layer.Remove(groupId);
            Assert.AreEqual(0, layer.Groups().Count());
            storage.Verify(s => s.Release(It.IsAny<PartInstanceId>()), Times.Exactly(5));
        }

        [Test]
        public void RemoveWithConnectedGroups()
        {
            int index = -1;
            var instanceIds = new List<PartInstanceId> 
                {
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                };
            var storage = new Mock<IStoreInstances>();
            {
                storage.Setup(
                        s => s.Construct(
                            It.IsAny<GroupPartDefinition>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Returns(
                        () =>
                        {
                            index++;
                            return instanceIds[index];
                        });

                storage.Setup(
                        s => s.UpdateIfRequired(
                            It.IsAny<PartInstanceId>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Returns<PartInstanceId, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (i, l) =>
                        {
                            return new List<InstanceUpdate> 
                                { 
                                    new InstanceUpdate 
                                        {
                                            Instance = i,
                                            Change = InstanceChange.Updated,
                                        }
                                };
                        })
                    .Verifiable();

                storage.Setup(s => s.Release(It.IsAny<PartInstanceId>()))
                    .Returns<PartInstanceId>(
                        i =>
                        {
                            return new List<InstanceUpdate> 
                                { 
                                    new InstanceUpdate 
                                        {
                                            Instance = i,
                                            Change = InstanceChange.Removed,
                                        }
                                };
                        })
                    .Verifiable();
            }

            var layer = CompositionLayer.CreateInstanceWithoutTimeline(storage.Object);

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.That(layer.UnsatisfiedImports(secondId), Is.EquivalentTo(secondDefinition.GroupImports));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap>
                    {
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.That(
                layer.SatisfiedImports(secondId),
                Is.EquivalentTo(
                    new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                        { 
                            new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                        }));

            layer.Remove(firstId);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.That(layer.UnsatisfiedImports(secondId), Is.EquivalentTo(secondDefinition.GroupImports));
            storage.Verify(s => s.Release(It.IsAny<PartInstanceId>()), Times.Exactly(6));
        }

        [Test]
        public void Connect()
        {
            int index = -1;
            var instanceIds = new List<PartInstanceId> 
                {
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                };
            var storage = new Mock<IStoreInstances>();
            {
                storage.Setup(
                        s => s.Construct(
                            It.IsAny<GroupPartDefinition>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Callback<GroupPartDefinition, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (d, i) =>
                        {
                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(List<string>))))
                            {
                                Assert.AreEqual(0, i.Count());
                            }

                            if (d.Identity.Equals(TypeIdentity.CreateDefinition(typeof(List<double>))))
                            {
                                Assert.AreEqual(2, i.Count());
                                Assert.AreEqual(instanceIds[1], i.ElementAt(0).Item2);
                                Assert.AreEqual(instanceIds[2], i.ElementAt(1).Item2);
                            }
                        })
                    .Returns<GroupPartDefinition, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (d, i) =>
                        {
                            index++;
                            return instanceIds[index];
                        })
                    .Verifiable();

                storage.Setup(
                        s => s.UpdateIfRequired(
                            It.IsAny<PartInstanceId>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Callback<PartInstanceId, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (i, l) =>
                        {
                            if (i.Equals(instanceIds[5]))
                            {
                                Assert.AreEqual(1, l.Count());
                                Assert.AreEqual(instanceIds[0], l.ElementAt(0).Item2);
                            }
                        })
                    .Returns<PartInstanceId, IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>(
                        (i, l) =>
                        {
                            return new List<InstanceUpdate> 
                                { 
                                    new InstanceUpdate 
                                        {
                                            Instance = i,
                                            Change = InstanceChange.Updated,
                                        }
                                };
                        })
                    .Verifiable();
            }

            var layer = CompositionLayer.CreateInstanceWithoutTimeline(storage.Object);

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.That(layer.UnsatisfiedImports(secondId), Is.EquivalentTo(secondDefinition.GroupImports));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap> 
                    { 
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.That(
                layer.SatisfiedImports(secondId),
                Is.EquivalentTo(
                    new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                        { 
                            new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                        }));

            storage.Verify(
                s => s.Construct(
                    It.IsAny<GroupPartDefinition>(),
                    It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()),
                Times.Exactly(7));
        }

        [Test]
        public void DisconnectImportFromExports()
        {
            int index = -1;
            var instanceIds = new List<PartInstanceId> 
                {
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                };
            var storage = new Mock<IStoreInstances>();
            {
                storage.Setup(
                        s => s.Construct(
                            It.IsAny<GroupPartDefinition>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Returns(
                        () =>
                        {
                            index++;
                            return instanceIds[index];
                        });

                storage.Setup(s => s.Release(It.IsAny<PartInstanceId>()))
                    .Returns<PartInstanceId>(
                        i =>
                        {
                            return new List<InstanceUpdate> 
                                { 
                                    new InstanceUpdate 
                                        {
                                            Instance = i,
                                            Change = InstanceChange.Removed,
                                        }
                                };
                        })
                    .Verifiable();
            }

            var layer = CompositionLayer.CreateInstanceWithoutTimeline(storage.Object);

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.That(layer.UnsatisfiedImports(secondId), Is.EquivalentTo(secondDefinition.GroupImports));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap>
                    {
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.That(
                layer.SatisfiedImports(secondId),
                Is.EquivalentTo(
                    new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                        { 
                            new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                        }));

            layer.Disconnect(secondId, firstId);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.That(layer.UnsatisfiedImports(secondId), Is.EquivalentTo(secondDefinition.GroupImports));
            storage.Verify(s => s.Release(It.IsAny<PartInstanceId>()), Times.Exactly(1));
        }

        [Test]
        public void DisconnectFromAll()
        {
            int index = -1;
            var instanceIds = new List<PartInstanceId> 
                {
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                    new PartInstanceId(),
                };
            var storage = new Mock<IStoreInstances>();
            {
                storage.Setup(
                        s => s.Construct(
                            It.IsAny<GroupPartDefinition>(),
                            It.IsAny<IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>>()))
                    .Returns(
                        () =>
                        {
                            index++;
                            return instanceIds[index];
                        });

                storage.Setup(s => s.Release(It.IsAny<PartInstanceId>()))
                    .Returns<PartInstanceId>(
                        i =>
                        {
                            return new List<InstanceUpdate> 
                                { 
                                    new InstanceUpdate 
                                        {
                                            Instance = i,
                                            Change = InstanceChange.Removed,
                                        }
                                };
                        })
                    .Verifiable();
            }

            var layer = CompositionLayer.CreateInstanceWithoutTimeline(storage.Object);

            var firstId = new GroupCompositionId();
            var firstDefinition = CreateExportingDefinition();
            layer.Add(firstId, firstDefinition);

            var secondId = new GroupCompositionId();
            var secondDefinition = CreateImportingDefinition();
            layer.Add(secondId, secondDefinition);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.That(layer.UnsatisfiedImports(secondId), Is.EquivalentTo(secondDefinition.GroupImports));

            layer.Connect(new GroupConnection(
                secondId,
                firstId,
                secondDefinition.GroupImports.First(),
                new List<PartImportToPartExportMap>
                    {
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(0),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(0)
                                }),
                        new PartImportToPartExportMap(
                            secondDefinition.GroupImports.First().ImportsToMatch.ElementAt(1),
                            new List<ExportRegistrationId> 
                                {
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(1),
                                    firstDefinition.GroupExport.ProvidedExports.ElementAt(2),
                                }),
                    }));

            Assert.IsFalse(layer.UnsatisfiedImports(secondId).Any());
            Assert.That(
                layer.SatisfiedImports(secondId),
                Is.EquivalentTo(
                    new List<Tuple<GroupImportDefinition, GroupCompositionId>> 
                        { 
                            new Tuple<GroupImportDefinition, GroupCompositionId>(secondDefinition.GroupImports.First(), firstId)
                        }));

            layer.Disconnect(firstId);

            Assert.IsFalse(layer.SatisfiedImports(secondId).Any());
            Assert.That(layer.UnsatisfiedImports(secondId), Is.EquivalentTo(secondDefinition.GroupImports));
            storage.Verify(s => s.Release(It.IsAny<PartInstanceId>()), Times.Exactly(1));
        }
    }
}
