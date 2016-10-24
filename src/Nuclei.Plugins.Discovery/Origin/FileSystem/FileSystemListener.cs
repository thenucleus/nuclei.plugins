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
using System.Linq;
using System.Reflection;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery.Properties;

namespace Nuclei.Plugins.Discovery.Origin.FileSystem
{
    /// <summary>
    /// Handles the detection of new, updated and removed plugin files stored in a directory.
    /// </summary>
    public sealed class FileSystemListener : IPluginListener
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The object that provides a virtualizing layer for the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The object that processes changes to files.
        /// </summary>
        private readonly List<IProcessPluginOriginChanges> _scanners
            = new List<IProcessPluginOriginChanges>();

        /// <summary>
        /// The collection of objects that watch the file system for newly added packages.
        /// </summary>
        private readonly IDictionary<string, IFileSystemWatcher> _watchers
            = new Dictionary<string, IFileSystemWatcher>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemListener"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="fileScanners">The collection of objects that scan plugin files.</param>
        /// <param name="watcherBuilder">The function that returns new <see cref="IFileSystemWatcher"/> instances.</param>
        /// <param name="diagnostics">The object providing the diagnostics methods for the application.</param>
        /// <param name="fileSystem">The object that provides a virtualizing layer for the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileScanners"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="watcherBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        public FileSystemListener(
            IConfiguration configuration,
            IEnumerable<IProcessPluginOriginChanges> fileScanners,
            Func<IFileSystemWatcher> watcherBuilder,
            SystemDiagnostics diagnostics,
            IFileSystem fileSystem)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (fileScanners == null)
            {
                throw new ArgumentNullException("fileScanners");
            }

            if (watcherBuilder == null)
            {
                throw new ArgumentNullException("watcherBuilder");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
            _fileSystem = fileSystem;
            _scanners.AddRange(fileScanners);

            var fileSearchPaths = configuration.HasValueFor(PluginConfigurationKeys.PluginLocations)
                ? configuration.Value(PluginConfigurationKeys.PluginLocations)
                : new[] { PluginDiscoveryConstants.DefaultPluginLocation };
            foreach (var path in fileSearchPaths)
            {
                var uri = new Uri(path);
                if (uri.IsFile || uri.IsUnc)
                {
                    var localPath = uri.LocalPath;

                    if (!_fileSystem.Path.IsPathRooted(localPath))
                    {
                        var exeDirectoryPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();
                        localPath = _fileSystem.Path.GetFullPath(_fileSystem.Path.Combine(exeDirectoryPath, localPath));
                    }

                    if (!_watchers.ContainsKey(localPath))
                    {
                        var watcher = watcherBuilder();
                        {
                            watcher.Path = localPath;
                            watcher.IncludeSubdirectories = true;
                            watcher.EnableRaisingEvents = false;
                            watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite;

                            watcher.Created += HandleFileCreated;
                            watcher.Changed += HandleFileChanged;
                            watcher.Deleted += HandleFileDeleted;
                            watcher.Renamed += HandleFileRenamed;
                        }

                        _watchers.Add(localPath, watcher);
                    }
                }
            }
        }

        /// <summary>
        /// Disables the uploading of packages.
        /// </summary>
        public void Disable()
        {
            foreach (var pair in _watchers)
            {
                pair.Value.EnableRaisingEvents = false;
            }

            _diagnostics.Log(
                LevelToLog.Info,
                Resources.LogMessage_FileSystemListener_FileDiscovery_Disabled);
        }

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        public void Enable()
        {
            _diagnostics.Log(
                LevelToLog.Info,
                Resources.LogMessage_FileSystemListener_FileDiscovery_Enabled);

            EnqueueExistingFiles();
            foreach (var pair in _watchers)
            {
                pair.Value.EnableRaisingEvents = true;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void EnqueueExistingFiles()
        {
            var newFiles = new List<string>();
            foreach (var file in _watchers.Keys.SelectMany(path => _fileSystem.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)))
            {
                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.LogMessage_FileSystemListener_LocatedFile_WithFilePath,
                        file));

                newFiles.Add(file);
            }

            foreach (var scanner in _scanners)
            {
                try
                {
                    var origins = newFiles
                        .Where(
                            p => scanner.AcceptedPluginTypes.Any(
                                t => t.Equals(new FilePluginType(_fileSystem.Path.GetExtension(p)))))
                        .Select(p => new PluginFileOrigin(p, _fileSystem.File.GetLastWriteTimeUtc(p), _fileSystem.File.GetLastWriteTimeUtc(p)))
                        .ToArray();
                    if (origins.Any())
                    {
                        scanner.Added(origins);
                    }
                }
                catch (Exception e)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_FileSystemListener_DiscoveredFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            string.Join(";", newFiles),
                            e));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void HandleFileChanged(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.LogMessage_FileSystemListener_UpdatedFile_WithFilePath,
                    e.FullPath));

            foreach (var scanner in _scanners)
            {
                try
                {
                    if (scanner.AcceptedPluginTypes.Any(t => t.Equals(new FilePluginType(_fileSystem.Path.GetExtension(e.FullPath)))))
                    {
                        var origin = new PluginFileOrigin(
                            e.FullPath,
                            _fileSystem.File.GetLastWriteTimeUtc(e.FullPath),
                            _fileSystem.File.GetLastWriteTimeUtc(e.FullPath));
                        scanner.Removed(origin);
                        scanner.Added(origin);
                    }
                }
                catch (Exception exception)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_FileSystemListener_DiscoveredFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            e.FullPath,
                            exception));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void HandleFileCreated(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.LogMessage_FileSystemListener_CreatedFile_WithFilePath,
                    e.FullPath));

            foreach (var scanner in _scanners)
            {
                try
                {
                    if (scanner.AcceptedPluginTypes.Any(t => t.Equals(new FilePluginType(_fileSystem.Path.GetExtension(e.FullPath)))))
                    {
                        var origin = new PluginFileOrigin(
                            e.FullPath,
                            _fileSystem.File.GetLastWriteTimeUtc(e.FullPath),
                            _fileSystem.File.GetLastWriteTimeUtc(e.FullPath));
                        scanner.Added(origin);
                    }
                }
                catch (Exception exception)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_FileSystemListener_DiscoveredFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            e.FullPath,
                            exception));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void HandleFileDeleted(object sender, FileSystemEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.LogMessage_FileSystemListener_RemovedFile_WithFilePath,
                    e.FullPath));

            foreach (var scanner in _scanners)
            {
                try
                {
                    if (scanner.AcceptedPluginTypes.Any(t => t.Equals(new FilePluginType(_fileSystem.Path.GetExtension(e.FullPath)))))
                    {
                        var origin = new PluginFileOrigin(e.FullPath);
                        scanner.Removed(origin);
                    }
                }
                catch (Exception exception)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_FileSystemListener_DeletedFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            e.FullPath,
                            exception));
                }
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Logging the exception but don't want to kill the process because one of the scanners can't handle the package.")]
        private void HandleFileRenamed(object sender, RenamedEventArgs e)
        {
            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.LogMessage_FileSystemListener_RenamedFile_WithOldAndNewFilePath,
                    e.OldFullPath,
                    e.FullPath));

            foreach (var scanner in _scanners)
            {
                try
                {
                    if (scanner.AcceptedPluginTypes.Any(t => t.Equals(new FilePluginType(_fileSystem.Path.GetExtension(e.OldFullPath)))))
                    {
                        var removed = new PluginFileOrigin(e.OldFullPath);
                        scanner.Removed(removed);
                    }

                    if (scanner.AcceptedPluginTypes.Any(t => t.Equals(new FilePluginType(_fileSystem.Path.GetExtension(e.FullPath)))))
                    {
                        var origin = new PluginFileOrigin(
                            e.FullPath,
                            _fileSystem.File.GetLastWriteTimeUtc(e.FullPath),
                            _fileSystem.File.GetLastWriteTimeUtc(e.FullPath));
                        scanner.Added(origin);
                    }
                }
                catch (Exception exception)
                {
                    _diagnostics.Log(
                        LevelToLog.Warn,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.LogMessage_FileSystemListener_DiscoveredFile_ScannerFailed_WithScannerTypeAndFilePathsAndError,
                            scanner.GetType(),
                            e.FullPath,
                            exception));
                }
            }
        }
    }
}
