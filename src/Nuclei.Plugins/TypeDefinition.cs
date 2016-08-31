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
using System.Linq;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Stores information about an type in a serializable form, i.e. without requiring the
    /// type in question to be loaded.
    /// </summary>
    [Serializable]
    public sealed class TypeDefinition : IEquatable<TypeDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TypeDefinition first, TypeDefinition second)
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
        public static bool operator !=(TypeDefinition first, TypeDefinition second)
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
        /// Creates a new instance of the <see cref="TypeDefinition"/> class based on the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static TypeDefinition CreateDefinition(Type type, Func<Type, TypeIdentity> identityGenerator)
        {
            {
                Lokad.Enforce.Argument(() => type);
                Lokad.Enforce.Argument(() => identityGenerator);
            }
            
            // Note that the following call may lead to a StackOverflow if the identityGenerator function 
            // isn't smart enough to verify if we're in the process of generating a TypeDefinition for a given
            // type. e.g. if we're handling System.Boolean we also have to process System.IComparable<System.Boolean>
            // which could lead to an infinite loop.
            var identity = TypeIdentity.CreateDefinition(type, identityGenerator);

            // Generic types that don't have a base generic type have themselves as their own generic type definition!
            // e.g. the generic type definition for IEnumerable<T> is ... IEnumerable<T>. So we only assign a 
            // generic type definition to a type that isn't one itself.
            return new TypeDefinition(
                identity,
                type.BaseType != null ? identityGenerator(type.BaseType) : null,
                type.GetInterfaces().Select(i => identityGenerator(i)).ToArray(),
                identity.IsGenericType && type != type.GetGenericTypeDefinition() ? identityGenerator(type.GetGenericTypeDefinition()) : null,
                type.IsClass,
                type.IsInterface);
        }

        /// <summary>
        /// The identity information for the type.
        /// </summary>
        private readonly TypeIdentity m_Identity;

        /// <summary>
        /// The base class for the current type. Is <c>null</c> if the current type
        /// doesn't have a base class.
        /// </summary>
        private readonly TypeIdentity m_Base;

        /// <summary>
        /// The interfaces that are inherited or implemented by the current type.
        /// </summary>
        private readonly TypeIdentity[] m_BaseInterfaces;

        /// <summary>
        /// The generic type definition for the current type if there is one; otherwise <see langword="null" />.
        /// </summary>
        private readonly TypeIdentity m_GenericTypeDefinition;

        /// <summary>
        /// A flag indicating if the current type is a class or not.
        /// </summary>
        private readonly bool m_IsClass;

        /// <summary>
        /// A flag indicating if the current type is an interface or not.
        /// </summary>
        private readonly bool m_IsInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDefinition"/> class.
        /// </summary>
        /// <param name="identity">The object that provides the serialized identity of the type.</param>
        /// <param name="baseType">
        /// The object that provides the serialized identity of the base class for the type. Maybe <see langword="null"/>.
        /// </param>
        /// <param name="baseInterfaces">The collection of identities for the implemented interfaces.</param>
        /// <param name="genericTypeDefinition">
        /// The generic type definition for the current type, or <see langword="null" /> if there is no generic type definition 
        /// for the current type.
        /// </param>
        /// <param name="isClass">Indicates if the type is a class.</param>
        /// <param name="isInterface">Indicates if a type is an interface.</param>
        private TypeDefinition(
            TypeIdentity identity,
            TypeIdentity baseType,
            TypeIdentity[] baseInterfaces,
            TypeIdentity genericTypeDefinition,
            bool isClass,
            bool isInterface)
        {
            {
                Debug.Assert(identity != null, "The identity object should not be null.");
                Debug.Assert(baseInterfaces != null, "The base interfaces array should not be null.");
            }

            m_Identity = identity;
            m_Base = baseType;
            m_BaseInterfaces = baseInterfaces;
            m_GenericTypeDefinition = genericTypeDefinition;
            m_IsClass = isClass;
            m_IsInterface = isInterface;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public TypeIdentity Identity
        {
            get
            {
                return m_Identity;
            }
        }

        /// <summary>
        /// Gets the base or parent type for the type.
        /// </summary>
        public TypeIdentity BaseType
        {
            get
            {
                return m_Base;
            }
        }

        /// <summary>
        /// Gets the type information for all the interfaces that are implemented by the current type.
        /// </summary>
        public IEnumerable<TypeIdentity> BaseInterfaces
        {
            get
            {
                return m_BaseInterfaces;
            }
        }

        /// <summary>
        /// Gets the identity of the type that is the generic type definition for the 
        /// current type, or <see langword="null" /> if the current type has no
        /// generic type definition.
        /// </summary>
        public TypeIdentity GenericTypeDefinition
        {
            get
            {
                return m_GenericTypeDefinition;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type is a class.
        /// </summary>
        public bool IsClass
        {
            get
            {
                return m_IsClass;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type is an interface.
        /// </summary>
        public bool IsInterface
        {
            get
            {
                return m_IsInterface;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="TypeDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="TypeDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="TypeDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(TypeDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) && Identity.Equals(other.Identity);
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

            var id = obj as TypeDefinition;
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
                hash = (hash * 23) ^ Identity.GetHashCode();
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
            return Identity.ToString();
        }
    }
}
