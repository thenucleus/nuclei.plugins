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
using System.Reflection;
using System.Text;
using Moq;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core.NuGet;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Core.v2;
using NuGet.Protocol.Core.v3;
using NuGet.Protocol.Core.v3.LocalRepositories;
using NuGet.Versioning;
using NUnit.Framework;

namespace Test.Unit.Plugins.Core.NuGet
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NuGetPackageInstallerTest
    {
        [Test]
        public void Create()
        {
            var installer = new NuGetPackageInstaller(
                new Mock<IConfiguration>().Object,
                new Mock<ISourceRepositoryProvider>().Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            Assert.IsNotNull(installer);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.NuGet.NuGetPackageInstaller",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullConfiguration()
        {
            Assert.Throws<ArgumentNullException>(
                () => new NuGetPackageInstaller(
                    null,
                    new Mock<ISourceRepositoryProvider>().Object,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null)));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.NuGet.NuGetPackageInstaller",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullDiagnostics()
        {
            Assert.Throws<ArgumentNullException>(
                () => new NuGetPackageInstaller(
                    new Mock<IConfiguration>().Object,
                    new Mock<ISourceRepositoryProvider>().Object,
                    null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.NuGet.NuGetPackageInstaller",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullRepositoryProvider()
        {
            Assert.Throws<ArgumentNullException>(
                () => new NuGetPackageInstaller(
                    new Mock<IConfiguration>().Object,
                    null,
                    new SystemDiagnostics(new Mock<ILogger>().Object, null)));
        }

        [Test]
        public void Install()
        {
            var outputDirectory = Path.Combine(
                Assembly.GetExecutingAssembly().LocalDirectoryPath(),
                "nuget",
                "install_test");
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(false);
            }

            var nugetSettings = Settings.LoadDefaultSettings(
                Assembly.GetExecutingAssembly().LocalDirectoryPath(),
                configFileName: null,
                machineWideSettings: null);

            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            providers.AddRange(Repository.Provider.GetCoreV2());  // Add v2 API support
            var repositoryProvider = new SourceRepositoryProvider(
                nugetSettings,
                providers);

            var installer = new NuGetPackageInstaller(
               configuration.Object,
               repositoryProvider,
               new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var packageName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.TestPackage",
                Assembly.GetExecutingAssembly().GetName().Name);
            var packageVersion = new NuGetVersion(1, 2, 3);
            installer.Install(
                new PackageIdentity(packageName, packageVersion),
                outputDirectory);

            Assert.IsTrue(Directory.Exists(outputDirectory));

            var expectedPackageDirectory = Path.Combine(
                outputDirectory,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}",
                    packageName,
                    packageVersion));
            Assert.IsTrue(Directory.Exists(expectedPackageDirectory));
            Assert.IsTrue(
                File.Exists(
                    Path.Combine(
                        expectedPackageDirectory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.{1}.nupkg",
                            packageName,
                            packageVersion))));

            var libDirectory = Path.Combine(expectedPackageDirectory, "lib", "net452");
            Assert.IsTrue(
                File.Exists(
                    Path.Combine(
                        libDirectory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.dll",
                            Assembly.GetExecutingAssembly().GetName().Name))));
        }

        [Test]
        public void InstallWithEmptyOutputPath()
        {
            var installer = new NuGetPackageInstaller(
                new Mock<IConfiguration>().Object,
                new Mock<ISourceRepositoryProvider>().Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            Assert.Throws<ArgumentException>(
                () => installer.Install(
                    new PackageIdentity("a", new NuGetVersion(1, 2, 3)),
                    string.Empty));
        }

        [Test]
        public void InstallWithNullIdentity()
        {
            var installer = new NuGetPackageInstaller(
                new Mock<IConfiguration>().Object,
                new Mock<ISourceRepositoryProvider>().Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            Assert.Throws<ArgumentNullException>(() => installer.Install(null, @"c:\temp"));
        }

        [Test]
        public void InstallWithNullOutputPath()
        {
            var installer = new NuGetPackageInstaller(
                new Mock<IConfiguration>().Object,
                new Mock<ISourceRepositoryProvider>().Object,
                new SystemDiagnostics(new Mock<ILogger>().Object, null));

            Assert.Throws<ArgumentNullException>(
                () => installer.Install(
                    new PackageIdentity("a", new NuGetVersion(1, 2, 3)),
                    null));
        }

        [Test]
        public void InstallWithPostInstallFunction()
        {
            var outputDirectory = Path.Combine(
                Assembly.GetExecutingAssembly().LocalDirectoryPath(),
                "nuget",
                "install_test");
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKeyBase>()))
                    .Returns(false);
            }

            var nugetSettings = Settings.LoadDefaultSettings(
                Assembly.GetExecutingAssembly().LocalDirectoryPath(),
                configFileName: null,
                machineWideSettings: null);

            List<Lazy<INuGetResourceProvider>> providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());  // Add v3 API support
            providers.AddRange(Repository.Provider.GetCoreV2());  // Add v2 API support
            var repositoryProvider = new SourceRepositoryProvider(
                nugetSettings,
                providers);

            var installer = new NuGetPackageInstaller(
               configuration.Object,
               repositoryProvider,
               new SystemDiagnostics(new Mock<ILogger>().Object, null));

            var packageName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.TestPackage",
                Assembly.GetExecutingAssembly().GetName().Name);
            var packageVersion = new NuGetVersion(1, 2, 3);

            var nugetRootInstallDirectory = string.Empty;
            var nugetPackageInstallDirectory = string.Empty;
            PackageIdentity installedPackageId = null;

            var id = new PackageIdentity(packageName, packageVersion);
            installer.Install(
                id,
                outputDirectory,
                (rootDirectory, installDirectory, packageId) =>
                {
                    nugetRootInstallDirectory = rootDirectory;
                    nugetPackageInstallDirectory = installDirectory;
                    installedPackageId = packageId;
                });

            Assert.IsTrue(Directory.Exists(outputDirectory));
            Assert.AreEqual(outputDirectory, nugetRootInstallDirectory);

            var expectedPackageDirectory = Path.Combine(
                outputDirectory,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}",
                    packageName,
                    packageVersion));
            Assert.IsTrue(Directory.Exists(expectedPackageDirectory));
            Assert.AreEqual(expectedPackageDirectory, nugetPackageInstallDirectory);

            Assert.IsTrue(
                File.Exists(
                    Path.Combine(
                        expectedPackageDirectory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.{1}.nupkg",
                            packageName,
                            packageVersion))));
            Assert.AreEqual(id, installedPackageId);

            var libDirectory = Path.Combine(expectedPackageDirectory, "lib", "net452");
            Assert.IsTrue(
                File.Exists(
                    Path.Combine(
                        libDirectory,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}.dll",
                            Assembly.GetExecutingAssembly().GetName().Name))));
        }

        [SetUp]
        public void Setup()
        {
            var content =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageRestore>
    <add key=""enabled"" value=""True"" />
  </packageRestore>
  <config>
    <add key=""repositorypath"" value=""packages"" />
  </config>
  <packageSources>
    <clear />
    <add key = ""Test"" value=""{0}"" />
  </packageSources>
  <disabledPackageSources>
    <add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" protocolVersion=""3"" />
    <add key=""NuGet official package source"" value=""https://nuget.org/api/v2/"" />
  </disabledPackageSources>
</configuration>
";

            var outputDirectory = Assembly.GetExecutingAssembly().LocalDirectoryPath();
            using (var writer = new StreamWriter(Path.Combine(outputDirectory, "nuget.config"), false, Encoding.UTF8))
            {
                writer.Write(string.Format(CultureInfo.InvariantCulture, content, outputDirectory));
            }
        }
    }
}
