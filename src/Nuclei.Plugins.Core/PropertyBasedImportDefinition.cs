//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Stores information about an imported property in serialized form, i.e. without requiring the
    /// owning type in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class PropertyBasedImportDefinition : SerializableImportDefinition
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PropertyBasedImportDefinition first, PropertyBasedImportDefinition second)
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
        public static bool operator !=(PropertyBasedImportDefinition first, PropertyBasedImportDefinition second)
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
        /// Creates a new instance of the <see cref="PropertyBasedImportDefinition"/> class based on the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
        /// <param name="requiredTypeIdentityForMef">The type identity of the export type as expected by MEF.</param>
        /// <param name="cardinality">
        ///     One of the enumeration values that indicates the cardinality of the export object required by the import definition.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a parts;
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Recomposable",
            Justification = "MEF uses the same term, so we're not going to make up some other one.")]
        public static PropertyBasedImportDefinition CreateDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            string requiredTypeIdentityForMef,
            ImportCardinality cardinality,
            bool isRecomposable,
            CreationPolicy creationPolicy,
            PropertyInfo property)
        {
            return CreateDefinition(
                contractName,
                requiredTypeIdentity,
                requiredTypeIdentityForMef,
                cardinality,
                isRecomposable,
                creationPolicy,
                property,
                t => TypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PropertyBasedImportDefinition"/> class based on
        /// the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
        /// <param name="requiredTypeIdentityForMef">The type identity of the export type as expected by MEF.</param>
        /// <param name="cardinality">
        ///     One of the enumeration values that indicates the cardinality of the export object required by the import definition.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a parts;
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Recomposable",
            Justification = "MEF uses the same term, so we're not going to make up some other one.")]
        public static PropertyBasedImportDefinition CreateDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            string requiredTypeIdentityForMef,
            ImportCardinality cardinality,
            bool isRecomposable,
            CreationPolicy creationPolicy,
            PropertyInfo property,
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

            return new PropertyBasedImportDefinition(
                contractName,
                requiredTypeIdentity,
                requiredTypeIdentityForMef,
                cardinality,
                isRecomposable,
                creationPolicy,
                identityGenerator(property.DeclaringType),
                PropertyDefinition.CreateDefinition(property, identityGenerator));
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        private readonly PropertyDefinition _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBasedImportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
        /// <param name="requiredTypeIdentityForMef">The type identity of the export type as expected by MEF.</param>
        /// <param name="cardinality">
        ///     One of the enumeration values that indicates the cardinality of the export object required by the import definition.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a parts;
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="declaringType">The type that defines the property.</param>
        /// <param name="property">The property for which the current object stores the serialized data.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        private PropertyBasedImportDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            string requiredTypeIdentityForMef,
            ImportCardinality cardinality,
            bool isRecomposable,
            CreationPolicy creationPolicy,
            TypeIdentity declaringType,
            PropertyDefinition property)
            : base(
                contractName,
                requiredTypeIdentity,
                requiredTypeIdentityForMef,
                cardinality,
                isRecomposable,
                false,
                creationPolicy,
                declaringType)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            _property = property;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializableImportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializableImportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializableImportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool Equals(SerializableImportDefinition other)
        {
            var otherType = other as PropertyBasedImportDefinition;
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(otherType, null)
                && string.Equals(ContractName, otherType.ContractName, StringComparison.OrdinalIgnoreCase)
                && RequiredTypeIdentity.Equals(otherType.RequiredTypeIdentity)
                && string.Equals(RequiredTypeIdentityForMef, otherType.RequiredTypeIdentityForMef, StringComparison.OrdinalIgnoreCase)
                && Property == otherType.Property;
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

            var id = obj as PropertyBasedImportDefinition;
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
                hash = (hash * 23) ^ ContractName.GetHashCode();
                hash = (hash * 23) ^ RequiredTypeIdentity.GetHashCode();
                hash = (hash * 23) ^ RequiredTypeIdentityForMef.GetHashCode();
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
                "Importing [{0}] on {1}",
                ContractName,
                Property);
        }
    }
}
