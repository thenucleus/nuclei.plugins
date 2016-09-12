//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the interface for objects that handle plugin requests.
    /// </summary>
    public interface ISatisfyPluginRequests
    {
        /// <summary>
        /// Returns a value indicating if the repository contains a <see cref="TypeDefinition"/>
        /// for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <see langword="true" /> if the repository contains the <c>TypeDefinition</c> for the given type;
        /// otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ContainsDefinitionForType(TypeIdentity type);

        /// <summary>
        /// Returns a value indicating if the repository contains a <see cref="TypeDefinition"/>
        /// for the given type.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>
        /// <see langword="true" /> if the repository contains the <c>TypeDefinition</c> for the given type;
        /// otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ContainsDefinitionForType(string fullyQualifiedName);

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        TypeIdentity IdentityByName(string fullyQualifiedName);

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The requested type definition.</returns>
        TypeDefinition TypeByIdentity(TypeIdentity type);

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the type with the given name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>The requested type definition.</returns>
        TypeDefinition TypeByName(string fullyQualifiedName);

        /// <summary>
        /// Returns a value indicating whether the given <c>child</c> type is derived from the given <c>parent</c> type.
        /// </summary>
        /// <param name="parent">The parent type.</param>
        /// <param name="child">The child type.</param>
        /// <returns>
        /// <see langword="true" /> if the child derives from the given parent; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsSubTypeOf(TypeIdentity parent, TypeIdentity child);

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        IEnumerable<PartDefinition> Parts();

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part.</returns>
        PartDefinition Part(TypeIdentity type);
    }
}
