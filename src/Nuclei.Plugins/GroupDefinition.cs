//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Apollo.Core.Extensions.Plugins;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Stores the serialized information for a plugin group.
    /// </summary>
    [Serializable]
    public sealed class GroupDefinition
    {
        /// <summary>
        /// The ID of the group.
        /// </summary>
        private readonly GroupRegistrationId m_Id;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDefinition"/> class.
        /// </summary>
        /// <param name="groupName">The unique name of the group.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groupName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="groupName"/> is an empty string.
        /// </exception>
        public GroupDefinition(string groupName)
        {
            {
                Lokad.Enforce.Argument(() => groupName);
                Lokad.Enforce.Argument(() => groupName, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Id = new GroupRegistrationId(groupName);
        }

        /// <summary>
        /// Gets the ID of the group.
        /// </summary>
        public GroupRegistrationId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets or sets the collection that contains all the part definitions for the current group.
        /// </summary>
        public IEnumerable<GroupPartDefinition> Parts
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection that maps the imports to the connected exports.
        /// </summary>
        public IEnumerable<PartImportToPartExportMap> InternalConnections
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the schedule for the current group.
        /// </summary>
        public ScheduleDefinition Schedule
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the export for this group.
        /// </summary>
        public GroupExportDefinition GroupExport
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of imports for this group.
        /// </summary>
        public IEnumerable<GroupImportDefinition> GroupImports
        {
            get;
            set;
        }
    }
}
