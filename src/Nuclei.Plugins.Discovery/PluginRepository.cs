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
using Apollo.Core.Base.Plugins;
using Nuclei.Plugins;
using Apollo.Core.Host.Properties;
using QuickGraph;
using QuickGraph.Algorithms.RankedShortestPath;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Stores information about all the known type, part and group definitions.
    /// </summary>
    internal sealed class PluginRepository : IPluginRepository
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection that keeps track of all the known types and their definitions.
        /// </summary>
        private readonly Dictionary<TypeIdentity, TypeDefinition> m_Types
            = new Dictionary<TypeIdentity, TypeDefinition>();

        /// <summary>
        /// The graph that links the different types according to their inheritance order.
        /// </summary>
        /// <remarks>
        /// Note that edges run from the more derived type to the less derived type.
        /// </remarks>
        private readonly BidirectionalGraph<TypeIdentity, Edge<TypeIdentity>> m_TypeGraph
            = new BidirectionalGraph<TypeIdentity, Edge<TypeIdentity>>();

        /// <summary>
        /// The collection that keeps track of all the known parts and their definitions.
        /// </summary>
        private readonly Dictionary<TypeIdentity, Tuple<PartDefinition, PluginFileInfo>> m_Parts
            = new Dictionary<TypeIdentity, Tuple<PartDefinition, PluginFileInfo>>();

        /// <summary>
        /// The collection that keeps track of all the known groups and their definitions.
        /// </summary>
        private readonly Dictionary<GroupRegistrationId, Tuple<GroupDefinition, PluginFileInfo>> m_Groups
            = new Dictionary<GroupRegistrationId, Tuple<GroupDefinition, PluginFileInfo>>();

        /// <summary>
        /// The collection that keeps track of all the known plugin files.
        /// </summary>
        private readonly List<PluginFileInfo> m_PluginFiles
            = new List<PluginFileInfo>();

        /// <summary>
        /// Returns a collection containing the descriptions of all the known plugins.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known plugins.
        /// </returns>
        public IEnumerable<PluginFileInfo> KnownPluginFiles()
        {
            lock (m_Lock)
            {
                return m_PluginFiles.ToList();
            }
        }

        /// <summary>
        /// Removes all the plugins related to the given plugin files.
        /// </summary>
        /// <param name="deletedFiles">The collection of plugin file paths that were removed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="deletedFiles"/> is <see langword="null" />.
        /// </exception>
        public void RemovePlugins(IEnumerable<string> deletedFiles)
        {
            {
                Lokad.Enforce.Argument(() => deletedFiles);
            }

            lock (m_Lock)
            {
                var filesToDelete = m_PluginFiles
                    .Join(
                        deletedFiles,
                        pluginFile => pluginFile.Path,
                        filePath => filePath,
                        (pluginFile, filePath) => pluginFile)
                    .ToList();
                foreach (var file in filesToDelete)
                {
                    m_PluginFiles.Remove(file);
                }

                var groupsToDelete = m_Groups
                    .Join(
                        filesToDelete,
                        p => p.Value.Item2,
                        file => file,
                        (pair, file) => pair.Key)
                    .ToList();
                foreach (var group in groupsToDelete)
                {
                    m_Groups.Remove(group);
                }

                var typesToDelete = m_Parts
                    .Join(
                        filesToDelete,
                        p => p.Value.Item2,
                        file => file,
                        (pair, file) => pair.Key)
                    .ToList();
                foreach (var type in typesToDelete)
                {
                    m_Parts.Remove(type);
                    m_Types.Remove(type);
                    m_TypeGraph.RemoveVertex(type);
                }
            }
        }

        /// <summary>
        /// Adds a new type definition to the repository.
        /// </summary>
        /// <param name="type">The type definition.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DuplicateTypeDefinitionException">
        ///     Thrown if <paramref name="type"/> is already registered with the repository.
        /// </exception>
        public void AddType(TypeDefinition type)
        {
            lock (m_Lock)
            {
                {
                    Lokad.Enforce.Argument(() => type);
                    Lokad.Enforce.With<DuplicateTypeDefinitionException>(
                        !m_Types.ContainsKey(type.Identity),
                        Resources.Exceptions_Messages_DuplicateTypeDefinition);
                }

                AddTypeToGraph(type);
                m_Types.Add(type.Identity, type);
            }
        }

        private void AddTypeToGraph(TypeDefinition type)
        {
            var derivedTypes = m_Types
                .Where(
                    p =>
                    {
                        return p.Value.BaseInterfaces.Contains(type.Identity)
                            || ((p.Value.BaseType != null) && p.Value.BaseType.Equals(type.Identity))
                            || ((p.Value.GenericTypeDefinition != null) && p.Value.GenericTypeDefinition.Equals(type.Identity));
                    })
                .Select(p => p.Key)
                .ToList();

            m_TypeGraph.AddVertex(type.Identity);
            if ((type.BaseType != null) && m_TypeGraph.ContainsVertex(type.BaseType))
            {
                m_TypeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, type.BaseType));
            }

            if ((type.GenericTypeDefinition != null) && m_TypeGraph.ContainsVertex(type.GenericTypeDefinition))
            {
                m_TypeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, type.GenericTypeDefinition));
            }

            foreach (var baseInterface in type.BaseInterfaces)
            {
                if ((baseInterface != null) && m_TypeGraph.ContainsVertex(baseInterface))
                {
                    m_TypeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, baseInterface));
                }
            }

            foreach (var derivedType in derivedTypes)
            {
                m_TypeGraph.AddEdge(new Edge<TypeIdentity>(derivedType, type.Identity));
            }
        }

        /// <summary>
        /// Adds a new part to the repository.
        /// </summary>
        /// <param name="part">The part definition.</param>
        /// <param name="pluginFileInfo">The file info of the assembly which owns the part.</param>
        public void AddPart(PartDefinition part, PluginFileInfo pluginFileInfo)
        {
            lock (m_Lock)
            {
                {
                    Lokad.Enforce.Argument(() => part);
                    Lokad.Enforce.Argument(() => pluginFileInfo);
                    Lokad.Enforce.With<DuplicatePartDefinitionException>(
                        !m_Parts.ContainsKey(part.Identity), 
                        Resources.Exceptions_Messages_DuplicatePartDefinition);
                }

                m_Parts.Add(part.Identity, new Tuple<PartDefinition, PluginFileInfo>(part, pluginFileInfo));
                if (!m_PluginFiles.Contains(pluginFileInfo))
                {
                    m_PluginFiles.Add(pluginFileInfo);
                }
            }
        }

        /// <summary>
        /// Adds a new part group to the repository.
        /// </summary>
        /// <param name="group">The part group definition.</param>
        /// <param name="pluginFileInfo">The file info of the assembly which owns the group.</param>
        public void AddGroup(GroupDefinition group, PluginFileInfo pluginFileInfo)
        {
            lock (m_Lock)
            {
                {
                    Lokad.Enforce.Argument(() => group);
                    Lokad.Enforce.Argument(() => pluginFileInfo);
                    Lokad.Enforce.With<DuplicateGroupDefinitionException>(
                        !m_Groups.ContainsKey(group.Id),
                        Resources.Exceptions_Messages_DuplicateGroupDefinition);
                }

                m_Groups.Add(group.Id, new Tuple<GroupDefinition, PluginFileInfo>(group, pluginFileInfo));
                if (!m_PluginFiles.Contains(pluginFileInfo))
                {
                    m_PluginFiles.Add(pluginFileInfo);
                }
            }
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
            lock (m_Lock)
            {
                return (type != null) && m_Types.ContainsKey(type);
            }
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
            lock (m_Lock)
            {
                return !string.IsNullOrWhiteSpace(fullyQualifiedName) && m_Types.Any(p => p.Key.AssemblyQualifiedName.Equals(fullyQualifiedName));
            }
        }

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        public TypeIdentity IdentityByName(string fullyQualifiedName)
        {
            {
                Lokad.Enforce.Argument(() => fullyQualifiedName);
                Lokad.Enforce.Argument(() => fullyQualifiedName, Lokad.Rules.StringIs.NotEmpty);
            }

            lock (m_Lock)
            {
                return m_Types.Where(p => p.Key.AssemblyQualifiedName.Equals(fullyQualifiedName)).Select(p => p.Key).FirstOrDefault();
            }
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByIdentity(TypeIdentity type)
        {
            lock (m_Lock)
            {
                {
                    Lokad.Enforce.Argument(() => type);
                    Lokad.Enforce.With<UnknownTypeDefinitionException>(
                        m_Types.ContainsKey(type),
                        Resources.Exceptions_Messages_UnknownTypeDefinition);
                }

                return m_Types[type];
            }
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the type with the given name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByName(string fullyQualifiedName)
        {
            lock (m_Lock)
            {
                var typeIdentity = IdentityByName(fullyQualifiedName);
                return TypeByIdentity(typeIdentity);
            }
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
            var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<TypeIdentity, Edge<TypeIdentity>>(
                m_TypeGraph,
                e => 1.0);

            algorithm.ShortestPathCount = 10;
            algorithm.Compute(child, parent);
            return algorithm.ComputedShortestPathCount > 0;
        }

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        public IEnumerable<PartDefinition> Parts()
        {
            lock (m_Lock)
            {
                return m_Parts.Select(p => p.Value.Item1).ToList();
            }
        }

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part.</returns>
        public PartDefinition Part(TypeIdentity type)
        {
            lock (m_Lock)
            {
                {
                    Lokad.Enforce.Argument(() => type);
                    Lokad.Enforce.With<UnknownPartDefinitionException>(
                        m_Parts.ContainsKey(type),
                        Resources.Exceptions_Messages_UnknownPartDefinition);
                }

                return m_Parts[type].Item1;
            }
        }

        /// <summary>
        /// Returns a collection containing all known groups.
        /// </summary>
        /// <returns>The collection containing all known groups.</returns>
        public IEnumerable<GroupDefinition> Groups()
        {
            lock (m_Lock)
            {
                return m_Groups.Select(p => p.Value.Item1).ToList();
            }
        }

        /// <summary>
        /// Returns the group that was registered with the given ID.
        /// </summary>
        /// <param name="groupRegistrationId">The registration ID.</param>
        /// <returns>The requested type.</returns>
        public GroupDefinition Group(GroupRegistrationId groupRegistrationId)
        {
            lock (m_Lock)
            {
                {
                    Lokad.Enforce.Argument(() => groupRegistrationId);
                    Lokad.Enforce.With<UnknownGroupDefinitionException>(
                        m_Groups.ContainsKey(groupRegistrationId),
                        Resources.Exceptions_Messages_UnknownGroupDefinition);
                }

                return m_Groups[groupRegistrationId].Item1;
            }
        }
    }
}
