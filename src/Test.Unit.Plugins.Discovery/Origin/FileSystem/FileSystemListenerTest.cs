//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Core.Assembly;
using Nuclei.Plugins.Discovery.Assembly;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Discovery.Origin.FileSystem
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class FileSystemListenerTest
    {
        [Test]
        public void Disable()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            var scanner = new Mock<IProcessPluginOriginChanges>();

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();
            {
                fileSystemWatcher.SetupAllProperties();
                fileSystemWatcher.Object.EnableRaisingEvents = true;
            }

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            Assert.AreEqual(path, fileSystemWatcher.Object.Path);
            Assert.IsFalse(fileSystemWatcher.Object.EnableRaisingEvents);
            Assert.IsTrue(fileSystemWatcher.Object.IncludeSubdirectories);
            Assert.AreEqual(
                NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                fileSystemWatcher.Object.NotifyFilter);

            fileSystemWatcher.Object.EnableRaisingEvents = true;
            listener.Disable();
            Assert.IsFalse(fileSystemWatcher.Object.EnableRaisingEvents);
        }

        [Test]
        public void EnableWithFilesMatchingMultipleScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] plugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => plugins1 = p)
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            PluginOrigin[] plugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => plugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();
            {
                fileSystemWatcher.SetupAllProperties();
                fileSystemWatcher.Object.EnableRaisingEvents = true;
            }

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFilePath = Path.Combine(path, "file.dll");
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[] { pluginFilePath }));
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            Assert.AreEqual(path, fileSystemWatcher.Object.Path);
            Assert.IsFalse(fileSystemWatcher.Object.EnableRaisingEvents);

            listener.Enable();
            Assert.IsTrue(fileSystemWatcher.Object.EnableRaisingEvents);
            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(plugins1);
            Assert.AreEqual(1, plugins1.Length);

            var origin = plugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(plugins2);
            Assert.AreEqual(1, plugins2.Length);

            origin = plugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        public void EnableWithFilesMatchingScanner()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] plugins = null;
            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => plugins = p)
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();
            {
                fileSystemWatcher.SetupAllProperties();
                fileSystemWatcher.Object.EnableRaisingEvents = true;
            }

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFilePath = Path.Combine(path, "file.dll");
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[] { pluginFilePath }));
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            Assert.AreEqual(path, fileSystemWatcher.Object.Path);
            Assert.IsFalse(fileSystemWatcher.Object.EnableRaisingEvents);

            listener.Enable();
            Assert.IsTrue(fileSystemWatcher.Object.EnableRaisingEvents);
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(plugins);
            Assert.AreEqual(1, plugins.Length);

            var origin = plugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        public void EnableWithFilesNotMatchingScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".txt") })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();
            {
                fileSystemWatcher.SetupAllProperties();
                fileSystemWatcher.Object.EnableRaisingEvents = true;
            }

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[] { Path.Combine(path, "file.dll") }));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            Assert.AreEqual(path, fileSystemWatcher.Object.Path);
            Assert.IsFalse(fileSystemWatcher.Object.EnableRaisingEvents);

            listener.Enable();
            Assert.IsTrue(fileSystemWatcher.Object.EnableRaisingEvents);
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());
        }

        [Test]
        public void EnableWithScannerThrowing()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] plugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => plugins1 = p)
                    .Throws<ArgumentException>()
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            PluginOrigin[] plugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => plugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();
            {
                fileSystemWatcher.SetupAllProperties();
                fileSystemWatcher.Object.EnableRaisingEvents = true;
            }

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFilePath = Path.Combine(path, "file.dll");
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[] { pluginFilePath }));
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            Assert.AreEqual(path, fileSystemWatcher.Object.Path);
            Assert.IsFalse(fileSystemWatcher.Object.EnableRaisingEvents);

            listener.Enable();
            Assert.IsTrue(fileSystemWatcher.Object.EnableRaisingEvents);
            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(plugins1);
            Assert.AreEqual(1, plugins1.Length);

            var origin = plugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(plugins2);
            Assert.AreEqual(1, plugins2.Length);

            origin = plugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        public void EnableWithoutFiles()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[0])
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();
            {
                fileSystemWatcher.SetupAllProperties();
                fileSystemWatcher.Object.EnableRaisingEvents = true;
            }

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            Assert.AreEqual(path, fileSystemWatcher.Object.Path);
            Assert.IsFalse(fileSystemWatcher.Object.EnableRaisingEvents);

            listener.Enable();
            Assert.IsTrue(fileSystemWatcher.Object.EnableRaisingEvents);
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Never());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileChangedWithFilesMatchingMultipleScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins1 = null;
            PluginOrigin[] removedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins1 = p)
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins1 = p)
                    .Verifiable();
            }

            PluginOrigin[] addedPlugins2 = null;
            PluginOrigin[] removedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins2 = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Changed += null,
                new FileSystemEventArgs(WatcherChangeTypes.Changed, path, pluginFile));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins1);
            Assert.AreEqual(1, addedPlugins1.Length);

            var origin = addedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins1);
            Assert.AreEqual(1, removedPlugins1.Length);

            origin = removedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins2);
            Assert.AreEqual(1, addedPlugins2.Length);

            origin = addedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins2);
            Assert.AreEqual(1, removedPlugins2.Length);

            origin = removedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileChangedWithFilesMatchingScanner()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins = null;
            PluginOrigin[] removedPlugins = null;
            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins = p)
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Changed += null,
                new FileSystemEventArgs(WatcherChangeTypes.Changed, path, pluginFile));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins);
            Assert.AreEqual(1, addedPlugins.Length);

            var origin = addedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins);
            Assert.AreEqual(1, removedPlugins.Length);

            origin = removedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileChangedWithFilesNotMatchingScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".txt") })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Changed += null,
                new FileSystemEventArgs(WatcherChangeTypes.Changed, path, pluginFile));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileChangedWithScannerThrowing()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins1 = null;
            PluginOrigin[] removedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins1 = p)
                    .Throws<ArgumentException>()
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins1 = p)
                    .Verifiable();
            }

            PluginOrigin[] addedPlugins2 = null;
            PluginOrigin[] removedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins2 = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Changed += null,
                new FileSystemEventArgs(WatcherChangeTypes.Changed, path, pluginFile));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins1);
            Assert.AreEqual(1, addedPlugins1.Length);

            var origin = addedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins1);
            Assert.AreEqual(1, removedPlugins1.Length);

            origin = removedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins2);
            Assert.AreEqual(1, addedPlugins2.Length);

            origin = addedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins2);
            Assert.AreEqual(1, removedPlugins2.Length);

            origin = removedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileCreatedWithFilesMatchingMultipleScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins1 = p)
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            PluginOrigin[] addedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, path, pluginFile));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(addedPlugins1);
            Assert.AreEqual(1, addedPlugins1.Length);

            var origin = addedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(addedPlugins2);
            Assert.AreEqual(1, addedPlugins2.Length);

            origin = addedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileCreatedWithFilesMatchingScanner()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins = null;
            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins = p)
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, path, pluginFile));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(addedPlugins);
            Assert.AreEqual(1, addedPlugins.Length);

            var origin = addedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileCreatedWithFilesNotMatchingScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".txt") })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, path, pluginFile));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileCreatedWithScannerThrowing()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins1 = p)
                    .Throws<ArgumentException>()
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            PluginOrigin[] addedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Created += null,
                new FileSystemEventArgs(WatcherChangeTypes.Created, path, pluginFile));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(addedPlugins1);
            Assert.AreEqual(1, addedPlugins1.Length);

            var origin = addedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(addedPlugins2);
            Assert.AreEqual(1, addedPlugins2.Length);

            origin = addedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileDeletedWithFilesMatchingMultipleScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] removedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins1 = p)
                    .Verifiable();
            }

            PluginOrigin[] removedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins2 = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Deleted += null,
                new FileSystemEventArgs(WatcherChangeTypes.Deleted, path, pluginFile));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(removedPlugins1);
            Assert.AreEqual(1, removedPlugins1.Length);

            var origin = removedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(removedPlugins2);
            Assert.AreEqual(1, removedPlugins2.Length);

            origin = removedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileDeletedWithFilesMatchingScanner()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins = null;
            PluginOrigin[] removedPlugins = null;
            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins = p)
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Deleted += null,
                new FileSystemEventArgs(WatcherChangeTypes.Deleted, path, pluginFile));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(removedPlugins);
            Assert.AreEqual(1, removedPlugins.Length);

            var origin = removedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileDeletedWithFilesNotMatchingScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".txt") })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Deleted += null,
                new FileSystemEventArgs(WatcherChangeTypes.Deleted, path, pluginFile));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileDeletedWithScannerThrowing()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] removedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Throws<ArgumentException>()
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins1 = p)
                    .Verifiable();
            }

            PluginOrigin[] removedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins2 = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Deleted += null,
                new FileSystemEventArgs(WatcherChangeTypes.Deleted, path, pluginFile));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(removedPlugins1);
            Assert.AreEqual(1, removedPlugins1.Length);

            var origin = removedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Once());
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(removedPlugins2);
            Assert.AreEqual(1, removedPlugins2.Length);

            origin = removedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileRenamedWithFilesMatchingMultipleScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins1 = null;
            PluginOrigin[] removedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins1 = p)
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins1 = p)
                    .Verifiable();
            }

            PluginOrigin[] addedPlugins2 = null;
            PluginOrigin[] removedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins2 = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);

            var oldFileName = "old.dll";
            var oldFilePath = Path.Combine(path, oldFileName);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Renamed += null,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, path, pluginFile, oldFileName));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins1);
            Assert.AreEqual(1, addedPlugins1.Length);

            var origin = addedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins1);
            Assert.AreEqual(1, removedPlugins1.Length);

            origin = removedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(oldFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins2);
            Assert.AreEqual(1, addedPlugins2.Length);

            origin = addedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins2);
            Assert.AreEqual(1, removedPlugins2.Length);

            origin = removedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(oldFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileRenamedWithFilesMatchingScanner()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins = null;
            PluginOrigin[] removedPlugins = null;
            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins = p)
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);

            var oldFileName = "old.dll";
            var oldFilePath = Path.Combine(path, oldFileName);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Renamed += null,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, path, pluginFile, oldFileName));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins);
            Assert.AreEqual(1, addedPlugins.Length);

            var origin = addedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins);
            Assert.AreEqual(1, removedPlugins.Length);

            origin = removedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(oldFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileRenamedWithFilesNotMatchingScanners()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".txt") })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);

            var oldFileName = "old.dll";
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Renamed += null,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, path, pluginFile, oldFileName));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileRenamedWithOldFileMatchingScanner()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins = null;
            PluginOrigin[] removedPlugins = null;
            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins = p)
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.txt";
            var pluginFilePath = Path.Combine(path, pluginFile);

            var oldFileName = "old.dll";
            var oldFilePath = Path.Combine(path, oldFileName);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Renamed += null,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, path, pluginFile, oldFileName));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Never());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(removedPlugins);
            Assert.AreEqual(1, removedPlugins.Length);

            var origin = removedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(oldFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileRenamedWithNewFileMatchingScanner()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins = null;
            var scanner = new Mock<IProcessPluginOriginChanges>();
            {
                scanner.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins = p)
                    .Verifiable();
                scanner.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);

            var oldFileName = "old.txt";
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Renamed += null,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, path, pluginFile, oldFileName));
            scanner.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Never());

            Assert.IsNotNull(addedPlugins);
            Assert.AreEqual(1, addedPlugins.Length);

            var origin = addedPlugins[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1804:RemoveUnusedLocals",
            MessageId = "listener",
            Justification = "Cannot remove the object under tests. Object is used through the event handlers.")]
        public void FileRenamedWithScannerThrowing()
        {
            var path = @"c:\this\is\a\fake\directory";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value<string[]>(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(new[] { path });
            }

            PluginOrigin[] addedPlugins1 = null;
            PluginOrigin[] removedPlugins1 = null;
            var scanner1 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner1.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner1.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins1 = p)
                    .Throws<ArgumentException>()
                    .Verifiable();
                scanner1.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins1 = p)
                    .Verifiable();
            }

            PluginOrigin[] addedPlugins2 = null;
            PluginOrigin[] removedPlugins2 = null;
            var scanner2 = new Mock<IProcessPluginOriginChanges>();
            {
                scanner2.Setup(s => s.AcceptedPluginTypes)
                    .Returns(new IPluginType[] { new FilePluginType(".dll", s => new PluginAssemblyOrigin(s)) })
                    .Verifiable();
                scanner2.Setup(s => s.Added(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => addedPlugins2 = p)
                    .Verifiable();
                scanner2.Setup(s => s.Removed(It.IsAny<PluginOrigin[]>()))
                    .Callback<PluginOrigin[]>(p => removedPlugins2 = p)
                    .Verifiable();
            }

            var fileSystemWatcher = new Mock<IFileSystemWatcher>();

            Func<IFileSystemWatcher> watcherBuilder = () => fileSystemWatcher.Object;

            var pluginFile = "file.dll";
            var pluginFilePath = Path.Combine(path, pluginFile);

            var oldFileName = "old.dll";
            var oldFilePath = Path.Combine(path, oldFileName);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(pluginFilePath, "Stuff"));
                fileSystem.Setup(f => f.Path)
                    .Returns(new PathWrapper());
            }

            var listener = new FileSystemListener(
                configuration.Object,
                new[] { scanner1.Object, scanner2.Object },
                watcherBuilder,
                new SystemDiagnostics(new Mock<ILogger>().Object, null),
                fileSystem.Object);

            fileSystemWatcher.Raise(
                f => f.Renamed += null,
                new RenamedEventArgs(WatcherChangeTypes.Renamed, path, pluginFile, oldFileName));

            scanner1.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner1.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner1.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins1);
            Assert.AreEqual(1, addedPlugins1.Length);

            var origin = addedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins1);
            Assert.AreEqual(1, removedPlugins1.Length);

            origin = removedPlugins1[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(oldFilePath, origin.FilePath);

            scanner2.Verify(s => s.AcceptedPluginTypes, Times.Exactly(2));
            scanner2.Verify(s => s.Added(It.IsAny<PluginOrigin[]>()), Times.Once());
            scanner2.Verify(s => s.Removed(It.IsAny<PluginOrigin[]>()), Times.Once());

            Assert.IsNotNull(addedPlugins2);
            Assert.AreEqual(1, addedPlugins2.Length);

            origin = addedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(pluginFilePath, origin.FilePath);

            Assert.IsNotNull(removedPlugins2);
            Assert.AreEqual(1, removedPlugins2.Length);

            origin = removedPlugins2[0] as PluginAssemblyOrigin;
            Assert.IsNotNull(origin);
            Assert.AreEqual(oldFilePath, origin.FilePath);
        }
    }
}
