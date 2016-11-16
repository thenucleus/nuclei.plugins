//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Nuclei.Plugins.Discovery.Origin.FileSystem
{
    /// <summary>
    /// Watches the file system for changes.
    /// </summary>
    public sealed class FileSystemWatcherProxy : IFileSystemWatcher
    {
        /// <summary>
        /// The object that watches the file system for changes.
        /// </summary>
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        /// A flag used to indicate if the current instance has been disposed.
        /// </summary>
        private volatile bool _disposedValue = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemWatcherProxy"/> class.
        /// </summary>
        /// <param name="watcher">The <see cref="FileSystemWatcher"/> that watches the file system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="watcher"/> is <see langword="null" />.
        /// </exception>
        public FileSystemWatcherProxy(FileSystemWatcher watcher)
        {
            if (watcher == null)
            {
                throw new ArgumentNullException("watcher");
            }

            _watcher = watcher;

            _watcher.Changed += Changed;
            _watcher.Created += Created;
            _watcher.Deleted += Deleted;
            _watcher.Error += Error;
            _watcher.Renamed += Renamed;
        }

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is changed.
        /// </summary>
        public event FileSystemEventHandler Changed;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is created.
        /// </summary>
        public event FileSystemEventHandler Created;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is deleted.
        /// </summary>
        public event FileSystemEventHandler Deleted;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _watcher.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the component is enabled.
        /// </summary>
        public bool EnableRaisingEvents
        {
            get
            {
                return _watcher.EnableRaisingEvents;
            }

            set
            {
                _watcher.EnableRaisingEvents = value;
            }
        }

        /// <summary>
        /// Occurs when this instance is unable to continue monitoring changes or when the internal buffer overflows.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Gets or sets the filter string used to determine what files are monitored in a directory. The default is "*.*" (Watches all files.)
        /// </summary>
        public string Filter
        {
            get
            {
                return _watcher.Filter;
            }

            set
            {
                _watcher.Filter = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether subdirectories within the specified path should be monitored.
        /// </summary>
        public bool IncludeSubdirectories
        {
            get
            {
                return _watcher.IncludeSubdirectories;
            }

            set
            {
                _watcher.IncludeSubdirectories = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of changes to watch for.
        /// </summary>
        public NotifyFilters NotifyFilter
        {
            get
            {
                return _watcher.NotifyFilter;
            }

            set
            {
                _watcher.NotifyFilter = value;
            }
        }

        /// <summary>
        /// Gets or sets the path of the directory to watch.
        /// </summary>
        public string Path
        {
            get
            {
                return _watcher.Path;
            }

            set
            {
                _watcher.Path = value;
            }
        }

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is renamed.
        /// </summary>
        public event RenamedEventHandler Renamed;
    }
}
