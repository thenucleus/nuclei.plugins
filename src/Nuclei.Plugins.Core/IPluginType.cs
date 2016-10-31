//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the interface for classes that define a specific type of plugin, e.g. a plugin file or a given type.
    /// </summary>
    public interface IPluginType : IEquatable<IPluginType>
    {
        /// <summary>
        /// Makes a copy of the current instance.
        /// </summary>
        /// <returns>A copy of the current instance.</returns>
        IPluginType Clone();

        /// <summary>
        /// Returns the origin for the specific plugin.
        /// </summary>
        /// <param name="source">The source of the plugin.</param>
        /// <returns>The <see cref="PluginOrigin"/> object for the plugin.</returns>
        PluginOrigin Origin(string source);
    }
}
