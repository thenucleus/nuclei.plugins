//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery;
using Nuclei.Plugins.Discovery.Assembly;
using Nuclei.Plugins.Discovery.Container;
using Nuclei.Plugins.Discovery.Origin.FileSystem;
using NUnit.Framework;

namespace Nuclei.Plugins.Samples
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class FileSystemListenerSample
    {
        private static ILogger NewLogger()
        {
            var result = new Mock<ILogger>();
            {
                result.SetupAllProperties();
            }

            return result.Object;
        }

        [Test]
        public void Enable()
        {
            var fileSystem = new FileSystem();
            var diagnostics = new SystemDiagnostics(NewLogger(), null);

            var configuration = new ApplicationConfiguration(
                new[]
                {
                    PluginDiscoveryConfigurationKeys.PluginSearchDirectories,
                },
                "samples");

            var repository = new PluginRepository();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => new RemoteAssemblyScanner(r, new LogForwardingPipe(diagnostics));

            var scanners = new IProcessPluginOriginChanges[]
                {
                    new AssemblyPluginProcessor(
                        repository,
                        scannerBuilder,
                        fileSystem),
                };

            using (var proxy = new FileSystemWatcherProxy(new FileSystemWatcher()))
            {
                Func<IFileSystemWatcher> watcherBuilder = () => proxy;
                var listener = new FileSystemListener(
                    configuration,
                    scanners,
                    watcherBuilder,
                    diagnostics,
                    fileSystem);

                listener.Enable();
            }
        }
    }
}
