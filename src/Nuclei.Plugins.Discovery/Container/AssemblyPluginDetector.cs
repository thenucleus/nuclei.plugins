//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery.Origin.FileSystem;

namespace Nuclei.Plugins.Discovery.Container
{
    /// <summary>
    /// Provides the methods to find and scan plugins available in assembly files.
    /// </summary>
    public sealed class AssemblyPluginDetector : IProcessPluginOriginChanges
    {
        private static readonly IPluginType[] _acceptedPluginTypes = new[]
            {
                new FilePluginType("dll"),
            };

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
        /// Initializes a new instance of the <see cref="AssemblyPluginDetector"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the parts and the part groups.</param>
        /// <param name="scannerBuilder">The function that is used to create an assembly scanner.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        public AssemblyPluginDetector(
            IPluginRepository repository,
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (scannerBuilder == null)
            {
                throw new ArgumentNullException("scannerBuilder");
            }

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
            var filesToAdd = newPlugins
                .OfType<PluginFileOrigin>();
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

        private void StorePlugins(IEnumerable<PluginFileOrigin> filesToScan)
        {
            if (!filesToScan.Any())
            {
                return;
            }

            var scanner = _scannerBuilder(_repository);
            scanner.Scan(filesToScan);
        }
    }
}
