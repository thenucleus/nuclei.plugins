//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery.Properties;

namespace Nuclei.Plugins.Discovery.Container.Assembly
{
    /// <summary>
    /// Provides the methods to find and scan plugins available in assembly files.
    /// </summary>
    public sealed class AssemblyPluginDetector : IProcessPluginOriginChanges
    {
        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The collection containing all listeners.
        /// </summary>
        private readonly List<IPluginListener> _listeners
            = new List<IPluginListener>();

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
        /// <param name="listeners">The collection of object that detect changes to plugin files.</param>
        /// <param name="scannerBuilder">The function that is used to create an assembly scanner.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="listeners"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="scannerBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public AssemblyPluginDetector(
            IPluginRepository repository,
            IEnumerable<IPluginListener> listeners,
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder,
            SystemDiagnostics diagnostics)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (listeners == null)
            {
                throw new ArgumentNullException("listeners");
            }

            if (scannerBuilder == null)
            {
                throw new ArgumentNullException("scannerBuilder");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
            _repository = repository;
            _scannerBuilder = scannerBuilder;

            foreach (var listener in listeners)
            {
                _listeners.Add(listener);
                listener.OnPluginDetected += HandlePluginDetected;
            }
        }

        private void HandlePluginDetected(object sender, PluginFoundEventArgs e)
        {
            var knownFiles = _repository.KnownPluginOrigins().OfType<PluginFileOrigin>();

            var changedKnownFiles = knownFiles
                .Where(p => files.Any(f => string.Equals(p.FilePath, f, StringComparison.OrdinalIgnoreCase)))
                .Where(p => _fileSystem.File.GetLastWriteTimeUtc(p.FilePath) > p.LastWriteTimeUtc)
                .Select(p => p.FilePath);

            var changedFilePaths = new HashSet<string>(files);
            changedFilePaths.SymmetricExceptWith(knownFiles.Select(p => p.FilePath));

            var newFiles = changedFilePaths.Where(file => _fileSystem.File.Exists(file));

            RemoveDeletedPlugins(changedFilePaths);
            StorePlugins(changedKnownFiles.Concat(newFiles));

            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Plugins_LogMessage_Detector_FileScanCompleted,
                    directory));
        }

        private void RemoveDeletedPlugins(IEnumerable<string> changedFilePaths)
        {
            var deletedFiles = changedFilePaths.Where(file => !_fileSystem.File.Exists(file)).Select(file => new PluginFileOrigin(file));
            _repository.RemovePlugins(deletedFiles);
        }

        private void StorePlugins(IEnumerable<string> filesToScan)
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
