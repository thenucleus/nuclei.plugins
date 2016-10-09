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
    public sealed class PropertyBasedDiscoverableMember : SerializableDiscoverableMemberDefinition
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PropertyBasedDiscoverableMember first, PropertyBasedDiscoverableMember second)
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
        public static bool operator !=(PropertyBasedDiscoverableMember first, PropertyBasedDiscoverableMember second)
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
        /// Creates a new instance of the <see cref="PropertyBasedExportDefinition"/> class based on the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <param name="metadata">The collection of metadata attached to the member.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="metadata"/> is <see langword="null" />.
        /// </exception>
        public static PropertyBasedDiscoverableMember CreateDefinition(PropertyInfo property, IDictionary<string, string> metadata)
        {
            return CreateDefinition(property, metadata, t => TypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PropertyBasedExportDefinition"/> class based on
        /// the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <param name="metadata">The collection of metadata attached to the member.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="metadata"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static PropertyBasedDiscoverableMember CreateDefinition(
            PropertyInfo property,
            IDictionary<string, string> metadata,
            Func<Type, TypeIdentity> identityGenerator)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (identityGenerator == null)
            {
                throw new ArgumentNullException("identityGenerator");
            }

            return new PropertyBasedDiscoverableMember(
                identityGenerator(property.DeclaringType),
                metadata,
                PropertyDefinition.CreateDefinition(property, identityGenerator));
        }

        /// <summary>
        /// The property.
        /// </summary>
        private readonly PropertyDefinition _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBasedDiscoverableMember"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the property on which the import is placed.</param>
        /// <param name="metadata">The collection of metadata attached to the member.</param>
        /// <param name="property">The property for which the current object stores the serialized data.</param>
        private PropertyBasedDiscoverableMember(
            TypeIdentity declaringType,
            IDictionary<string, string> metadata,
            PropertyDefinition property)
            : base(declaringType, metadata)
        {
            {
                Debug.Assert(property != null, "The property object shouldn't be null.");
            }

            _property = property;
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
            var otherType = other as PropertyBasedDiscoverableMember;
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(otherType, null)
                && Property.Equals(otherType.Property);
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

            var id = obj as PropertyBasedDiscoverableMember;
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
                hash = (hash * 23) ^ Property.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        public PropertyDefinition Property
        {
            get
            {
                return _property;
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
                "Discoverable property {0}",
                Property);
        }
    }
}
