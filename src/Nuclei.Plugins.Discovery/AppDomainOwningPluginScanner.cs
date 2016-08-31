//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apollo.Core.Host.Properties;
using Apollo.Utilities;
using Nuclei.Diagnostics;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Provides an <see cref="IAssemblyScanner"/> wrapper that loads the actual scanner into a <c>AppDomain</c>, provides the data
    /// to that scanner and then unloads the <c>AppDomain</c> when the scanning process is complete.
    /// </summary>
    internal sealed class AppDomainOwningPluginScanner : IAssemblyScanner
    {
        /// <summary>
        /// The function that builds an <c>AppDomain</c> when requested.
        /// </summary>
        private readonly Func<string, AppDomainPaths, AppDomain> m_AppDomainBuilder;

        /// <summary>
        /// The object that stores information about all the known parts and part groups.
        /// </summary>
        private readonly IPluginRepository m_Repository;

        /// <summary>
        /// The object that provides the diagnostics for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainOwningPluginScanner"/> class.
        /// </summary>
        /// <param name="appDomainBuilder">The function that is used to create a new <c>AppDomain</c> which will be used to scan plugins.</param>
        /// <param name="repository">The object that contains the information about all the known parts and part groups.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        public AppDomainOwningPluginScanner(
            Func<string, AppDomainPaths, AppDomain> appDomainBuilder, 
            IPluginRepository repository,
            SystemDiagnostics diagnostics)
        {
            {
                Debug.Assert(appDomainBuilder != null, "The AppDomain building function should not be a null reference.");
                Debug.Assert(repository != null, "The repository object should not be a null reference.");
                Debug.Assert(diagnostics != null, "The diagnostics object should not be a null reference.");
            }

            m_AppDomainBuilder = appDomainBuilder;
            m_Repository = repository;
            m_Diagnostics = diagnostics;
        }

        /// <summary>
        /// Scans the assemblies for which the given file paths have been provided and 
        /// returns the plugin description information.
        /// </summary>
        /// <param name="assemblyFilesToScan">
        /// The collection that contains the file paths to all the assemblies to be scanned.
        /// </param>
        public void Scan(IEnumerable<string> assemblyFilesToScan)
        {
            var domain = m_AppDomainBuilder(Resources.Plugins_PluginScanDomainName, AppDomainPaths.Plugins);
            try
            {
                // Inject the actual scanner
                var loader = domain.CreateInstanceAndUnwrap(
                    typeof(AppDomainPluginClassLoader).Assembly.FullName,
                    typeof(AppDomainPluginClassLoader).FullName) as AppDomainPluginClassLoader;

                var logger = new LogForwardingPipe(m_Diagnostics);
                var repositoryProxy = new PluginRepositoryProxy(m_Repository);
                var scannerProxy = loader.Load(repositoryProxy, logger);
                scannerProxy.Scan(assemblyFilesToScan);
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }
        }
    }
}
