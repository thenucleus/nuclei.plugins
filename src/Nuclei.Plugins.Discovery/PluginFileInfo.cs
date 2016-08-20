//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Describes the location and last write time for a plugin assembly.
    /// </summary>
    [Serializable]
    internal sealed class PluginFileInfo
    {
        /// <summary>
        /// The file path of the plugin file.
        /// </summary>
        private readonly string m_Path;

        /// <summary>
        /// The last time, in coordinated universal time (UTC), that the plugin file was modified.
        /// </summary>
        private readonly DateTimeOffset m_LastWriteTimeUtc;

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
            {
                Lokad.Enforce.Argument(() => path);
                Lokad.Enforce.Argument(() => path, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Path = path;
            m_LastWriteTimeUtc = lastWriteTimeUtc;
        }

        /// <summary>
        /// Gets the file path for the plugin file.
        /// </summary>
        public string Path
        {
            get
            {
                return m_Path;
            }
        }

        /// <summary>
        /// Gets the last time, in coordinated universal time (UTC), that the plugin file was modified.
        /// </summary>
        public DateTimeOffset LastWriteTimeUtc
        {
            get
            {
                return m_LastWriteTimeUtc;
            }
        }
    }
}
