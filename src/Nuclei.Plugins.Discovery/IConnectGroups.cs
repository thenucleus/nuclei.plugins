//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Plugins;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the interface for objects that assist in connecting part groups.
    /// </summary>
    internal interface IConnectGroups
    {
        /// <summary>
        /// Returns a value indicating if the given import would accept the given export.
        /// </summary>
        /// <param name="importDefinition">The import definition.</param>
        /// <param name="exportDefinition">The export definition.</param>
        /// <returns>
        ///     <see langword="true" /> if the given import would accept the given export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Accepts(GroupImportDefinition importDefinition, GroupExportDefinition exportDefinition);

        /// <summary>
        /// Returns a value indicating if the given export matches the provided selection criteria.
        /// </summary>
        /// <param name="exportDefinition">The export definition.</param>
        /// <param name="selectionCriteria">The collection containing all the selection criteria.</param>
        /// <returns>
        ///     <see langword="true" /> if the given export passes the selection criteria; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ExportPassesSelectionCriteria(GroupExportDefinition exportDefinition, IDictionary<string, object> selectionCriteria);

        /// <summary>
        /// Returns a collection containing all the groups provide an export which matches the given selection criteria.
        /// </summary>
        /// <param name="selectionCriteria">The collection containing all the selection criteria.</param>
        /// <returns>A collection containing all the groups that provide an export which matches the selection criteria.</returns>
        IEnumerable<GroupDefinition> MatchingGroups(IDictionary<string, object> selectionCriteria);

        /// <summary>
        /// Returns a collection containing all the groups which provide an export that can satisfy the given group import.
        /// </summary>
        /// <param name="groupToLinkTo">The import definition which should be satisfied.</param>
        /// <returns>A collection containing all the groups which satisfy the import condition.</returns>
        IEnumerable<GroupDefinition> MatchingGroups(GroupImportDefinition groupToLinkTo);

        /// <summary>
        /// Returns a collection containing all the groups which satisfy the given group import and match the given
        /// selection criteria.
        /// </summary>
        /// <param name="groupToLinkTo">The import definition which should be satisfied.</param>
        /// <param name="selectionCriteria">The collection containing all the selection criteria.</param>
        /// <returns>
        /// A collection containing all the groups which satisfy the given group import and match the given
        /// selection criteria.
        /// </returns>
        IEnumerable<GroupDefinition> MatchingGroups(GroupImportDefinition groupToLinkTo, IDictionary<string, object> selectionCriteria);

        /// <summary>
        /// Creates the connection data that describes how the importing group and the exporting group should be connected.
        /// </summary>
        /// <param name="importingGroup">The definition of the group containing the import.</param>
        /// <param name="importDefinition">The import definition.</param>
        /// <param name="exportingGroup">The definition of the group containing the export.</param>
        /// <returns>A collection that describes how the parts of the importing and exporting group should be connected.</returns>
        IEnumerable<PartImportToPartExportMap> GenerateConnectionFor(
            GroupDefinition importingGroup, 
            GroupImportDefinition importDefinition,
            GroupDefinition exportingGroup);
    }
}
