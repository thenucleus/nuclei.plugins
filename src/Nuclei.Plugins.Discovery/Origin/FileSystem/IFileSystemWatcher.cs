//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Nuclei.Plugins.Discovery.Origin.FileSystem
{
    /// <summary>
    /// Defines the interface for objects that watch the file system for changes.
    /// </summary>
    public interface IFileSystemWatcher : IDisposable
    {
        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is changed.
        /// </summary>
        event FileSystemEventHandler Changed;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is created.
        /// </summary>
        event FileSystemEventHandler Created;

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is deleted.
        /// </summary>
        event FileSystemEventHandler Deleted;

        /// <summary>
        /// Gets or sets a value indicating whether the component is enabled.
        /// </summary>
        bool EnableRaisingEvents
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when this instance is unable to continue monitoring changes or when the internal buffer overflows.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1716:IdentifiersShouldNotMatchKeywords",
            MessageId = "Error",
            Justification = "This interface should match with the FileSystemWatcher which defines an event called Error.")]
        event ErrorEventHandler Error;

        /// <summary>
        /// Gets or sets the filter string used to determine what files are monitored in a directory. The default is "*.*" (Watches all files.)
        /// </summary>
        string Filter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether subdirectories within the specified path should be monitored.
        /// </summary>
        bool IncludeSubdirectories
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of changes to watch for.
        /// </summary>
        NotifyFilters NotifyFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of the directory to watch.
        /// </summary>
        string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when a file or directory in the specified <see cref="Path"/> is renamed.
        /// </summary>
        event RenamedEventHandler Renamed;
    }
}
