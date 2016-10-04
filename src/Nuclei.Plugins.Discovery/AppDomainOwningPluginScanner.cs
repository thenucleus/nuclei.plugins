//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Reflection;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery.Properties;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Provides an <see cref="IAssemblyScanner"/> wrapper that loads the actual scanner into a <c>AppDomain</c>, provides the data
    /// to that scanner and then unloads the <c>AppDomain</c> when the scanning process is complete.
    /// </summary>
    public sealed class AppDomainOwningPluginScanner : IAssemblyScanner
    {
        /// <summary>
        /// The name for the plugins directory.
        /// </summary>
        private const string PluginsDirectoryName = "plugins";

        /// <summary>
        /// The function that builds an <c>AppDomain</c> when requested.
        /// </summary>
        private readonly CreateAppDomain _appDomainBuilder;

        /// <summary>
        /// The object that provides the configuration for the application.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// The object that provides the diagnostics for the system.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The object that provides an abstraction of the file system.
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// The object that stores information about all the known parts and part groups.
        /// </summary>
        private readonly IPluginRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainOwningPluginScanner"/> class.
        /// </summary>
        /// <param name="appDomainBuilder">The function that is used to create a new <c>AppDomain</c> which will be used to scan plugins.</param>
        /// <param name="repository">The object that contains the information about all the known parts and part groups.</param>
        /// <param name="configuration">The object that provides the configuration for the application.</param>
        /// <param name="fileSystem">The object that provides an abstraction of the file system.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="appDomainBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="repository"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="fileSystem"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public AppDomainOwningPluginScanner(
            CreateAppDomain appDomainBuilder,
            IPluginRepository repository,
            IConfiguration configuration,
            IFileSystem fileSystem,
            SystemDiagnostics diagnostics)
        {
            if (appDomainBuilder == null)
            {
                throw new ArgumentNullException("appDomainBuilder");
            }

            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _appDomainBuilder = appDomainBuilder;
            _configuration = configuration;
            _diagnostics = diagnostics;
            _fileSystem = fileSystem;
            _repository = repository;
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
            var paths = _configuration.HasValueFor(PluginDiscoveryConfigurationKeys.PluginDirectories)
                ? _configuration.Value(PluginDiscoveryConfigurationKeys.PluginDirectories)
                : new string[]
                    {
                        _fileSystem.Path.Combine(Assembly.GetExecutingAssembly().LocalDirectoryPath(), PluginsDirectoryName)
                    };

            var domain = _appDomainBuilder(Resources.Plugins_PluginScanDomainName, paths);
            try
            {
                // Inject the actual scanner
                var loader = domain.CreateInstanceAndUnwrap(
                    typeof(AppDomainPluginClassLoader).Assembly.FullName,
                    typeof(AppDomainPluginClassLoader).FullName) as AppDomainPluginClassLoader;

                var logger = new LogForwardingPipe(_diagnostics);
                var repositoryProxy = new PluginRepositoryProxy(_repository);
                var scannerProxy = loader.Load(repositoryProxy, logger);
                scannerProxy.Scan(assemblyFilesToScan);
            }
            finally
            {
                if ((domain != null) && !AppDomain.CurrentDomain.Equals(domain))
                {
                    AppDomain.Unload(domain);
                }
            }
        }
    }
}
