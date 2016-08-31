//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Base.Plugins;

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Defines the interface for objects that store part groups and their connections.
    /// </summary>
    internal interface IStoreGroupsAndConnections
    {
        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="id">The ID of the group that is being added.</param>
        /// <param name="group">The group that should be added to the graph.</param>
        void Add(GroupCompositionId id, GroupDefinition group);

        /// <summary>
        /// Removes the group that is related to the specified ID.
        /// </summary>
        /// <param name="group">The ID of the group that should be removed.</param>
        void Remove(GroupCompositionId group);

        /// <summary>
        /// Returns the collection of all known groups.
        /// </summary>
        /// <returns>The collection of all known groups.</returns>
        IEnumerable<GroupCompositionId> Groups();

        /// <summary>
        /// Returns the <see cref="GroupDefinition"/> that was registered with the given ID.
        /// </summary>
        /// <param name="id">The composition ID of the group.</param>
        /// <returns>The definition for the group with the given ID.</returns>
        GroupDefinition Group(GroupCompositionId id);

        /// <summary>
        /// Connects the exporting group with the importing group via the given import.
        /// </summary>
        /// <param name="connection">The object that describes how the group import and the group export should be connected.</param>
        void Connect(GroupConnection connection);

        /// <summary>
        /// Disconnects the exporting group from the importing group.
        /// </summary>
        /// <param name="importingGroup">The composition ID of the importing group.</param>
        /// <param name="exportingGroup">The composition ID of the exporting group.</param>
        void Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup);

        /// <summary>
        /// Disconnects the group from all imports and exports.
        /// </summary>
        /// <param name="group">The composition ID of the group.</param>
        void Disconnect(GroupCompositionId group);

        /// <summary>
        /// Returns a collection of all imports owned by the specified group that have been provided with an export.
        /// </summary>
        /// <param name="importOwner">The composition ID of the group that owns the imports.</param>
        /// <returns>A collection containing all the imports with the group ID of the group providing the connected export.</returns>
        IEnumerable<Tuple<GroupImportDefinition, GroupCompositionId>> SatisfiedImports(GroupCompositionId importOwner);

        /// <summary>
        /// Returns a collection of all imports owned by the specified group that have not been provided with an export.
        /// </summary>
        /// <param name="importOwner">The composition ID of the group that owns the imports.</param>
        /// <returns>A collection containing all imports that have not been provided with an export.</returns>
        IEnumerable<GroupImportDefinition> UnsatisfiedImports(GroupCompositionId importOwner);
    }
}
