//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core.NuGet.Properties;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;

using NuGetConfiguration = NuGet.Configuration;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Provides methods for installing NuGet packages in a given directory.
    /// </summary>
    public sealed class NuGetPackageInstaller : IInstallPackages
    {
        private static NuGetConfiguration.PackageSource ResolveSource(IEnumerable<NuGetConfiguration.PackageSource> availableSources, string source)
        {
            var resolvedSource = availableSources.FirstOrDefault(
                f => f.Source.Equals(source, StringComparison.OrdinalIgnoreCase) ||
                    f.Name.Equals(source, StringComparison.OrdinalIgnoreCase));

            if (resolvedSource == null)
            {
                Uri result = null;
                if (!Uri.TryCreate(source, UriKind.Absolute, out result))
                {
                    return null;
                }

                return new NuGetConfiguration.PackageSource(source);
            }
            else
            {
                return resolvedSource;
            }
        }

        /// <summary>
        /// Provides the configuration for the application.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// Abstracts the file system.
        /// </summary>
        private readonly System.IO.Abstractions.IFileSystem _fileSystem;

        /// <summary>
        /// Provides NuGet repositories for a given source.
        /// </summary>
        private readonly ISourceRepositoryProvider _nugetRepositoryProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstaller"/> class.
        /// </summary>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <param name="nugetRepositoryProvider">The object that provides repositories for a given source.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="nugetRepositoryProvider"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public NuGetPackageInstaller(
            IConfiguration configuration,
            ISourceRepositoryProvider nugetRepositoryProvider,
            SystemDiagnostics diagnostics,
            System.IO.Abstractions.IFileSystem fileSystem)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (nugetRepositoryProvider == null)
            {
                throw new ArgumentNullException("nugetRepositoryProvider");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            _configuration = configuration;
            _diagnostics = diagnostics;
            _fileSystem = fileSystem;
            _nugetRepositoryProvider = nugetRepositoryProvider;
        }

        private IReadOnlyCollection<NuGetConfiguration.PackageSource> GetPackageSources(NuGetConfiguration.ISettings settings)
        {
            var sourceProvider = new NuGetConfiguration.PackageSourceProvider(settings);
            var availableSources = sourceProvider.LoadPackageSources().Where(source => source.IsEnabled);
            var packageSources = new List<NuGetConfiguration.PackageSource>();

            var configuredSources = _configuration.HasValueFor(CoreNuGetConfigurationKeys.NugetFeeds)
                ? _configuration.Value(CoreNuGetConfigurationKeys.NugetFeeds)
                : new string[0];

            foreach (var source in configuredSources)
            {
                var resolvedSource = ResolveSource(availableSources, source);
                if (resolvedSource != null)
                {
                    packageSources.Add(resolvedSource);
                }
            }

            if (configuredSources.Length == 0)
            {
                packageSources.AddRange(availableSources);
            }

            return packageSources;
        }

        /// <summary>
        /// Installs a given version of a package and its dependencies.
        /// </summary>
        /// <param name="name">The ID of the package.</param>
        /// <param name="outputLocation">The full path of the directory where the packages should be installed.</param>
        /// <param name="postInstallAction">An action that is run after each package is installed.</param>
        public void Install(
            PackageIdentity name,
            string outputLocation,
            PackagePostInstall postInstallAction = null)
        {
            var nugetProject = new PluginNuGetProject(
                outputLocation,
                new PackagePathResolver(outputLocation),
                postInstallAction);
            if (nugetProject.PackageExists(name))
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.LogMessage_NuGetPackageInstaller_PackageExists_WithPackageId,
                        name));
                return;
            }

            var nugetSettings = NuGetConfiguration.Settings.LoadDefaultSettings(
                Assembly.GetExecutingAssembly().LocalDirectoryPath(),
                configFileName: null,
                machineWideSettings: null);

            var packageManager = new NuGetPackageManager(_nugetRepositoryProvider, nugetSettings, outputLocation);

            var packageSources = GetPackageSources(nugetSettings);
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.LogMessage_NuGetPackageInstaller_InstallingPackageFromSources_WithPackageIdAndSources,
                    name,
                    string.Join(Environment.NewLine, packageSources.Select(p => p.Source).ToArray())));
            var primaryRepositories = packageSources.Select(_nugetRepositoryProvider.CreateRepository);

            var resolutionContext = new ResolutionContext(
                DependencyBehavior.Lowest,
                includePrelease: false,
                includeUnlisted: true,
                versionConstraints: VersionConstraints.None);

            var projectContext = new NuGetPackageInstallerContext(_diagnostics)
            {
                PackageExtractionContext = new PackageExtractionContext()
            };

            using (var cacheContext = new SourceCacheContext())
            {
                cacheContext.NoCache = false;

                packageManager.InstallPackageAsync(
                    nugetProject,
                    name,
                    resolutionContext,
                    projectContext,
                    primaryRepositories,
                    Enumerable.Empty<SourceRepository>(),
                    CancellationToken.None)
                    .Wait();
            }
        }
    }
}
