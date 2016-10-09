//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins.Core
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
        /// Returns the discoverable member that has the given method as declaring member.
        /// </summary>
        /// <param name="definition">The declaring method.</param>
        /// <returns>The requested discoverable member.</returns>
        MethodBasedDiscoverableMember DiscoverableMember(MethodDefinition definition);

        /// <summary>
        /// Returns the discoverable member that has the given property as declaring member.
        /// </summary>
        /// <param name="definition">The declaring property.</param>
        /// <returns>The requested discoverable member.</returns>
        PropertyBasedDiscoverableMember DiscoverableMember(PropertyDefinition definition);

        /// <summary>
        /// Returns the discoverable member that has the given type as declaring member.
        /// </summary>
        /// <param name="identity">The declaring type.</param>
        /// <returns>The requested discoverable member.</returns>
        TypeBasedDiscoverableMember DiscoverableMember(TypeIdentity identity);

        /// <summary>
        /// Returns a collection containing all known discoverable members.
        /// </summary>
        /// <returns>The collection containing all known discoverable members.</returns>
        IEnumerable<SerializableDiscoverableMemberDefinition> DiscoverableMembers();

        /// <summary>
        /// Returns a collection containing all known discoverable members which have the given key value pair in their
        /// metadata set.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>
        ///     The collection containing all known discoverable members with the given key-value pair in their given
        ///     metadata set.
        /// </returns>
        IEnumerable<SerializableDiscoverableMemberDefinition> DiscoverableMembersWithMetadata(string key, string value);

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        TypeIdentity IdentityByName(string fullyQualifiedName);

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
        bool IsSubtypeOf(TypeIdentity parent, TypeIdentity child);

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part.</returns>
        PartDefinition Part(TypeIdentity type);

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        IEnumerable<PartDefinition> Parts();

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
    }
}
