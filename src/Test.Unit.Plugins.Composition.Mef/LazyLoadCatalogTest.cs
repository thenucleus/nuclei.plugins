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
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Core.Assembly;
using Nuclei.Plugins.Discovery.Container;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Composition.Mef
{
    [TestFixture]
    public sealed class LazyLoadCatalogTest
    {
        private static IEnumerable<TypeDefinition> _types;
        private static IEnumerable<PartDefinition> _parts;

        [Test]
        public void ComposeWithImportingConstructorWithEnumerable()
        {
            var repository = CreateRepository(
                typeof(ExportOnPropertyWithEnumerable),
                typeof(ImportOnConstructorWithEnumerable));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithEnumerable();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);
            Assert.AreEqual(1, importer.Import.Count());
            Assert.AreEqual(typeof(MockExportingInterfaceImplementation), importer.Import.First().GetType());
        }

        [Test]
        public void ComposeWithImportingConstructorWithEnumerableFromMany()
        {
            var repository = CreateRepository(
                typeof(MockExportingInterfaceImplementation),
                typeof(ExportOnTypeWithType),
                typeof(ImportOnConstructorWithMany));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithMany();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);
            Assert.AreEqual(2, importer.Import.Count());

            var list = importer.Import.ToList();
            var unrolledList = list.Select(m => m.GetType()).ToList();
            Assert.Contains(typeof(MockExportingInterfaceImplementation), unrolledList);
            Assert.Contains(typeof(ExportOnTypeWithType), unrolledList);
        }

        [Test]
        public void ComposeWithImportingConstructorWithEnumerableFuncFromMany()
        {
            var repository = CreateRepository(
                typeof(ExportOnAnotherMethod),
                typeof(ExportOnMethod),
                typeof(ImportOnConstructorWithCollectionOfFunc));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithEnumerableFunc();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);
            Assert.AreEqual(2, importer.Import.Count());

            var list = importer.Import.ToList();
            Assert.AreEqual(typeof(Func<IExportingInterface>), list[0].GetType());
            Assert.AreEqual(typeof(Func<IExportingInterface>), list[1].GetType());

            var unrolledList = list.Select(m => m().GetType()).ToList();
            Assert.Contains(typeof(MockExportingInterfaceImplementation), unrolledList);
            Assert.Contains(typeof(MockChildExportingInterfaceImplementation), unrolledList);
        }

        [Test]
        public void ComposeWithImportingConstructorWithEnumerableLazyFromMany()
        {
            var repository = CreateRepository(
                typeof(ExportOnPropertyWithType),
                typeof(ExportOnProperty),
                typeof(ImportOnConstructorWithCollectionOfLazy));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithEnumerableLazy();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);
            Assert.AreEqual(2, importer.Import.Count());

            var list = importer.Import.ToList();
            Assert.AreEqual(typeof(Lazy<IExportingInterface>), list[0].GetType());
            Assert.IsFalse(list[0].IsValueCreated);

            Assert.AreEqual(typeof(Lazy<IExportingInterface>), list[1].GetType());
            Assert.IsFalse(list[1].IsValueCreated);

            var unrolledList = list.Select(l => l.Value.GetType()).ToList();
            Assert.Contains(typeof(MockExportingInterfaceImplementation), unrolledList);
            Assert.Contains(typeof(MockChildExportingInterfaceImplementation), unrolledList);
        }

        [Test]
        public void ComposeWithImportingConstructorWithExportFactory()
        {
            var repository = CreateRepository(
                typeof(ExportOnPropertyWithType),
                typeof(ImportOnConstructorWithExportFactory));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithExportFactory();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);

            using (var instance = importer.Import.CreateExport())
            {
                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.Value);
                Assert.AreEqual(typeof(MockExportingInterfaceImplementation), instance.Value.GetType());
            }
        }

        [Test]
        public void ComposeWithImportingConstructorWithFunc()
        {
            var repository = CreateRepository(
                typeof(ExportOnMethod),
                typeof(ImportOnConstructorWithFunc));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithFunc();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);
            Assert.AreEqual(typeof(MockChildExportingInterfaceImplementation), importer.Import().GetType());
        }

        [Test]
        public void ComposeWithImportingConstructorWithLazy()
        {
            var repository = CreateRepository(
                typeof(ExportOnProperty),
                typeof(ImportOnConstructorWithLazy));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithLazy();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);
            Assert.IsFalse(importer.Import.IsValueCreated);
            Assert.AreEqual(typeof(MockChildExportingInterfaceImplementation), importer.Import.Value.GetType());
        }

        [Test]
        public void ComposeWithImportingConstructorWithMultipleImports()
        {
            var repository = CreateRepository(
                typeof(ExportOnTypeWithType),
                typeof(ImportOnPropertyWithType),
                typeof(ImportOnConstructorWithMultipleImports));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithMultipleImports();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import1);
            Assert.AreEqual(typeof(ExportOnTypeWithType), importer.Import1.GetType());

            Assert.IsNotNull(importer.Import2);
            Assert.AreEqual(typeof(ImportOnPropertyWithType), importer.Import2.GetType());
        }

        [Test]
        public void ComposeWithImportingConstructorWithType()
        {
            var repository = CreateRepository(
                typeof(ExportOnTypeWithType),
                typeof(ImportOnConstructorWithType));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportGetterForImportOnConstructorWithType();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.Import);
            var importer = obj.Import;

            Assert.IsNotNull(importer.Import);
            Assert.AreEqual(typeof(ExportOnTypeWithType), importer.Import.GetType());
        }

        [Test]
        public void ComposeWithImportingPropertyWithEnumerable()
        {
            var repository = CreateRepository(typeof(ExportOnPropertyWithEnumerable));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithEnumerable();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.AreEqual(1, obj.ImportingProperty.Count());
            Assert.AreEqual(typeof(MockExportingInterfaceImplementation), obj.ImportingProperty.First().GetType());
        }

        [Test]
        public void ComposeWithImportingPropertyWithEnumerableFromMany()
        {
            var repository = CreateRepository(typeof(MockExportingInterfaceImplementation), typeof(ExportOnTypeWithType));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithEnumerableFromMany();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.AreEqual(2, obj.ImportingProperty.Count());

            var list = obj.ImportingProperty.ToList();
            var unrolledList = list.Select(m => m.GetType()).ToList();
            Assert.Contains(typeof(MockExportingInterfaceImplementation), unrolledList);
            Assert.Contains(typeof(ExportOnTypeWithType), unrolledList);
        }

        [Test]
        public void ComposeWithImportingPropertyWithEnumerableFuncFromMany()
        {
            var repository = CreateRepository(typeof(ExportOnAnotherMethod), typeof(ExportOnMethod));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithCollectionOfFunc();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.AreEqual(2, obj.ImportingProperty.Count());

            var list = obj.ImportingProperty.ToList();
            Assert.AreEqual(typeof(Func<IExportingInterface>), list[0].GetType());
            Assert.AreEqual(typeof(Func<IExportingInterface>), list[1].GetType());

            var unrolledList = list.Select(m => m().GetType()).ToList();
            Assert.Contains(typeof(MockExportingInterfaceImplementation), unrolledList);
            Assert.Contains(typeof(MockChildExportingInterfaceImplementation), unrolledList);
        }

        [Test]
        public void ComposeWithImportingPropertyWithEnumerableLazyFromMany()
        {
            var repository = CreateRepository(typeof(ExportOnPropertyWithType), typeof(ExportOnProperty));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithCollectionOfLazy();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.AreEqual(2, obj.ImportingProperty.Count());

            var list = obj.ImportingProperty.ToList();
            Assert.AreEqual(typeof(Lazy<IExportingInterface>), list[0].GetType());
            Assert.IsFalse(list[0].IsValueCreated);

            Assert.AreEqual(typeof(Lazy<IExportingInterface>), list[1].GetType());
            Assert.IsFalse(list[1].IsValueCreated);

            var unrolledList = list.Select(l => l.Value.GetType()).ToList();
            Assert.Contains(typeof(MockExportingInterfaceImplementation), unrolledList);
            Assert.Contains(typeof(MockChildExportingInterfaceImplementation), unrolledList);
        }

        [Test]
        public void ComposeWithImportingPropertyWithExportFactory()
        {
            var repository = CreateRepository(typeof(ExportOnPropertyWithType));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithExportFactory();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            using (var instance = obj.ImportingProperty.CreateExport())
            {
                Assert.IsNotNull(instance);
                Assert.IsNotNull(instance.Value);
                Assert.AreEqual(typeof(MockExportingInterfaceImplementation), instance.Value.GetType());
            }
        }

        [Test]
        public void ComposeWithImportingPropertyWithFunc()
        {
            var repository = CreateRepository(typeof(ExportOnMethod));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithFunc();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.AreEqual(typeof(MockChildExportingInterfaceImplementation), obj.ImportingProperty().GetType());
        }

        [Test]
        public void ComposeWithImportingPropertyWithLazy()
        {
            var repository = CreateRepository(typeof(ExportOnProperty));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithLazy();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.IsFalse(obj.ImportingProperty.IsValueCreated);
            Assert.AreEqual(typeof(MockChildExportingInterfaceImplementation), obj.ImportingProperty.Value.GetType());
        }

        [Test]
        public void ComposeWithImportingPropertyWithType()
        {
            var repository = CreateRepository(typeof(ExportOnTypeWithType));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnProperty();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.AreEqual(typeof(ExportOnTypeWithType), obj.ImportingProperty.GetType());
        }

        [Test]
        public void ComposeWithNestedImport()
        {
            var repository = CreateRepository(typeof(ExportOnTypeWithType), typeof(ImportOnPropertyWithType));
            var catalog = new LazyLoadCatalog(repository, new ILoadTypesFromPlugins[] { new PluginAssemblyTypeLoader() });
            var container = new CompositionContainer(catalog);

            var obj = new ImportOnPropertyWithImport();
            container.ComposeParts(obj);

            Assert.IsNotNull(obj.ImportingProperty);
            Assert.AreEqual(typeof(ImportOnPropertyWithType), obj.ImportingProperty.GetType());

            var importer = obj.ImportingProperty;
            Assert.IsNotNull(importer.ImportingProperty);
            Assert.AreEqual(typeof(ExportOnTypeWithType), importer.ImportingProperty.GetType());
        }

        private IPluginRepository CreateRepository(params Type[] exportingTypesToAdd)
        {
            var typeNames = exportingTypesToAdd.Select(t => t.AssemblyQualifiedName).ToList();

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

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithEnumerableFunc
        {
            [Import]
            public ImportOnConstructorWithCollectionOfFunc Import
            {
                get;
                set;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithEnumerableLazy
        {
            [Import]
            public ImportOnConstructorWithCollectionOfLazy Import
            {
                get;
                set;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithExportFactory
        {
            [Import]
            public ImportOnConstructorWithExportFactory Import
            {
                get;
                set;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithFunc
        {
            [Import]
            public ImportOnConstructorWithFunc Import
            {
                get;
                set;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithLazy
        {
            [Import]
            public ImportOnConstructorWithLazy Import
            {
                get;
                set;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithMany
        {
            [Import]
            public ImportOnConstructorWithMany Import
            {
                get;
                set;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithMultipleImports
        {
            [Import]
            public ImportOnConstructorWithMultipleImports Import
            {
                get;
                set;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Used to get the import type that use a constructor import.")]
        public sealed class ImportGetterForImportOnConstructorWithType
        {
            [Import]
            public ImportOnConstructorWithType Import
            {
                get;
                set;
            }
        }
    }
}
