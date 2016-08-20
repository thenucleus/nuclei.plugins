//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Provides methods for the creation of one or more part groups.
    /// </summary>
    internal sealed class GroupDefinitionBuilder : IRegisterGroupDefinitions, IOwnScheduleDefinitions
    {
        /// <summary>
        /// The function that creates a schedule builder.
        /// </summary>
        private readonly Func<IBuildFixedSchedules> m_BuilderGenerator;

        /// <summary>
        /// The function that generates type identities.
        /// </summary>
        private readonly Func<Type, TypeIdentity> m_IdentityGenerator;

        /// <summary>
        /// The method that is used to store the generated group definitions.
        /// </summary>
        private readonly IPluginRepository m_Repository;

        /// <summary>
        /// The object that matches part imports with part exports.
        /// </summary>
        private readonly IConnectParts m_ImportEngine;

        /// <summary>
        /// The object that stores the file information for the assembly that contains
        /// the group exporter.
        /// </summary>
        private readonly PluginFileInfo m_FileInfo;
       
        /// <summary>
        /// The collection that holds the objects that have been registered for the
        /// current group.
        /// </summary>
        private Dictionary<Type, List<GroupPartDefinition>> m_Objects
            = new Dictionary<Type, List<GroupPartDefinition>>();

        /// <summary>
        /// The collection that holds the connections for the current group.
        /// </summary>
        private Dictionary<ImportRegistrationId, List<ExportRegistrationId>> m_Connections
            = new Dictionary<ImportRegistrationId, List<ExportRegistrationId>>();

        /// <summary>
        /// The collection of actions that are registered for all the schedules in the part group.
        /// </summary>
        private Dictionary<ScheduleActionRegistrationId, ScheduleElementId> m_Actions
            = new Dictionary<ScheduleActionRegistrationId, ScheduleElementId>();

        /// <summary>
        /// The collection of conditions that are registered for all the schedules in the part group.
        /// </summary>
        private Dictionary<ScheduleConditionRegistrationId, ScheduleElementId> m_Conditions
            = new Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>();

        /// <summary>
        /// The schedule for the current group.
        /// </summary>
        private ISchedule m_Schedule;

        /// <summary>
        /// The export for the group.
        /// </summary>
        private GroupExportMap m_GroupExport;

        /// <summary>
        /// The collection that holds all the imports for the group.
        /// </summary>
        private Dictionary<string, GroupImportMap> m_GroupImports
            = new Dictionary<string, GroupImportMap>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDefinitionBuilder"/> class.
        /// </summary>
        /// <param name="repository">The object that stores information about all the known parts and part groups.</param>
        /// <param name="importEngine">The object that matches part imports with part exports.</param>
        /// <param name="identityGenerator">The function that generates type identity objects.</param>
        /// <param name="builderGenerator">The function that is used to create schedule builders.</param>
        /// <param name="fileInfo">The file info for the assembly that owns the group exporter.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="importEngine"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="identityGenerator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="builderGenerator"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileInfo"/> is <see langword="null" />.
        /// </exception>
        public GroupDefinitionBuilder(
            IPluginRepository repository,
            IConnectParts importEngine,
            Func<Type, TypeIdentity> identityGenerator,
            Func<IBuildFixedSchedules> builderGenerator,
            PluginFileInfo fileInfo)
        {
            {
                Lokad.Enforce.Argument(() => repository);
                Lokad.Enforce.Argument(() => importEngine);
                Lokad.Enforce.Argument(() => identityGenerator);
                Lokad.Enforce.Argument(() => builderGenerator);
                Lokad.Enforce.Argument(() => fileInfo);
            }

            m_BuilderGenerator = builderGenerator;
            m_ImportEngine = importEngine;
            m_IdentityGenerator = identityGenerator;
            m_Repository = repository;
            m_FileInfo = fileInfo;
        }

        /// <summary>
        /// Returns an object that can be used to register schedules for the current group.
        /// </summary>
        /// <returns>The schedule builder for the current group.</returns>
        public IRegisterSchedules RegisterSchedule()
        {
            return new ScheduleDefinitionBuilder(this, m_BuilderGenerator());
        }

        /// <summary>
        /// Registers a new instance of the given type.
        /// </summary>
        /// <param name="type">The type to create a new instance from.</param>
        /// <returns>
        /// An object that provides a unique ID for the registered object and provides the IDs for the imports, exports,
        /// conditions and actions on that object.
        /// </returns>
        public IPartRegistration RegisterObject(Type type)
        {
            var plugin = m_Repository.Parts().FirstOrDefault(p => p.Identity.Equals(type));
            if (plugin == null)
            {
                throw new UnknownPluginTypeException();
            }

            if (!m_Objects.ContainsKey(type))
            {
                m_Objects.Add(type, new List<GroupPartDefinition>());
            }

            var collection = m_Objects[type];

            var exports = plugin.Exports.ToDictionary(
                e => new ExportRegistrationId(type, collection.Count, e.ContractName),
                e => e);
            var imports = plugin.Imports.ToDictionary(
                i => new ImportRegistrationId(type, collection.Count, i.ContractName),
                i => i);
            var actions = plugin.Actions.ToDictionary(
                a => new ScheduleActionRegistrationId(type, collection.Count, a.ContractName),
                a => a);
            var conditions = plugin.Conditions.ToDictionary(
                c => new ScheduleConditionRegistrationId(type, collection.Count, c.ContractName),
                c => c);

            var registration = new GroupPartDefinition(
                m_IdentityGenerator(type),
                collection.Count,
                exports,
                imports,
                actions,
                conditions);
            collection.Add(registration);

            return registration;
        }

        /// <summary>
        /// Connects the export with the import.
        /// </summary>
        /// <param name="importRegistration">The ID of the import.</param>
        /// <param name="exportRegistration">The ID of the export.</param>
        public void Connect(ImportRegistrationId importRegistration, ExportRegistrationId exportRegistration)
        {
            {
                Lokad.Enforce.Argument(() => importRegistration);
                Lokad.Enforce.Argument(() => exportRegistration);
            }

            var import = m_Objects.SelectMany(t => t.Value).PartImportById(importRegistration);
            var export = m_Objects.SelectMany(t => t.Value).PartExportById(exportRegistration);
            if (!m_ImportEngine.Accepts(import, export))
            {
                throw new CannotMapExportToImportException();
            }

            if (!m_Connections.ContainsKey(importRegistration))
            {
                m_Connections.Add(importRegistration, new List<ExportRegistrationId>());
            }

            var list = m_Connections[importRegistration];
            if ((import.Cardinality == ImportCardinality.ExactlyOne || import.Cardinality == ImportCardinality.ZeroOrOne) && (list.Count > 0))
            {
                list[0] = exportRegistration;
            }
            else
            {
                list.Add(exportRegistration);
            }
        }

        /// <summary>
        /// Defines an export for the group. The export is created with the specified name
        /// and all the open exports and the group schedule.
        /// </summary>
        /// <param name="contractName">The contract name for the group export.</param>
        /// <remarks>Only one export can be defined per group.</remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        public void DefineExport(string contractName)
        {
            m_GroupExport = new GroupExportMap(contractName);
        }

        /// <summary>
        /// Defines an import for the group with the given insert point.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="insertPoint">The point at which the imported schedule will be placed in the group schedule.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="DuplicateContractNameException">
        ///     Thrown if <paramref name="contractName"/> already exists in the collection of imports.
        /// </exception>
        public void DefineImport(string contractName, InsertVertex insertPoint)
        {
            DefineImport(contractName, insertPoint, null);
        }

        /// <summary>
        /// Defines an import for the group with the given imports that should be satisfied.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="importsToSatisfy">The imports that should be satisfied.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="DuplicateContractNameException">
        ///     Thrown if <paramref name="contractName"/> already exists in the collection of imports.
        /// </exception>
        public void DefineImport(string contractName, IEnumerable<ImportRegistrationId> importsToSatisfy)
        {
            DefineImport(contractName, null, importsToSatisfy);
        }

        /// <summary>
        /// Defines an import for the group with the given insert point and the given imports that should be satisfied.
        /// </summary>
        /// <param name="contractName">The contract name for the group import.</param>
        /// <param name="insertPoint">The point at which the imported schedule will be placed in the group schedule.</param>
        /// <param name="importsToSatisfy">The imports that should be satisfied.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="DuplicateContractNameException">
        ///     Thrown if <paramref name="contractName"/> already exists in the collection of imports.
        /// </exception>
        public void DefineImport(string contractName, InsertVertex insertPoint, IEnumerable<ImportRegistrationId> importsToSatisfy)
        {
            if (m_GroupImports.ContainsKey(contractName))
            {
                throw new DuplicateContractNameException();
            }

            var import = new GroupImportMap(contractName, insertPoint, importsToSatisfy);
            m_GroupImports.Add(contractName, import);
        }

        /// <summary>
        /// Registers a group with the currently stored data.
        /// </summary>
        /// <param name="name">The name of the newly created group.</param>
        /// <returns>The registration ID of the group.</returns>
        public GroupRegistrationId Register(string name)
        {
            var definition = new GroupDefinition(name);
            definition.Parts = m_Objects.SelectMany(p => p.Value).ToList();
            definition.InternalConnections = m_Connections.Select(
                p => new PartImportToPartExportMap(p.Key, (IEnumerable<ExportRegistrationId>)p.Value));

            if (m_Schedule != null)
            {
                definition.Schedule = ScheduleDefinition.CreateDefinition(
                    definition.Id,
                    m_Schedule,
                    m_Actions.ToDictionary(p => p.Value, p => p.Key),
                    m_Conditions.ToDictionary(p => p.Value, p => p.Key));
            }

            if (m_GroupExport != null)
            {
                definition.GroupExport = GroupExportDefinition.CreateDefinition(
                    m_GroupExport.ContractName, 
                    definition.Id, 
                    NonLinkedExports());
            }

            if (m_GroupImports.Count > 0)
            {
                definition.GroupImports = m_GroupImports.Select(
                        i => GroupImportDefinition.CreateDefinition(
                            i.Value.ContractName, 
                            definition.Id, 
                            i.Value.InsertPoint, 
                            i.Value.ObjectImports))
                    .ToList();
            }

            Clear();

            m_Repository.AddGroup(definition, m_FileInfo);
            return definition.Id;
        }

        private IEnumerable<ExportRegistrationId> NonLinkedExports()
        {
            return m_Objects
                .SelectMany(p => p.Value)
                .SelectMany(o => o.RegisteredExports)
                .Where(e => !m_Connections.Values.SelectMany(l => l).Contains(e))
                .ToList();
        }

        /// <summary>
        /// Clears the registrations stored for the group that is under construction.
        /// </summary>
        public void Clear()
        {
            m_Objects = new Dictionary<Type, List<GroupPartDefinition>>();
            m_Connections = new Dictionary<ImportRegistrationId, List<ExportRegistrationId>>();

            m_Actions = new Dictionary<ScheduleActionRegistrationId, ScheduleElementId>();
            m_Conditions = new Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>();
            m_Schedule = null;

            m_GroupExport = null;
            m_GroupImports = new Dictionary<string, GroupImportMap>();
        }

        /// <summary>
        /// Stores the created schedule and it's associated actions and conditions.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actionMap">The collection mapping the registered actions to the schedule element that holds the action.</param>
        /// <param name="conditionMap">The collection mapping the registered conditions to the schedule element that holds the condition.</param>
        /// <returns>The ID of the newly created schedule.</returns>
        public ScheduleId StoreSchedule(
            ISchedule schedule,
            Dictionary<ScheduleActionRegistrationId, ScheduleElementId> actionMap,
            Dictionary<ScheduleConditionRegistrationId, ScheduleElementId> conditionMap)
        {
            {
                Debug.Assert(schedule != null, "The schedule should not be a null reference.");
                Debug.Assert(actionMap != null, "The collection of actions should not be a null reference.");
                Debug.Assert(conditionMap != null, "The collection of conditions should not be a null reference.");
            }

            var id = new ScheduleId();

            m_Schedule = schedule;
            foreach (var pair in actionMap)
            {
                m_Actions.Add(pair.Key, pair.Value);
            }

            foreach (var pair in conditionMap)
            {
                m_Conditions.Add(pair.Key, pair.Value);
            }

            return id;
        }
    }
}
