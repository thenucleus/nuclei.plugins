//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Stores information about an discoverable type in serialized form, i.e. without requiring the
    /// type in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class MethodBasedDiscoverableMember : SerializableDiscoverableMemberDefinition
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MethodBasedDiscoverableMember first, MethodBasedDiscoverableMember second)
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
        public static bool operator !=(MethodBasedDiscoverableMember first, MethodBasedDiscoverableMember second)
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
        /// Creates a new instance of the <see cref="MethodBasedDiscoverableMember"/> class
        /// based on the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method for which the current object stores the serialized data.</param>
        /// <param name="metadata">The collection of metadata attached to the member.</param>
        /// <returns>The serialized definition for the given method.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="method"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="metadata"/> is <see langword="null" />.
        /// </exception>
        public static MethodBasedDiscoverableMember CreateDefinition(MethodInfo method, IDictionary<string, string> metadata)
        {
            return CreateDefinition(method, metadata, t => TypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MethodBasedExportDefinition"/> class based
        /// on the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method for which the current object stores the serialized data.</param>
        /// <param name="metadata">The collection of metadata attached to the member.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given method.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="method"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="metadata"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static MethodBasedDiscoverableMember CreateDefinition(
            MethodInfo method,
            IDictionary<string, string> metadata,
            Func<Type, TypeIdentity> identityGenerator)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            if (identityGenerator == null)
            {
                throw new ArgumentNullException("identityGenerator");
            }

            return new MethodBasedDiscoverableMember(
                identityGenerator(method.DeclaringType),
                metadata,
                MethodDefinition.CreateDefinition(method, identityGenerator));
        }

        /// <summary>
        /// The name of the method.
        /// </summary>
        private readonly MethodDefinition _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBasedDiscoverableMember"/> class.
        /// </summary>
        /// <param name="declaringType">The type which declares the method on which the import is placed.</param>
        /// <param name="metadata">The collection of metadata attached to the member.</param>
        /// <param name="method">The method for which the current object stores the serialized data.</param>
        private MethodBasedDiscoverableMember(
            TypeIdentity declaringType,
            IDictionary<string, string> metadata,
            MethodDefinition method)
            : base(declaringType, metadata)
        {
            {
                Debug.Assert(method != null, "The method object should not be null.");
            }

            _method = method;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializableExportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializableExportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializableExportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(SerializableDiscoverableMemberDefinition other)
        {
            var otherType = other as MethodBasedDiscoverableMember;
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(otherType, null)
                && DeclaringType == otherType.DeclaringType
                && Method == otherType.Method;
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as MethodBasedDiscoverableMember;
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
                hash = (hash * 23) ^ DeclaringType.GetHashCode();
                hash = (hash * 23) ^ Method.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public MethodDefinition Method
        {
            get
            {
                return _method;
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
                "Discoverable method {0}",
                Method);
        }
    }
}
