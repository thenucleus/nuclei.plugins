//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Stores information about a schedule for a component group in a serialized form, i.e. without requiring the
    /// assembly which defines the group to be loaded.
    /// </summary>
    [Serializable]
    public sealed class ScheduleDefinition : IEquatable<ScheduleDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ScheduleDefinition first, ScheduleDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ScheduleDefinition first, ScheduleDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ScheduleDefinition"/> class.
        /// </summary>
        /// <param name="containingGroup">The ID of the group that has registered the schedule.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actions">The collection that maps a schedule element to an action.</param>
        /// <param name="conditions">The collection that maps a schedule element to a condition.</param>
        /// <returns>The newly created definition.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="containingGroup"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="schedule"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="actions"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="conditions"/> is <see langword="null" />.
        /// </exception>
        public static ScheduleDefinition CreateDefinition(
            GroupRegistrationId containingGroup,
            ISchedule schedule,
            IDictionary<ScheduleElementId, ScheduleActionRegistrationId> actions,
            IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> conditions)
        {
            {
                Lokad.Enforce.Argument(() => containingGroup);
                Lokad.Enforce.Argument(() => schedule);
                Lokad.Enforce.Argument(() => actions);
                Lokad.Enforce.Argument(() => conditions);
            }

            return new ScheduleDefinition(containingGroup, schedule, actions, conditions);
        }

        /// <summary>
        /// The ID of the group that has registered the schedule.
        /// </summary>
        private readonly GroupRegistrationId m_GroupId;

        /// <summary>
        /// The schedule that is described by this definition.
        /// </summary>
        private readonly ISchedule m_Schedule;

        /// <summary>
        /// The collection that maps a schedule element to a schedule action.
        /// </summary>
        private readonly IDictionary<ScheduleElementId, ScheduleActionRegistrationId> m_Actions;

        /// <summary>
        /// The collection that maps a schedule element to a schedule condition.
        /// </summary>
        private readonly IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> m_Conditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDefinition"/> class.
        /// </summary>
        /// <param name="containingGroup">The ID of the group that has registered the schedule.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actions">The collection that maps a schedule element to an action.</param>
        /// <param name="conditions">The collection that maps a schedule element to a condition.</param>
        private ScheduleDefinition(
            GroupRegistrationId containingGroup,
            ISchedule schedule,
            IDictionary<ScheduleElementId, ScheduleActionRegistrationId> actions,
            IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> conditions)
        {
            {
                Debug.Assert(containingGroup != null, "The containing group ID should not be a null reference.");
                Debug.Assert(schedule != null, "The schedule should not be a null reference.");
                Debug.Assert(actions != null, "The collection of actions should not be a null reference.");
                Debug.Assert(conditions != null, "The collection of conditions should not be a null reference.");
            }

            m_GroupId = containingGroup;
            m_Schedule = schedule;
            m_Actions = actions;
            m_Conditions = conditions;
        }

        /// <summary>
        /// Gets the ID of the group that has registered the schedule.
        /// </summary>
        public GroupRegistrationId ContainingGroup
        {
            get
            {
                return m_GroupId;
            }
        }

        /// <summary>
        /// Gets the schedule for the current group.
        /// </summary>
        public ISchedule Schedule
        {
            get
            {
                return m_Schedule;
            }
        }

        /// <summary>
        /// Gets the collection that maps a schedule element to a schedule action.
        /// </summary>
        public IDictionary<ScheduleElementId, ScheduleActionRegistrationId> Actions
        {
            get
            {
                return m_Actions;
            }
        }

        /// <summary>
        /// Gets the collection that maps a schedule element to a schedule condition.
        /// </summary>
        public IDictionary<ScheduleElementId, ScheduleConditionRegistrationId> Conditions
        {
            get
            {
                return m_Conditions;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="ScheduleDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ScheduleDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="ScheduleDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(ScheduleDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) 
                && ContainingGroup == other.ContainingGroup;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as ScheduleDefinition;
            return Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ ContainingGroup.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Exporting a schedule on {0}",
                ContainingGroup);
        }
    }
}
