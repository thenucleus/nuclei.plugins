﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Apollo.Core.Extensions.Plugins
{
    /// <summary>
    /// Defines the interface for objects that store information about the registration of an object
    /// in a group of plugin components.
    /// </summary>
    public interface IPartRegistration
    {
        /// <summary>
        /// Gets the ID of the current registration.
        /// </summary>
        PartRegistrationId Id
        {
            get;
        }

        /// <summary>
        /// Gets the collection of exports that have been registered for the current object.
        /// </summary>
        IEnumerable<ExportRegistrationId> RegisteredExports
        {
            get;
        }

        /// <summary>
        /// Gets the collection of imports that have been registered for the current object.
        /// </summary>
        IEnumerable<ImportRegistrationId> RegisteredImports
        {
            get;
        }

        /// <summary>
        /// Gets the collection of schedule actions that have been registered for the current object. 
        /// </summary>
        IEnumerable<ScheduleActionRegistrationId> RegisteredActions
        {
            get;
        }

        /// <summary>
        /// Gets the collection of schedule conditions that have been registered for the current object.
        /// </summary>
        IEnumerable<ScheduleConditionRegistrationId> RegisteredConditions
        {
            get;
        }
    }
}
