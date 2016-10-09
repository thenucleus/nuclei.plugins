//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines a proxy for <see cref="IPluginRepository"/> objects.
    /// </summary>
    /// <design>
    /// This class is meant to serve as a proxy for the real plugin repository in a remote <c>AppDomain</c>
    /// so that the <see cref="RemoteAssemblyScanner"/> is able to refer to the plugin repository without
    /// needing duplicates of the repository.
    /// </design>
    internal sealed class PluginRepositoryProxy : MarshalByRefObject, IPluginRepository
    {
        /// <summary>
        /// The object that stores all the part and part group information.
        /// </summary>
        private readonly IPluginRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginRepositoryProxy"/> class.
        /// </summary>
        /// <param name="repository">The object that stores all the parts and part groups.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        public PluginRepositoryProxy(IPluginRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        /// <summary>
        /// Adds a new discoverable member to the repository.
        /// </summary>
        /// <param name="member">The member that should be added.</param>
        /// <param name="pluginOrigin">The origin of the assembly that owns the discoverable member</param>
        public void AddDiscoverableMember(SerializableDiscoverableMemberDefinition member, PluginOrigin pluginOrigin)
        {
            _repository.AddDiscoverableMember(member, pluginOrigin);
        }

        /// <summary>
        /// Adds a new part to the repository.
        /// </summary>
        /// <param name="part">The part definition.</param>
        /// <param name="pluginOrigin">The origin of the assembly which owns the part.</param>
        public void AddPart(PartDefinition part, PluginOrigin pluginOrigin)
        {
            _repository.AddPart(part, pluginOrigin);
        }

        /// <summary>
        /// Adds a new type definition to the repository.
        /// </summary>
        /// <param name="type">The type definition.</param>
        public void AddType(TypeDefinition type)
        {
            _repository.AddType(type);
        }

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
        public bool ContainsDefinitionForType(string fullyQualifiedName)
        {
            return _repository.ContainsDefinitionForType(fullyQualifiedName);
        }

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
        public bool ContainsDefinitionForType(TypeIdentity type)
        {
            return _repository.ContainsDefinitionForType(type);
        }

        /// <summary>
        /// Returns the discoverable member that has the given method as declaring member.
        /// </summary>
        /// <param name="definition">The declaring method.</param>
        /// <returns>The requested discoverable member.</returns>
        public MethodBasedDiscoverableMember DiscoverableMember(MethodDefinition definition)
        {
            return _repository.DiscoverableMember(definition);
        }

        /// <summary>
        /// Returns the discoverable member that has the given property as declaring member.
        /// </summary>
        /// <param name="definition">The declaring property.</param>
        /// <returns>The requested discoverable member.</returns>
        public PropertyBasedDiscoverableMember DiscoverableMember(PropertyDefinition definition)
        {
            return _repository.DiscoverableMember(definition);
        }

        /// <summary>
        /// Returns the discoverable member that has the given type as declaring member.
        /// </summary>
        /// <param name="identity">The declaring type.</param>
        /// <returns>The requested discoverable member.</returns>
        public TypeBasedDiscoverableMember DiscoverableMember(TypeIdentity identity)
        {
            return _repository.DiscoverableMember(identity);
        }

        /// <summary>
        /// Returns a collection containing all known discoverable members.
        /// </summary>
        /// <returns>The collection containing all known discoverable members.</returns>
        public IEnumerable<SerializableDiscoverableMemberDefinition> DiscoverableMembers()
        {
            return _repository.DiscoverableMembers();
        }

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
        public IEnumerable<SerializableDiscoverableMemberDefinition> DiscoverableMembersWithMetadata(string key, string value)
        {
            return _repository.DiscoverableMembersWithMetadata(key, value);
        }

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        public TypeIdentity IdentityByName(string fullyQualifiedName)
        {
            return _repository.IdentityByName(fullyQualifiedName);
        }

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
        public bool IsSubtypeOf(TypeIdentity parent, TypeIdentity child)
        {
            return _repository.IsSubtypeOf(parent, child);
        }

        /// <summary>
        /// Returns a collection containing the descriptions of all the known plugins.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known plugins.
        /// </returns>
        public IEnumerable<PluginOrigin> KnownPluginOrigins()
        {
            return _repository.KnownPluginOrigins();
        }

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part.</returns>
        public PartDefinition Part(TypeIdentity type)
        {
            return _repository.Part(type);
        }

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        public IEnumerable<PartDefinition> Parts()
        {
            return _repository.Parts();
        }

        /// <summary>
        /// Removes all the plugins related to the given plugin origins.
        /// </summary>
        /// <param name="deletedPlugins">The collection of plugins that were removed.</param>
        public void RemovePlugins(IEnumerable<PluginOrigin> deletedPlugins)
        {
            _repository.RemovePlugins(deletedPlugins);
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByIdentity(TypeIdentity type)
        {
            return _repository.TypeByIdentity(type);
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the type with the given name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByName(string fullyQualifiedName)
        {
            return _repository.TypeByName(fullyQualifiedName);
        }
    }
}
