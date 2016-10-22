//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the interface for objects that handle plugin detection.
    /// </summary>
    public interface IProcessPluginOriginChanges
    {
        /// <summary>
        /// Gets a collection that describes what types of plugins are accepted by the processor.
        /// </summary>
        IEnumerable<IPluginType> AcceptedPluginTypes
        {
            get;
        }

        /// <summary>
        /// Processes the added plugins.
        /// </summary>
        /// <param name="newPlugins">The collection that contains the names of all the new plugins.</param>
        void Added(params PluginOrigin[] newPlugins);

        /// <summary>
        /// Processes the removed plugins.
        /// </summary>
        /// <param name="removedPlugins">The collection that contains the names of all the plugins that were removed.</param>
        void Removed(params PluginOrigin[] removedPlugins);
    }
}
