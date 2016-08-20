//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nuclei.Communication;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Defines the interface for objects that provide commands related to group composition.
    /// </summary>
    public interface ICompositionCommands : ICommandSet
    {
        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="id">The ID of the group.</param>
        /// <param name="group">The group that should be added to the graph.</param>
        /// <returns>A task that will finish when the action has completed.</returns>
        Task Add(GroupCompositionId id, GroupDefinition group);

        /// <summary>
        /// Removes the <see cref="GroupDefinition"/> related to the given <paramref name="id"/> from the graph.
        /// </summary>
        /// <param name="id">The ID of the group that should be removed.</param>
        /// <returns>A task that will finish when the action has completed.</returns>
        Task Remove(GroupCompositionId id);

        /// <summary>
        /// Connects a group import to a group export with the given part and schedule connections described
        /// by the <paramref name="connection"/> object.
        /// </summary>
        /// <param name="connection">The object that describes how the group import and the group export should be connected.</param>
        /// <returns>A task that will finish when the connection action has completed.</returns>
        Task Connect(GroupConnection connection);

        /// <summary>
        /// Disconnects the given groups from each other.
        /// </summary>
        /// <param name="importingGroup">The ID of the importing group.</param>
        /// <param name="exportingGroup">The ID of the exporting group.</param>
        /// <returns>A task that will finish when the disconnection action has completed.</returns>
        Task Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup);

        /// <summary>
        /// Disconnects the given group from all imports and exports.
        /// </summary>
        /// <param name="group">The group that should be disconnected.</param>
        /// <returns>A task that will finish when the disconnection action has completed.</returns>
        Task Disconnect(GroupCompositionId group);

        /// <summary>
        /// Returns a collection containing all the group imports that have not been satisfied.
        /// </summary>
        /// <param name="includeOptionalImports">A flag that indicates if the optional imports should be included.</param>
        /// <returns>
        /// A task that will return the collection of unsatisfied imports.
        /// </returns>
        Task<List<Tuple<GroupCompositionId, GroupImportDefinition>>> NonSatisfiedImports(bool includeOptionalImports);

        /// <summary>
        /// Returns an object containing the current state of the composition graph.
        /// </summary>
        /// <returns>A task that will return the current state of the composition graph.</returns>
        Task<GroupCompositionState> CurrentState();
    }
}
