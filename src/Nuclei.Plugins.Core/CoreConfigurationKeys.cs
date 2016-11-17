//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKeyBase"/> objects for the plugin system.
    /// </summary>
    public static class CoreConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKeyBase"/> that is used to retrieve the directory in which
        /// assemblies from NuGet packages are copied so that they can be used.
        /// </summary>
        [SuppressMessage(
                "Microsoft.Security",
                "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
                Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey<string> AssemblyCacheLocation
                = new ConfigurationKey<string>("AssemblyCacheLocation");

        /// <summary>
        /// Returns a collection containing all the configuration keys for the diagnostics section.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the diagnostics section.</returns>
        public static IEnumerable<ConfigurationKeyBase> ToCollection()
        {
            return new List<ConfigurationKeyBase>
                {
                    AssemblyCacheLocation,
                };
        }
    }
}
