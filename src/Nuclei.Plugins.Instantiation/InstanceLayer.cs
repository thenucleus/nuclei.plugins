//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Dataset.Properties;
using Nuclei.Plugins;
using Apollo.Utilities.History;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Stores the part instances and their connections.
    /// </summary>
    internal sealed class InstanceLayer : IStoreInstances, IAmHistoryEnabled
    {
        /// <summary>
        /// The history index of the non-history object collection field.
        /// </summary>
        private const byte PartDefinitionsIndex = 0;

        /// <summary>
        /// The history index of the parts collection field.
        /// </summary>
        private const byte PartsIndex = 1;

        /// <summary>
        /// The history index of the history object collection field.
        /// </summary>
        private const byte HistoryObjectsIndex = 2;

        /// <summary>
        /// The history index of the instance connection field.
        /// </summary>
        private const byte InstanceConnectionIndex = 3;

        /// <summary>
        /// Defines an <see cref="IEqualityComparer{T}"/> that determines if the type of a parameter is 
        /// assignable to the type of another parameter.
        /// </summary>
        private sealed class IsAssignableEqualityComparer : IEqualityComparer<ParameterInfo>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type T to compare.</param>
            /// <param name="y">The second object of type T to compare.</param>
            /// <returns><see langword="true" /> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
            public bool Equals(ParameterInfo x, ParameterInfo y)
            {
                return x != null && y != null && x.ParameterType.IsAssignableFrom(y.ParameterType);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <param name="obj">The <see cref="ParameterInfo"/> for which a hash code is to be returned.</param>
            /// <returns>A hash code for the specified object.</returns>
            /// <exception cref="ArgumentNullException">
            ///     Thrown if <paramref name="obj"/> is <see langword="null" />.
            /// </exception>
            public int GetHashCode(ParameterInfo obj)
            {
                {
                    Lokad.Enforce.Argument(() => obj);
                }

                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// Creates a default layer that isn't linked to a timeline.
        /// </summary>
        /// <remarks>
        /// This method is provided for testing purposes only.
        /// </remarks>
        /// <returns>The newly created instance.</returns>
        internal static InstanceLayer CreateInstanceWithoutTimeline()
        {
            return new InstanceLayer(
                new HistoryId(),
                new DictionaryHistory<PartRegistrationId, GroupPartDefinition>(),
                new DictionaryHistory<PartInstanceId, PartRegistrationId>(),
                new DictionaryHistory<PartInstanceId, IAmHistoryEnabled>(),
                new BidirectionalGraphHistory<PartInstanceId, PartImportExportEdge<PartInstanceId>>());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="InstanceLayer"/> class with the given 
        /// history information.
        /// </summary>
        /// <param name="id">The history ID for the part instance layer.</param>
        /// <param name="members">The collection that holds all the members for the current object.</param>
        /// <param name="constructorArguments">The optional constructor arguments.</param>
        /// <returns>A new instance of the <see cref="InstanceLayer"/> class.</returns>
        internal static InstanceLayer CreateInstance(
            HistoryId id,
            IEnumerable<Tuple<byte, IStoreTimelineValues>> members,
            params object[] constructorArguments)
        {
            {
                Debug.Assert(members.Count() == 4, "There should be 4 members.");
            }

            IDictionaryTimelineStorage<PartRegistrationId, GroupPartDefinition> partDefinitions = null;
            IDictionaryTimelineStorage<PartInstanceId, PartRegistrationId> parts = null;
            IDictionaryTimelineStorage<PartInstanceId, IAmHistoryEnabled> historyInstances = null;
            IBidirectionalGraphHistory<PartInstanceId, PartImportExportEdge<PartInstanceId>> instanceConnections = null;
            foreach (var member in members)
            {
                if (member.Item1 == PartDefinitionsIndex)
                {
                    partDefinitions = member.Item2 as IDictionaryTimelineStorage<PartRegistrationId, GroupPartDefinition>;
                    continue;
                }

                if (member.Item1 == PartsIndex)
                {
                    parts = member.Item2 as IDictionaryTimelineStorage<PartInstanceId, PartRegistrationId>;
                    continue;
                }

                if (member.Item1 == HistoryObjectsIndex)
                {
                    historyInstances = member.Item2 as IDictionaryTimelineStorage<PartInstanceId, IAmHistoryEnabled>;
                    continue;
                }

                if (member.Item1 == InstanceConnectionIndex)
                {
                    instanceConnections = member.Item2 as IBidirectionalGraphHistory<PartInstanceId, PartImportExportEdge<PartInstanceId>>;
                }

                throw new UnknownMemberException();
            }

            return new InstanceLayer(
                id,
                partDefinitions,
                parts,
                historyInstances,
                instanceConnections);
        }

        /// <summary>
        /// Loads the <see cref="Type"/> for the given identity.
        /// </summary>
        /// <param name="typeIdentity">The identity of the type that should be loaded.</param>
        /// <returns>The requested type.</returns>
        /// <exception cref="UnableToLoadPluginTypeException">
        ///     Thrown when something goes wrong while loading the assembly containing the type or the type.
        /// </exception>
        private static Type LoadType(TypeIdentity typeIdentity)
        {
            try
            {
                return Type.GetType(typeIdentity.AssemblyQualifiedName, true, false);
            }
            catch (TargetInvocationException e)
            {
                // Type initializer throw an exception
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (TypeLoadException e)
            {
                // Type is not found, typeName contains invalid characters, typeName represents an array type with an invalid size,
                // typeName represents and array of TypedReference
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (ArgumentException e)
            {
                // typeName contains invalid syntax, typeName represents a generic type that has a pointer, a ByRef type or Void as one of
                // its type arguments, typeName represents a generic type that has an incorrect number of type arguments, typeName
                // represents a generic type, and one of its arguments does not satisfy the constraints for the corresponding type parameter
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (FileNotFoundException e)
            {
                // The assembly or one of its dependencies was not found
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
            catch (BadImageFormatException e)
            {
                // The assembly or one of its dependencies was not valid
                throw new UnableToLoadPluginTypeException(Resources.Exceptions_Messages_UnableToLoadPluginType, e);
            }
        }

        private static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Translates an object to a delegate.
        /// </summary>
        /// <param name="owner">The object which needs to be retrieved from the delegate.</param>
        /// <returns>A delegate that will return the object.</returns>
        /// <typeparam name="T">The type of object that is returned by the function.</typeparam>
        [Lokad.Quality.UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Code is called by reflection.")]
        private static Func<T> ToFunc<T>(object owner)
        {
            Func<T> func = () => (T)owner;

            return func;
        }

        /// <summary>
        /// Translates property info to a delegate.
        /// </summary>
        /// <param name="owner">The object from which the property value needs to be retrieved from the delegate.</param>
        /// <param name="property">The property to get.</param>
        /// <returns>A delegate that will return the property value.</returns>
        /// <typeparam name="T">The type of object that is returned by the function.</typeparam>
        [Lokad.Quality.UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Code is called by reflection.")]
        private static Func<T> ToFunc<T>(object owner, PropertyInfo property)
        {
            Func<T> func =
                () =>
                {
                    var result = property.GetValue(owner);
                    return (T)result;
                };

            return func;
        }

        private static MethodInfo MethodInfoForDelegate(Type delegateType)
        {
            return delegateType.GetMethod("Invoke");
        }

        private static MethodInfo GetMethodInfoFromDefinition(Type type, MethodDefinition methodDefinition)
        {
            return type.GetMethod(
                methodDefinition.MethodName,
                methodDefinition.Parameters.Select(p => LoadType(p.Identity)).ToArray());
        }

        private static PropertyInfo GetPropertyInfoFromDefinition(Type type, PropertyDefinition propertyDefinition)
        {
            return type.GetProperty(propertyDefinition.PropertyName);
        }

        private static bool IsMethodCompatibleWithDelegate(Type delegateType, MethodInfo exportedMethod)
        {
            var delegateMethod = MethodInfoForDelegate(delegateType);

            bool isCompatible = delegateMethod.ReturnType.IsAssignableFrom(exportedMethod.ReturnType);
            isCompatible = isCompatible && delegateMethod.GetParameters().SequenceEqual(
                exportedMethod.GetParameters(),
                new IsAssignableEqualityComparer());

            return isCompatible;
        }

        private static void DestroyInstance(object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// The collection of part instances that should not be tracked in history. All part instances in this
        /// collection will be disposed of when history roll-back or roll-forward occurs.
        /// </summary>
        private readonly IDictionary<PartInstanceId, object> m_NonHistoryInstances
            = new Dictionary<PartInstanceId, object>();

        /// <summary>
        /// The ID used by the timeline to uniquely identify the current object.
        /// </summary>
        private readonly HistoryId m_HistoryId;

        /// <summary>
        /// The collection containing the part definitions for the parts that do not participate in history tracking.
        /// </summary>
        [FieldIndexForHistoryTracking(PartDefinitionsIndex)]
        private readonly IDictionaryTimelineStorage<PartRegistrationId, GroupPartDefinition> m_PartDefinitions;

        /// <summary>
        /// The collection containing the part definitions for the parts that do not participate in history tracking.
        /// </summary>
        [FieldIndexForHistoryTracking(PartsIndex)]
        private readonly IDictionaryTimelineStorage<PartInstanceId, PartRegistrationId> m_InstanceToDefinitionMap;

        /// <summary>
        /// The collection of part instances that are tracked in history.
        /// </summary>
        [FieldIndexForHistoryTracking(HistoryObjectsIndex)]
        private readonly IDictionaryTimelineStorage<PartInstanceId, IAmHistoryEnabled> m_HistoryInstances;

        /// <summary>
        /// The graph that determines how the different instances are connected.
        /// </summary>
        /// <design>
        /// Note that the edges point from the export to the import.
        /// </design>
        [FieldIndexForHistoryTracking(InstanceConnectionIndex)]
        private readonly IBidirectionalGraphHistory<PartInstanceId, PartImportExportEdge<PartInstanceId>> m_InstanceConnections;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceLayer"/> class.
        /// </summary>
        /// <param name="id">The ID used by the timeline to uniquely identify the current object.</param>
        /// <param name="partDefinitions">The collection that holds all the part definitions.</param>
        /// <param name="parts">The collection that maps part instances to their definition.</param>
        /// <param name="historyInstances">The collection that stores the part instances that partake in history tracking.</param>
        /// <param name="instanceConnections">The graph that tracks the connections between the different part instances.</param>
        private InstanceLayer(
            HistoryId id,
            IDictionaryTimelineStorage<PartRegistrationId, GroupPartDefinition> partDefinitions,
            IDictionaryTimelineStorage<PartInstanceId, PartRegistrationId> parts,
            IDictionaryTimelineStorage<PartInstanceId, IAmHistoryEnabled> historyInstances,
            IBidirectionalGraphHistory<PartInstanceId, PartImportExportEdge<PartInstanceId>> instanceConnections)
        {
            {
                Debug.Assert(id != null, "The history ID object should not be a null reference.");
                Debug.Assert(partDefinitions != null, "The part definition collection should not be a null reference.");
                Debug.Assert(parts != null, "The parts collection should not be a null reference.");
                Debug.Assert(historyInstances != null, "The history objects collection should not be a null reference.");
                Debug.Assert(instanceConnections != null, "The instance graph should not be a null reference.");
            }

            m_HistoryId = id;
            m_PartDefinitions = partDefinitions;
            m_InstanceToDefinitionMap = parts;
            m_HistoryInstances = historyInstances;
            m_InstanceConnections = instanceConnections;

            m_PartDefinitions.OnExternalValueUpdate +=
                (s, e) =>
                {
                    // Create all the objects again and connect them(?)
                    m_NonHistoryInstances.Clear();

                    // Recreate objects
                    RebuildInstances(m_InstanceToDefinitionMap.Select(p => p.Key).Except(m_HistoryInstances.Select(p => p.Key)));
                };
        }

        private void RebuildInstances(IEnumerable<PartInstanceId> instancesToRebuild)
        {
            var graph = new BidirectionalGraph<PartInstanceId, PartImportExportEdge<PartInstanceId>>();
            foreach (var instance in instancesToRebuild)
            {
                if (!graph.ContainsVertex(instance))
                {
                    graph.AddVertex(instance);
                }

                var inEdges = m_InstanceConnections.InEdges(instance);
                foreach (var inEdge in inEdges)
                {
                    // Only connect edges to the vertices in the collection of items that we want to recreate
                    // because we're trying to determine which instance to create first
                    if (instancesToRebuild.Contains(inEdge.Source))
                    {
                        if (!graph.ContainsVertex(inEdge.Source))
                        {
                            graph.AddVertex(inEdge.Source);
                        }

                        var edge = new PartImportExportEdge<PartInstanceId>(
                            inEdge.Target,
                            inEdge.ImportRegistration,
                            inEdge.Source,
                            inEdge.ExportRegistration);
                        if (!graph.ContainsEdge(edge))
                        {
                            graph.AddEdge(edge);
                        }
                    }
                }
            }

            foreach (var instance in graph.TopologicalSort())
            {
                var importConnections = m_InstanceConnections
                    .InEdges(instance)
                    .Select(
                        e => new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            e.ImportRegistration,
                            e.Source,
                            e.ExportRegistration));

                UpdateIfRequired(instance, importConnections);
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
        /// Adds a new part definition to the layer and creates an instance of that part with the given constructor parameters.
        /// </summary>
        /// <param name="partDefinition">The part definition of which an instance should be created.</param>
        /// <param name="importConnections">The collection mapping the part imports to the exports and the parts that provide those exports.</param>
        /// <returns>The ID of the newly created part.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="partDefinition"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="importConnections"/> is <see langword="null" />.
        /// </exception>
        public PartInstanceId Construct(
            GroupPartDefinition partDefinition,
            IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> importConnections)
        {
            {
                Lokad.Enforce.Argument(() => partDefinition);
                Lokad.Enforce.Argument(() => importConnections);
            }

            // First store the definition and the instance ID, then we 
            // pretend that this whole thing is an update from the empty state (which it sort of is)
            // and let the UpdateIfRequired function figure it all out
            if (!m_PartDefinitions.ContainsKey(partDefinition.Id))
            {
                m_PartDefinitions.Add(partDefinition.Id, partDefinition);
            }

            var instanceId = new PartInstanceId();
            m_InstanceToDefinitionMap.Add(instanceId, partDefinition.Id);
            m_InstanceConnections.AddVertex(instanceId);

            var result = UpdateIfRequired(instanceId, importConnections, false, false);
            if (!result.Any(i => i.Instance.Equals(instanceId)))
            {
                throw new ConstructionOfPluginTypeFailedException();
            }

            return instanceId;
        }

        /// <summary>
        /// Updates the connections of an existing part if they don't match the provided collection. This may result 
        /// in the part object being recreated.
        /// </summary>
        /// <param name="instance">The ID of the part that should be updated.</param>
        /// <param name="importConnections">
        /// The collection mapping the part imports to the exports and the parts that provides those exports.
        /// </param>
        /// <returns>A collection containing the state information for all instances that were touched.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="instance"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="importConnections"/> is <see langword="null" />.
        /// </exception>
        public IEnumerable<InstanceUpdate> UpdateIfRequired(
            PartInstanceId instance,
            IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> importConnections)
        {
            {
                Lokad.Enforce.Argument(() => instance);
                Lokad.Enforce.Argument(() => importConnections);
            }

            return UpdateIfRequired(instance, importConnections, false, false);
        }

        /// <summary>
        /// Updates the connections of an existing part if they don't match the provided collection. This may result 
        /// in the part object being recreated.
        /// </summary>
        /// <param name="instance">The ID of the part that should be updated.</param>
        /// <param name="importConnections">
        /// The collection mapping the part imports to the exports and the parts that provides those exports.
        /// </param>
        /// <param name="shouldDeleteInstance">A flag that indicates if the instance should be removed.</param>
        /// <param name="forceUpdate">A flag that indicate if all imports should be forced to update.</param>
        /// <returns>A collection containing the state information for all instances that were touched.</returns>
        private IEnumerable<InstanceUpdate> UpdateIfRequired(
            PartInstanceId instance,
            IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> importConnections,
            bool shouldDeleteInstance,
            bool forceUpdate)
        {
            {
                Debug.Assert(instance != null, "The instance ID should not be a null reference.");
                Debug.Assert(importConnections != null, "The import connections collection should not be a null reference.");
            }

            var updatedInstances = new List<InstanceUpdate>();
            if (!m_InstanceToDefinitionMap.ContainsKey(instance))
            {
                return updatedInstances;
            }

            // Determine if there are any changes that need to be applied to the object
            var importEdges = m_InstanceConnections.InEdges(instance);
            var differentImports = importEdges
                .Select(
                    e => new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                        e.ImportRegistration,
                        e.Source,
                        e.ExportRegistration))
                .Except(importConnections);

            if (!differentImports.Any() && (InstanceById(instance) != null) && !shouldDeleteInstance && !forceUpdate)
            {
                return updatedInstances;
            }

            // The definition should always either have more imports defined or the same number, never less
            // because the excessive imports are not recognized. Hence doing this comparison will tell us
            // if we're missing required imports
            var definition = DefinitionForInstance(instance);
            var missingRequiredImports = definition
                .RegisteredImports
                .Where(i => definition.Import(i).IsPrerequisite)
                .Except(
                    importConnections
                        .Where(t => definition.Import(t.Item1).IsPrerequisite)
                        .Select(t => t.Item1));

            var shouldRecreate = InstanceById(instance) == null 
                || forceUpdate 
                || differentImports.Any(p => definition.Import(p.Item1).IsPrerequisite);
            var shouldUpdate = forceUpdate || shouldRecreate || differentImports.Any();
            var shouldDelete = (shouldDeleteInstance || missingRequiredImports.Any()) && (InstanceById(instance) != null);

            if (!shouldDelete && (shouldUpdate || shouldRecreate))
            {
                // If there are any changes to any of the constructor imports then we have to rebuild the object
                if (shouldRecreate)
                {
                    DestroyInstance(InstanceById(instance));
                    StoreInstanceById(instance, ConstructInstance(instance, importConnections));
                }

                // Update any of the property imports. We do this in all cases because if there were
                // changes to the constructor imports we now have a new object. If there were no changes
                // to the constructor imports then there must be some changes to the property imports, either
                // way we'll need to update the property imports
                UpdatePropertyImports(instance, importConnections);

                // Store the new connections by removing all the import edges and then re-creating them
                m_InstanceConnections.RemoveInEdgeIf(instance, edge => true);
                foreach (var pair in importConnections)
                {
                    m_InstanceConnections.AddEdge(
                        new PartImportExportEdge<PartInstanceId>(
                            instance,
                            pair.Item1,
                            pair.Item2,
                            pair.Item3));
                }

                if (shouldRecreate || forceUpdate)
                {
                    var list = UpdateDependencies(instance);
                    updatedInstances.AddRange(list);
                }

                updatedInstances.Add(
                    new InstanceUpdate
                    {
                        Instance = instance,
                        Change = shouldRecreate ? InstanceChange.Reconstructed : InstanceChange.Updated,
                    });
            }
            else
            {
                var list = RemoveInstanceAndUpdateDependencies(instance);
                updatedInstances.AddRange(list);
                updatedInstances.Add(
                    new InstanceUpdate
                        {
                            Instance = instance,
                            Change = InstanceChange.Removed,
                        });
            }

            return updatedInstances;
        }

        private GroupPartDefinition DefinitionForInstance(PartInstanceId instance)
        {
            var partRegistrationId = m_InstanceToDefinitionMap[instance];
            return m_PartDefinitions[partRegistrationId];
        }

        private object InstanceById(PartInstanceId instance)
        {
            if (!m_HistoryInstances.ContainsKey(instance) && !m_NonHistoryInstances.ContainsKey(instance))
            {
                return null;
            }

            return m_HistoryInstances.ContainsKey(instance) ? m_HistoryInstances[instance] : m_NonHistoryInstances[instance];
        }

        private void StoreInstanceById(PartInstanceId instance, object instanceObj)
        {
            var historyObj = instanceObj as IAmHistoryEnabled;
            if (historyObj != null)
            {
                if (!m_HistoryInstances.ContainsKey(instance))
                {
                    m_HistoryInstances.Add(instance, historyObj);
                }
                else
                {
                    m_HistoryInstances[instance] = historyObj;
                }
            }
            else
            {
                if (!m_NonHistoryInstances.ContainsKey(instance))
                {
                    m_NonHistoryInstances.Add(instance, instanceObj);
                }
                else
                {
                    m_NonHistoryInstances[instance] = instanceObj;
                }
            }
        }

        private object ConstructInstance(
            PartInstanceId instance,
            IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> importConnections)
        {
            var definition = DefinitionForInstance(instance);
            var constructorImports = importConnections
                .Where(p => definition.Import(p.Item1) is ConstructorBasedImportDefinition)
                .Select(
                    p => new Tuple<ConstructorBasedImportDefinition, PartInstanceId, ExportRegistrationId>(
                        definition.Import(p.Item1) as ConstructorBasedImportDefinition,
                        p.Item2,
                        p.Item3));

            var parameterObjects = ConstructorImports(constructorImports);
            var type = LoadType(definition.Identity);
            try
            {
                return Activator.CreateInstance(type, parameterObjects);
            }
            catch (NotSupportedException e)
            {
                // type is a TypeBuilder, Type is a TypeReference, ArgIterator, Void, RuntimeArgumentHandle or an array of either
                // one of those types, the assembly that contains type is a dynamic assembly that was created with 
                // AssemblyBuilderAccess.Save, the constructor that best matches args has varargs arguments
                throw new ConstructionOfPluginTypeFailedException(Resources.Exceptions_Messages_ConstructionOfPluginTypeFailed, e);
            }
            catch (TargetInvocationException e)
            {
                // The constructor being called throws an exception
                throw new ConstructionOfPluginTypeFailedException(Resources.Exceptions_Messages_ConstructionOfPluginTypeFailed, e);
            }
            catch (MissingMethodException e)
            {
                // No matching public constructor was found
                throw new ConstructionOfPluginTypeFailedException(Resources.Exceptions_Messages_ConstructionOfPluginTypeFailed, e);
            }
            catch (MethodAccessException e)
            {
                // The caller does not have permission to call this constructor
                throw new ConstructionOfPluginTypeFailedException(Resources.Exceptions_Messages_ConstructionOfPluginTypeFailed, e);
            }
            catch (MemberAccessException e)
            {
                // Cannot create an instance of an abstract class or this method was invoked with a late-binding mechanism
                throw new ConstructionOfPluginTypeFailedException(Resources.Exceptions_Messages_ConstructionOfPluginTypeFailed, e);
            }
        }

        /// <summary>
        /// Creates instances of all the constructor parameters.
        /// </summary>
        /// <param name="constructorImports">The collection containing all the constructor imports and their matched exports.</param>
        /// <returns>An array containing the constructor parameters in the correct order.</returns>
        private object[] ConstructorImports(
            IEnumerable<Tuple<ConstructorBasedImportDefinition, PartInstanceId, ExportRegistrationId>> constructorImports)
        {
            if (!constructorImports.Any())
            {
                return new object[0];
            }

            // MEF only allows one constructor to have the ImportingConstructorAttribute
            // so all the constructor imports for the current type are from the same
            // constructor
            ConstructorDefinition constructor = constructorImports.First().Item1.Constructor;
            var constructorParameters = new List<Tuple<ParameterDefinition, List<Tuple<PartInstanceId, ExportRegistrationId>>>>();
            foreach (var pair in constructorImports)
            {
                var parameter = pair.Item1.Parameter;
                var index = constructorParameters.FindIndex(p => p.Item1.Equals(parameter));
                if (index > -1)
                {
                    var tuple = constructorParameters[index];
                    tuple.Item2.Add(new Tuple<PartInstanceId, ExportRegistrationId>(pair.Item2, pair.Item3));
                }
                else
                {
                    constructorParameters.Add(
                        new Tuple<ParameterDefinition, List<Tuple<PartInstanceId, ExportRegistrationId>>>(
                            parameter,
                            new List<Tuple<PartInstanceId, ExportRegistrationId>> 
                                { 
                                    new Tuple<PartInstanceId, ExportRegistrationId>(pair.Item2, pair.Item3) 
                                }));
                }
            }

            constructorParameters.Sort(
                (first, second) => constructor.Parameters.IndexOf(first.Item1) - constructor.Parameters.IndexOf(second.Item1));

            return constructorParameters
                .Select(
                    (p, index) =>
                    {
                        var type = LoadType(p.Item1.Identity);
                        return ExportsForImportedType(type, p.Item2);
                    })
                .ToArray();
        }

        private object ExportsForImportedType(Type parameterType, IEnumerable<Tuple<PartInstanceId, ExportRegistrationId>> exports)
        {
            if (parameterType.IsGenericType)
            {
                var genericBaseType = parameterType.GetGenericTypeDefinition();
                if (genericBaseType == typeof(IEnumerable<>))
                {
                    var genericArguments = parameterType.GetGenericArguments();
                    Debug.Assert(genericArguments.Length == 1, "IEnumerable<T> should only have 1 generic parameter.");

                    var genericParameter = genericArguments[0];
                    var listType = typeof(List<>).MakeGenericType(genericParameter);
                    var list = Activator.CreateInstance(listType) as IList;
                    foreach (var export in exports)
                    {
                        list.Add(ExportForImportedType(genericParameter, export.Item1, export.Item2));
                    }

                    return list;
                }
            }

            // We don't care what it is, it's not a collection. Note that we (currently) do
            // not support Lazy<IEnumerable<T>> or any of those kinds of things
            var pair = exports.First();
            return ExportForImportedType(parameterType, pair.Item1, pair.Item2);
        }

        private object ExportForImportedType(Type parameterType, PartInstanceId exportingPart, ExportRegistrationId exportId)
        {
            var exportDefinition = ExportDefinitionForPart(exportingPart, exportId);
            if (parameterType.IsGenericType)
            {
                var genericBaseType = parameterType.GetGenericTypeDefinition();
                var genericParameters = parameterType.GetGenericArguments();
                if (genericBaseType == typeof(Lazy<>))
                {
                    var lazyType = typeof(Lazy<>).MakeGenericType(genericParameters[0]);
                    return Activator.CreateInstance(
                        lazyType,
                        ParameterlessFunctionFromExport(typeof(Func<>).MakeGenericType(genericParameters[0]), exportingPart, exportDefinition));
                }

                if (genericBaseType == typeof(Lazy<,>))
                {
                    throw new NotImplementedException();
                }

                if (genericBaseType == typeof(Func<>))
                {
                    return ParameterlessFunctionFromExport(parameterType, exportingPart, exportDefinition);
                }

                // The only other option(s) that we support is that the export could be a method
                // which matches the import signature, meaning that the import needs to be a delegate
                var methodExport = exportDefinition as MethodBasedExportDefinition;
                if ((methodExport != null) && typeof(Delegate).IsAssignableFrom(parameterType))
                {
                    var exportedMethod = GetMethodInfoFromDefinition(LoadType(methodExport.DeclaringType), methodExport.Method);
                    if (IsMethodCompatibleWithDelegate(parameterType, exportedMethod))
                    {
                        return CreateDelegateFromMethod(parameterType, exportingPart, exportedMethod);
                    }
                }
            }

            // The parameter type may be generic, but it's not one of the special ones, so the output only
            // depends on the export.
            return ReturnValueFromExport(exportingPart, exportDefinition);
        }

        private SerializableExportDefinition ExportDefinitionForPart(PartInstanceId exportingInstance, ExportRegistrationId exportId)
        {
            var exportingPartDefinition = DefinitionForInstance(exportingInstance);
            return exportingPartDefinition.Export(exportId);
        }

        private object ParameterlessFunctionFromExport(
            Type delegateType,
            PartInstanceId exportingInstance,
            SerializableExportDefinition exportDefinition)
        {
            var obj = InstanceById(exportingInstance);

            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                var exportedMethod = GetMethodInfoFromDefinition(LoadType(methodExport.DeclaringType), methodExport.Method);
                if (!IsMethodCompatibleWithDelegate(delegateType, exportedMethod))
                {
                    throw new ConstructionOfPluginTypeFailedException();
                }

                return CreateDelegateFromMethod(delegateType, exportingInstance, exportedMethod);
            }

            var propertyExport = exportDefinition as PropertyBasedExportDefinition;
            if (propertyExport != null)
            {
                return PropertyExportToFuncImport(delegateType, obj, propertyExport);
            }

            var typeExport = exportDefinition as TypeBasedExportDefinition;
            Debug.Assert(typeExport != null, "The export should really be a type export.");
            return TypeExportToFuncImport(delegateType, obj);
        }

        private object CreateDelegateFromMethod(Type delegateType, PartInstanceId instance, MethodInfo method)
        {
            var obj = InstanceById(instance);
            return Delegate.CreateDelegate(delegateType, obj, method);
        }

        private object PropertyExportToFuncImport(Type delegateType, object exportingObject, PropertyBasedExportDefinition propertyExport)
        {
            var propertyInfo = GetPropertyInfoFromDefinition(exportingObject.GetType(), propertyExport.Property);
            var method = GetType()
                .GetMethod(
                    "ToFunc",
                    BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(object), typeof(PropertyInfo) },
                    null)
                .MakeGenericMethod(MethodInfoForDelegate(delegateType).ReturnType);
            return method.Invoke(null, new[] { exportingObject, propertyInfo });
        }

        private object TypeExportToFuncImport(Type delegateType, object exportingObject)
        {
            var method = GetType()
                .GetMethod(
                    "ToFunc",
                    BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(object) },
                    null)
                .MakeGenericMethod(MethodInfoForDelegate(delegateType).ReturnType);
            return method.Invoke(null, new[] { exportingObject });
        }

        private object ReturnValueFromExport(PartInstanceId exportingInstance, SerializableExportDefinition exportDefinition)
        {
            var obj = InstanceById(exportingInstance);

            var methodExport = exportDefinition as MethodBasedExportDefinition;
            if (methodExport != null)
            {
                if (methodExport.Method.Parameters.Count != 0)
                {
                    throw new ConstructionOfPluginTypeFailedException();
                }

                var methodInfo = GetMethodInfoFromDefinition(obj.GetType(), methodExport.Method);
                return methodInfo.Invoke(obj, null);
            }

            var propertyExport = exportDefinition as PropertyBasedExportDefinition;
            if (propertyExport != null)
            {
                var propertyInfo = GetPropertyInfoFromDefinition(obj.GetType(), propertyExport.Property);
                return propertyInfo.GetValue(obj);
            }

            return obj;
        }

        private void UpdatePropertyImports(
            PartInstanceId instance,
            IEnumerable<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> importConnections)
        {
            var definition = DefinitionForInstance(instance);
            var propertyImports = definition
                .RegisteredImports
                .Where(i => definition.Import(i) is PropertyBasedImportDefinition)
                .SelectMany(
                    p =>
                    {
                        var tuples = importConnections
                            .Where(t => t.Item1.Equals(p))
                            .Select(
                                t => new Tuple<PropertyBasedImportDefinition, PartInstanceId, ExportRegistrationId>(
                                    definition.Import(p) as PropertyBasedImportDefinition,
                                    t.Item2,
                                    t.Item3));

                        return tuples.Any()
                            ? tuples
                            : new List<Tuple<PropertyBasedImportDefinition, PartInstanceId, ExportRegistrationId>>
                                {
                                    new Tuple<PropertyBasedImportDefinition, PartInstanceId, ExportRegistrationId>(
                                        definition.Import(p) as PropertyBasedImportDefinition,
                                        null,
                                        null)
                                };
                    });

            var instanceObj = InstanceById(instance);
            AssignPropertyImports(instanceObj, propertyImports);
        }

        private void AssignPropertyImports(
            object importObject,
            IEnumerable<Tuple<PropertyBasedImportDefinition, PartInstanceId, ExportRegistrationId>> propertyImports)
        {
            var sortedImports = new List<Tuple<PropertyDefinition, List<Tuple<PartInstanceId, ExportRegistrationId>>>>();
            foreach (var pair in propertyImports)
            {
                var property = pair.Item1.Property;
                var index = sortedImports.FindIndex(p => p.Item1.Equals(property));
                if (index > 1)
                {
                    if (pair.Item2 != null)
                    {
                        var tuple = sortedImports[index];
                        tuple.Item2.Add(new Tuple<PartInstanceId, ExportRegistrationId>(pair.Item2, pair.Item3));
                    }
                }
                else
                {
                    var list = new List<Tuple<PartInstanceId, ExportRegistrationId>>();
                    if (pair.Item2 != null)
                    {
                        list.Add(new Tuple<PartInstanceId, ExportRegistrationId>(pair.Item2, pair.Item3));
                    }

                    sortedImports.Add(new Tuple<PropertyDefinition, List<Tuple<PartInstanceId, ExportRegistrationId>>>(property, list));
                }
            }

            foreach (var pair in sortedImports)
            {
                var type = LoadType(pair.Item1.PropertyType);
                var exportObject = (pair.Item2.Count != 0) ? ExportsForImportedType(type, pair.Item2) : GetDefaultValue(type);

                var info = GetPropertyInfoFromDefinition(importObject.GetType(), pair.Item1);
                info.SetValue(importObject, exportObject);
            }
        }

        private IEnumerable<InstanceUpdate> UpdateDependencies(PartInstanceId instance)
        {
            var result = new List<InstanceUpdate>();

            var dependantInstances = m_InstanceConnections.OutEdges(instance).Select(e => e.Target);
            foreach (var dependant in dependantInstances)
            {
                var newExports = m_InstanceConnections
                    .InEdges(dependant)
                    .Select(
                        e => new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            e.ImportRegistration,
                            e.Source,
                            e.ExportRegistration));
                var list = UpdateIfRequired(dependant, newExports, false, true);
                result.AddRange(list);
            }

            return result;
        }

        private IEnumerable<InstanceUpdate> RemoveInstanceAndUpdateDependencies(PartInstanceId instance)
        {
            var result = new List<InstanceUpdate>();

            var dependantInstances = m_InstanceConnections.OutEdges(instance).Select(e => e.Target).ToList();
            foreach (var dependant in dependantInstances)
            {
                var newExports = m_InstanceConnections
                    .InEdges(dependant)
                    .Where(e => !e.Source.Equals(instance))
                    .Select(
                        e => new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            e.ImportRegistration,
                            e.Source,
                            e.ExportRegistration));
                var list = UpdateIfRequired(dependant, newExports, false, false);
                result.AddRange(list);
            }

            var obj = InstanceById(instance);
            DestroyInstance(obj);

            m_InstanceConnections.RemoveVertex(instance);
            m_InstanceToDefinitionMap.Remove(instance);

            if (obj is IAmHistoryEnabled)
            {
                m_HistoryInstances.Remove(instance);
            }
            else
            {
                m_NonHistoryInstances.Remove(instance);
            }

            return result;
        }

        /// <summary>
        /// Removes the part instance with the given ID from the layer.
        /// </summary>
        /// <remarks>
        /// Note that removing a part may also remove other parts if the current part was used as a constructor parameter for those parts.
        /// </remarks>
        /// <param name="instance">The ID of the part instance.</param>
        /// <returns>A collection containing the state information for all instances that were touched.</returns>
        public IEnumerable<InstanceUpdate> Release(PartInstanceId instance)
        {
            return UpdateIfRequired(instance, Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>(), true, true);
        }

        /// <summary>
        /// Returns a value indicating if an instance exists for the given instance ID.
        /// </summary>
        /// <param name="instance">The ID of the instance.</param>
        /// <returns>
        ///     <see langword="true" /> if an instance exists for the given ID; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasInstanceFor(PartInstanceId instance)
        {
            return m_InstanceToDefinitionMap.ContainsKey(instance);
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<PartInstanceId> GetEnumerator()
        {
            foreach (var pair in m_InstanceToDefinitionMap)
            {
                yield return pair.Key;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
