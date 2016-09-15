//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Stores information about an assembly in a serializable form, i.e. without requiring the
    /// assembly in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class AssemblyDefinition : IEquatable<AssemblyDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(AssemblyDefinition first, AssemblyDefinition second)
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
        public static bool operator !=(AssemblyDefinition first, AssemblyDefinition second)
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
        /// Creates a new instance of the <see cref="AssemblyDefinition"/> class based on the given <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given assembly.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assembly"/> is <see langword="null" />.
        /// </exception>
        public static AssemblyDefinition CreateDefinition(Assembly assembly)
        {
            return new AssemblyDefinition(assembly);
        }

        /// <summary>
        /// The name of the assembly.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The version of the assembly.
        /// </summary>
        private readonly Version _version;

        /// <summary>
        /// The culture of the assembly.
        /// </summary>
        private readonly CultureInfo _culture;

        /// <summary>
        /// The public key token of the assembly.
        /// </summary>
        private readonly string _publicKeyToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyDefinition"/> class.
        /// </summary>
        /// <param name="assembly">The assembly for which the data should be stored.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assembly"/> is <see langword="null" />.
        /// </exception>
        private AssemblyDefinition(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            var assemblyName = assembly.GetName();
            var publicKeyToken = string.Join(
                string.Empty,
                assemblyName.GetPublicKeyToken().Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));

            _name = assemblyName.Name;
            _version = assemblyName.Version;
            _culture = assemblyName.CultureInfo;
            _publicKeyToken = publicKeyToken;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the version of the assembly.
        /// </summary>
        public Version Version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// Gets the culture for the assembly.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
        }

        /// <summary>
        /// Gets the public key token for the assembly.
        /// </summary>
        public string PublicKeyToken
        {
            get
            {
                return _publicKeyToken;
            }
        }

        /// <summary>
        /// Gets the full name of the assembly.
        /// </summary>
        public string FullName
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}, Version={1}, Culture={2}, PublicKeyToken={3}",
                    Name,
                    Version,
                    !Culture.Equals(CultureInfo.InvariantCulture) ? Culture.Name : "neutral",
                    PublicKeyToken);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="AssemblyDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="AssemblyDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="AssemblyDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(AssemblyDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase)
                && Version.Equals(other.Version)
                && Culture.Equals(other.Culture)
                && string.Equals(PublicKeyToken, other.PublicKeyToken, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as AssemblyDefinition;
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
                hash = (hash * 23) ^ Name.GetHashCode();
                hash = (hash * 23) ^ Version.GetHashCode();
                hash = (hash * 23) ^ Culture.GetHashCode();
                if (PublicKeyToken != null)
                {
                    hash = (hash * 23) ^ PublicKeyToken.GetHashCode();
                }

                return hash;
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
            return FullName;
        }
    }
}
