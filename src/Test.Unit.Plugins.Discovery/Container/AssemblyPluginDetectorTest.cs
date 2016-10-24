//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Discovery.Origin.FileSystem;
using NUnit.Framework;

namespace Nuclei.Plugins.Discovery.Container
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class AssemblyPluginDetectorTest
    {
        private sealed class MockScanner : IAssemblyScanner
        {
            private IEnumerable<PluginFileOrigin> _files;

            public void Scan(IEnumerable<PluginFileOrigin> assemblyFilesToScan)
            {
                _files = assemblyFilesToScan;
            }

            public IEnumerable<PluginFileOrigin> FilesToScan
            {
                get
                {
                    return _files;
                }
            }
        }

        [Test]
        public void Added()
        {
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginOrigins())
                    .Returns(Enumerable.Empty<PluginFileOrigin>());
            }

            var files = new List<PluginFileOrigin>
                {
                    new PluginFileOrigin(@"c:\temp\foobar.dll", DateTimeOffset.Now, DateTimeOffset.Now),
                    new PluginFileOrigin(@"c:\temp\foobar2.dll", DateTimeOffset.Now.AddHours(-2), DateTimeOffset.Now),
                };

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new AssemblyPluginDetector(
                repository.Object,
                scannerBuilder);

            var origins = files.Cast<PluginOrigin>().ToArray();
            detector.Added(origins.ToArray());
            Assert.That(scanner.FilesToScan, Is.EquivalentTo(files));
        }

        [Test]
        public void Removed()
        {
            var files = new List<PluginFileOrigin>
                {
                    new PluginFileOrigin(@"c:\temp\foobar.dll", DateTimeOffset.Now, DateTimeOffset.Now),
                    new PluginFileOrigin(@"c:\temp\foobar2.dll", DateTimeOffset.Now.AddHours(-2), DateTimeOffset.Now),
                };

            IEnumerable<PluginOrigin> removedPlugins = null;
            var repository = new Mock<IPluginRepository>();
            {
                repository.Setup(r => r.KnownPluginOrigins())
                    .Returns(files);
                repository.Setup(r => r.RemovePlugins(It.IsAny<IEnumerable<PluginOrigin>>()))
                    .Callback<IEnumerable<PluginOrigin>>(i => removedPlugins = i);
            }

            var scanner = new MockScanner();
            Func<IPluginRepository, IAssemblyScanner> scannerBuilder = r => scanner;

            var detector = new AssemblyPluginDetector(
                repository.Object,
                scannerBuilder);

            var origins = files.Cast<PluginOrigin>().ToArray();
            detector.Removed(origins);
            Assert.IsNotNull(removedPlugins);
            Assert.AreSame(origins, removedPlugins);
        }
    }
}
