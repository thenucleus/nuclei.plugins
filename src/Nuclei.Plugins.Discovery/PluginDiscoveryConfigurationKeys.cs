﻿//-----------------------------------------------------------------------
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
    /// Defines the <see cref="ConfigurationKeyBase"/> objects for the plugin discovery system.
    /// </summary>
    public static class PluginDiscoveryConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKeyBase"/> that is used to retrieve the plugin directories for the application.
        /// </summary>
        [SuppressMessage(
                "Microsoft.Security",
                "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
                Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey<string[]> PluginDirectories
                = new ConfigurationKey<string[]>("PluginDirectories");

        /// <summary>
        /// The configuration key that is used to retrieve the paths in which
        /// plugin files are placed.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey objects are immutable.")]
        public static readonly ConfigurationKey<string[]> PluginSearchDirectories
            = new ConfigurationKey<string[]>("PluginSearchDirectories");

        /// <summary>
        /// Returns a collection containing all the configuration keys for the diagnostics section.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the diagnostics section.</returns>
        public static IEnumerable<ConfigurationKeyBase> ToCollection()
        {
            return new List<ConfigurationKeyBase>
                {
                    PluginDirectories,
                    PluginSearchDirectories,
                };
        }
    }
}
