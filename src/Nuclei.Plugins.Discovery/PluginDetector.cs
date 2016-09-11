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
using Nuclei.Plugins.Discovery.Properties;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Provides the methods to find and scan plugins available in assembly files.
    /// </summary>
    internal sealed class PluginDetector
    {
        /// <summary>
        /// The objects that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The abstraction layer for the file system.
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
        /// Initializes a new instance of the <see cref="PluginDetector"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the parts and the part groups.</param>
        /// <param name="scannerBuilder">The function that is used to create an assembly scanner.</param>
        /// <param name="fileSystem">The abstraction layer for the file system.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
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
        public PluginDetector(
            IPluginRepository repository,
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder,
            IFileSystem fileSystem,
            SystemDiagnostics diagnostics)
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

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
            _fileSystem = fileSystem;
            _repository = repository;
            _scannerBuilder = scannerBuilder;
        }

        private void RemoveDeletedPlugins(IEnumerable<string> changedFilePaths)
        {
            var deletedFiles = changedFilePaths.Where(file => !_fileSystem.File.Exists(file));
            _repository.RemovePlugins(deletedFiles);
        }

        /// <summary>
        /// Searches the given directory for plugins.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="directory"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     Thrown if <paramref name="directory"/> does not exist.
        /// </exception>
        public void SearchDirectory(string directory)
        {
            if (directory == null)
            {
                throw new ArgumentNullException("directory");
            }

            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Plugins_LogMessage_Detector_FileScanStarted_WithDirectory,
                    directory));

            IEnumerable<string> files = Enumerable.Empty<string>();
            try
            {
                files = _fileSystem.Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException e)
            {
                // Something went wrong with the file IO. That probably means we don't have a complete list
                // so we just exit to prevent any issues from occuring.
                _diagnostics.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Detector_FileScanFailed_WithDirectoryAndException,
                        directory,
                        e));

                return;
            }
            catch (IOException e)
            {
                // Something went wrong with the file IO. That probably means we don't have a complete list
                // so we just exit to prevent any issues from occuring.
                _diagnostics.Log(
                    LevelToLog.Error,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Plugins_LogMessage_Detector_FileScanFailed_WithDirectoryAndException,
                        directory,
                        e));

                return;
            }

            var knownFiles = _repository.KnownPluginFiles();

            var changedKnownFiles = knownFiles
                .Where(p => files.Exists(f => string.Equals(p.Path, f, StringComparison.OrdinalIgnoreCase)))
                .Where(p => _fileSystem.File.GetLastWriteTimeUtc(p.Path) > p.LastWriteTimeUtc)
                .Select(p => p.Path);

            var changedFilePaths = new HashSet<string>(files);
            changedFilePaths.SymmetricExceptWith(knownFiles.Select(p => p.Path));

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
