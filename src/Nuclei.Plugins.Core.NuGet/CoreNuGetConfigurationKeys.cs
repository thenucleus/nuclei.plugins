//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKeyBase"/> objects for the plugin discovery system.
    /// </summary>
    public static class CoreNuGetConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKeyBase"/> that is used to retrieve the directory in which
        /// NuGet packages can be installed.
        /// </summary>
        [SuppressMessage(
                "Microsoft.Security",
                "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
                Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey<string> LocalInstallLocation
                = new ConfigurationKey<string>("LocalInstallLocation");

        /// <summary>
        /// The <see cref="ConfigurationKeyBase"/> that is used to retrieve the plugin directories for the application.
        /// </summary>
        [SuppressMessage(
                "Microsoft.Security",
                "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
                Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey<string[]> NuGetFeeds
                = new ConfigurationKey<string[]>("NuGetFeeds");

        /// <summary>
        /// Returns a collection containing all the configuration keys for the diagnostics section.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the diagnostics section.</returns>
        public static IEnumerable<ConfigurationKeyBase> ToCollection()
        {
            return new List<ConfigurationKeyBase>
                {
                    LocalInstallLocation,
                    NuGetFeeds,
                };
        }
    }
}
