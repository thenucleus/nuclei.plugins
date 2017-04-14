//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Nuclei.Plugins.Core.Assembly.Properties;

namespace Nuclei.Plugins.Core.Assembly
{
    /// <summary>
    /// Describes the location and last write time for a plugin assembly.
    /// </summary>
    [Serializable]
    public sealed class PluginAssemblyOrigin : PluginOrigin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginAssemblyOrigin"/> class.
        /// </summary>
        /// <param name="path">The full path to the plugin file.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path"/> is an empty string.
        /// </exception>
        public PluginAssemblyOrigin(string path)
            : base(new PluginAssemblyOriginData(path, DateTimeOffset.MinValue, DateTimeOffset.MinValue))
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString, "path");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginAssemblyOrigin"/> class.
        /// </summary>
        /// <param name="path">The full path to the plugin file.</param>
        /// <param name="creationTimeUtc">The time the file was created.</param>
        /// <param name="lastWriteTimeUtc">The last time the file was changed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="path"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="path"/> is an empty string.
        /// </exception>
        public PluginAssemblyOrigin(string path, DateTimeOffset creationTimeUtc, DateTimeOffset lastWriteTimeUtc)
            : base(new PluginAssemblyOriginData(path, creationTimeUtc, lastWriteTimeUtc))
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString, "path");
            }
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override PluginOrigin Clone(PluginOriginData value)
        {
            var fileOriginData = value as PluginAssemblyOriginData;
            Debug.Assert(fileOriginData != null, "The internal value should be a PluginAssemblyOriginData instance.");

            return new PluginAssemblyOrigin(fileOriginData.FilePath, fileOriginData.CreationTimeUtc, fileOriginData.LastWriteTimeUtc);
        }

        /// <summary>
        /// Gets the time the file was created.
        /// </summary>
        public DateTimeOffset CreationTimeUtc
        {
            get
            {
                return OriginData.CreationTimeUtc;
            }
        }

        /// <summary>
        /// Gets the full file path for the file that contains the plugins.
        /// </summary>
        public string FilePath
        {
            get
            {
                return OriginData.FilePath;
            }
        }

        /// <summary>
        /// Gets the last time the file that contains the plugins was written to.
        /// </summary>
        public DateTimeOffset LastWriteTimeUtc
        {
            get
            {
                return OriginData.LastWriteTimeUtc;
            }
        }

        private PluginAssemblyOriginData OriginData
        {
            get
            {
                return InternalValue as PluginAssemblyOriginData;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Plugin file {0}  was created at: {1} and last modified at: {2})",
                OriginData.FilePath,
                OriginData.CreationTimeUtc,
                OriginData.LastWriteTimeUtc);
        }

        [Serializable]
        private sealed class PluginAssemblyOriginData : PluginOriginData
        {
            /// <summary>
            /// The time the file was created.
            /// </summary>
            private readonly DateTimeOffset _creationTimeUtc;

            /// <summary>
            /// The file path for the plugin file.
            /// </summary>
            private readonly string _filePath;

            /// <summary>
            /// The last time, in coordinated universal time (UTC), that the plugin file was modified.
            /// </summary>
            private readonly DateTimeOffset _lastWriteTimeUtc;

            /// <summary>
            /// Initializes a new instance of the <see cref="PluginAssemblyOriginData"/> class.
            /// </summary>
            /// <param name="path">The full path to the plugin file.</param>
            /// <param name="creationTimeUtc">The date and time the plugin file was created in UTC time.</param>
            /// <param name="lastWriteTimeUtc">The date and time the plugin file was last changed in UTC time.</param>
            public PluginAssemblyOriginData(string path, DateTimeOffset creationTimeUtc, DateTimeOffset lastWriteTimeUtc)
            {
                _filePath = path;
                _creationTimeUtc = creationTimeUtc;
                _lastWriteTimeUtc = lastWriteTimeUtc;
            }

            /// <summary>
            /// Compares the current instance with another object of the same type and returns an integer that
            /// indicates whether the current instance precedes, follows, or occurs in the same position in the
            /// sort order as the other object.
            /// </summary>
            /// <param name="other">The value of the object with which the current object is being compared.</param>
            /// <returns>
            /// A 32-bit signed integer that indicates the relative order of the objects being compared.
            /// The return value has these meanings:
            /// Value
            /// Meaning
            /// Less than zero
            /// The current instance is less than <paramref name="other"/>.
            /// Zero
            /// The current instance is equal to <paramref name="other"/>.
            /// Greater than zero
            /// The current instance is greater than <paramref name="other"/>.
            /// </returns>
            protected override int CompareValues(PluginOriginData other)
            {
                // We don't strictly need to use the ReferenceEquals method but
                // it seems more consistent to use it.
                if (ReferenceEquals(other, null))
                {
                    return 1;
                }

                // Check if other is a null reference by using ReferenceEquals because
                // we overload the == operator. If other isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                var data = other as PluginAssemblyOriginData;
                if (ReferenceEquals(data, null))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_CompareArgument_WithTypes,
                            other.GetType().FullName,
                            GetType().FullName),
                        @"other");
                }

                var result = string.Compare(FilePath, data.FilePath, StringComparison.OrdinalIgnoreCase);
                if (result == 0)
                {
                    result = CreationTimeUtc.CompareTo(data.CreationTimeUtc);
                    if (result == 0)
                    {
                        result = LastWriteTimeUtc.CompareTo(data.LastWriteTimeUtc);
                    }
                }

                return result;
            }

            /// <summary>
            /// Gets the time the file was created.
            /// </summary>
            public DateTimeOffset CreationTimeUtc
            {
                get
                {
                    return _creationTimeUtc;
                }
            }

            /// <summary>
            /// Determines whether the specified <see cref="object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="object"/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                var data = obj as PluginAssemblyOriginData;
                return Equals(data);
            }

            /// <summary>
            /// Determines whether the specified <see cref="PluginOriginData"/> is equal to this instance.
            /// </summary>
            /// <param name="other">The <see cref="PluginOriginData"/> to compare with this instance.</param>
            /// <returns>
            ///     <see langword="true"/> if the specified <see cref="PluginOriginData"/> is equal to this instance;
            ///     otherwise, <see langword="false"/>.
            /// </returns>
            public override bool Equals(PluginOriginData other)
            {
                var data = other as PluginAssemblyOriginData;

                // Check if other is a null reference by using ReferenceEquals because
                // we overload the == operator. If other isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                return !ReferenceEquals(data, null)
                    && string.Equals(FilePath, data.FilePath, StringComparison.OrdinalIgnoreCase)
                    && CreationTimeUtc.Equals(data.CreationTimeUtc)
                    && LastWriteTimeUtc.Equals(data.LastWriteTimeUtc);
            }

            /// <summary>
            /// Gets the full file path for the file that contains the plugins.
            /// </summary>
            public string FilePath
            {
                get
                {
                    return _filePath;
                }
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
            /// </returns>
            public override int GetHashCode()
            {
                // As obtained from the Jon Skeet answer to:
                // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
                // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
                //
                // Overflow is fine, just wrap
                unchecked
                {
                    // Pick a random prime number
                    int hash = 17;

                    // Mash the hash together with yet another random prime number
                    hash = (hash * 23) ^ _creationTimeUtc.GetHashCode();
                    hash = (hash * 23) ^ _filePath.GetHashCode();
                    hash = (hash * 23) ^ _lastWriteTimeUtc.GetHashCode();

                    return hash;
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

            /// <summary>
            /// Returns a <see cref="string"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="string"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} (created: {1}, last modified {2})",
                    _filePath,
                    _creationTimeUtc,
                    _lastWriteTimeUtc);
            }
        }
    }
}
