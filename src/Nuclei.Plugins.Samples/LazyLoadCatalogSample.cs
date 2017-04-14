//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Composition.Mef;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Core.Assembly;
using Nuclei.Plugins.Discovery.Container;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Samples
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class LazyLoadCatalogSample
    {
        private static IEnumerable<TypeDefinition> _types;
        private static IEnumerable<PartDefinition> _parts;

        private static IPluginRepository CreateRepository()
        {
            var types = new Type[]
                {
                    typeof(ExportOnPropertyWithEnumerable),
                    typeof(ImportOnConstructorWithEnumerable)
                };
            var typeNames = types.Select(t => t.AssemblyQualifiedName).ToList();

            var repository = new PluginRepository();
            foreach (var type in _types)
            {
                repository.AddType(type);
            }

            var origin = new PluginAssemblyOrigin(Assembly.GetExecutingAssembly().LocalFilePath());
            foreach (var part in _parts)
            {
                if (typeNames.Contains(part.Identity.AssemblyQualifiedName))
                {
                    repository.AddPart(part, origin);
                }
            }

            return repository;
        }

        [Test]
        public void Create()
        {
            var repository = CreateRepository();
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithEnumerable();
            container.ComposeParts(obj);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            try
            {
                var types = new List<TypeDefinition>();
                var parts = new List<PartDefinition>();
                var repository = new Mock<IPluginRepository>();
                {
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<string>()))
                        .Returns<string>(n => types.Any(t => t.Identity.AssemblyQualifiedName.Equals(n)));
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<TypeIdentity>()))
                        .Returns<TypeIdentity>(n => types.Any(t => t.Identity.Equals(n)));
                    repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                        .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).First());
                    repository.Setup(r => r.Parts())
                        .Returns(parts);
                    repository.Setup(r => r.AddType(It.IsAny<TypeDefinition>()))
                        .Callback<TypeDefinition>(types.Add);
                    repository.Setup(r => r.AddPart(It.IsAny<PartDefinition>(), It.IsAny<PluginOrigin>()))
                        .Callback<PartDefinition, PluginOrigin>((p, f) => parts.Add(p));
                }

                var scanner = new RemoteAssemblyScanner(
                    repository.Object,
                    new Mock<ILogMessagesFromRemoteAppDomains>().Object);

                var localPath = Assembly.GetExecutingAssembly().LocalFilePath();
                scanner.Scan(new Dictionary<string, PluginOrigin> { { localPath, new PluginAssemblyOrigin(localPath) } });

                _types = types;
                _parts = parts;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception in LazyLoadCatalogTest.Setup: {0}",
                        e));

                throw;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithEnumerable
        {
            [Import]
            public ImportOnConstructorWithEnumerable Import
            {
                get;
                set;
            }
        }
    }
}
