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
    /// Defines a registration of a given object type for a group of plugin components.
    /// </summary>
    [Serializable]
    public sealed class GroupPartDefinition : IPartRegistration
    {
        /// <summary>
        /// The ID of the current registration.
        /// </summary>
        private readonly PartRegistrationId m_Id;

        /// <summary>
        /// The type of the object being registered.
        /// </summary>
        private readonly TypeIdentity m_Type;

        /// <summary>
        /// The index of the object indicating the number of object of the 
        /// current type that have been registered with the group before the current
        /// registration.
        /// </summary>
        private readonly int m_Index;

        /// <summary>
        /// The collection of export registrations for the current object.
        /// </summary>
        private readonly Dictionary<ExportRegistrationId, SerializableExportDefinition> m_Exports;

        /// <summary>
        /// The collection of import registrations for the current object.
        /// </summary>
        private readonly Dictionary<ImportRegistrationId, SerializableImportDefinition> m_Imports;

        /// <summary>
        /// The collection of schedule action registrations for the current object.
        /// </summary>
        private readonly Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition> m_Actions;

        /// <summary>
        /// The collection of schedule condition registrations for the current object.
        /// </summary>
        private readonly Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition> m_Conditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupPartDefinition"/> class.
        /// </summary>
        /// <param name="partType">The type of object for which this ID is valid.</param>
        /// <param name="number">The index of the object in the owning group.</param>
        /// <param name="exports">The collection of export registrations for the current object.</param>
        /// <param name="imports">The collection of import registrations for the current object.</param>
        /// <param name="actions">The collection of schedule action registrations for the current object.</param>
        /// <param name="conditions">The collection of schedule import registrations for the current object.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="partType"/> is <see langword="null" />.
        /// </exception>
        public GroupPartDefinition(
            TypeIdentity partType,
            int number,
            Dictionary<ExportRegistrationId, SerializableExportDefinition> exports,
            Dictionary<ImportRegistrationId, SerializableImportDefinition> imports,
            Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition> actions,
            Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition> conditions)
        {
            {
                Lokad.Enforce.Argument(() => partType);
            }

            m_Id = new PartRegistrationId(partType.AssemblyQualifiedName, number);
            m_Type = partType;
            m_Index = number;
            m_Exports = exports ?? new Dictionary<ExportRegistrationId, SerializableExportDefinition>();
            m_Imports = imports ?? new Dictionary<ImportRegistrationId, SerializableImportDefinition>();
            m_Actions = actions ?? new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>();
            m_Conditions = conditions ?? new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>();
        }

        /// <summary>
        /// Gets the ID of the current registration.
        /// </summary>
        public PartRegistrationId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the type of object that has been registered.
        /// </summary>
        public TypeIdentity Identity
        {
            get
            {
                return m_Type;
            }
        }

        /// <summary>
        /// Gets the index of the object that has been registered with the current type.
        /// </summary>
        public int Index
        {
            get
            {
                return m_Index;
            }
        }

        /// <summary>
        /// Gets the collection of exports that have been registered for the current object.
        /// </summary>
        public IEnumerable<ExportRegistrationId> RegisteredExports
        {
            get
            {
                return m_Exports.Keys;
            }
        }

        /// <summary>
        /// Gets the collection of imports that have been registered for the current object.
        /// </summary>
        public IEnumerable<ImportRegistrationId> RegisteredImports
        {
            get
            {
                return m_Imports.Keys;
            }
        }

        /// <summary>
        /// Gets the collection of schedule actions that have been registered for the current object. 
        /// </summary>
        public IEnumerable<ScheduleActionRegistrationId> RegisteredActions
        {
            get
            {
                return m_Actions.Keys;
            }
        }

        /// <summary>
        /// Gets the collection of schedule conditions that have been registered for the current object.
        /// </summary>
        public IEnumerable<ScheduleConditionRegistrationId> RegisteredConditions
        {
            get
            {
                return m_Conditions.Keys;
            }
        }

        /// <summary>
        /// Returns the export definition that was registered with the given ID.
        /// </summary>
        /// <param name="id">The ID of the export.</param>
        /// <returns>The requested export definition.</returns>
        public SerializableExportDefinition Export(ExportRegistrationId id)
        {
            return m_Exports[id];
        }

        /// <summary>
        /// Returns the import definition that was registered with the given ID.
        /// </summary>
        /// <param name="id">The ID of the import.</param>
        /// <returns>The requested import definition.</returns>
        public SerializableImportDefinition Import(ImportRegistrationId id)
        {
            return m_Imports[id];
        }

        /// <summary>
        /// Returns the action definition that was registered with the given ID.
        /// </summary>
        /// <param name="id">The ID of the action.</param>
        /// <returns>The requested action definition.</returns>
        public ScheduleActionDefinition Action(ScheduleActionRegistrationId id)
        {
            return m_Actions[id];
        }

        /// <summary>
        /// Returns the condition definition that was registered with the given ID.
        /// </summary>
        /// <param name="id">The ID of the condition.</param>
        /// <returns>The requested condition definition.</returns>
        public ScheduleConditionDefinition Condition(ScheduleConditionRegistrationId id)
        {
            return m_Conditions[id];
        }
    }
}
