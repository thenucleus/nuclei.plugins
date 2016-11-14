//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core.Assembly;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Core.NuGet
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PluginNuGetTypeLoaderTest
    {
        [Test]
        public void CreateWithNullConfiguration()
        {
            var installer = new Mock<IInstallPackages>();
            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            var fileSystem = new Mock<IFileSystem>();

            Assert.Throws<ArgumentNullException>(
                () => new PluginNuGetTypeLoader(
                    null,
                    installer.Object,
                    fileCopy,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null)));
        }

        [Test]
        public void CreateWithNullDiagnostics()
        {
            var configuration = new Mock<IConfiguration>();
            var installer = new Mock<IInstallPackages>();
            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            var fileSystem = new Mock<IFileSystem>();

            Assert.Throws<ArgumentNullException>(
                () => new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    fileCopy,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    null));
        }

        [Test]
        public void CreateWithNullFileCopy()
        {
            var configuration = new Mock<IConfiguration>();
            var installer = new Mock<IInstallPackages>();
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            var fileSystem = new Mock<IFileSystem>();

            Assert.Throws<ArgumentNullException>(
                () => new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    null,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null)));
        }

        [Test]
        public void CreateWithNullFileSystem()
        {
            var configuration = new Mock<IConfiguration>();
            var installer = new Mock<IInstallPackages>();
            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();

            Assert.Throws<ArgumentNullException>(
                () => new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    fileCopy,
                    resolutionPaths.Object,
                    null,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null)));
        }

        [Test]
        public void CreateWithNullPackageInstaller()
        {
            var configuration = new Mock<IConfiguration>();
            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            var fileSystem = new Mock<IFileSystem>();

            Assert.Throws<ArgumentNullException>(
                () => new PluginNuGetTypeLoader(
                    configuration.Object,
                    null,
                    fileCopy,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null)));
        }

        [Test]
        public void CreateWithNullResolutionPaths()
        {
            var configuration = new Mock<IConfiguration>();
            var installer = new Mock<IInstallPackages>();
            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();
            var fileSystem = new Mock<IFileSystem>();

            Assert.Throws<ArgumentNullException>(
                () => new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    fileCopy,
                    null,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null)));
        }

        [Test]
        public void Load()
        {
            var type = typeof(TypeLoader);
            var packageToLoad = new PackageIdentity("a", new NuGetVersion(1, 2, 3));
            var origin = new PluginNuGetOrigin(packageToLoad);

            var localInstallDirectory = @"c:\localinstall";
            var assemblyCacheDirectory = @"c:\assemblycache";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value(It.IsAny<ConfigurationKey<string>>()))
                    .Returns<ConfigurationKey<string>>(
                        k =>
                        {
                            if (k.Equals(CoreNuGetConfigurationKeys.LocalInstallLocation))
                            {
                                return localInstallDirectory;
                            }

                            return assemblyCacheDirectory;
                        });
            }

            PackageIdentity processedIdentity = null;
            string installDestination = null;
            var installer = new Mock<IInstallPackages>();
            {
                installer.Setup(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()))
                    .Callback<PackageIdentity, string, PackagePostInstall>(
                        (id, destination, postInstall) =>
                        {
                            processedIdentity = id;
                            installDestination = destination;
                        })
                    .Verifiable();
            }

            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();

            string resolutionPath = null;
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            {
                resolutionPaths.Setup(r => r.Add(It.IsAny<string>()))
                    .Callback<string>(s => resolutionPath = s)
                    .Verifiable();
                resolutionPaths.Setup(r => r.IsOnResolutionList(It.IsAny<string>()))
                    .Returns(false);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var loader = new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    fileCopy,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var loadedType = loader.Load(origin, type.AssemblyQualifiedName);
            Assert.AreEqual(type, loadedType);

            installer.Verify(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()), Times.Once());
            Assert.AreEqual(packageToLoad, processedIdentity);
            Assert.AreEqual(localInstallDirectory, installDestination);

            resolutionPaths.Verify(r => r.Add(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(
                Path.Combine(
                    assemblyCacheDirectory,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        packageToLoad.Id,
                        packageToLoad.Version)),
                resolutionPath);
        }

        [Test]
        public void LoadWithDirectoryCreateFailure()
        {
            var type = typeof(TypeLoader);
            var packageToLoad = new PackageIdentity("a", new NuGetVersion(1, 2, 3));
            var origin = new PluginNuGetOrigin(packageToLoad);

            var localInstallDirectory = @"c:\localinstall";
            var assemblyCacheDirectory = @"c:\assemblycache";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value(It.IsAny<ConfigurationKey<string>>()))
                    .Returns<ConfigurationKey<string>>(
                        k =>
                        {
                            if (k.Equals(CoreNuGetConfigurationKeys.LocalInstallLocation))
                            {
                                return localInstallDirectory;
                            }

                            return assemblyCacheDirectory;
                        });
            }

            var installer = new Mock<IInstallPackages>();
            {
                installer.Setup(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()))
                    .Verifiable();
            }

            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();

            string resolutionPath = null;
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            {
                resolutionPaths.Setup(r => r.Add(It.IsAny<string>()))
                    .Callback<string>(s => resolutionPath = s)
                    .Verifiable();
                resolutionPaths.Setup(r => r.IsOnResolutionList(It.IsAny<string>()))
                    .Returns(false);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0], true));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var loader = new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    fileCopy,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var loadedType = loader.Load(origin, type.AssemblyQualifiedName);
            Assert.AreEqual(type, loadedType);

            installer.Verify(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()), Times.Never());

            resolutionPaths.Verify(r => r.Add(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(
                Path.Combine(
                    assemblyCacheDirectory,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        packageToLoad.Id,
                        packageToLoad.Version)),
                resolutionPath);
        }

        [Test]
        public void LoadWithPackageInstallFailure()
        {
            var type = typeof(TypeLoader);
            var packageToLoad = new PackageIdentity("a", new NuGetVersion(1, 2, 3));
            var origin = new PluginNuGetOrigin(packageToLoad);

            var localInstallDirectory = @"c:\localinstall";
            var assemblyCacheDirectory = @"c:\assemblycache";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value(It.IsAny<ConfigurationKey<string>>()))
                    .Returns<ConfigurationKey<string>>(
                        k =>
                        {
                            if (k.Equals(CoreNuGetConfigurationKeys.LocalInstallLocation))
                            {
                                return localInstallDirectory;
                            }

                            return assemblyCacheDirectory;
                        });
            }

            var installer = new Mock<IInstallPackages>();
            {
                installer.Setup(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()))
                    .Throws<NuGetPackageInstallFailedException>()
                    .Verifiable();
            }

            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();

            string resolutionPath = null;
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            {
                resolutionPaths.Setup(r => r.Add(It.IsAny<string>()))
                    .Callback<string>(s => resolutionPath = s)
                    .Verifiable();
                resolutionPaths.Setup(r => r.IsOnResolutionList(It.IsAny<string>()))
                    .Returns(false);
            }

            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new string[0]));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var loader = new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    fileCopy,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var loadedType = loader.Load(origin, type.AssemblyQualifiedName);
            Assert.AreEqual(type, loadedType);

            installer.Verify(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()), Times.Once());

            resolutionPaths.Verify(r => r.Add(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(
                Path.Combine(
                    assemblyCacheDirectory,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        packageToLoad.Id,
                        packageToLoad.Version)),
                resolutionPath);
        }

        [Test]
        public void LoadWithPreInstalledPackage()
        {
            var type = typeof(TypeLoader);
            var packageToLoad = new PackageIdentity("a", new NuGetVersion(1, 2, 3));
            var origin = new PluginNuGetOrigin(packageToLoad);

            var localInstallDirectory = @"c:\localinstall";
            var assemblyCacheDirectory = @"c:\assemblycache";
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(true);
                configuration.Setup(c => c.Value(It.IsAny<ConfigurationKey<string>>()))
                    .Returns<ConfigurationKey<string>>(
                        k =>
                        {
                            if (k.Equals(CoreNuGetConfigurationKeys.LocalInstallLocation))
                            {
                                return localInstallDirectory;
                            }

                            return assemblyCacheDirectory;
                        });
            }

            var installer = new Mock<IInstallPackages>();
            {
                installer.Setup(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()))
                    .Verifiable();
            }

            CopyPackageFiles fileCopy = (id, pattern, source, destination) => new List<string>();

            string resolutionPath = null;
            var resolutionPaths = new Mock<IProvideAssemblyResolutionPaths>();
            {
                resolutionPaths.Setup(r => r.Add(It.IsAny<string>()))
                    .Callback<string>(s => resolutionPath = s)
                    .Verifiable();
                resolutionPaths.Setup(r => r.IsOnResolutionList(It.IsAny<string>()))
                    .Returns(false);
            }

            var expectedCacheLocation = Path.Combine(
                assemblyCacheDirectory,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}",
                    packageToLoad.Id,
                    packageToLoad.Version));
            var fileSystem = new Mock<IFileSystem>();
            {
                fileSystem.Setup(f => f.Directory)
                    .Returns(new MockDirectory(new[] { expectedCacheLocation }));
                fileSystem.Setup(f => f.Path)
                    .Returns(new MockPath());
            }

            var loader = new PluginNuGetTypeLoader(
                    configuration.Object,
                    installer.Object,
                    fileCopy,
                    resolutionPaths.Object,
                    fileSystem.Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var loadedType = loader.Load(origin, type.AssemblyQualifiedName);
            Assert.AreEqual(type, loadedType);

            installer.Verify(i => i.Install(It.IsAny<PackageIdentity>(), It.IsAny<string>(), It.IsAny<PackagePostInstall>()), Times.Never());

            resolutionPaths.Verify(r => r.Add(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(expectedCacheLocation, resolutionPath);
        }

        [Test]
        public void LoadWithInvalidOriginType()
        {
            var type = typeof(TypeLoader);
            var origin = new PluginAssemblyOrigin(@"c:\temp\myassembly.dll");

            var loader = new PluginNuGetTypeLoader(
                new Mock<IConfiguration>().Object,
                new Mock<IInstallPackages>().Object,
                (id, pattern, source, destination) => new List<string>(),
                new Mock<IProvideAssemblyResolutionPaths>().Object,
                new Mock<IFileSystem>().Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));
            Assert.Throws<InvalidPluginOriginException>(() => loader.Load(origin, type.AssemblyQualifiedName));
        }

        [Test]
        public void LoadWithNullOrigin()
        {
            var type = typeof(TypeLoader);

            var loader = new PluginNuGetTypeLoader(
                new Mock<IConfiguration>().Object,
                new Mock<IInstallPackages>().Object,
                (id, pattern, source, destination) => new List<string>(),
                new Mock<IProvideAssemblyResolutionPaths>().Object,
                new Mock<IFileSystem>().Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));
            Assert.Throws<InvalidPluginOriginException>(() => loader.Load(null, type.AssemblyQualifiedName));
        }

        [Test]
        public void ValidOriginType()
        {
            var loader = new PluginNuGetTypeLoader(
                new Mock<IConfiguration>().Object,
                new Mock<IInstallPackages>().Object,
                (id, pattern, source, destination) => new List<string>(),
                new Mock<IProvideAssemblyResolutionPaths>().Object,
                new Mock<IFileSystem>().Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));
            Assert.AreEqual(typeof(PluginNuGetOrigin), loader.ValidOriginType);
        }
    }
}
