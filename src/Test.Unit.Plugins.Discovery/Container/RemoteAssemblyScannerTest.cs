﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using Nuclei.Diagnostics.Logging;
using Nuclei.Plugins.Core;
using Nuclei.Plugins.Core.Assembly;
using Nuclei.Plugins.Discovery.Assembly;
using Nuclei.Plugins.Discovery.Origin.FileSystem;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Discovery.Container
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RemoteAssemblyScannerTest
    {
        private static IEnumerable<SerializableDiscoverableMemberDefinition> _discoverableMembers;
        private static IEnumerable<PartDefinition> _parts;
        private static IEnumerable<TypeDefinition> _types;

        [Test]
        public void AddDiscoverableMemberWithMethodMember()
        {
            var memberInfo = typeof(DiscoverableMemberOnMethod).GetMethod("DiscoverableMethod", new Type[0]);
            var metadata = memberInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();

            var id = TypeIdentity.CreateDefinition(typeof(DiscoverableMemberOnMethod));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _discoverableMembers.Where(p => p.DeclaringType.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First() as MethodBasedDiscoverableMember;
            Assert.IsNotNull(plugin);

            Assert.AreEqual(id, plugin.DeclaringType);
            Assert.AreEqual(MethodDefinition.CreateDefinition(memberInfo), plugin.Method);

            Assert.AreEqual(1, plugin.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), plugin.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), plugin.Metadata.Values.First());
        }

        [Test]
        public void AddDiscoverableMemberWithPropertyMember()
        {
            var memberInfo = typeof(DiscoverableMemberOnProperty).GetProperty("DiscoverableProperty");
            var metadata = memberInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();

            var id = TypeIdentity.CreateDefinition(typeof(DiscoverableMemberOnProperty));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _discoverableMembers.Where(p => p.DeclaringType.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First() as PropertyBasedDiscoverableMember;
            Assert.IsNotNull(plugin);

            Assert.AreEqual(id, plugin.DeclaringType);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(memberInfo), plugin.Property);

            Assert.AreEqual(1, plugin.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), plugin.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), plugin.Metadata.Values.First());
        }

        [Test]
        public void AddDiscoverableMemberWithTypeMember()
        {
            var memberInfo = typeof(DiscoverableMemberOnType);
            var metadata = memberInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();

            var id = TypeIdentity.CreateDefinition(memberInfo);
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _discoverableMembers.Where(p => p.DeclaringType.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First() as TypeBasedDiscoverableMember;
            Assert.IsNotNull(plugin);

            Assert.AreEqual(id, plugin.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(memberInfo), plugin.DeclaringType);

            Assert.AreEqual(1, plugin.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), plugin.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), plugin.Metadata.Values.First());
        }

        [Test]
        public void ExportOnMethod()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethod));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);

            // for some unknown reason MEF adds () to the exported type on a method. No clue why what so ever....!!!
            Assert.AreEqual(typeof(IExportingInterface).FullName + "()", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethod).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethodWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithName));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnMethodWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithName).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnMethodWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithType));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as MethodBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                MethodDefinition.CreateDefinition(
                    typeof(ExportOnMethodWithType).GetMethod("ExportingMethod")),
                export.Method);
        }

        [Test]
        public void ExportOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnProperty));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnProperty).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnPropertyWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithName));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnPropertyWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithName).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnPropertyWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnPropertyWithType));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as PropertyBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ExportOnPropertyWithType).GetProperty("ExportingProperty")),
                export.Property);
        }

        [Test]
        public void ExportOnType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnType));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(ExportOnType).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnTypeWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithName));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual("OnTypeWithName", export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ExportOnTypeWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ExportOnTypeWithType));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.IsFalse(plugin.Imports.Any());
            Assert.AreEqual(1, plugin.Exports.Count());

            var export = plugin.Exports.First() as TypeBasedExportDefinition;
            Assert.IsNotNull(export);
            Assert.AreEqual(typeof(IExportingInterface).FullName, export.ContractName);
            Assert.AreEqual(id, export.DeclaringType);
        }

        [Test]
        public void ImportOnConstructor()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructor));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(IExportingInterface) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructor).GetConstructor(new[] { typeof(IExportingInterface) }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithEnumerable()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithEnumerable));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("System.Collections.Generic.IEnumerable(Test.Mocks.IExportingInterface)", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithEnumerable).GetConstructor(new[] { typeof(IEnumerable<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithEnumerable).GetConstructor(
                        new[]
                        {
                            typeof(IEnumerable<IExportingInterface>)
                        }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithEnumerableOfFunc()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithCollectionOfFunc));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface()", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<Func<IExportingInterface>>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithCollectionOfFunc).GetConstructor(new[] { typeof(IEnumerable<Func<IExportingInterface>>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithCollectionOfFunc).GetConstructor(
                        new[]
                        {
                            typeof(IEnumerable<Func<IExportingInterface>>)
                        }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithEnumerableOfLazy()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithCollectionOfLazy));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<Lazy<IExportingInterface>>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithCollectionOfLazy).GetConstructor(new[] { typeof(IEnumerable<Lazy<IExportingInterface>>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithCollectionOfLazy).GetConstructor(
                        new[]
                        {
                            typeof(IEnumerable<Lazy<IExportingInterface>>)
                        }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithExportFactory()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithExportFactory));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(ExportFactory<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsTrue(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithExportFactory).GetConstructor(new[] { typeof(ExportFactory<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithExportFactory).GetConstructor(
                        new[]
                        {
                            typeof(ExportFactory<IExportingInterface>)
                        }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithFunc()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithFunc));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface()", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Func<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithFunc).GetConstructor(new[] { typeof(Func<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithFunc).GetConstructor(new[] { typeof(Func<IExportingInterface>) }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithImportMany()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithMany));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithMany).GetConstructor(new[] { typeof(IEnumerable<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithMany).GetConstructor(new[] { typeof(IEnumerable<IExportingInterface>) }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithLazy()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithLazy));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Lazy<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithLazy).GetConstructor(new[] { typeof(Lazy<IExportingInterface>) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithLazy).GetConstructor(new[] { typeof(Lazy<IExportingInterface>) }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithName));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnConstructor", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(IExportingInterface) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithName).GetConstructor(new[] { typeof(IExportingInterface) }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnConstructorWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithType));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as ConstructorBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsTrue(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                ConstructorDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(IExportingInterface) })),
                import.Constructor);
            Assert.AreEqual(
                ParameterDefinition.CreateDefinition(
                    typeof(ImportOnConstructorWithType).GetConstructor(new[] { typeof(IExportingInterface) }).GetParameters().First(),
                    t => TypeIdentity.CreateDefinition(t)),
                import.Parameter);
        }

        [Test]
        public void ImportOnProperty()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnProperty));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsTrue(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnProperty).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithEnumerable()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithEnumerable));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("System.Collections.Generic.IEnumerable(Test.Mocks.IExportingInterface)", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithEnumerable).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithEnumerableOfFunc()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithCollectionOfFunc));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface()", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<Func<IExportingInterface>>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithCollectionOfFunc).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithEnumerableOfLazy()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithCollectionOfLazy));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<Lazy<IExportingInterface>>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithCollectionOfLazy).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithExportFactory()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithExportFactory));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(ExportFactory<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsTrue(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithExportFactory).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithFunc()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithFunc));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface()", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Func<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithFunc).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithImportMany()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithEnumerableFromMany));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithEnumerableFromMany).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithLazy()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithLazy));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("Test.Mocks.IExportingInterface", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Lazy<IExportingInterface>)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithLazy).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithName()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithName));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual("ImportOnProperty", import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithName).GetProperty("ImportingProperty")),
                import.Property);
        }

        [Test]
        public void ImportOnPropertyWithType()
        {
            var id = TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithType));
            Assert.IsTrue(_types.Any(s => s.Identity.Equals(id)));

            var plugins = _parts.Where(p => p.Identity.Equals(id));
            Assert.IsTrue(plugins.Count() == 1);

            var plugin = plugins.First();
            Assert.AreEqual(1, plugin.Imports.Count());

            var import = plugin.Imports.First() as PropertyBasedImportDefinition;
            Assert.IsNotNull(import);
            Assert.AreEqual(typeof(IExportingInterface).FullName, import.ContractName);
            Assert.AreEqual(id, import.DeclaringType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IExportingInterface)), import.RequiredTypeIdentity);
            Assert.IsFalse(import.IsExportFactory);
            Assert.IsFalse(import.IsPrerequisite);
            Assert.IsFalse(import.IsRecomposable);
            Assert.AreEqual(
                PropertyDefinition.CreateDefinition(
                    typeof(ImportOnPropertyWithType).GetProperty("ImportingProperty")),
                import.Property);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            try
            {
                var discoverableMembers = new List<SerializableDiscoverableMemberDefinition>();
                var parts = new List<PartDefinition>();
                var types = new List<TypeDefinition>();
                var repository = new Mock<IPluginRepository>();
                {
                    repository.Setup(r => r.AddDiscoverableMember(It.IsAny<SerializableDiscoverableMemberDefinition>(), It.IsAny<PluginOrigin>()))
                        .Callback<SerializableDiscoverableMemberDefinition, PluginOrigin>((s, f) => discoverableMembers.Add(s));
                    repository.Setup(r => r.AddPart(It.IsAny<PartDefinition>(), It.IsAny<PluginOrigin>()))
                        .Callback<PartDefinition, PluginOrigin>((p, f) => parts.Add(p));
                    repository.Setup(r => r.AddType(It.IsAny<TypeDefinition>()))
                        .Callback<TypeDefinition>(types.Add);
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<string>()))
                        .Returns<string>(n => types.Any(t => t.Identity.AssemblyQualifiedName.Equals(n)));
                    repository.Setup(r => r.ContainsDefinitionForType(It.IsAny<TypeIdentity>()))
                        .Returns<TypeIdentity>(n => types.Any(t => t.Identity.Equals(n)));
                    repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                        .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).First());
                    repository.Setup(r => r.Parts())
                        .Returns(parts);
                }

                var scanner = new RemoteAssemblyScanner(
                    repository.Object,
                    new Mock<ILogMessagesFromRemoteAppDomains>().Object);

                var localPath = System.Reflection.Assembly.GetExecutingAssembly().LocalFilePath();
                scanner.Scan(new Dictionary<string, PluginOrigin> { { localPath,  new PluginAssemblyOrigin(localPath) } });

                _types = types;
                _parts = parts;
                _discoverableMembers = discoverableMembers;
            }
            catch (Exception e)
            {
                Trace.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception in RemoteAssemblyScannerTest.Setup: {0}",
                        e));

                throw;
            }
        }
    }
}
