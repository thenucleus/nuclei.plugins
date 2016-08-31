//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Moq;
using NUnit.Framework;

namespace Nuclei.Plugins.Instantiation
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CompositionCommandsTest
    {
        [Test]
        public void Add()
        {
            var lockKey = new DatasetLockKey();
            var datasetLock = new Mock<ITrackDatasetLocks>();
            {
                datasetLock.Setup(d => d.LockForWriting())
                    .Returns(lockKey)
                    .Verifiable();
                datasetLock.Setup(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()))
                    .Callback<DatasetLockKey>(key => Assert.AreSame(lockKey, key))
                    .Verifiable();
            }

            var originalId = new GroupCompositionId();
            var originalDefinition = new GroupDefinition("a");
            var storage = new Mock<IStoreGroupsAndConnections>();
            {
                storage.Setup(s => s.Add(It.IsAny<GroupCompositionId>(), It.IsAny<GroupDefinition>()))
                    .Callback<GroupCompositionId, GroupDefinition>(
                        (id, def) =>
                        {
                            Assert.AreSame(originalId, id);
                            Assert.AreSame(originalDefinition, def);
                        });
            }

            var commands = new CompositionCommands(datasetLock.Object, storage.Object);
            var task = commands.Add(originalId, originalDefinition);
            task.Wait();

            datasetLock.Verify(d => d.LockForWriting(), Times.Once());
            datasetLock.Verify(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()), Times.Once());
        }

        [Test]
        public void Remove()
        {
            var lockKey = new DatasetLockKey();
            var datasetLock = new Mock<ITrackDatasetLocks>();
            {
                datasetLock.Setup(d => d.LockForWriting())
                    .Returns(lockKey)
                    .Verifiable();
                datasetLock.Setup(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()))
                    .Callback<DatasetLockKey>(key => Assert.AreSame(lockKey, key))
                    .Verifiable();
            }

            var originalId = new GroupCompositionId();
            var storage = new Mock<IStoreGroupsAndConnections>();
            {
                storage.Setup(s => s.Remove(It.IsAny<GroupCompositionId>()))
                    .Callback<GroupCompositionId>(
                        id =>
                        {
                            Assert.AreSame(originalId, id);
                        });
            }

            var commands = new CompositionCommands(datasetLock.Object, storage.Object);
            var task = commands.Remove(originalId);
            task.Wait();

            datasetLock.Verify(d => d.LockForWriting(), Times.Once());
            datasetLock.Verify(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()), Times.Once());
        }

        [Test]
        public void Connect()
        {
            var lockKey = new DatasetLockKey();
            var datasetLock = new Mock<ITrackDatasetLocks>();
            {
                datasetLock.Setup(d => d.LockForWriting())
                    .Returns(lockKey)
                    .Verifiable();
                datasetLock.Setup(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()))
                    .Callback<DatasetLockKey>(key => Assert.AreSame(lockKey, key))
                    .Verifiable();
            }

            var originalConnection = new GroupConnection(
                new GroupCompositionId(),
                new GroupCompositionId(),
                GroupImportDefinition.CreateDefinition(
                    "a",
                    new GroupRegistrationId("a"),
                    null,
                    Enumerable.Empty<ImportRegistrationId>()),
                Enumerable.Empty<PartImportToPartExportMap>());
            var storage = new Mock<IStoreGroupsAndConnections>();
            {
                storage.Setup(s => s.Connect(It.IsAny<GroupConnection>()))
                    .Callback<GroupConnection>(
                        id =>
                        {
                            Assert.AreSame(originalConnection, id);
                        });
            }

            var commands = new CompositionCommands(datasetLock.Object, storage.Object);
            var task = commands.Connect(originalConnection);
            task.Wait();

            datasetLock.Verify(d => d.LockForWriting(), Times.Once());
            datasetLock.Verify(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()), Times.Once());
        }

        [Test]
        public void DisconnectImportFromExport()
        {
            var lockKey = new DatasetLockKey();
            var datasetLock = new Mock<ITrackDatasetLocks>();
            {
                datasetLock.Setup(d => d.LockForWriting())
                    .Returns(lockKey)
                    .Verifiable();
                datasetLock.Setup(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()))
                    .Callback<DatasetLockKey>(key => Assert.AreSame(lockKey, key))
                    .Verifiable();
            }

            var originalImportId = new GroupCompositionId();
            var originalExportId = new GroupCompositionId();
            var storage = new Mock<IStoreGroupsAndConnections>();
            {
                storage.Setup(s => s.Disconnect(It.IsAny<GroupCompositionId>(), It.IsAny<GroupCompositionId>()))
                    .Callback<GroupCompositionId, GroupCompositionId>(
                        (importId, exportId) =>
                        {
                            Assert.AreSame(originalImportId, importId);
                            Assert.AreSame(originalExportId, exportId);
                        });
            }

            var commands = new CompositionCommands(datasetLock.Object, storage.Object);
            var task = commands.Remove(originalImportId);
            task.Wait();

            datasetLock.Verify(d => d.LockForWriting(), Times.Once());
            datasetLock.Verify(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()), Times.Once());
        }

        [Test]
        public void DisconnectFromAll()
        {
            var lockKey = new DatasetLockKey();
            var datasetLock = new Mock<ITrackDatasetLocks>();
            {
                datasetLock.Setup(d => d.LockForWriting())
                    .Returns(lockKey)
                    .Verifiable();
                datasetLock.Setup(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()))
                    .Callback<DatasetLockKey>(key => Assert.AreSame(lockKey, key))
                    .Verifiable();
            }

            var originalId = new GroupCompositionId();
            var storage = new Mock<IStoreGroupsAndConnections>();
            {
                storage.Setup(s => s.Disconnect(It.IsAny<GroupCompositionId>()))
                    .Callback<GroupCompositionId>(
                        id =>
                        {
                            Assert.AreSame(originalId, id);
                        });
            }

            var commands = new CompositionCommands(datasetLock.Object, storage.Object);
            var task = commands.Disconnect(originalId);
            task.Wait();

            datasetLock.Verify(d => d.LockForWriting(), Times.Once());
            datasetLock.Verify(d => d.RemoveWriteLock(It.IsAny<DatasetLockKey>()), Times.Once());
        }

        [Test]
        public void NonSatisfiedImports()
        {
            var lockKey = new DatasetLockKey();
            var datasetLock = new Mock<ITrackDatasetLocks>();
            {
                datasetLock.Setup(d => d.LockForReading())
                    .Returns(lockKey)
                    .Verifiable();
                datasetLock.Setup(d => d.RemoveReadLock(It.IsAny<DatasetLockKey>()))
                    .Callback<DatasetLockKey>(key => Assert.AreSame(lockKey, key))
                    .Verifiable();
            }

            var groups = new Dictionary<GroupCompositionId, IEnumerable<GroupImportDefinition>> 
                {
                    {
                        new GroupCompositionId(),
                        new List<GroupImportDefinition>
                            {
                                GroupImportDefinition.CreateDefinition(
                                    "a",
                                    new GroupRegistrationId("a"),
                                    null,
                                    Enumerable.Empty<ImportRegistrationId>()),
                                GroupImportDefinition.CreateDefinition(
                                    "b",
                                    new GroupRegistrationId("a"),
                                    null,
                                    Enumerable.Empty<ImportRegistrationId>()),
                            }
                    },
                    {
                        new GroupCompositionId(),
                        new List<GroupImportDefinition>
                            {
                                GroupImportDefinition.CreateDefinition(
                                    "c",
                                    new GroupRegistrationId("c"),
                                    null,
                                    Enumerable.Empty<ImportRegistrationId>()),
                                GroupImportDefinition.CreateDefinition(
                                    "d",
                                    new GroupRegistrationId("c"),
                                    null,
                                    Enumerable.Empty<ImportRegistrationId>()),
                            }
                    },
                };
            var storage = new Mock<IStoreGroupsAndConnections>();
            {
                storage.Setup(s => s.Groups())
                    .Returns(groups.Keys);
                storage.Setup(s => s.UnsatisfiedImports(It.IsAny<GroupCompositionId>()))
                    .Returns<GroupCompositionId>(id => groups[id]);
            }

            var commands = new CompositionCommands(datasetLock.Object, storage.Object);
            var task = commands.NonSatisfiedImports(false);
            var results = task.Result;
            Assert.That(
                results,
                Is.EquivalentTo(
                    groups.SelectMany(p => p.Value.Select(v => new Tuple<GroupCompositionId, GroupImportDefinition>(p.Key, v)))));

            datasetLock.Verify(d => d.LockForReading(), Times.Once());
            datasetLock.Verify(d => d.RemoveReadLock(It.IsAny<DatasetLockKey>()), Times.Once());
        }

        [Test]
        public void CurrentState()
        {
            var lockKey = new DatasetLockKey();
            var datasetLock = new Mock<ITrackDatasetLocks>();
            {
                datasetLock.Setup(d => d.LockForReading())
                    .Returns(lockKey)
                    .Verifiable();
                datasetLock.Setup(d => d.RemoveReadLock(It.IsAny<DatasetLockKey>()))
                    .Callback<DatasetLockKey>(key => Assert.AreSame(lockKey, key))
                    .Verifiable();
            }

            var groups = new Dictionary<GroupCompositionId, Tuple<GroupDefinition, Tuple<GroupImportDefinition, GroupCompositionId>>>
                {
                    {
                        new GroupCompositionId(),
                        new Tuple<GroupDefinition, Tuple<GroupImportDefinition, GroupCompositionId>>(
                                new GroupDefinition("a"),
                                new Tuple<GroupImportDefinition, GroupCompositionId>(
                                    GroupImportDefinition.CreateDefinition(
                                        "c",
                                        new GroupRegistrationId("c"),
                                        null,
                                        Enumerable.Empty<ImportRegistrationId>()),
                                    new GroupCompositionId()))
                    },
                    {
                        new GroupCompositionId(),
                        new Tuple<GroupDefinition, Tuple<GroupImportDefinition, GroupCompositionId>>(
                                new GroupDefinition("d"),
                                new Tuple<GroupImportDefinition, GroupCompositionId>(
                                    GroupImportDefinition.CreateDefinition(
                                        "e",
                                        new GroupRegistrationId("e"),
                                        null,
                                        Enumerable.Empty<ImportRegistrationId>()),
                                    new GroupCompositionId()))
                    },
                };
            var storage = new Mock<IStoreGroupsAndConnections>();
            {
                storage.Setup(s => s.Groups())
                    .Returns(groups.Keys);
                storage.Setup(s => s.Group(It.IsAny<GroupCompositionId>()))
                    .Returns<GroupCompositionId>(id => groups[id].Item1);
                storage.Setup(s => s.SatisfiedImports(It.IsAny<GroupCompositionId>()))
                    .Returns<GroupCompositionId>(id => new List<Tuple<GroupImportDefinition, GroupCompositionId>> { groups[id].Item2 });
            }

            var commands = new CompositionCommands(datasetLock.Object, storage.Object);
            var task = commands.CurrentState();
            var results = task.Result;
            Assert.That(
                results.Groups,
                Is.EquivalentTo(
                    groups.Select(p => new Tuple<GroupCompositionId, GroupDefinition>(p.Key, p.Value.Item1))));

            Assert.That(
                results.Connections,
                Is.EquivalentTo(
                    groups.Select(
                        p => new Tuple<GroupCompositionId, GroupImportDefinition, GroupCompositionId>(
                            p.Key,
                            p.Value.Item2.Item1,
                            p.Value.Item2.Item2))));

            datasetLock.Verify(d => d.LockForReading(), Times.Once());
            datasetLock.Verify(d => d.RemoveReadLock(It.IsAny<DatasetLockKey>()), Times.Once());
        }
    }
}
