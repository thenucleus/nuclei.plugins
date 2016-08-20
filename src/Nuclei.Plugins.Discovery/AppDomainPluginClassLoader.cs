//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base;
using Apollo.Core.Base.Scheduling;
using Apollo.Utilities;
using Autofac;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Plugins.Discovery
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
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Cannot make this static because then it wouldn't be executed on the remote instance, but on the local proxy.")]
        public IAssemblyScanner Load(IPluginRepository repository, ILogMessagesFromRemoteAppDomains logger)
        {
            try
            {
                var builder = new ContainerBuilder();
                {
                    builder.RegisterModule(new SchedulingModule());

                    builder.Register(c => new PartImportEngine(
                            c.Resolve<ISatisfyPluginRequests>()))
                        .As<IConnectParts>();
                    
                    builder.RegisterInstance(repository)
                        .As<IPluginRepository>()
                        .As<ISatisfyPluginRequests>();
                }

                var container = builder.Build();

                Func<IBuildFixedSchedules> scheduleBuilder = () => container.Resolve<IBuildFixedSchedules>();
                return new RemoteAssemblyScanner(
                    container.Resolve<IPluginRepository>(),
                    container.Resolve<IConnectParts>(),
                    logger,
                    scheduleBuilder);
            }
            catch (Exception e)
            {
                logger.Log(LevelToLog.Error, e.ToString());
                throw;
            }
        }
    }
}
