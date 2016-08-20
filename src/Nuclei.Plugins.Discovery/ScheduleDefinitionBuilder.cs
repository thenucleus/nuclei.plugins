//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Apollo.Utilities.History;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Provides methods for the creation of schedules that belong to a specific part group.
    /// </summary>
    internal sealed class ScheduleDefinitionBuilder : IRegisterSchedules
    {
        /// <summary>
        /// The collection of actions that are registered for the current schedule.
        /// </summary>
        private readonly Dictionary<ScheduleActionRegistrationId, ScheduleElementId> m_Actions
            = new Dictionary<ScheduleActionRegistrationId, ScheduleElementId>();

        /// <summary>
        /// The collection of conditions that are registered for the current schedule.
        /// </summary>
        private readonly Dictionary<ScheduleConditionRegistrationId, ScheduleElementId> m_Conditions
            = new Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>();

        /// <summary>
        /// The object that owns the group to which the current schedule will belong.
        /// </summary>
        private readonly IOwnScheduleDefinitions m_Owner;

        /// <summary>
        /// The object that handles the construction of the schedules.
        /// </summary>
        private readonly IBuildFixedSchedules m_Builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDefinitionBuilder"/> class.
        /// </summary>
        /// <param name="owner">The object that is building the part group to which the current schedule will belong.</param>
        /// <param name="scheduleBuilder">The object that is used to build the schedule.</param>
        public ScheduleDefinitionBuilder(IOwnScheduleDefinitions owner, IBuildFixedSchedules scheduleBuilder)
        {
            {
                Debug.Assert(owner != null, "The owner object should not be a null reference.");
                Debug.Assert(scheduleBuilder != null, "The schedule builder object should not be a null reference.");
            }

            m_Owner = owner;
            m_Builder = scheduleBuilder;
        }

        /// <summary>
        /// Adds the executing action with the specified ID to the schedule.
        /// </summary>
        /// <param name="action">The ID of the action that should be added.</param>
        /// <returns>The vertex that contains the information about the given action.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="action"/> is <see langword="null" />.
        /// </exception>
        public ExecutingActionVertex AddExecutingAction(ScheduleActionRegistrationId action)
        {
            {
                Lokad.Enforce.Argument(() => action);
            }

            var scheduleAction = ToScheduleAction(action);
            return m_Builder.AddExecutingAction(scheduleAction);
        }

        private ScheduleElementId ToScheduleAction(ScheduleActionRegistrationId action)
        {
            if (!m_Actions.ContainsKey(action))
            {
                m_Actions.Add(action, new ScheduleElementId());
            }

            return m_Actions[action];
        }

        /// <summary>
        /// Adds the schedule with the specified ID as a sub-schedule to the current schedule.
        /// </summary>
        /// <param name="schedule">The ID of the sub-schedule.</param>
        /// <returns>The vertex that contains the information about the given sub-schedule.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        public SubScheduleVertex AddSubSchedule(ScheduleId schedule)
        {
            {
                Lokad.Enforce.Argument(() => schedule);
            }

            return m_Builder.AddSubSchedule(schedule);
        }

        /// <summary>
        /// Adds a vertex that indicates the start of a synchronization block over which the given variables
        /// should be synchronized when the block ends.
        /// </summary>
        /// <param name="variables">The collection of variables that should be synchronized.</param>
        /// <returns>The vertex that contains the synchronization information.</returns>
        public SynchronizationStartVertex AddSynchronizationStart(IEnumerable<IScheduleVariable> variables)
        {
            return m_Builder.AddSynchronizationStart(variables);
        }

        /// <summary>
        /// Adds a vertex that indicates the end of a synchronization block.
        /// </summary>
        /// <param name="startPoint">The vertex that forms the start point of the block.</param>
        /// <returns>The vertex that indicates the end of a synchronization block.</returns>
        public SynchronizationEndVertex AddSynchronizationEnd(SynchronizationStartVertex startPoint)
        {
            return m_Builder.AddSynchronizationEnd(startPoint);
        }

        /// <summary>
        /// Adds a vertex which indicates that the current values of all history-enabled data should
        /// be stored in the <see cref="Timeline"/> so that it is possible to revert to the
        /// current point in time later on.
        /// </summary>
        /// <returns>The vertex that indicates that the current state should be stored in the <see cref="Timeline"/>.</returns>
        public MarkHistoryVertex AddHistoryMarkingPoint()
        {
            return m_Builder.AddHistoryMarkingPoint();
        }

        /// <summary>
        /// Adds a vertex which can be replaced by another set of vertices.
        /// </summary>
        /// <returns>The vertex that indicates a place in the schedule where new vertices can be inserted.</returns>
        public InsertVertex AddInsertPoint()
        {
            return m_Builder.AddInsertPoint();
        }

        /// <summary>
        /// Adds a vertex which can be replaced by another set of vertices.
        /// </summary>
        /// <param name="maximumNumberOfInserts">The maximum number of times another vertex can be inserted in place of the insert vertex.</param>
        /// <returns>The vertex that indicates a place in the schedule where new vertices can be inserted.</returns>
        public InsertVertex AddInsertPoint(int maximumNumberOfInserts)
        {
            return m_Builder.AddInsertPoint(maximumNumberOfInserts);
        }

        /// <summary>
        /// Links the given start vertex to the end vertex.
        /// </summary>
        /// <param name="source">The start vertex.</param>
        /// <param name="target">The end vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="source"/> to <paramref name="target"/>.
        /// </param>
        public void LinkTo(IScheduleVertex source, IScheduleVertex target, ScheduleConditionRegistrationId traverseCondition = null)
        {
            {
                Lokad.Enforce.Argument(() => source);
                Lokad.Enforce.Argument(() => target);
            }

            ScheduleElementId condition = ToScheduleCondition(traverseCondition);
            m_Builder.LinkTo(source, target, condition);
        }

        private ScheduleElementId ToScheduleCondition(ScheduleConditionRegistrationId traverseCondition = null)
        {
            ScheduleElementId condition = null;
            if (traverseCondition != null)
            {
                if (!m_Conditions.ContainsKey(traverseCondition))
                {
                    m_Conditions.Add(traverseCondition, new ScheduleElementId());
                }

                condition = m_Conditions[traverseCondition];
            }

            return condition;
        }

        /// <summary>
        /// Links the start point of the schedule to the given vertex.
        /// </summary>
        /// <param name="target">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from the start point to <paramref name="target"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="target"/> is <see langword="null" />.
        /// </exception>
        public void LinkFromStart(IScheduleVertex target, ScheduleConditionRegistrationId traverseCondition = null)
        {
            {
                Lokad.Enforce.Argument(() => target);
            }

            ScheduleElementId condition = ToScheduleCondition(traverseCondition);
            m_Builder.LinkFromStart(target, condition);
        }

        /// <summary>
        /// Links the given vertex to the end point of the schedule.
        /// </summary>
        /// <param name="source">The vertex.</param>
        /// <param name="traverseCondition">
        /// The ID of the condition that determines if it is possible to move from <paramref name="source"/> to the end point.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="source"/> is <see langword="null" />.
        /// </exception>
        public void LinkToEnd(IScheduleVertex source, ScheduleConditionRegistrationId traverseCondition = null)
        {
            {
                Lokad.Enforce.Argument(() => source);
            }

            ScheduleElementId condition = ToScheduleCondition(traverseCondition);
            m_Builder.LinkToEnd(source, condition);
        }

        /// <summary>
        /// Registers the schedule with the system.
        /// </summary>
        /// <returns>The ID of the schedule.</returns>
        public ScheduleId Register()
        {
            var schedule = m_Builder.Build();
            return m_Owner.StoreSchedule(schedule, m_Actions, m_Conditions);
        }
    }
}
