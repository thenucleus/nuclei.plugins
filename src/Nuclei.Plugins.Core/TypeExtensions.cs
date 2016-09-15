//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines helper and extension methods for <see cref="TypeIdentity"/> and <see cref="TypeDefinition"/> instances.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// A collection that caches type identity objects for the four standard generic types
        /// which MEF can automatically convert to.
        /// </summary>
        /// <design>
        /// We store this mapping so that we only have to create the instances of the different
        /// generic types once. Creating one shouldn't be very costly but for each type there is a
        /// possibility of some sub-types being created (for the generic parameters etc.). And given
        /// that these elements never change we can pre-create them and store them.
        /// </design>
        private static readonly IDictionary<Type, TypeIdentity> _specialCasesCache
            = new Dictionary<Type, TypeIdentity>
            {
                { typeof(IEnumerable<>), TypeIdentity.CreateDefinition(typeof(IEnumerable<>)) },
                { typeof(Lazy<>), TypeIdentity.CreateDefinition(typeof(Lazy<>)) },
                { typeof(Lazy<,>), TypeIdentity.CreateDefinition(typeof(Lazy<,>)) },
                { typeof(Func<>), TypeIdentity.CreateDefinition(typeof(Func<>)) },
                { typeof(Func<,>), TypeIdentity.CreateDefinition(typeof(Func<,>)) },
                { typeof(Func<,,>), TypeIdentity.CreateDefinition(typeof(Func<,,>)) },
                { typeof(Func<,,,>), TypeIdentity.CreateDefinition(typeof(Func<,,,>)) },
                { typeof(Action<>), TypeIdentity.CreateDefinition(typeof(Action<>)) },
                { typeof(Action<,>), TypeIdentity.CreateDefinition(typeof(Action<,>)) },
                { typeof(Action<,,>), TypeIdentity.CreateDefinition(typeof(Action<,,>)) },
                { typeof(Action<,,,>), TypeIdentity.CreateDefinition(typeof(Action<,,,>)) },
            };

        /// <summary>
        /// Returns a value indicating if a type is based on a given open generic type.
        /// </summary>
        /// <param name="openGeneric">The open generic type, e.g. List{T}.</param>
        /// <param name="type">The type that may or may not be based on the open generic type.</param>
        /// <param name="toDefinition">The function that translates a <see cref="TypeIdentity"/> to a <see cref="TypeDefinition"/>.</param>
        /// <returns>
        ///     <see langword="true" /> if the type is based on the given open generic type; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool OpenGenericIsAssignableFrom(
            TypeIdentity openGeneric,
            TypeDefinition type,
            Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            // Terminate recursion
            if ((type == null) || openGeneric.Equals(type))
            {
                return false;
            }

            // typeToCheck is a closure of openGenericType
            var isClosureOfGenericType = type.Identity.IsGenericType && openGeneric.Equals(type.GenericTypeDefinition);

            // typeToCheck is the subclass of a closure of openGenericType
            var isSubClassOfClosure = OpenGenericIsAssignableFrom(openGeneric, toDefinition(type.BaseType), toDefinition);

            // typeToCheck inherits from an interface which is the closure of openGenericType
            var inheritsClosureInterface = type.BaseInterfaces.Any(
                interfaceType => OpenGenericIsAssignableFrom(openGeneric, toDefinition(interfaceType), toDefinition));

            return isClosureOfGenericType || isSubClassOfClosure || inheritsClosureInterface;
        }

        /// <summary>
        /// Returns a value indicating if the given type is based on the <see cref="Lazy{T}"/> or
        /// <see cref="Lazy{T, TMetadata}"/> open generic types.
        /// </summary>
        /// <param name="importType">The type that may or may not be based on the open generic type.</param>
        /// <param name="toDefinition">The function that translates a <see cref="TypeIdentity"/> to a <see cref="TypeDefinition"/>.</param>
        /// <returns>
        ///     <see langword="true" /> if the type is based on the <see cref="Lazy{T}"/> or <see cref="Lazy{T, TMetadata}"/> open generic type;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsLazy(this TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Lazy<>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Lazy<,>)], importType, toDefinition);
        }

        /// <summary>
        /// Returns a value indicating if the given type is based on the <see cref="Func{T}"/>, <see cref="Func{T1, TResult}"/>,
        /// <see cref="Func{T1, T2, TResult}"/> or <see cref="Func{T1, T2, T3, TResult}"/> open generic types.
        /// </summary>
        /// <param name="importType">The type that may or may not be based on the open generic type.</param>
        /// <param name="toDefinition">The function that translates a <see cref="TypeIdentity"/> to a <see cref="TypeDefinition"/>.</param>
        /// <returns>
        ///     <see langword="true" /> if the type is based on the <see cref="Func{T}"/>, <see cref="Func{T1, TResult}"/>,
        /// <see cref="Func{T1, T2, TResult}"/> or <see cref="Func{T1, T2, T3, TResult}"/> open generic type; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsFunc(this TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Func<>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Func<,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Func<,,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Func<,,,>)], importType, toDefinition);
        }

        /// <summary>
        /// Returns a value indicating if the given type is based on the <see cref="Action{T}"/>, <see cref="Action{T1, T2}"/>,
        /// <see cref="Action{T1, T2, T3}"/> or <see cref="Action{T1, T2, T3, T4}"/> open generic types.
        /// </summary>
        /// <param name="importType">The type that may or may not be based on the open generic type.</param>
        /// <param name="toDefinition">The function that translates a <see cref="TypeIdentity"/> to a <see cref="TypeDefinition"/>.</param>
        /// <returns>
        ///     <see langword="true" /> if the type is based on the <see cref="Action{T}"/>, <see cref="Action{T1, T2}"/>,
        /// <see cref="Action{T1, T2, T3}"/> or <see cref="Action{T1, T2, T3, T4}"/> open generic type; otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsAction(this TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Action<>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Action<,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Action<,,>)], importType, toDefinition)
                || OpenGenericIsAssignableFrom(_specialCasesCache[typeof(Action<,,,>)], importType, toDefinition);
        }

        /// <summary>
        /// Returns a value indicating if the given type is based on the <see cref="IEnumerable{T}"/> open generic type.
        /// </summary>
        /// <param name="importType">The type that may or may not be based on the open generic type.</param>
        /// <param name="toDefinition">The function that translates a <see cref="TypeIdentity"/> to a <see cref="TypeDefinition"/>.</param>
        /// <returns>
        ///     <see langword="true" /> if the type is based on the <see cref="IEnumerable{T}"/> open generic type;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsCollection(this TypeDefinition importType, Func<TypeIdentity, TypeDefinition> toDefinition)
        {
            return OpenGenericIsAssignableFrom(_specialCasesCache[typeof(IEnumerable<>)], importType, toDefinition);
        }
    }
}
