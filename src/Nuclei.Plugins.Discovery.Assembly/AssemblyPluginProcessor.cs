//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery.Container;

namespace Nuclei.Plugins.Discovery.Assembly
{
    /// <summary>
    /// Provides the methods to find and scan plugins available in assembly files.
    /// </summary>
    public sealed class AssemblyPluginProcessor : IProcessPluginOriginChanges
    {
        /// <summary>
        /// The collection of plugin types that the current processor can handle.
        /// </summary>
        private readonly IPluginType[] _acceptedPluginTypes;

        /// <summary>
        /// The object that provides an abstraction of the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

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
        /// Initializes a new instance of the <see cref="AssemblyPluginProcessor"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the parts and the part groups.</param>
        /// <param name="scannerBuilder">The function that is used to create an assembly scanner.</param>
        /// <param name="fileSystem">The object that provides an abstraction of the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public AssemblyPluginProcessor(
            IPluginRepository repository,
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder,
            IFileSystem fileSystem)
        {
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

            _fileSystem = fileSystem;
            _repository = repository;
            _scannerBuilder = scannerBuilder;

            _acceptedPluginTypes = new[]
                {
                    new FilePluginType(
                        CoreConstants.AssemblyExtension,
                        s => new PluginAssemblyOrigin(s, _fileSystem.File.GetCreationTimeUtc(s), _fileSystem.File.GetLastWriteTimeUtc(s)))
                };
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
            var filesToAdd = newPlugins
                .OfType<PluginAssemblyOrigin>();
            StorePlugins(filesToAdd);
        }

        /// <summary>
        /// Processes the removed files.
        /// </summary>
        /// <param name="removedPlugins">The collection that contains the names of all the plugins that were removed.</param>
        public void Removed(params PluginOrigin[] removedPlugins)
        {
            _repository.RemovePlugins(removedPlugins);
        }

        private void StorePlugins(IEnumerable<PluginAssemblyOrigin> filesToScan)
        {
            if (!filesToScan.Any())
            {
                return;
            }

            var scanner = _scannerBuilder(_repository);
            scanner.Scan(
                filesToScan.ToDictionary(
                    f => f.FilePath,
                    f => (PluginOrigin)f));
        }
    }
}
