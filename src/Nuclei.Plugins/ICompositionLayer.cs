//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Defines the interface for objects which store the graph of components groups that describes how the different
    /// groups are connected.
    /// </summary>
    public interface ICompositionLayer
    {
        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="group">The group that should be added to the graph.</param>
        /// <returns>
        /// A task which returns the ID for the group.
        /// </returns>
        Task<GroupCompositionId> Add(GroupDefinition group);

        /// <summary>
        /// Removes the group that is related to the specified ID.
        /// </summary>
        /// <param name="group">The ID of the group that should be removed.</param>
        /// <returns>A task which indicates when the removal has taken place.</returns>
        Task Remove(GroupCompositionId group);

        /// <summary>
        /// Returns a value indicating if the graph contains a group for the given ID.
        /// </summary>
        /// <param name="id">The ID for which the graph is searched.</param>
        /// <returns>
        ///     <see langword="true" /> if the graph contains a group with the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Contains(GroupCompositionId id);

        /// <summary>
        /// Returns the <see cref="GroupDefinition"/> which is related to the given ID.
        /// </summary>
        /// <param name="id">The ID of the requested group.</param>
        /// <returns>The requested group.</returns>
        GroupDefinition Group(GroupCompositionId id);

        /// <summary>
        /// Returns the collection of all known groups.
        /// </summary>
        /// <returns>The collection of all known groups.</returns>
        IEnumerable<GroupCompositionId> Groups();

        /// <summary>
        /// Connects the given export to the given import.
        /// </summary>
        /// <param name="importingGroup">The ID of the group that owns the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <param name="exportingGroup">The ID of the group that owns the export.</param>
        /// <returns>A task which indicates when the connection has taken place.</returns>
        Task Connect(GroupCompositionId importingGroup, GroupImportDefinition importDefinition, GroupCompositionId exportingGroup);

        /// <summary>
        /// Disconnects the two groups.
        /// </summary>
        /// <remarks>
        /// This method assumes that two groups will only be connected via one import - export relation. This
        /// method will remove all connections from the exporting group to the importing group.
        /// </remarks>
        /// <param name="importingGroup">The ID of the group that owns the import.</param>
        /// <param name="exportingGroup">The ID of the group that owns the export.</param>
        /// <returns>A task which indicates when the disconnection has taken place.</returns>
        Task Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup);

        /// <summary>
        /// Disconnects all connection to and from the given group.
        /// </summary>
        /// <param name="group">The ID of the group.</param>
        /// <returns>A task which indicates when the disconnection has taken place.</returns>
        Task Disconnect(GroupCompositionId group);

        /// <summary>
        /// Returns a value indicating if the given import is connected to anything.
        /// </summary>
        /// <param name="importingGroup">The ID of the group owning the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <returns>
        /// <see langword="true" /> if the import is connected to an export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsConnected(GroupCompositionId importingGroup, GroupImportDefinition importDefinition);

        /// <summary>
        /// Returns a value indicating if the given import is connected to the given export.
        /// </summary>
        /// <param name="importingGroup">The ID of the group owning the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <param name="exportingGroup">The ID of the group owning the export.</param>
        /// <returns>
        /// <see langword="true" /> if the import is connected to an export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsConnected(GroupCompositionId importingGroup, GroupImportDefinition importDefinition, GroupCompositionId exportingGroup);

        /// <summary>
        /// Returns the group information indicating which export the given import is connected to.
        /// </summary>
        /// <param name="importingGroup">The ID of the group owning the import.</param>
        /// <param name="importDefinition">The import.</param>
        /// <returns>The ID of the group the given import is connected to, if there is a connection; otherwise, <see langword="null" />.</returns>
        GroupCompositionId ConnectedTo(GroupCompositionId importingGroup, GroupImportDefinition importDefinition);
    }
}
