//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Stores information about a method in a serializable form, i.e. without requiring the
    /// owning type to be loaded.
    /// </summary>
    [Serializable]
    public sealed class MethodDefinition : IEquatable<MethodDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(MethodDefinition first, MethodDefinition second)
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
        public static bool operator !=(MethodDefinition first, MethodDefinition second)
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
        /// Creates a new instance of the <see cref="MethodDefinition"/> class based on the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method for which a serialized definition needs to be created.</param>
        /// <param name="identityGenerator">The function that creates type identities.</param>
        /// <returns>The serialized definition for the given method.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="method"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        public static MethodDefinition CreateDefinition(
            MethodInfo method,
            Func<Type, TypeIdentity> identityGenerator)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (identityGenerator == null)
            {
                throw new ArgumentNullException("identityGenerator");
            }

            return new MethodDefinition(
                identityGenerator(method.DeclaringType),
                method.Name,
                !method.ReturnType.Equals(typeof(void)) ? identityGenerator(method.ReturnType) : null,
                method.GetParameters().Select(p => ParameterDefinition.CreateDefinition(p, identityGenerator)).ToList());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MethodDefinition"/> class based on the given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method for which a serialized definition needs to be created.</param>
        /// <returns>The serialized definition for the given method.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="method"/> is <see langword="null" />.
        /// </exception>
        public static MethodDefinition CreateDefinition(MethodInfo method)
        {
            return CreateDefinition(method, t => TypeIdentity.CreateDefinition(t));
        }

        /// <summary>
        /// The type that owns the current method.
        /// </summary>
        private readonly TypeIdentity _declaringType;

        /// <summary>
        /// The name of the method.
        /// </summary>
        private readonly string _methodName;

        /// <summary>
        /// The return type of the method.
        /// </summary>
        private readonly TypeIdentity _returnType;

        /// <summary>
        /// The collection of parameters for the method.
        /// </summary>
        private readonly List<ParameterDefinition> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDefinition"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the method.</param>
        /// <param name="name">The name of the method.</param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameters">The collection containing the parameters for the method.</param>
        private MethodDefinition(
            TypeIdentity declaringType,
            string name,
            TypeIdentity returnType,
            List<ParameterDefinition> parameters)
        {
            {
                Debug.Assert(declaringType != null, "The declaring type should not be null.");
                Debug.Assert(!string.IsNullOrEmpty(name), "The name should not be an empty string");
                Debug.Assert(parameters != null, "The parameter array should not be null.");
            }

            _declaringType = declaringType;
            _methodName = name;
            _returnType = returnType;
            _parameters = parameters;
        }

        /// <summary>
        /// Gets the type that owns the current method.
        /// </summary>
        public TypeIdentity DeclaringType
        {
            get
            {
                return _declaringType;
            }
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        public string MethodName
        {
            get
            {
                return _methodName;
            }
        }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        public TypeIdentity ReturnType
        {
            get
            {
                return _returnType;
            }
        }

        /// <summary>
        /// Gets the collection containing the parameters for the method.
        /// </summary>
        public ReadOnlyCollection<ParameterDefinition> Parameters
        {
            get
            {
                return _parameters.AsReadOnly();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="MethodDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="MethodDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="MethodDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(MethodDefinition other)
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
                && string.Equals(MethodName, other.MethodName, StringComparison.OrdinalIgnoreCase)
                && Parameters.SequenceEqual(other.Parameters);
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

            var id = obj as MethodDefinition;
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
                hash = (hash * 23) ^ MethodName.GetHashCode();
                foreach (var parameter in Parameters)
                {
                    hash = (hash * 23) ^ parameter.GetHashCode();
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
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}.{2}({3})",
                ReturnType,
                DeclaringType,
                MethodName,
                string.Join(", ", Parameters.Select(p => p.ToString())));
        }
    }
}
