//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Plugins.Discovery.Properties;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Describes the location and last write time for a plugin assembly.
    /// </summary>
    [Serializable]
    public sealed class PluginFileInfo
    {
        /// <summary>
        /// The file path of the plugin file.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The last time, in coordinated universal time (UTC), that the plugin file was modified.
        /// </summary>
        private readonly DateTimeOffset _lastWriteTimeUtc;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFileInfo"/> class.
        /// </summary>
        /// <param name="path">The full path to the plugin assembly.</param>
        /// <param name="lastWriteTimeUtc">The last time the assembly was changed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path"/> is an empty string.
        /// </exception>
        public PluginFileInfo(string path, DateTimeOffset lastWriteTimeUtc)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "path");
            }

            _path = path;
            _lastWriteTimeUtc = lastWriteTimeUtc;
        }

        /// <summary>
        /// Gets the last time, in coordinated universal time (UTC), that the plugin file was modified.
        /// </summary>
        public DateTimeOffset LastWriteTimeUtc
        {
            get
            {
                return _lastWriteTimeUtc;
            }
        }

        /// <summary>
        /// Gets the file path for the plugin file.
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
        }
    }
}
