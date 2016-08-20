//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Stores the information that describes the connection between two different part groups.
    /// </summary>
    [Serializable]
    public sealed class GroupConnection
    {
        /// <summary>
        /// The ID of the group that provides the import.
        /// </summary>
        private readonly GroupCompositionId m_ImportingGroup;

        /// <summary>
        /// The ID of the group that provides the export.
        /// </summary>
        private readonly GroupCompositionId m_ExportingGroup;

        /// <summary>
        /// The import definition that should be connected to.
        /// </summary>
        private readonly GroupImportDefinition m_Import;

        /// <summary>
        /// The collection that contains the mapping of the part imports to the part exports.
        /// </summary>
        private readonly IEnumerable<PartImportToPartExportMap> m_PartConnections;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupConnection"/> class.
        /// </summary>
        /// <param name="importingGroup">The ID of the group that provides the import.</param>
        /// <param name="exportingGroup">The ID of the group that provides the export.</param>
        /// <param name="import">The import definition that should be connected to.</param>
        /// <param name="partConnections">The collection that contains the mapping of the part imports to the part exports.</param>
        public GroupConnection(
            GroupCompositionId importingGroup,
            GroupCompositionId exportingGroup,
            GroupImportDefinition import,
            IEnumerable<PartImportToPartExportMap> partConnections)
        {
            {
                Lokad.Enforce.Argument(() => importingGroup);
                Lokad.Enforce.Argument(() => exportingGroup);
                Lokad.Enforce.Argument(() => import);
                Lokad.Enforce.Argument(() => partConnections);
            }

            m_ImportingGroup = importingGroup;
            m_ExportingGroup = exportingGroup;
            m_Import = import;
            m_PartConnections = partConnections;
        }

        /// <summary>
        /// Gets the ID of the group that provides the import.
        /// </summary>
        public GroupCompositionId ImportingGroup 
        {
            get
            {
                return m_ImportingGroup;
            }
        }

        /// <summary>
        /// Gets the ID of the group that provides the export.
        /// </summary>
        public GroupCompositionId ExportingGroup 
        {
            get
            {
                return m_ExportingGroup;
            }
        }

        /// <summary>
        /// Gets the import definition that should be connected to.
        /// </summary>
        public GroupImportDefinition GroupImport 
        {
            get
            {
                return m_Import;
            }
        }

        /// <summary>
        /// Gets the collection that contains the mapping of the part imports to the part exports.
        /// </summary>
        public IEnumerable<PartImportToPartExportMap> PartConnections 
        {
            get
            {
                return m_PartConnections;
            }
        }
    }
}
