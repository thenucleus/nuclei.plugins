//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.Extensions.Plugins;
using QuickGraph;

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Defines an <see cref="Edge{T}"/> that links a part export to a part import.
    /// </summary>
    /// <typeparam name="T">The type of vertex that the current edge type creates a connection between.</typeparam>
    internal sealed class PartImportExportEdge<T> : Edge<T>
    {
        /// <summary>
        /// The registration ID of the import.
        /// </summary>
        private ImportRegistrationId m_Import;

        /// <summary>
        /// The registration ID of the export.
        /// </summary>
        private ExportRegistrationId m_Export;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartImportExportEdge{T}"/> class.
        /// </summary>
        /// <param name="importPart">The ID of the part that provides the import.</param>
        /// <param name="importId">The registration ID of the import.</param>
        /// <param name="exportPart">The ID of the part that provides the export.</param>
        /// <param name="exportId">The registration ID of the export.</param>
        public PartImportExportEdge(
            T importPart, 
            ImportRegistrationId importId, 
            T exportPart, 
            ExportRegistrationId exportId)
            : base(exportPart, importPart)
        {
            {
                Debug.Assert(importId != null, "The import ID should not be a null reference.");
                Debug.Assert(exportId != null, "The export ID should not be a null reference.");
            }

            m_Import = importId;
            m_Export = exportId;
        }

        /// <summary>
        /// Gets the registration ID of the import.
        /// </summary>
        public ImportRegistrationId ImportRegistration
        {
            get
            {
                return m_Import;
            }
        }

        /// <summary>
        /// Gets the registration ID of the export.
        /// </summary>
        public ExportRegistrationId ExportRegistration
        {
            get
            {
                return m_Export;
            }
        }
    }
}
