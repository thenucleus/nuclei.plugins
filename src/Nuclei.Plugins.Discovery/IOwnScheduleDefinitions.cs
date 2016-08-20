//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the interface for objects that store newly created schedules.
    /// </summary>
    internal interface IOwnScheduleDefinitions
    {
        /// <summary>
        /// Stores the created schedule and it's associated actions and conditions.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="actionMap">The collection mapping the registered actions to the schedule element that holds the action.</param>
        /// <param name="conditionMap">The collection mapping the registered conditions to the schedule element that holds the condition.</param>
        /// <returns>The ID of the newly created schedule.</returns>
        ScheduleId StoreSchedule(
            ISchedule schedule,
            Dictionary<ScheduleActionRegistrationId, ScheduleElementId> actionMap,
            Dictionary<ScheduleConditionRegistrationId, ScheduleElementId> conditionMap);
    }
}
