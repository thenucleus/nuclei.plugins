//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Nuclei.Plugins;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class PluginRepositoryTest
    {
        [Test]
        public void AddTypeWithStandaloneClassType()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(object), identityGenerator);
            repository.AddType(definition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(object).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(object))));

            Assert.AreSame(definition, repository.TypeByName(typeof(object).AssemblyQualifiedName));
            Assert.AreSame(definition, repository.TypeByIdentity(TypeIdentity.CreateDefinition(typeof(object))));
        }

        [Test]
        public void AddTypeWithStandaloneGenericClassType()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(Lazy<>), identityGenerator);
            repository.AddType(definition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(Lazy<>).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(Lazy<>))));

            Assert.AreSame(definition, repository.TypeByName(typeof(Lazy<>).AssemblyQualifiedName));
            Assert.AreSame(definition, repository.TypeByIdentity(TypeIdentity.CreateDefinition(typeof(Lazy<>))));
        }

        [Test]
        public void AddTypeWithStandaloneInterfaceType()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(IEnumerable), identityGenerator);
            repository.AddType(definition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(IEnumerable).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(IEnumerable))));

            Assert.AreSame(definition, repository.TypeByName(typeof(IEnumerable).AssemblyQualifiedName));
            Assert.AreSame(definition, repository.TypeByIdentity(TypeIdentity.CreateDefinition(typeof(IEnumerable))));
        }

        [Test]
        public void AddTypeWithStandaloneGenericInterfaceType()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(IComparer<>), identityGenerator);
            repository.AddType(definition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(IComparer<>).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(IComparer<>))));

            Assert.AreSame(definition, repository.TypeByName(typeof(IComparer<>).AssemblyQualifiedName));
            Assert.AreSame(definition, repository.TypeByIdentity(TypeIdentity.CreateDefinition(typeof(IComparer<>))));
        }

        [Test]
        public void AddTypeWithParentTypeFirst()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var objectDefinition = TypeDefinition.CreateDefinition(typeof(object), identityGenerator);
            repository.AddType(objectDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(object).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(object))));

            var stringDefinition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(stringDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(string).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(string))));

            Assert.IsTrue(repository.IsSubTypeOf(TypeIdentity.CreateDefinition(typeof(object)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void AddTypeWithParentTypeLast()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var stringDefinition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(stringDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(string).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(string))));

            var objectDefinition = TypeDefinition.CreateDefinition(typeof(object), identityGenerator);
            repository.AddType(objectDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(object).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(object))));

            Assert.IsTrue(repository.IsSubTypeOf(TypeIdentity.CreateDefinition(typeof(object)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void AddTypeWithParentInterfaceFirst()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var comparableDefinition = TypeDefinition.CreateDefinition(typeof(IComparable), identityGenerator);
            repository.AddType(comparableDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(IComparable).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(IComparable))));

            var stringDefinition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(stringDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(string).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(string))));

            Assert.IsTrue(repository.IsSubTypeOf(TypeIdentity.CreateDefinition(typeof(IComparable)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void AddTypeWithParentInterfaceLast()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);

            var stringDefinition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(stringDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(string).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(string))));

            var comparableDefinition = TypeDefinition.CreateDefinition(typeof(IComparable), identityGenerator);
            repository.AddType(comparableDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(IComparable).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(IComparable))));

            Assert.IsTrue(repository.IsSubTypeOf(TypeIdentity.CreateDefinition(typeof(IComparable)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void AddTypeWithGenericParentFirst()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var enumerableDefinition = TypeDefinition.CreateDefinition(typeof(IEnumerable<>), identityGenerator);
            repository.AddType(enumerableDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(IEnumerable<>).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(IEnumerable<>))));

            var listDefinition = TypeDefinition.CreateDefinition(typeof(List<>), identityGenerator);
            repository.AddType(listDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(List<>).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(List<>))));

            Assert.IsTrue(
                repository.IsSubTypeOf(
                    TypeIdentity.CreateDefinition(typeof(IEnumerable<>)), 
                    TypeIdentity.CreateDefinition(typeof(List<>))));
        }

        [Test]
        public void AddTypeWithGenericParentLast()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var listDefinition = TypeDefinition.CreateDefinition(typeof(List<>), identityGenerator);
            repository.AddType(listDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(List<>).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(List<>))));
            
            var enumerableDefinition = TypeDefinition.CreateDefinition(typeof(IEnumerable<>), identityGenerator);
            repository.AddType(enumerableDefinition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(IEnumerable<>).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(IEnumerable<>))));

            Assert.IsTrue(
                repository.IsSubTypeOf(
                    TypeIdentity.CreateDefinition(typeof(IEnumerable<>)), 
                    TypeIdentity.CreateDefinition(typeof(List<>))));
        }

        [Test]
        public void AddPart()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            PartDefinition definition = new PartDefinition
                {
                    Identity = identityGenerator(typeof(ExportOnProperty)),
                };
            
            var fileInfo = new PluginFileInfo("a", DateTimeOffset.Now);
            repository.AddPart(definition, fileInfo);
            
            var parts = repository.Parts();
            Assert.AreEqual(1, parts.Count());
            Assert.AreSame(definition, parts.First());
            Assert.AreSame(definition, repository.Part(TypeIdentity.CreateDefinition(typeof(ExportOnProperty))));

            var files = repository.KnownPluginFiles();
            Assert.AreEqual(1, files.Count());
            Assert.AreSame(fileInfo, files.First()); 
        }

        [Test]
        public void AddGroup()
        {
            var repository = new PluginRepository();
            var definition = new GroupDefinition("a");

            var fileInfo = new PluginFileInfo("a", DateTimeOffset.Now);
            repository.AddGroup(definition, fileInfo);

            var groups = repository.Groups();
            Assert.AreEqual(1, groups.Count());
            Assert.AreSame(definition, groups.First());
            Assert.AreSame(definition, repository.Group(new GroupRegistrationId("a")));

            var files = repository.KnownPluginFiles();
            Assert.AreEqual(1, files.Count());
            Assert.AreSame(fileInfo, files.First()); 
        }

        [Test]
        public void RemovePlugins()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            PartDefinition partDefinition = new PartDefinition
                {
                    Identity = identityGenerator(typeof(ExportOnProperty)),
                };

            var partFileInfo = new PluginFileInfo("a", DateTimeOffset.Now);
            repository.AddPart(partDefinition, partFileInfo);
            
            var groupDefinition = new GroupDefinition("b");
            var groupFileInfo = new PluginFileInfo("c", DateTimeOffset.Now);
            repository.AddGroup(groupDefinition, groupFileInfo);

            Assert.That(
                repository.KnownPluginFiles(),
                Is.EquivalentTo(
                    new List<PluginFileInfo> 
                    { 
                        partFileInfo,
                        groupFileInfo,
                    }));

            repository.RemovePlugins(
                new List<string>
                    {
                        partFileInfo.Path
                    });

            Assert.That(
                repository.KnownPluginFiles(),
                Is.EquivalentTo(
                    new List<PluginFileInfo> 
                    { 
                        groupFileInfo,
                    }));
            Assert.AreEqual(0, repository.Parts().Count());
            Assert.AreEqual(1, repository.Groups().Count());
            Assert.IsFalse(repository.ContainsDefinitionForType(typeof(ExportOnProperty).AssemblyQualifiedName));
        }

        [Test]
        public void RemovePluginsWithChildType()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            PartDefinition parentDefinition = new PartDefinition
                {
                    Identity = identityGenerator(typeof(MockExportingInterfaceImplementation)),
                };

            var parentFileInfo = new PluginFileInfo("a", DateTimeOffset.Now);
            repository.AddPart(parentDefinition, parentFileInfo);

            PartDefinition childDefinition = new PartDefinition
                {
                    Identity = identityGenerator(typeof(MockChildExportingInterfaceImplementation)),
                };

            var childFileInfo = new PluginFileInfo("b", DateTimeOffset.Now);
            repository.AddPart(childDefinition, childFileInfo);
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation))));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
            Assert.IsTrue(
                repository.IsSubTypeOf(
                    TypeIdentity.CreateDefinition(typeof(object)),
                    TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));

            repository.RemovePlugins(new string[] { childFileInfo.Path });
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation))));
            Assert.IsFalse(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
        }

        [Test]
        public void RemovePluginsWithParentType()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            PartDefinition parentDefinition = new PartDefinition
            {
                Identity = identityGenerator(typeof(MockExportingInterfaceImplementation)),
            };

            var parentFileInfo = new PluginFileInfo("a", DateTimeOffset.Now);
            repository.AddPart(parentDefinition, parentFileInfo);

            PartDefinition childDefinition = new PartDefinition
            {
                Identity = identityGenerator(typeof(MockChildExportingInterfaceImplementation)),
            };

            var childFileInfo = new PluginFileInfo("b", DateTimeOffset.Now);
            repository.AddPart(childDefinition, childFileInfo);
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation))));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
            Assert.IsTrue(
                repository.IsSubTypeOf(
                    TypeIdentity.CreateDefinition(typeof(object)),
                    TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));

            repository.RemovePlugins(new string[] { parentFileInfo.Path });
            Assert.IsFalse(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation))));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
            Assert.IsFalse(
                repository.IsSubTypeOf(
                    TypeIdentity.CreateDefinition(typeof(object)),
                    TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
        }
    }
}
