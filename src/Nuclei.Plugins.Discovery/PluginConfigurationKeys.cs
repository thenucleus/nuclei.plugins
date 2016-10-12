//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the configuration keys for the plugin discovery part of the application.
    /// </summary>
    public static class PluginConfigurationKeys
    {
        /// <summary>
        /// The configuration key that is used to retrieve the paths in which
        /// single plugin files are placed.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey<string[]> PluginLocations
            = new ConfigurationKey<string[]>("PluginLocations");

        /// <summary>
        /// Returns a collection containing all the configuration keys for the application.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the application.</returns>
        public static IEnumerable<ConfigurationKeyBase> ToCollection()
        {
            return new List<ConfigurationKeyBase>
                {
                    PluginLocations,
                };
        }
    }
}
