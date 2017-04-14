//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Nuclei.Plugins.Core.Properties;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines an <see cref="IPluginType"/> that references plugins in a file with a given extension.
    /// </summary>
    public sealed class FilePluginType : IPluginType
    {
        private static readonly Func<string, PluginOrigin> NullOriginBuilder = s => null;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(FilePluginType first, FilePluginType second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(FilePluginType first, FilePluginType second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// The file extension of the plugin file.
        /// </summary>
        private readonly string _extension;

        /// <summary>
        /// The function used to create a <see cref="PluginOrigin"/> instance.
        /// </summary>
        private readonly Func<string, PluginOrigin> _originBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePluginType"/> class.
        /// </summary>
        /// <param name="extension">The file extension of the plugin file.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="extension"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="extension"/> is an empty string.
        /// </exception>
        public FilePluginType(string extension)
            : this(extension, NullOriginBuilder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePluginType"/> class.
        /// </summary>
        /// <param name="extension">The file extension of the plugin file.</param>
        /// <param name="originBuilder">The function used to create a <see cref="PluginOrigin"/> instance.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="extension"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="extension"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="originBuilder"/> is <see langword="null" />.
        /// </exception>
        public FilePluginType(string extension, Func<string, PluginOrigin> originBuilder)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(extension);
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "extension");
            }

            if (originBuilder == null)
            {
                throw new ArgumentNullException("originBuilder");
            }

            _extension = extension;
            _originBuilder = originBuilder;
        }

        /// <summary>
        /// Makes a copy of the current instance.
        /// </summary>
        /// <returns>A copy of the current instance.</returns>
        public IPluginType Clone()
        {
            return new FilePluginType(_extension, _originBuilder);
        }

        /// <summary>
        /// Determines whether the specified <see cref="IPluginType"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="IPluginType"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="IPluginType"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(IPluginType other)
        {
            var filePluginType = other as FilePluginType;

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(filePluginType, null) && string.Equals(_extension, filePluginType._extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as FilePluginType;
            return Equals(id);
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
                hash = (hash * 23) ^ _extension.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns the origin for the specific plugin.
        /// </summary>
        /// <param name="source">The source of the plugin.</param>
        /// <returns>The <see cref="PluginOrigin"/> object for the plugin.</returns>
        public PluginOrigin Origin(string source)
        {
            return _originBuilder(source);
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
                "Plugin files with extension {0}",
                _extension);
        }
    }
}
