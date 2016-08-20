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
    /// Stores the state of the group composition in the dataset at a given point in time.
    /// </summary>
    [Serializable]
    public sealed class GroupCompositionState
    {
        /// <summary>
        /// The collection that contains the descriptions of all the known groups.
        /// </summary>
        private readonly IEnumerable<Tuple<GroupCompositionId, GroupDefinition>> m_Groups;
        
        /// <summary>
        /// The collection that contains all the known connections.
        /// </summary>
        private readonly IEnumerable<Tuple<GroupCompositionId, GroupImportDefinition, GroupCompositionId>> m_Connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCompositionState"/> class.
        /// </summary>
        /// <param name="groups">The collection that contains the descriptions of all the known groups.</param>
        /// <param name="connections">The collection that contains all the known connections.</param>
        public GroupCompositionState(
            IEnumerable<Tuple<GroupCompositionId, GroupDefinition>> groups,
            IEnumerable<Tuple<GroupCompositionId, GroupImportDefinition, GroupCompositionId>> connections)
        {
            {
                Lokad.Enforce.Argument(() => groups);
                Lokad.Enforce.Argument(() => connections);
            }

            m_Groups = groups;
            m_Connections = connections;
        }

        /// <summary>
        /// Gets a collection containing all the groups and their ID numbers.
        /// </summary>
        public IEnumerable<Tuple<GroupCompositionId, GroupDefinition>> Groups
        {
            get
            {
                return m_Groups;
            }
        }

        /// <summary>
        /// Gets a collection containing all the connections between the different groups.
        /// </summary>
        public IEnumerable<Tuple<GroupCompositionId, GroupImportDefinition, GroupCompositionId>> Connections
        {
            get 
            {
                return m_Connections;
            }
        }
    }
}
