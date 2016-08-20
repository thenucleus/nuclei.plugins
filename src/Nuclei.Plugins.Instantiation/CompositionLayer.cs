//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Dataset.Properties;
using Apollo.Core.Extensions.Plugins;
using Apollo.Utilities.History;

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Defines the graph of components groups that describes how the different
    /// groups are connected.
    /// </summary>
    internal sealed class CompositionLayer : IStoreGroupsAndConnections, IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the definitions field.
        /// </summary>
        private const byte GroupDefinitionIndex = 0;

        /// <summary>
        /// The history index of the groups field.
        /// </summary>
        private const byte GroupCompositionIndex = 1;

        /// <summary>
        /// The history index of the group connections field.
        /// </summary>
        private const byte GroupConnectionIndex = 2;

        /// <summary>
        /// The history index of the part definition field.
        /// </summary>
        private const byte PartCompositionIndex = 3;

        /// <summary>
        /// The history index of the part connections field.
        /// </summary>
        private const byte PartConnectionIndex = 4;

        /// <summary>
        /// The history index of the part instance field.
        /// </summary>
        private const byte PartInstanceIndex = 5;

        /// <summary>
        /// Creates a default layer that isn't linked to a timeline.
        /// </summary>
        /// <param name="instanceStorage">The object that stores the instances of the parts.</param>
        /// <remarks>
        /// This method is provided for testing purposes only.
        /// </remarks>
        /// <returns>The newly created instance.</returns>
        internal static CompositionLayer CreateInstanceWithoutTimeline(IStoreInstances instanceStorage)
        {
            var history = new ValueHistory<IStoreInstances>
                {
                    Current = instanceStorage
                };
            return new CompositionLayer(
                new HistoryId(),
                new DictionaryHistory<GroupRegistrationId, GroupDefinition>(),
                new DictionaryHistory<GroupCompositionId, GroupRegistrationId>(),
                new BidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge>(),
                new DictionaryHistory<PartCompositionId, PartCompositionInfo>(),
                new BidirectionalGraphHistory<PartCompositionId, PartImportExportEdge<PartCompositionId>>(),
                history);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CompositionLayer"/> class with the given 
        /// history information.
        /// </summary>
        /// <param name="id">The history ID for the dataset storage.</param>
        /// <param name="members">The collection that holds all the members for the current object.</param>
        /// <param name="constructorArguments">The optional constructor arguments.</param>
        /// <returns>A new instance of the <see cref="CompositionLayer"/> class.</returns>
        internal static CompositionLayer CreateInstance(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 6, "There should be 6 members.");
            }

            IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition> definitions = null;
            IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId> groups = null;
            IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> graph = null;
            IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo> parts = null;
            IBidirectionalGraphHistory<PartCompositionId, PartImportExportEdge<PartCompositionId>> partConnections = null;
            IVariableTimeline<IStoreInstances> instances = null;
            foreach (var member in members)
            {
                if (member.Item1 == GroupDefinitionIndex)
                {
                    definitions = member.Item2 as IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition>;
                    continue;
                }

                if (member.Item1 == GroupCompositionIndex)
                {
                    groups = member.Item2 as IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId>;
                    continue;
                }

                if (member.Item1 == GroupConnectionIndex)
                {
                    graph = member.Item2 as IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge>;
                    continue;
                }

                if (member.Item1 == PartCompositionIndex)
                {
                    parts = member.Item2 as IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo>;
                    continue;
                }

                if (member.Item1 == PartConnectionIndex)
                {
                    partConnections = member.Item2 as IBidirectionalGraphHistory<PartCompositionId, PartImportExportEdge<PartCompositionId>>;
                    continue;
                }

                if (member.Item1 == PartInstanceIndex)
                {
                    instances = member.Item2 as IVariableTimeline<IStoreInstances>;
                    continue;
                }

                throw new UnknownMemberException();
            }

            return new CompositionLayer(
                id,
                definitions,
                groups,
                graph,
                parts,
                partConnections,
                instances);
        }

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// The collection that contains all the currently selected definitions.
        /// </summary>
        [FieldIndexForHistoryTracking(GroupDefinitionIndex)]
        private readonly IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition> m_Definitions;

        /// <summary>
        /// The collection that contains all the currently selected groups.
        /// </summary>
        [FieldIndexForHistoryTracking(GroupCompositionIndex)]
        private readonly IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId> m_Groups;

        /// <summary>
        /// The graph that determines how the different groups are connected.
        /// </summary>
        /// <design>
        /// Note that the edges point from the export to the import.
        /// </design>
        [FieldIndexForHistoryTracking(GroupConnectionIndex)]
        private readonly IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> m_GroupConnections;

        /// <summary>
        /// The collection that contains all the parts for the currently selected groups.
        /// </summary>
        [FieldIndexForHistoryTracking(PartCompositionIndex)]
        private readonly IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo> m_Parts;

        /// <summary>
        /// The graph that determines how the different parts are connected.
        /// </summary>
        /// <design>
        /// Note that the edges point from the export to the import.
        /// </design>
        [FieldIndexForHistoryTracking(PartConnectionIndex)]
        private readonly IBidirectionalGraphHistory<PartCompositionId, PartImportExportEdge<PartCompositionId>> m_PartConnections;

        /// <summary>
        /// The object that stores the instances of the objects.
        /// </summary>
        [FieldIndexForHistoryTracking(PartInstanceIndex)]
        private readonly IVariableTimeline<IStoreInstances> m_Instances;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionLayer"/> class.
        /// </summary>
        /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
        /// <param name="definitions">The collection containing all the known group definitions.</param>
        /// <param name="groups">The collection containing the known groups for the current dataset.</param>
        /// <param name="groupConnections">The graph that describes the connections between the groups.</param>
        /// <param name="parts">The collection that contains all the parts for the currently selected groups.</param>
        /// <param name="partConnections">The graph that determines how the different parts are connected.</param>
        /// <param name="instances">The object that stores the instances of the parts.</param>
        private CompositionLayer(
            HistoryId id,
            IDictionaryTimelineStorage<GroupRegistrationId, GroupDefinition> definitions,
            IDictionaryTimelineStorage<GroupCompositionId, GroupRegistrationId> groups,
            IBidirectionalGraphHistory<GroupCompositionId, GroupCompositionGraphEdge> groupConnections,
            IDictionaryTimelineStorage<PartCompositionId, PartCompositionInfo> parts,
            IBidirectionalGraphHistory<PartCompositionId, PartImportExportEdge<PartCompositionId>> partConnections,
            IVariableTimeline<IStoreInstances> instances)
        {
            {
                Debug.Assert(id != null, "The ID object should not be a null reference.");
                Debug.Assert(definitions != null, "The definition collectino should not be a null reference.");
                Debug.Assert(groups != null, "The groups collection should not be a null reference.");
                Debug.Assert(groupConnections != null, "The group connection graph should not be a null reference.");
                Debug.Assert(parts != null, "The parts collection should not be a null reference.");
                Debug.Assert(partConnections != null, "The part connection graph should not be a null reference.");
                Debug.Assert(instances != null, "The instance collection should not be a null reference.");
            }

            m_HistoryId = id;
            m_Definitions = definitions;
            m_Groups = groups;
            m_GroupConnections = groupConnections;
            m_Parts = parts;
            m_PartConnections = partConnections;
            m_Instances = instances;
        }

        /// <summary>
        /// Gets the current value for the instance store.
        /// </summary>
        private IStoreInstances Instances
        {
            get
            {
                return m_Instances.Current;
            }
        }

        /// <summary>
        /// Gets the ID which relates the object to the timeline.
        /// </summary>
        public HistoryId HistoryId
        {
            get
            {
                return m_HistoryId;
            }
        }

        /// <summary>
        /// Adds a new <see cref="GroupDefinition"/> to the graph and returns the ID for that group.
        /// </summary>
        /// <param name="id">The ID of the group that is being added.</param>
        /// <param name="group">The group that should be added to the graph.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="group"/> is <see langword="null" />.
        /// </exception>
        public void Add(GroupCompositionId id, GroupDefinition group)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => group);
            }

            if (!m_Definitions.ContainsKey(group.Id))
            {
                m_Definitions.Add(group.Id, group);
            }

            m_Groups.Add(id, group.Id);
            m_GroupConnections.AddVertex(id);

            foreach (var part in group.Parts)
            {
                var partId = new PartCompositionId(id, part.Id);
                m_Parts.Add(partId, new PartCompositionInfo(part));
                m_PartConnections.AddVertex(partId);
            }

            var parts = PartsForGroup(id);
            ConnectParts(group.InternalConnections, parts, parts);
        }

        private IEnumerable<Tuple<PartCompositionId, GroupPartDefinition>> PartsForGroup(GroupCompositionId group)
        {
            return m_Parts
                .Where(pair => pair.Key.Group.Equals(group))
                .Select(pair => new Tuple<PartCompositionId, GroupPartDefinition>(pair.Key, m_Parts[pair.Key].Definition));
        }

        private void ConnectParts(
            IEnumerable<PartImportToPartExportMap> connections,
            IEnumerable<Tuple<PartCompositionId, GroupPartDefinition>> importingParts,
            IEnumerable<Tuple<PartCompositionId, GroupPartDefinition>> exportingParts)
        {
            foreach (var map in connections)
            {
                var importingPart = importingParts.FirstOrDefault(p => p.Item2.RegisteredImports.Contains(map.Import));
                Debug.Assert(importingPart != null, "Cannot connect parts that are not registered.");

                foreach (var export in map.Exports)
                {
                    var exportingPart = exportingParts.FirstOrDefault(p => p.Item2.RegisteredExports.Contains(export));
                    m_PartConnections.AddEdge(
                        new PartImportExportEdge<PartCompositionId>(
                            importingPart.Item1,
                            map.Import,
                            exportingPart.Item1,
                            export));
                }
            }

            ConstructInstancesIfComplete(importingParts.Select(p => p.Item1));
        }

        private void ConstructInstancesIfComplete(IEnumerable<PartCompositionId> potentialCompleteParts)
        {
            foreach (var partId in potentialCompleteParts)
            {
                ConstructInstance(partId);
            }
        }

        private void ConstructInstance(PartCompositionId partId)
        {
            if (CanPartBeInstantiated(partId))
            {
                var satisfiedPartImports = SatisfiedPartImports(partId);
                foreach (var importPair in satisfiedPartImports)
                {
                    var exportingPartInfo = m_Parts[importPair.Item2];
                    if (exportingPartInfo.Instance == null)
                    {
                        ConstructInstance(importPair.Item2);
                    }
                }

                var partImports = ImportInformationForPart(satisfiedPartImports);
                var info = m_Parts[partId];
                if (info.Instance != null)
                {
                    var updatedInstances = Instances.UpdateIfRequired(info.Instance, partImports);
                    foreach (var instance in updatedInstances)
                    {
                        if (instance.Change == InstanceChange.Removed)
                        {
                            var updatedInfo = m_Parts.FirstOrDefault(p => instance.Instance.Equals(p.Value.Instance));
                            updatedInfo.Value.Instance = null;
                        }
                    }
                }
                else
                {
                    info.Instance = Instances.Construct(
                        info.Definition,
                        partImports);
                }
            }
        }

        private IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> ImportInformationForPart(
            IEnumerable<Tuple<ImportRegistrationId, PartCompositionId, ExportRegistrationId>> satisfiedPartImports)
        {
            return satisfiedPartImports
                .Where(p => m_Parts[p.Item2].Instance != null)
                .Select(
                    p => new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                        p.Item1,
                        m_Parts[p.Item2].Instance,
                        p.Item3));
        }

        private IEnumerable<Tuple<ImportRegistrationId, PartCompositionId, ExportRegistrationId>> SatisfiedPartImports(PartCompositionId partId)
        {
            return m_PartConnections
                    .InEdges(partId)
                    .Select(
                        edge => new Tuple<ImportRegistrationId, PartCompositionId, ExportRegistrationId>(
                            edge.ImportRegistration,
                            edge.Source,
                            edge.ExportRegistration));
        }

        private bool AreRequiredPartImportsSatisfied(PartCompositionId partId, IEnumerable<ImportRegistrationId> satisfiedRequiredImports)
        {
            var info = m_Parts[partId];
            var unsatisfiedImports = info.Definition
                .RegisteredImports
                .Where(id => info.Definition.Import(id).IsPrerequisite)
                .Except(satisfiedRequiredImports);
            return !unsatisfiedImports.Any();
        }

        private bool CanPartsBeInstantiated(IEnumerable<PartCompositionId> parts)
        {
            return parts.Aggregate(true, (b, p) => b && CanPartBeInstantiated(p));
        }

        private bool CanPartBeInstantiated(PartCompositionId partId)
        {
            var info = m_Parts[partId];
            if (info.Instance != null)
            {
                return true;
            }

            var satisfiedPartImports = SatisfiedPartImports(partId);
            var satisfiedRequiredPartImports = satisfiedPartImports.Where(p => info.Definition.Import(p.Item1).IsPrerequisite);
            return AreRequiredPartImportsSatisfied(partId, satisfiedRequiredPartImports.Select(p => p.Item1))
                && CanPartsBeInstantiated(satisfiedRequiredPartImports.Select(p => p.Item2));
        }

        /// <summary>
        /// Removes the group that is related to the specified ID.
        /// </summary>
        /// <param name="group">The ID of the group that should be removed.</param>
        public void Remove(GroupCompositionId group)
        {
            if (group == null)
            {
                return;
            }

            if (!m_Groups.ContainsKey(group))
            {
                return;
            }

            var definitionId = m_Groups[group];
            var definition = m_Definitions[definitionId];

            foreach (var part in definition.Parts)
            {
                var partId = new PartCompositionId(group, part.Id);
                Debug.Assert(m_Parts.ContainsKey(partId), "The part collection should have the given part ID.");
                Debug.Assert(m_PartConnections.ContainsVertex(partId), "The part connections graph should have the given part ID.");

                var info = m_Parts[partId];
                if (info.Instance != null)
                {
                    ReleaseInstance(partId);
                }

                m_PartConnections.RemoveVertex(partId);
                m_Parts.Remove(partId);
            }

            Debug.Assert(m_GroupConnections.ContainsVertex(group), "The connections graph should have the given group ID.");
            m_GroupConnections.RemoveVertex(group);

            m_Groups.Remove(group);
            if (!m_Groups.Any(p => p.Value.Equals(definitionId)))
            {
                m_Definitions.Remove(definitionId);
            }
        }

        private void ReleaseInstance(PartCompositionId partId)
        {
            var info = m_Parts[partId];
            if (info.Instance != null)
            {
                DisconnectPartInstanceFromImportingParts(partId);

                var updatedInstances = Instances.Release(info.Instance);
                foreach (var instance in updatedInstances)
                {
                    if (instance.Change == InstanceChange.Removed)
                    {
                        var updatedInfo = m_Parts.FirstOrDefault(p => instance.Instance.Equals(p.Value.Instance));
                        updatedInfo.Value.Instance = null;
                    }
                }
            }
        }

        private void DisconnectPartInstanceFromImportingParts(PartCompositionId partId)
        {
            var importingParts = m_PartConnections
                .OutEdges(partId)
                .Select(edge => new Tuple<PartCompositionId, ImportRegistrationId>(edge.Target, edge.ImportRegistration));

            foreach (var pair in importingParts)
            {
                DisconnectInstanceFromExport(pair.Item1, pair.Item2, partId);
            }
        }

        private void DisconnectInstanceFromExport(
            PartCompositionId importingInstance, 
            ImportRegistrationId importId, 
            PartCompositionId exportingInstance)
        {
            var importingPartInfo = m_Parts[importingInstance];
            if (importingPartInfo.Instance != null)
            {
                var importDefinition = importingPartInfo.Definition.Import(importId);
                if (importDefinition.IsPrerequisite)
                {
                    ReleaseInstance(importingInstance);
                }
                else
                {
                    var satisfiedPartImports = SatisfiedPartImports(importingInstance)
                        .Where(p => !p.Item2.Equals(exportingInstance));
                    var partImports = ImportInformationForPart(satisfiedPartImports);
                    var updatedInstances = Instances.UpdateIfRequired(importingPartInfo.Instance, partImports);
                    foreach (var instance in updatedInstances)
                    {
                        if (instance.Change == InstanceChange.Removed)
                        {
                            var updatedInfo = m_Parts.FirstOrDefault(p => instance.Instance.Equals(p.Value.Instance));
                            updatedInfo.Value.Instance = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the collection of all known groups.
        /// </summary>
        /// <returns>The collection of all known groups.</returns>
        public IEnumerable<GroupCompositionId> Groups()
        {
            return m_Groups.Keys;
        }

        /// <summary>
        /// Returns the <see cref="GroupDefinition"/> that was registered with the given ID.
        /// </summary>
        /// <param name="id">The composition ID of the group.</param>
        /// <returns>The definition for the group with the given ID.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownGroupCompositionIdException">
        ///     Thrown if <paramref name="id"/> does not belong to a known group.
        /// </exception>
        public GroupDefinition Group(GroupCompositionId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            var definitionId = m_Groups[id];

            Debug.Assert(m_Definitions.ContainsKey(definitionId), "There should be a definition with the given definition ID.");
            return m_Definitions[definitionId];
        }

        /// <summary>
        /// Connects the exporting group with the importing group via the given import.
        /// </summary>
        /// <param name="connection">The object that describes how the group import and the group export should be connected.</param>
        public void Connect(GroupConnection connection)
        {
            {
                Lokad.Enforce.Argument(() => connection);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(connection.ImportingGroup),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(connection.ExportingGroup),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            m_GroupConnections.AddEdge(new GroupCompositionGraphEdge(connection.ImportingGroup, connection.GroupImport, connection.ExportingGroup));

            var importingParts = PartsForGroup(connection.ImportingGroup);
            var exportingParts = PartsForGroup(connection.ExportingGroup);
            ConnectParts(connection.PartConnections, importingParts, exportingParts);
        }

        /// <summary>
        /// Disconnects the exporting group from the importing group.
        /// </summary>
        /// <param name="importingGroup">The composition ID of the importing group.</param>
        /// <param name="exportingGroup">The composition ID of the exporting group.</param>
        public void Disconnect(GroupCompositionId importingGroup, GroupCompositionId exportingGroup)
        {
            if ((importingGroup == null) || (exportingGroup == null))
            {
                return;
            }

            if (!m_GroupConnections.ContainsVertex(importingGroup) || (!m_GroupConnections.ContainsVertex(exportingGroup)))
            {
                return;
            }

            var importDefinition = m_GroupConnections
                .InEdges(importingGroup)
                .Where(edge => edge.Source.Equals(exportingGroup))
                .Select(edge => edge.Import)
                .FirstOrDefault();
            m_GroupConnections.RemoveInEdgeIf(importingGroup, edge => edge.Source.Equals(exportingGroup));

            DisconnectParts(importingGroup, importDefinition, exportingGroup);
        }

        private void DisconnectParts(GroupCompositionId importingGroup, GroupImportDefinition importDefinition, GroupCompositionId exportingGroup)
        {
            var definitionId = m_Groups[importingGroup];
            var importingGroupDefinition = m_Definitions[definitionId];
            var importingParts = importDefinition
                .ImportsToMatch
                .Select(id => importingGroupDefinition.Parts.PartByImport(id))
                .Join(
                    m_Parts,
                    partDef => new PartCompositionId(importingGroup, partDef.Id),
                    pair => pair.Key,
                    (partDef, pair) => pair.Key);

            definitionId = m_Groups[exportingGroup];
            var exportingGroupDefinition = m_Definitions[definitionId];
            var exportingParts = exportingGroupDefinition.GroupExport
                .ProvidedExports
                .Select(id => exportingGroupDefinition.Parts.PartByExport(id))
                .Join(
                    m_Parts,
                    partDef => new PartCompositionId(exportingGroup, partDef.Id),
                    pair => pair.Key,
                    (partDef, pair) => pair.Key);

            foreach (var importingPart in importingParts)
            {
                var matchedExports = m_PartConnections
                    .InEdges(importingPart)
                    .Where(edge => exportingParts.Contains(edge.Source))
                    .Select(edge => new Tuple<ImportRegistrationId, PartCompositionId>(edge.ImportRegistration, edge.Source));

                foreach (var pair in matchedExports)
                {
                    DisconnectInstanceFromExport(importingPart, pair.Item1, pair.Item2);
                }

                m_PartConnections.RemoveInEdgeIf(importingPart, edge => exportingParts.Contains(edge.Source));
            }
        }

        /// <summary>
        /// Disconnects the group from all imports and exports.
        /// </summary>
        /// <param name="group">The composition ID of the group.</param>
        public void Disconnect(GroupCompositionId group)
        {
            if ((group == null) || !m_GroupConnections.ContainsVertex(group))
            {
                return;
            }

            var matchingExports = m_GroupConnections
                .InEdges(group)
                .Select(edge => new Tuple<GroupCompositionId, GroupImportDefinition>(edge.Source, edge.Import));
            foreach (var pair in matchingExports)
            {
                DisconnectParts(group, pair.Item2, pair.Item1);
            }

            var matchingImports = m_GroupConnections
                .OutEdges(group)
                .Select(edge => new Tuple<GroupCompositionId, GroupImportDefinition>(edge.Target, edge.Import));
            foreach (var pair in matchingImports)
            {
                DisconnectParts(pair.Item1, pair.Item2, group);
            }

            m_GroupConnections.RemoveInEdgeIf(group, edge => true);
            m_GroupConnections.RemoveOutEdgeIf(group, edge => true);
        }

        /// <summary>
        /// Returns a collection of all imports owned by the specified group that have been provided with an export.
        /// </summary>
        /// <param name="importOwner">The composition ID of the group that owns the imports.</param>
        /// <returns>A collection containing all the imports with the group ID of the group providing the connected export.</returns>
        public IEnumerable<Tuple<GroupImportDefinition, GroupCompositionId>> SatisfiedImports(GroupCompositionId importOwner)
        {
            {
                Lokad.Enforce.Argument(() => importOwner);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(importOwner),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            return m_GroupConnections.InEdges(importOwner).Select(e => new Tuple<GroupImportDefinition, GroupCompositionId>(e.Import, e.Source));
        }

        /// <summary>
        /// Returns a collection of all imports owned by the specified group that have not been provided with an export.
        /// </summary>
        /// <param name="importOwner">The composition ID of the group that owns the imports.</param>
        /// <returns>A collection containing all imports that have not been provided with an export.</returns>
        public IEnumerable<GroupImportDefinition> UnsatisfiedImports(GroupCompositionId importOwner)
        {
            {
                Lokad.Enforce.Argument(() => importOwner);
                Lokad.Enforce.With<UnknownGroupCompositionIdException>(
                    m_Groups.ContainsKey(importOwner),
                    Resources.Exceptions_Messages_UnknownGroupCompositionId);
            }

            var definitionId = m_Groups[importOwner];

            Debug.Assert(m_Definitions.ContainsKey(definitionId), "There should be a definition for the current group.");
            var definition = m_Definitions[definitionId];

            var inEdges = m_GroupConnections.InEdges(importOwner);
            return definition.GroupImports.Where(i => !inEdges.Any(e => e.Import.Equals(i)));
        }
    }
}
