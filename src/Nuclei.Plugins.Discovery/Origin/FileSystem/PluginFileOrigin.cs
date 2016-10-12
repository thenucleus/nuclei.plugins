//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Discovery.Origin.FileSystem
{
    /// <summary>
    /// Describes the location and last write time for a plugin assembly.
    /// </summary>
    [Serializable]
    public sealed class PluginFileOrigin : PluginOrigin
    {
        /// <summary>
        /// The last time, in coordinated universal time (UTC), that the plugin file was modified.
        /// </summary>
        private readonly DateTimeOffset _lastWriteTimeUtc;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFileOrigin"/> class.
        /// </summary>
        /// <param name="path">The full path to the plugin file.</param>
        public PluginFileOrigin(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginFileOrigin"/> class.
        /// </summary>
        /// <param name="path">The full path to the plugin file.</param>
        /// <param name="lastWriteTimeUtc">The last time the assembly was changed.</param>
        public PluginFileOrigin(string path, DateTimeOffset lastWriteTimeUtc)
            : this(path)
        {
            _lastWriteTimeUtc = lastWriteTimeUtc;
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override PluginOrigin Clone(string value)
        {
            return new PluginFileOrigin(value);
        }

        /// <summary>
        /// Gets the full file path for the file that contains the plugins.
        /// </summary>
        public string FilePath
        {
            get
            {
                return InternalValue;
            }
        }

        /// <summary>
        /// Gets the last time the file that contains the plugins was written to.
        /// </summary>
        public DateTimeOffset LastWriteTimeUtc
        {
            get
            {
                return _lastWriteTimeUtc;
            }
        }
    }
}
