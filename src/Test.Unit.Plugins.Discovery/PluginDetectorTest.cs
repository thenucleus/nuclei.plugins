//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PluginDetectorTest
    {
        private sealed class MockScanner : IAssemblyScanner
        {
            private IEnumerable<string> _files;

            public void Scan(IEnumerable<string> assemblyFilesToScan)
            {
                _files = assemblyFilesToScan;
            }

            public IEnumerable<string> FilesToScan
            {
                get
                {
                    return _files;
                }
            }
        }

        [Test]
        public void SearchWithDeletedFilesOnly()
        {
            var pluginFiles = new List<PluginFileOrigin>
                {
                    new PluginFileOrigin(@"c:\temp\foobar.dll", DateTimeOffset.Now),
                    new PluginFileOrigin(@"c:\temp\foobar2.dll", DateTimeOffset.Now.AddHours(-2)),
                };

            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(pluginFiles);
            }

            var mockFile = new MockFile(new Dictionary<string, string>());
            var mockDirectory = new MockDirectory(new List<string>());
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
                fileSystem.Setup(f => f.Directory)
                    .Returns(mockDirectory);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.IsNull(scanner.FilesToScan);
        }

        [Test]
        public void SearchWithKnownFilesOnly()
        {
            var files = new List<string>
                        {
                            @"c:\temp\foobar.dll",
                            @"c:\temp\foobar2.dll"
                        };

            var pluginFiles = new List<PluginFileOrigin>
                {
                    new PluginFileOrigin(files[0], DateTimeOffset.Now.AddHours(-2)),
                    new PluginFileOrigin(files[1], DateTimeOffset.Now.AddHours(-2)),
                };

            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(pluginFiles);
            }

            var mockFile = new MockFile(files.ToDictionary(f => f, f => string.Empty));
            var mockDirectory = new MockDirectory(files);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
                fileSystem.Setup(f => f.Directory)
                    .Returns(mockDirectory);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.That(scanner.FilesToScan, Is.EquivalentTo(files));
        }

        [Test]
        public void SearchWithNewFilesOnly()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(Enumerable.Empty<PluginFileOrigin>());
            }

            var files = new List<string>
                        {
                            @"c:\temp\foobar.dll",
                            @"c:\temp\foobar2.dll"
                        };
            var mockFile = new MockFile(files.ToDictionary(f => f, f => string.Empty));
            var mockDirectory = new MockDirectory(files);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
                fileSystem.Setup(f => f.Directory)
                    .Returns(mockDirectory);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.That(scanner.FilesToScan, Is.EquivalentTo(files));
        }

        [Test]
        public void SearchWithUpdatedFilesOnly()
        {
            var files = new List<string>
                        {
                            @"c:\temp\foobar.dll",
                            @"c:\temp\foobar2.dll"
                        };

            var pluginFiles = new List<PluginFileOrigin>
                {
                    new PluginFileOrigin(files[0], DateTimeOffset.Now),
                    new PluginFileOrigin(files[1], DateTimeOffset.Now.AddHours(-2)),
                };

            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginFiles())
                    .Returns(pluginFiles);
            }

            var mockFile = new MockFile(files.ToDictionary(f => f, f => string.Empty));
            var mockDirectory = new MockDirectory(files);
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(mockFile);
                fileSystem.Setup(f => f.Directory)
                    .Returns(mockDirectory);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new PluginDetector(
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            detector.SearchDirectory(@"c:\temp");
            Assert.That(scanner.FilesToScan, Is.EquivalentTo(new List<string> { files[1] }));
        }
    }
}
