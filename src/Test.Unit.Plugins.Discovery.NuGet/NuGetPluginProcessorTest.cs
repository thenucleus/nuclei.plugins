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
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Core.NuGet;
using Nuclei.Plugins.Discovery.Container;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Discovery.NuGet
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NuGetPluginProcessorTest
    {
        private sealed class MockScanner : IAssemblyScanner
        {
            private IDictionary<string, PluginOrigin> _files;

            public void Scan(IDictionary<string, PluginOrigin> assemblyFilesToScan)
            {
                _files = assemblyFilesToScan;
            }

            public IDictionary<string, PluginOrigin> FilesToScan
            {
                get
                {
                    return _files;
                }
            }
        }

        [Test]
        public void Added()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(false);
            }

            var packageInstaller = new Mock<IInstallPackages>();
            {
                packageInstaller.Setup(p => p.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()))
                    .Callback<PackageIdentity, string, PackagePostInstall>((id, p, install) => install("a", "b", id));
            }

            var binaries = new List<string>
                {
                    "a.dll",
                    "b.dll",
                    "c.txt",
                };
            CopyPackageFiles copier = (id, pattern, source, destination) => binaries;

            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginOrigins())
                    .Returns(Enumerable.Empty<PluginNuGetOrigin>());
            }

            var packages = new List<PluginNuGetOrigin>
                {
                    new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3))),
                };

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new List<string>()));
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(new Dictionary<string, string>()));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var detector = new NuGetPluginProcessor(
                configuration.Object,
                packageInstaller.Object,
                copier,
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var origins = packages.Cast<PluginOrigin>().ToArray();
            detector.Added(origins.ToArray());
            Assert.That(scanner.FilesToScan.Keys, Is.EquivalentTo(binaries.Take(2)));
        }

        [Test]
        public void Removed()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(false);
            }

            var packageInstaller = new Mock<IInstallPackages>();
            CopyPackageFiles copier = (id, pattern, source, destination) => new List<string>();

            IEnumerable<PluginOrigin> removedPlugins = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.RemovePlugins(It.IsAny<IEnumerable<PluginOrigin>>()))
                    .Callback<IEnumerable<PluginOrigin>>(i => removedPlugins = i);
            }

            var packages = new List<PluginNuGetOrigin>
                {
                    new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3))),
                };

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.File)
                    .Returns(new MockFile(new Dictionary<string, string>()));
            }

            var detector = new NuGetPluginProcessor(
                configuration.Object,
                packageInstaller.Object,
                copier,
                repository.Object,
                scannerBuilder,
                fileSystem.Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var origins = packages.Cast<PluginOrigin>().ToArray();
            detector.Removed(origins);
            Assert.IsNotNull(removedPlugins);
            Assert.AreSame(origins, removedPlugins);
        }
    }
}
