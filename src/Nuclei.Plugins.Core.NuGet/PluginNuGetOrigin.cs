//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Nuclei.Plugins.Core.NuGet.Properties;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Describes the identity of a NuGet package that contains plugins.
    /// </summary>
    [Serializable]
    public sealed class PluginNuGetOrigin : PluginOrigin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNuGetOrigin"/> class.
        /// </summary>
        /// <param name="id">The identity of the nuget package.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        public PluginNuGetOrigin(PackageIdentity id)
            : base(new PluginNuGetOriginData(id))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNuGetOrigin"/> class.
        /// </summary>
        /// <param name="name">The name of the nuget package.</param>
        /// <param name="version">The version of the nuget package.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="name"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="name"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="version"/> is an empty string.
        /// </exception>
        private PluginNuGetOrigin(string name, string version)
            : base(new PluginNuGetOriginData(name, version))
        {
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
            var fileOriginData = value as PluginNuGetOriginData;
            Debug.Assert(fileOriginData != null, "The internal value should be a PluginNuGetOriginData instance.");

            return new PluginNuGetOrigin(fileOriginData.PackageName, fileOriginData.PackageVersion);
        }

        /// <summary>
        /// Gets the identity of the NuGet package.
        /// </summary>
        public PackageIdentity Identity
        {
            get
            {
                return new PackageIdentity(OriginData.PackageName, new NuGetVersion(OriginData.PackageVersion));
            }
        }

        private PluginNuGetOriginData OriginData
        {
            get
            {
                return InternalValue as PluginNuGetOriginData;
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
                "Plugin package {0} {1}",
                OriginData.PackageName,
                OriginData.PackageVersion);
        }

        [Serializable]
        private sealed class PluginNuGetOriginData : PluginOriginData
        {
            /// <summary>
            /// The name of the package.
            /// </summary>
            private readonly string _packageName;

            /// <summary>
            /// The version of the package.
            /// </summary>
            private readonly string _packageVersion;

            /// <summary>
            /// Initializes a new instance of the <see cref="PluginNuGetOriginData"/> class.
            /// </summary>
            /// <param name="id">The ID of the package.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="id"/> is <see langword="null" />.
            /// </exception>
            public PluginNuGetOriginData(PackageIdentity id)
            {
                if (id == null)
                {
                    throw new ArgumentNullException("id");
                }

                _packageName = id.Id;
                _packageVersion = id.Version.ToNormalizedString();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="PluginNuGetOriginData"/> class.
            /// </summary>
            /// <param name="name">The name of the package.</param>
            /// <param name="version">The version of the package.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="name"/> is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentException">
            ///     Thrown if <paramref name="name"/> is an empty string.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="version"/> is <see langword="null" />.
            /// </exception>
            /// <exception cref="ArgumentException">
            ///     Thrown if <paramref name="version"/> is an empty string.
            /// </exception>
            public PluginNuGetOriginData(string name, string version)
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }

                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(Properties.Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString, "name");
                }

                if (version == null)
                {
                    throw new ArgumentNullException("version");
                }

                if (string.IsNullOrEmpty(version))
                {
                    throw new ArgumentException(Properties.Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString, "version");
                }

                _packageName = name;
                _packageVersion = version;
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
                var data = other as PluginNuGetOriginData;
                if (ReferenceEquals(data, null))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resources.Exceptions_Messages_CompareArgument_WithTypes,
                            other.GetType().FullName,
                            GetType().FullName),
                        @"other");
                }

                var result = string.Compare(PackageName, data.PackageName, StringComparison.OrdinalIgnoreCase);
                if (result == 0)
                {
                    result = string.Compare(PackageVersion, data.PackageVersion, StringComparison.OrdinalIgnoreCase);
                }

                return result;
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

                var data = obj as PluginNuGetOriginData;
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
                var data = other as PluginNuGetOriginData;

                // Check if other is a null reference by using ReferenceEquals because
                // we overload the == operator. If other isn't actually null then
                // we get an infinite loop where we're constantly trying to compare to null.
                return !ReferenceEquals(data, null)
                    && string.Equals(PackageName, data.PackageName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(PackageVersion, data.PackageVersion, StringComparison.OrdinalIgnoreCase);
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
                    hash = (hash * 23) ^ _packageName.GetHashCode();
                    hash = (hash * 23) ^ _packageVersion.GetHashCode();

                    return hash;
                }
            }

            /// <summary>
            /// Gets the name of the package
            /// </summary>
            public string PackageName
            {
                get
                {
                    return _packageName;
                }
            }

            /// <summary>
            /// Gets the version of the package
            /// </summary>
            public string PackageVersion
            {
                get
                {
                    return _packageVersion;
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
                    "{0} {1}",
                    _packageName,
                    _packageVersion);
            }
        }
    }
}
