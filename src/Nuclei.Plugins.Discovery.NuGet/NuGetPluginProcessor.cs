//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Core.NuGet;
using Nuclei.Plugins.Discovery.Container;
using Nuclei.Plugins.Discovery.NuGet.Properties;
using NuGet.Packaging;

namespace Nuclei.Plugins.Discovery.NuGet
{
    /// <summary>
    /// Provides the methods to find and scan plugins available in NuGet packages.
    /// </summary>
    public sealed class NuGetPluginProcessor : IProcessPluginOriginChanges
    {
        private static readonly IPluginType[] _acceptedPluginTypes = new[]
            {
                new FilePluginType(
                    "nupkg",
                    p =>
                    {
                        using (var reader = new PackageArchiveReader(p))
                        {
                            return new PluginNuGetOrigin(reader.GetIdentity());
                        }
                    }),
            };

        /// <summary>
        /// The object that provides the diagnostics for the system.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The object that provides an abstraction of the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The object that installs NuGet packages.
        /// </summary>
        private readonly IInstallPackages _packageInstaller;

        /// <summary>
        /// The object that stores information about all the parts and the part groups.
        /// </summary>
        private readonly IPluginRepository _repository;

        /// <summary>
        /// The function that returns a reference to an assembly scanner which has been
        /// created in the given AppDomain.
        /// </summary>
        private readonly Func<IPluginRepository, IAssemblyScanner> _scannerBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPluginProcessor"/> class.
        /// </summary>
        /// <param name="packageInstaller"> The object that installs NuGet packages.</param>
        /// <param name="repository">The object that stores information about all the parts and the part groups.</param>
        /// <param name="scannerBuilder">The function that is used to create an NuGet package scanner.</param>
        /// <param name="fileSystem">The object that provides an abstraction of the file system.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packageInstaller"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public NuGetPluginProcessor(
            IInstallPackages packageInstaller,
            IPluginRepository repository,
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder,
            IFileSystem fileSystem,
            SystemDiagnostics diagnostics)
        {
            if (packageInstaller == null)
            {
                throw new ArgumentNullException("packageInstaller");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (scannerBuilder == null)
            {
                throw new ArgumentNullException("scannerBuilder");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
            _fileSystem = fileSystem;
            _repository = repository;
            _scannerBuilder = scannerBuilder;
        }

        /// <summary>
        /// Gets a collection that describes what types of plugins are accepted by the processor.
        /// </summary>
        public IEnumerable<IPluginType> AcceptedPluginTypes
        {
            get
            {
                return _acceptedPluginTypes;
            }
        }

        /// <summary>
        /// Processes the added files.
        /// </summary>
        /// <param name="newPlugins">The collection that contains the names of all the new plugins.</param>
        public void Added(params PluginOrigin[] newPlugins)
        {
            var packagesToAdd = newPlugins
                .OfType<PluginNuGetOrigin>();
            StorePlugins(packagesToAdd);
        }

        /// <summary>
        /// Processes the removed files.
        /// </summary>
        /// <param name="removedPlugins">The collection that contains the names of all the plugins that were removed.</param>
        public void Removed(params PluginOrigin[] removedPlugins)
        {
            _repository.RemovePlugins(removedPlugins);
        }

        private void StorePlugins(IEnumerable<PluginNuGetOrigin> packagesToScan)
        {
            if (!packagesToScan.Any())
            {
                return;
            }

            foreach (var package in packagesToScan.Select(f => f.Identity))
            {
                var tempDirectory = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());
                var binPath = _fileSystem.Path.Combine(tempDirectory, "bin");
                if (!_fileSystem.Directory.Exists(binPath))
                {
                    _diagnostics.Log(
                        LevelToLog.Debug,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_PackageScanner_CreatingBinDirectory_WithPath,
                            binPath));

                    _fileSystem.Directory.CreateDirectory(binPath);
                }

                try
                {
                    var filesToScan = new Dictionary<string, PluginOrigin>();
                    _packageInstaller.Install(
                        package,
                        tempDirectory,
                        (outputLocation, path, id) =>
                        {
                            var copiedFiles = PackageUtilities.CopyPackageFilesToSinglePath(
                                path,
                                id,
                                "*.*",
                                binPath,
                                _diagnostics,
                                _fileSystem);

                            if (id.Equals(package))
                            {
                                var origin = new PluginNuGetOrigin(id);
                                var packageAssemblies = copiedFiles
                                    .Where(p => _fileSystem.Path.GetExtension(p).Equals("dll"))
                                    .ToDictionary(k => k, v => origin);
                                foreach (var pair in packageAssemblies)
                                {
                                    if (!filesToScan.ContainsKey(pair.Key))
                                    {
                                        filesToScan.Add(pair.Key, pair.Value);
                                    }
                                }
                            }
                        });

                    var scanner = _scannerBuilder(_repository);
                    scanner.Scan(filesToScan);
                }
                finally
                {
                    if (_fileSystem.Directory.Exists(tempDirectory))
                    {
                        _fileSystem.Directory.Delete(tempDirectory, true);
                    }
                }
            }
        }
    }
}
