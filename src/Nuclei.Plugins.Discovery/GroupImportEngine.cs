//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Provides helper method used to match group imports with group exports.
    /// </summary>
    internal sealed class GroupImportEngine : IConnectGroups
    {
        /// <summary>
        /// The object that stores information about all the available parts and part groups.
        /// </summary>
        private readonly ISatisfyPluginRequests m_Repository;
        
        /// <summary>
        /// The object that matches part imports with part exports.
        /// </summary>
        private readonly IConnectParts m_PartImportEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupImportEngine"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the available parts and part groups.</param>
        /// <param name="partImportEngine">The object that matches part imports with part exports.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="partImportEngine"/> is <see langword="null" />.
        /// </exception>
        public GroupImportEngine(ISatisfyPluginRequests repository, IConnectParts partImportEngine)
        {
            {
                Lokad.Enforce.Argument(() => repository);
                Lokad.Enforce.Argument(() => partImportEngine);
            }

            m_Repository = repository;
            m_PartImportEngine = partImportEngine;
        }
        
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
        public bool Accepts(GroupImportDefinition importDefinition, GroupExportDefinition exportDefinition)
        {
            if (importDefinition == null)
            {
                return true;
            }

            if (!string.Equals(importDefinition.ContractName, exportDefinition.ContractName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var importingGroup = m_Repository.Group(importDefinition.ContainingGroup);
            var exportingGroup = m_Repository.Group(exportDefinition.ContainingGroup);

            var isMatch = true;
            if (exportingGroup.Schedule != null)
            {
                isMatch = importDefinition.ScheduleInsertPosition != null;
            }

            if (isMatch)
            {
                var imports = importDefinition.ImportsToMatch.Select(i => importingGroup.Parts.PartImportById(i));
                var exports = exportDefinition.ProvidedExports.Select(e => exportingGroup.Parts.PartExportById(e));
                foreach (var import in imports)
                {
                    var foundMatch = false;
                    foreach (var export in exports)
                    {
                        if (m_PartImportEngine.Accepts(import, export))
                        {
                            foundMatch = true;
                        }
                    }

                    isMatch = isMatch && foundMatch;
                    if (!isMatch)
                    {
                        break;
                    }
                }
            }

            return isMatch;
        }
        
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
        public bool ExportPassesSelectionCriteria(GroupExportDefinition exportDefinition, IDictionary<string, object> selectionCriteria)
        {
            if (selectionCriteria == null || selectionCriteria.Count == 0)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Returns a collection containing all the groups provide an export which matches the given selection criteria.
        /// </summary>
        /// <param name="selectionCriteria">The collection containing all the selection criteria.</param>
        /// <returns>A collection containing all the groups that provide an export which matches the selection criteria.</returns>
        public IEnumerable<GroupDefinition> MatchingGroups(IDictionary<string, object> selectionCriteria)
        {
            return MatchingGroups(null, selectionCriteria);
        }

        /// <summary>
        /// Returns a collection containing all the groups which provide an export that can satisfy the given group import.
        /// </summary>
        /// <param name="groupToLinkTo">The import definition which should be satisfied.</param>
        /// <returns>A collection containing all the groups which satisfy the import condition.</returns>
        public IEnumerable<GroupDefinition> MatchingGroups(GroupImportDefinition groupToLinkTo)
        {
            return MatchingGroups(groupToLinkTo, null);
        }

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
        public IEnumerable<GroupDefinition> MatchingGroups(GroupImportDefinition groupToLinkTo, IDictionary<string, object> selectionCriteria)
        {
            return m_Repository.Groups()
                .Where(g => Accepts(groupToLinkTo, g.GroupExport) && ExportPassesSelectionCriteria(g.GroupExport, selectionCriteria));
        }

        /// <summary>
        /// Creates the connection data that describes how the importing group and the exporting group should be connected.
        /// </summary>
        /// <param name="importingGroup">The ID of the group containing the import.</param>
        /// <param name="importDefinition">The import definition.</param>
        /// <param name="exportingGroup">The ID of the group containing the export.</param>
        /// <returns>A collection that describes how the parts of the importing and exporting group should be connected.</returns>
        public IEnumerable<PartImportToPartExportMap> GenerateConnectionFor(
            GroupDefinition importingGroup,
            GroupImportDefinition importDefinition,
            GroupDefinition exportingGroup)
        {
            if ((importingGroup == null) || (importDefinition == null) || (exportingGroup == null))
            {
                return Enumerable.Empty<PartImportToPartExportMap>();
            }

            var importDefinitions = importDefinition
                .ImportsToMatch
                .Select(id => new Tuple<ImportRegistrationId, SerializableImportDefinition>(id, importingGroup.Parts.PartImportById(id)));
            var exportDefinitions = exportingGroup
                .GroupExport
                .ProvidedExports
                .Select(id => new Tuple<ExportRegistrationId, SerializableExportDefinition>(id, exportingGroup.Parts.PartExportById(id)))
                .ToList();

            var parts = new List<PartImportToPartExportMap>();
            foreach (var pair in importDefinitions)
            {
                var matchedExports = new List<Tuple<ExportRegistrationId, SerializableExportDefinition>>();
                foreach (var export in exportDefinitions)
                {
                    if (m_PartImportEngine.Accepts(pair.Item2, export.Item2))
                    {
                        matchedExports.Add(export);
                    }
                }

                parts.Add(new PartImportToPartExportMap(pair.Item1, matchedExports.Select(p => p.Item1).ToList()));
                foreach (var export in matchedExports)
                {
                    exportDefinitions.Remove(export);
                }
            }

            return parts;
        }
    }
}
