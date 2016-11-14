//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core.NuGet.Properties;
using NuGet.Packaging.Core;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Loads plugin assemblies from a NuGet package.
    /// </summary>
    public sealed class PluginNuGetTypeLoader : ILoadTypesFromPlugins
    {
        /// <summary>
        /// Provides the configuration settings for the application.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Provides the diagnostics method for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The function used to copy the files from the installed NuGet packages.
        /// </summary>
        private readonly CopyPackageFiles _fileCopy;

        /// <summary>
        /// The object that provides an abstraction of the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The object that installs NuGet packages.
        /// </summary>
        private readonly IInstallPackages _packageInstaller;

        /// <summary>
        /// Provides the ability to query and update the directories from which assemblies can be loaded.
        /// </summary>
        private readonly IProvideAssemblyResolutionPaths _resolutionPaths;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNuGetTypeLoader"/> class.
        /// </summary>
        /// <param name="configuration">Provides the configuration settings for the application.</param>
        /// <param name="packageInstaller">Provides the ability to install NuGet packages into a specific location.</param>
        /// <param name="fileCopy">The function used to copy the files from the installed NuGet packages.</param>
        /// <param name="resolutionPaths">Provides the ability to query and update the directories from which assemblies can be loaded.</param>
        /// <param name="fileSystem">The object that provides an abstraction of the file system.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageInstaller"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileCopy"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="resolutionPaths"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public PluginNuGetTypeLoader(
            IConfiguration configuration,
            IInstallPackages packageInstaller,
            CopyPackageFiles fileCopy,
            IProvideAssemblyResolutionPaths resolutionPaths,
            IFileSystem fileSystem,
            SystemDiagnostics diagnostics)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (packageInstaller == null)
            {
                throw new ArgumentNullException("packageInstaller");
            }

            if (fileCopy == null)
            {
                throw new ArgumentNullException("fileCopy");
            }

            if (resolutionPaths == null)
            {
                throw new ArgumentNullException("resolutionPaths");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _configuration = configuration;
            _diagnostics = diagnostics;
            _fileCopy = fileCopy;
            _fileSystem = fileSystem;
            _packageInstaller = packageInstaller;
            _resolutionPaths = resolutionPaths;
        }

        private PackagePostInstall CopyAssemblyFilesToCacheDirectory(string assemblyCacheLocation)
        {
            return (outputLocation, packagePath, packageId) =>
                {
                    _fileCopy(
                        packageId,
                        "*.*",
                        packagePath,
                        assemblyCacheLocation);
                };
        }

        private string GetAssemblyCacheLocation(PackageIdentity identity)
        {
            var baseCacheLocation = _configuration.HasValueFor(CoreNuGetConfigurationKeys.AssemblyCacheLocation)
                ? _configuration.Value(CoreNuGetConfigurationKeys.AssemblyCacheLocation)
                : NuGetConstants.DefaultAssemblyCacheLocation;

            return _fileSystem.Path.Combine(
                baseCacheLocation,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1}",
                    identity.Id,
                    identity.Version));
        }

        private string GetInstallLocation()
        {
            return _configuration.HasValueFor(CoreNuGetConfigurationKeys.LocalInstallLocation)
                ? _configuration.Value(CoreNuGetConfigurationKeys.LocalInstallLocation)
                : NuGetConstants.DefaultInstallLocation;
        }

        /// <summary>
        /// Loads the type defined by the <paramref name="assemblyFullyQualifiedTypeName"/> from the <paramref name="origin"/>.
        /// </summary>
        /// <param name="origin">The object that indicates where the assembly containing the desired type can be found.</param>
        /// <param name="assemblyFullyQualifiedTypeName">The assembly fully qualified name of the type.</param>
        /// <returns>The requested <see cref="Type"/>.</returns>
        public Type Load(PluginOrigin origin, string assemblyFullyQualifiedTypeName)
        {
            var nugetOrigin = origin as PluginNuGetOrigin;
            if (nugetOrigin == null)
            {
                throw new InvalidPluginOriginException();
            }

            var installLocation = GetInstallLocation();
            var assemblyCacheLocation = GetAssemblyCacheLocation(nugetOrigin.Identity);
            if (!_fileSystem.Directory.Exists(assemblyCacheLocation))
            {
                try
                {
                    _diagnostics.Log(
                        LevelToLog.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_PluginNuGetTypeLoader_InstallingPackage_WithPackageNameAndVersionAndDestination,
                            nugetOrigin.Identity.Id,
                            nugetOrigin.Identity.Version,
                            assemblyCacheLocation));

                    _fileSystem.Directory.CreateDirectory(assemblyCacheLocation);

                    _packageInstaller.Install(
                        nugetOrigin.Identity,
                        installLocation,
                        CopyAssemblyFilesToCacheDirectory(assemblyCacheLocation));
                }
                catch (IOException)
                {
                    // If we fail to install we need to make sure that the assemblyCacheDirectory is deleted
                    // because otherwise the next time we'll think it's all good.
                    if (_fileSystem.Directory.Exists(assemblyCacheLocation))
                    {
                        _diagnostics.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.LogMessage_PluginNuGetTypeLoader_RemovingDestination_WithDestination,
                                assemblyCacheLocation));

                        try
                        {
                            _fileSystem.Directory.Delete(assemblyCacheLocation, true);
                        }
                        catch (IOException ioException)
                        {
                            // Nothing we can do here. Ignore it
                            _diagnostics.Log(
                                LevelToLog.Error,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.LogMessage_PluginNuGetTypeLoader_FailedToDeleteDestination_WithDestinationAndException,
                                    assemblyCacheLocation,
                                    ioException));
                        }
                    }
                }
                catch (NuGetPackageInstallFailedException e)
                {
                    _diagnostics.Log(
                        LevelToLog.Error,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_PluginNuGetTypeLoader_FailedToInstallPackage_WithPackageNameAndVersionAndDestinationAndException,
                            nugetOrigin.Identity.Id,
                            nugetOrigin.Identity.Version,
                            assemblyCacheLocation,
                            e));

                    // If we fail to install we need to make sure that the assemblyCacheDirectory is deleted
                    // because otherwise the next time we'll think it's all good.
                    if (_fileSystem.Directory.Exists(assemblyCacheLocation))
                    {
                        _diagnostics.Log(
                            LevelToLog.Info,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.LogMessage_PluginNuGetTypeLoader_RemovingDestination_WithDestination,
                                assemblyCacheLocation));

                        try
                        {
                            _fileSystem.Directory.Delete(assemblyCacheLocation, true);
                        }
                        catch (IOException ioException)
                        {
                            // Nothing we can do here. Ignore it
                            _diagnostics.Log(
                                LevelToLog.Error,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    Resources.LogMessage_PluginNuGetTypeLoader_FailedToDeleteDestination_WithDestinationAndException,
                                    assemblyCacheLocation,
                                    ioException));
                        }
                    }
                }
            }

            if (!_resolutionPaths.IsOnResolutionList(assemblyCacheLocation))
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.LogMessage_PluginNuGetTypeLoader_AddingDestinationToAssemblyResolutionFolderList_WithDestination,
                        assemblyCacheLocation));

                _resolutionPaths.Add(assemblyCacheLocation);
            }

            return TypeLoader.FromFullyQualifiedName(assemblyFullyQualifiedTypeName);
        }

        /// <summary>
        /// Gets the type of the <see cref="PluginOrigin"/> that the current type loader can use.
        /// </summary>
        public Type ValidOriginType
        {
            get
            {
                return typeof(PluginNuGetOrigin);
            }
        }
    }
}
