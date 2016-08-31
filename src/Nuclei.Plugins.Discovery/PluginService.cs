//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the <see cref="KernelService"/> that handles the plugin interaction with the kernel.
    /// </summary>
    internal sealed class PluginService : KernelService
    {
        /// <summary>
        /// The object that stores the configuration for the current application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The object that detects the available plugins.
        /// </summary>
        private readonly PluginDetector m_Detector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginService"/> class.
        /// </summary>
        /// <param name="configuration">The object that stores the configuration for the current application.</param>
        /// <param name="detector">The object that detects the available plugins.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="detector"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public PluginService(IConfiguration configuration, PluginDetector detector, SystemDiagnostics diagnostics)
            : base(diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => detector);
            }

            m_Configuration = configuration;
            m_Detector = detector;
        }

        /// <summary>
        /// Provides derivative classes with a possibility to
        /// perform startup tasks.
        /// </summary>
        protected override void StartService()
        {
            if (!m_Configuration.HasValueFor(CoreConfigurationKeys.PluginLocation))
            {
                return;
            }

            var pluginDirectories = m_Configuration.Value<List<string>>(CoreConfigurationKeys.PluginLocation);
            foreach (var dir in pluginDirectories)
            {
                m_Detector.SearchDirectory(dir);
            }
        }
    }
}
