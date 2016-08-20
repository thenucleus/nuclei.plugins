//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Apollo.Core.Base.Plugins;
using QuickGraph;

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Defines an <see cref="Edge{T}"/> that links two part groups in the <see cref="CompositionLayer"/>.
    /// </summary>
    internal sealed class GroupCompositionGraphEdge : Edge<GroupCompositionId>
    {
        /// <summary>
        /// The import definition to which the connection points.
        /// </summary>
        private readonly GroupImportDefinition m_Import;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCompositionGraphEdge"/> class.
        /// </summary>
        /// <param name="importingGroup">The ID of the importing group.</param>
        /// <param name="importDefinition">The import definition.</param>
        /// <param name="exportingGroup">The ID of the exporting group.</param>
        public GroupCompositionGraphEdge(
            GroupCompositionId importingGroup, 
            GroupImportDefinition importDefinition, 
            GroupCompositionId exportingGroup)
            : base(exportingGroup, importingGroup)
        {
            {
                Debug.Assert(importDefinition != null, "The import definition should not be a null reference.");
            }

            m_Import = importDefinition;
        }

        /// <summary>
        /// Gets the import definition for which the current edge defines the connection.
        /// </summary>
        public GroupImportDefinition Import
        {
            get
            {
                return m_Import;
            }
        }
    }
}
