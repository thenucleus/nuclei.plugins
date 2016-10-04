﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Plugins.Core.Properties;
using QuickGraph;
using QuickGraph.Algorithms.RankedShortestPath;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Stores information about all the known type, part and group definitions.
    /// </summary>
    public sealed class PluginRepository : IPluginRepository
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The collection that keeps track of all the known parts and their definitions.
        /// </summary>
        private readonly Dictionary<TypeIdentity, Tuple<PartDefinition, PluginOrigin>> _parts
            = new Dictionary<TypeIdentity, Tuple<PartDefinition, PluginOrigin>>();

        /// <summary>
        /// The collection that keeps track of all the known plugin files.
        /// </summary>
        private readonly List<PluginOrigin> _pluginFiles
            = new List<PluginOrigin>();

        /// <summary>
        /// The collection that keeps track of all the known types and their definitions.
        /// </summary>
        private readonly Dictionary<TypeIdentity, TypeDefinition> _types
            = new Dictionary<TypeIdentity, TypeDefinition>();

        /// <summary>
        /// The graph that links the different types according to their inheritance order.
        /// </summary>
        /// <remarks>
        /// Note that edges run from the more derived type to the less derived type.
        /// </remarks>
        private readonly BidirectionalGraph<TypeIdentity, Edge<TypeIdentity>> _typeGraph
            = new BidirectionalGraph<TypeIdentity, Edge<TypeIdentity>>();

        /// <summary>
        /// Adds a new part to the repository.
        /// </summary>
        /// <param name="part">The part definition.</param>
        /// <param name="pluginFileInfo">The file info of the assembly which owns the part.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="part"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="pluginFileInfo"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="DuplicatePartDefinitionException">
        ///     Thrown if <paramref name="part"/> is already registered with the repository.
        /// </exception>
        public void AddPart(PartDefinition part, PluginOrigin pluginFileInfo)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }

            if (pluginFileInfo == null)
            {
                throw new ArgumentNullException("pluginFileInfo");
            }

            lock (_lock)
            {
                if (_parts.ContainsKey(part.Identity))
                {
                    throw new DuplicatePartDefinitionException();
                }

                _parts.Add(part.Identity, new Tuple<PartDefinition, PluginOrigin>(part, pluginFileInfo));
                if (!_pluginFiles.Contains(pluginFileInfo))
                {
                    _pluginFiles.Add(pluginFileInfo);
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
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            lock (_lock)
            {
                if (_types.ContainsKey(type.Identity))
                {
                    throw new DuplicateTypeDefinitionException();
                }

                AddTypeToGraph(type);
                _types.Add(type.Identity, type);
            }
        }

        private void AddTypeToGraph(TypeDefinition type)
        {
            var derivedTypes = _types
                .Where(
                    p =>
                    {
                        return p.Value.BaseInterfaces.Contains(type.Identity)
                            || ((p.Value.BaseType != null) && p.Value.BaseType.Equals(type.Identity))
                            || ((p.Value.GenericTypeDefinition != null) && p.Value.GenericTypeDefinition.Equals(type.Identity));
                    })
                .Select(p => p.Key)
                .ToList();

            _typeGraph.AddVertex(type.Identity);
            if ((type.BaseType != null) && _typeGraph.ContainsVertex(type.BaseType))
            {
                _typeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, type.BaseType));
            }

            if ((type.GenericTypeDefinition != null) && _typeGraph.ContainsVertex(type.GenericTypeDefinition))
            {
                _typeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, type.GenericTypeDefinition));
            }

            foreach (var baseInterface in type.BaseInterfaces)
            {
                if ((baseInterface != null) && _typeGraph.ContainsVertex(baseInterface))
                {
                    _typeGraph.AddEdge(new Edge<TypeIdentity>(type.Identity, baseInterface));
                }
            }

            foreach (var derivedType in derivedTypes)
            {
                _typeGraph.AddEdge(new Edge<TypeIdentity>(derivedType, type.Identity));
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
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ContainsDefinitionForType(string fullyQualifiedName)
        {
            lock (_lock)
            {
                return !string.IsNullOrWhiteSpace(fullyQualifiedName) && _types.Any(p => p.Key.AssemblyQualifiedName.Equals(fullyQualifiedName));
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
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool ContainsDefinitionForType(TypeIdentity type)
        {
            lock (_lock)
            {
                return (type != null) && _types.ContainsKey(type);
            }
        }

        /// <summary>
        /// Returns the identity for the type given by the name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name of the type.</param>
        /// <returns>The requested type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fullyQualifiedName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="fullyQualifiedName"/> is an empty string.
        /// </exception>
        public TypeIdentity IdentityByName(string fullyQualifiedName)
        {
            if (fullyQualifiedName == null)
            {
                throw new ArgumentNullException("fullyQualifiedName");
            }

            if (string.IsNullOrWhiteSpace(fullyQualifiedName))
            {
                throw new ArgumentException(Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString, "fullyQualifiedName");
            }

            lock (_lock)
            {
                return _types.Where(p => p.Key.AssemblyQualifiedName.Equals(fullyQualifiedName)).Select(p => p.Key).FirstOrDefault();
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
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsSubtypeOf(TypeIdentity parent, TypeIdentity child)
        {
            if (!ContainsDefinitionForType(parent))
            {
                return false;
            }

            if (!ContainsDefinitionForType(child))
            {
                return false;
            }

            var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<TypeIdentity, Edge<TypeIdentity>>(
                _typeGraph,
                e => 1.0);

            algorithm.ShortestPathCount = 10;
            algorithm.Compute(child, parent);
            return algorithm.ComputedShortestPathCount > 0;
        }

        /// <summary>
        /// Returns a collection containing the descriptions of all the known plugins.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known plugins.
        /// </returns>
        public IEnumerable<PluginOrigin> KnownPluginFiles()
        {
            lock (_lock)
            {
                return _pluginFiles.ToList();
            }
        }

        /// <summary>
        /// Returns the part that has the given type as declaring type.
        /// </summary>
        /// <param name="type">The declaring type.</param>
        /// <returns>The requested part.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownPartDefinitionException">
        ///     Thrown if there are no parts of the given <paramref name="type"/>.
        /// </exception>
        public PartDefinition Part(TypeIdentity type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            lock (_lock)
            {
                if (!_parts.ContainsKey(type))
                {
                    throw new UnknownPartDefinitionException();
                }

                return _parts[type].Item1;
            }
        }

        /// <summary>
        /// Returns a collection containing all known parts.
        /// </summary>
        /// <returns>The collection containing all known parts.</returns>
        public IEnumerable<PartDefinition> Parts()
        {
            lock (_lock)
            {
                return _parts.Select(p => p.Value.Item1).ToList();
            }
        }

        /// <summary>
        /// Removes all the plugins related to the given plugin files.
        /// </summary>
        /// <param name="deletedFiles">The collection of plugin file paths that were removed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="deletedFiles"/> is <see langword="null" />.
        /// </exception>
        public void RemovePlugins(IEnumerable<PluginOrigin> deletedFiles)
        {
            if (deletedFiles == null)
            {
                throw new ArgumentNullException("deletedFiles");
            }

            lock (_lock)
            {
                var filesToDelete = _pluginFiles
                    .Join(
                        deletedFiles,
                        pluginFile => pluginFile,
                        filePath => filePath,
                        (pluginFile, filePath) => pluginFile)
                    .ToList();
                foreach (var file in filesToDelete)
                {
                    _pluginFiles.Remove(file);
                }

                var typesToDelete = _parts
                    .Join(
                        filesToDelete,
                        p => p.Value.Item2,
                        file => file,
                        (pair, file) => pair.Key)
                    .ToList();
                foreach (var type in typesToDelete)
                {
                    _parts.Remove(type);
                    _types.Remove(type);
                    _typeGraph.RemoveVertex(type);
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The requested type definition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="type"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownTypeDefinitionException">
        ///     Thrown if no type with the given identity is known.
        /// </exception>
        public TypeDefinition TypeByIdentity(TypeIdentity type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            lock (_lock)
            {
                if (!_types.ContainsKey(type))
                {
                    throw new UnknownTypeDefinitionException();
                }

                return _types[type];
            }
        }

        /// <summary>
        /// Returns the <see cref="TypeDefinition"/> for the type with the given name.
        /// </summary>
        /// <param name="fullyQualifiedName">The fully qualified name for the type.</param>
        /// <returns>The requested type definition.</returns>
        public TypeDefinition TypeByName(string fullyQualifiedName)
        {
            lock (_lock)
            {
                var typeIdentity = IdentityByName(fullyQualifiedName);
                return TypeByIdentity(typeIdentity);
            }
        }
    }
}