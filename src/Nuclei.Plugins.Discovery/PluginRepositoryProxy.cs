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
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;

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
        private readonly IPluginRepository m_Repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginRepositoryProxy"/> class.
        /// </summary>
        /// <param name="repository">The object that stores all the parts and part groups.</param>
        public PluginRepositoryProxy(IPluginRepository repository)
        {
            {
                Debug.Assert(repository != null, "The repository object should not be a null reference.");
            }

            m_Repository = repository;
        }

        /// <summary>
        /// Returns a collection containing the descriptions of all the known plugins.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known plugins.
        /// </returns>
        public IEnumerable<PluginFileInfo> KnownPluginFiles()
        {
            return m_Repository.KnownPluginFiles();
        }

        /// <summary>
        /// Removes all the plugins related to the given plugin files.
        /// </summary>
        /// <param name="deletedFiles">The collection of plugin file paths that were removed.</param>
        public void RemovePlugins(IEnumerable<string> deletedFiles)
        {
            m_Repository.RemovePlugins(deletedFiles);
        }

        /// <summary>
        /// Adds a new type definition to the repository.
        /// </summary>
        /// <param name="type">The type definition.</param>
        public void AddType(TypeDefinition type)
        {
            m_Repository.AddType(type);
        }

        /// <summary>
        /// Adds a new part to the repository.
        /// </summary>
        /// <param name="part">The part definition.</param>
        /// <param name="pluginFileInfo">The file info of the assembly which owns the part.</param>
        public void AddPart(PartDefinition part, PluginFileInfo pluginFileInfo)
        {
            m_Repository.AddPart(part, pluginFileInfo);
        }

        /// <summary>
        /// Adds a new part group to the repository.
        /// </summary>
        /// <param name="group">The part group definition.</param>
        /// <param name="pluginFileInfo">The file info of the assembly which owns the group.</param>
        public void AddGroup(GroupDefinition group, PluginFileInfo pluginFileInfo)
        {
            m_Repository.AddGroup(group, pluginFileInfo);
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
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ContainsDefinitionForType(TypeIdentity type)
        {
            return m_Repository.ContainsDefinitionForType(type);
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
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ContainsDefinitionForType(string fullyQualifiedName)
        {
            return m_Repository.ContainsDefinitionForType(fullyQualifiedName);
        }

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        public TypeIdentity IdentityByName(string fullyQualifiedName)
        {
            return m_Repository.IdentityByName(fullyQualifiedName);
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByIdentity(TypeIdentity type)
        {
            return m_Repository.TypeByIdentity(type);
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the type with the given name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByName(string fullyQualifiedName)
        {
            return m_Repository.TypeByName(fullyQualifiedName);
        }

        /// <summary>
        /// Returns a value indicating whether the given <c>child</c> type is derived from the given <c>parent</c> type.
        /// </summary>
        /// <param name="parent">The parent type.</param>
        /// <param name="child">The child type.</param>
        /// <returns>
        /// <see langword="true" /> if the child derives from the given parent; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsSubTypeOf(TypeIdentity parent, TypeIdentity child)
        {
            return m_Repository.IsSubTypeOf(parent, child);
        }

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        public IEnumerable<PartDefinition> Parts()
        {
            return m_Repository.Parts();
        }

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part.</returns>
        public PartDefinition Part(TypeIdentity type)
        {
            return m_Repository.Part(type);
        }

        /// <summary>
        /// Returns a collection containing all known groups.
        /// </summary>
        /// <returns>The collection containing all known groups.</returns>
        public IEnumerable<GroupDefinition> Groups()
        {
            return m_Repository.Groups();
        }

        /// <summary>
        /// Returns the group that was registered with the given ID.
        /// </summary>
        /// <param name="groupRegistrationId">The registration ID.</param>
        /// <returns>The requested type.</returns>
        public GroupDefinition Group(GroupRegistrationId groupRegistrationId)
        {
            return m_Repository.Group(groupRegistrationId);
        }
    }
}
