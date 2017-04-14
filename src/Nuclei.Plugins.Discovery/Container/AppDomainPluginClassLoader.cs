//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Discovery.Container
{
    /// <summary>
    /// Defines methods to load an <see cref="IAssemblyScanner"/> object into a remote <c>AppDomain</c>.
    /// </summary>
    internal sealed class AppDomainPluginClassLoader : MarshalByRefObject
    {
        /// <summary>
        /// Loads the <see cref="IAssemblyScanner"/> object into the <c>AppDomain</c> in which the current
        /// object is currently loaded.
        /// </summary>
        /// <param name="repository">The object that contains all the part and part group information.</param>
        /// <param name="logger">The object that provides the logging for the remote <c>AppDomain</c>.</param>
        /// <returns>The newly created <see cref="IAssemblyScanner"/> object.</returns>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Cannot make this static because then it wouldn't be executed on the remote instance, but on the local proxy.")]
        public IAssemblyScanner Load(IPluginRepository repository, ILogMessagesFromRemoteAppDomains logger)
        {
            try
            {
                return new RemoteAssemblyScanner(
                    repository,
                    logger);
            }
            catch (Exception e)
            {
                logger.Log(LevelToLog.Error, e.ToString());
                throw;
            }
        }
    }
}
