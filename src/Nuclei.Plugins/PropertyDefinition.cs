//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Stores information about a property in a serializable form, i.e. without requiring the
    /// owning type to be loaded.
    /// </summary>
    [Serializable]
    public sealed class PropertyDefinition : IEquatable<PropertyDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PropertyDefinition first, PropertyDefinition second)
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
        public static bool operator !=(PropertyDefinition first, PropertyDefinition second)
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
        /// Creates a new instance of the <see cref="PropertyDefinition"/> class based on the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static PropertyDefinition CreateDefinition(
            PropertyInfo property,
            Func<Type, TypeIdentity> identityGenerator)
        {
            {
                Lokad.Enforce.Argument(() => property);
                Lokad.Enforce.Argument(() => identityGenerator);
            }

            return new PropertyDefinition(
                identityGenerator(property.DeclaringType),
                property.Name,
                identityGenerator(property.PropertyType));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MethodDefinition"/> class based on the given <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given property.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="property"/> is <see langword="null" />.
        /// </exception>
        public static PropertyDefinition CreateDefinition(PropertyInfo property)
        {
            return CreateDefinition(property, t => TypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// The type that owns the current method.
        /// </summary>
        private readonly TypeIdentity m_DeclaringType;

        /// <summary>
        /// The name of the property.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The type of the property.
        /// </summary>
        private readonly TypeIdentity m_PropertyType;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDefinition"/> class.
        /// </summary>
        /// <param name="declaringType">The object that stores the serialized identity for the declaring type of the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyType">The object that stores the serialized identity of the return type of the property.</param>
        private PropertyDefinition(TypeIdentity declaringType, string name, TypeIdentity propertyType)
        {
            {
                Debug.Assert(declaringType != null, "The declaring type object should not be null.");
                Debug.Assert(!string.IsNullOrEmpty(name), "The name should not be an empty string.");
                Debug.Assert(propertyType != null, "The property type object should not be null.");
            }

            m_DeclaringType = declaringType;
            m_Name = name;
            m_PropertyType = propertyType;
        }

        /// <summary>
        /// Gets the type that owns the current method.
        /// </summary>
        public TypeIdentity DeclaringType
        {
            get
            {
                return m_DeclaringType;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public TypeIdentity PropertyType
        {
            get
            {
                return m_PropertyType;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="PropertyDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="PropertyDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="PropertyDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(PropertyDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null)
                && DeclaringType.Equals(other.DeclaringType)
                && string.Equals(PropertyName, other.PropertyName, StringComparison.OrdinalIgnoreCase)
                && PropertyType.Equals(other.PropertyType);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as PropertyDefinition;
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
                hash = (hash * 23) ^ PropertyName.GetHashCode();
                hash = (hash * 23) ^ PropertyType.GetHashCode();
                
                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}.{2}",
                PropertyType,
                DeclaringType,
                PropertyName);
        }
    }
}
