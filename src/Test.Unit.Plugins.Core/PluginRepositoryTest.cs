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
using System.Reflection;
using Nuclei.Plugins.Discovery;
using Nuclei.Plugins.Discovery.Origin.FileSystem;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Core
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PluginRepositoryTest
    {
        [Test]
        public void AddDiscoverableMemberWithMethodMember()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            var memberInfo = typeof(DiscoverableMemberOnMethod).GetMethod("DiscoverableMethod", new Type[0]);
            var metadata = memberInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();
            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            var definition = MethodBasedDiscoverableMember.CreateDefinition(
                memberInfo,
                metadata,
                identityGenerator);

            var fileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddDiscoverableMember(definition, fileInfo);

            var discoverableMembers = repository.DiscoverableMembers();
            Assert.AreEqual(1, discoverableMembers.Count());
            Assert.AreSame(definition, discoverableMembers.First());
            Assert.AreSame(definition, repository.DiscoverableMember(MethodDefinition.CreateDefinition(memberInfo)));

            var files = repository.KnownPluginOrigins();
            Assert.AreEqual(1, files.Count());
            Assert.AreSame(fileInfo, files.First());
        }

        [Test]
        public void AddDiscoverableMemberWithPropertyMember()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            var memberInfo = typeof(DiscoverableMemberOnProperty).GetProperty("DiscoverableProperty");
            var metadata = memberInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();
            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            var definition = PropertyBasedDiscoverableMember.CreateDefinition(
                memberInfo,
                metadata,
                identityGenerator);

            var fileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddDiscoverableMember(definition, fileInfo);

            var discoverableMembers = repository.DiscoverableMembers();
            Assert.AreEqual(1, discoverableMembers.Count());
            Assert.AreSame(definition, discoverableMembers.First());
            Assert.AreSame(definition, repository.DiscoverableMember(PropertyDefinition.CreateDefinition(memberInfo)));

            var files = repository.KnownPluginOrigins();
            Assert.AreEqual(1, files.Count());
            Assert.AreSame(fileInfo, files.First());
        }

        [Test]
        public void AddDiscoverableMemberWithTypeMember()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            var memberInfo = typeof(DiscoverableMemberOnType);
            var metadata = memberInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();
            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            var definition = TypeBasedDiscoverableMember.CreateDefinition(
                memberInfo,
                metadata,
                identityGenerator);

            var fileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddDiscoverableMember(definition, fileInfo);

            var discoverableMembers = repository.DiscoverableMembers();
            Assert.AreEqual(1, discoverableMembers.Count());
            Assert.AreSame(definition, discoverableMembers.First());
            Assert.AreSame(definition, repository.DiscoverableMember(TypeIdentity.CreateDefinition(memberInfo)));

            var files = repository.KnownPluginOrigins();
            Assert.AreEqual(1, files.Count());
            Assert.AreSame(fileInfo, files.First());
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

            var fileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddPart(definition, fileInfo);

            var parts = repository.Parts();
            Assert.AreEqual(1, parts.Count());
            Assert.AreSame(definition, parts.First());
            Assert.AreSame(definition, repository.Part(TypeIdentity.CreateDefinition(typeof(ExportOnProperty))));

            var files = repository.KnownPluginOrigins();
            Assert.AreEqual(1, files.Count());
            Assert.AreSame(fileInfo, files.First());
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
                repository.IsSubtypeOf(
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
                repository.IsSubtypeOf(
                    TypeIdentity.CreateDefinition(typeof(IEnumerable<>)),
                    TypeIdentity.CreateDefinition(typeof(List<>))));
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

            Assert.IsTrue(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(IComparable)), TypeIdentity.CreateDefinition(typeof(string))));
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

            Assert.IsTrue(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(IComparable)), TypeIdentity.CreateDefinition(typeof(string))));
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

            Assert.IsTrue(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(object)), TypeIdentity.CreateDefinition(typeof(string))));
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

            Assert.IsTrue(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(object)), TypeIdentity.CreateDefinition(typeof(string))));
        }

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
        public void AddTypeWithTypeDerivingFromGenericType()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(List<int>), identityGenerator);
            repository.AddType(definition);

            Assert.IsTrue(repository.ContainsDefinitionForType(typeof(List<int>).AssemblyQualifiedName));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(List<int>))));

            Assert.AreSame(definition, repository.TypeByName(typeof(List<int>).AssemblyQualifiedName));
            Assert.AreSame(definition, repository.TypeByIdentity(TypeIdentity.CreateDefinition(typeof(List<int>))));
        }

        [Test]
        public void DiscoverableMemberWithMetadataWithExistingMember()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            var fileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);

            var methodInfo = typeof(DiscoverableMemberOnMethod).GetMethod("DiscoverableMethod", new Type[0]);
            var metadata = methodInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();
            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            var methodMember = MethodBasedDiscoverableMember.CreateDefinition(
                methodInfo,
                metadata,
                identityGenerator);

            repository.AddDiscoverableMember(methodMember, fileInfo);

            var propertyInfo = typeof(DiscoverableMemberOnProperty).GetProperty("DiscoverableProperty");
            metadata = propertyInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();
            var propertyMember = PropertyBasedDiscoverableMember.CreateDefinition(
                propertyInfo,
                metadata,
                identityGenerator);

            repository.AddDiscoverableMember(propertyMember, fileInfo);

            var discoverableMembers = repository.DiscoverableMembersWithMetadata("Name", "Method");
            Assert.AreEqual(1, discoverableMembers.Count());
            Assert.AreSame(methodMember, discoverableMembers.First());
            Assert.AreSame(methodMember, repository.DiscoverableMember(MethodDefinition.CreateDefinition(methodInfo)));
        }

        [Test]
        public void DiscoverableMemberWithMetadataWithNonExistingMember()
        {
            var currentlyBuilding = new Dictionary<Type, TypeIdentity>();
            var repository = new PluginRepository();

            var fileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);

            var methodInfo = typeof(DiscoverableMemberOnMethod).GetMethod("DiscoverableMethod", new Type[0]);
            var metadata = methodInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();
            Func<Type, TypeIdentity> identityGenerator = TypeIdentityBuilder.IdentityFactory(repository, currentlyBuilding);
            var methodMember = MethodBasedDiscoverableMember.CreateDefinition(
                methodInfo,
                metadata,
                identityGenerator);

            repository.AddDiscoverableMember(methodMember, fileInfo);

            var propertyInfo = typeof(DiscoverableMemberOnProperty).GetProperty("DiscoverableProperty");
            metadata = propertyInfo.GetCustomAttribute<MockDiscoverableMemberAttribute>().Metadata();
            var propertyMember = PropertyBasedDiscoverableMember.CreateDefinition(
                propertyInfo,
                metadata,
                identityGenerator);

            repository.AddDiscoverableMember(propertyMember, fileInfo);

            var discoverableMembers = repository.DiscoverableMembersWithMetadata("Name", "Type");
            Assert.AreEqual(0, discoverableMembers.Count());
        }

        [Test]
        public void IsSubtypeOfWithNonExistingChild()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(object), identityGenerator);
            repository.AddType(definition);

            Assert.IsFalse(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(object)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void IsSubtypeOfWithNonExistingParent()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(definition);

            Assert.IsFalse(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(double)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void IsSubtypeOfWithNullChild()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(definition);

            Assert.IsFalse(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(double)), null));
        }

        [Test]
        public void IsSubtypeOfWithNullParent()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(definition);

            Assert.IsFalse(repository.IsSubtypeOf(null, TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void IsSubtypeOfWithRelatedTypes()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(object), identityGenerator);
            repository.AddType(definition);

            definition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(definition);

            Assert.IsTrue(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(object)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void IsSubtypeOfWithSingleType()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(definition);

            Assert.IsFalse(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(string)), TypeIdentity.CreateDefinition(typeof(string))));
        }

        [Test]
        public void IsSubtypeOfWithUnrelatedTypes()
        {
            var repository = new PluginRepository();

            Func<Type, TypeIdentity> identityGenerator = t => TypeIdentity.CreateDefinition(t);
            var definition = TypeDefinition.CreateDefinition(typeof(string), identityGenerator);
            repository.AddType(definition);

            definition = TypeDefinition.CreateDefinition(typeof(double), identityGenerator);
            repository.AddType(definition);

            Assert.IsFalse(repository.IsSubtypeOf(TypeIdentity.CreateDefinition(typeof(double)), TypeIdentity.CreateDefinition(typeof(string))));
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

            var partFileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddPart(partDefinition, partFileInfo);

            Assert.That(
                repository.KnownPluginOrigins(),
                Is.EquivalentTo(
                    new List<PluginFileOrigin>
                    {
                        partFileInfo
                    }));

            repository.RemovePlugins(
                new[]
                    {
                        partFileInfo
                    });

            Assert.That(
                repository.KnownPluginOrigins(),
                Is.EquivalentTo(
                    new List<PluginFileOrigin>
                    {
                    }));
            Assert.AreEqual(0, repository.Parts().Count());
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

            var parentFileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddPart(parentDefinition, parentFileInfo);

            PartDefinition childDefinition = new PartDefinition
            {
                Identity = identityGenerator(typeof(MockChildExportingInterfaceImplementation)),
            };

            var childFileInfo = new PluginFileOrigin("b", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddPart(childDefinition, childFileInfo);
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation))));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
            Assert.IsTrue(
                repository.IsSubtypeOf(
                    TypeIdentity.CreateDefinition(typeof(object)),
                    TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));

            repository.RemovePlugins(new[] { childFileInfo });
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

            var parentFileInfo = new PluginFileOrigin("a", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddPart(parentDefinition, parentFileInfo);

            PartDefinition childDefinition = new PartDefinition
            {
                Identity = identityGenerator(typeof(MockChildExportingInterfaceImplementation)),
            };

            var childFileInfo = new PluginFileOrigin("b", DateTimeOffset.Now, DateTimeOffset.Now);
            repository.AddPart(childDefinition, childFileInfo);
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation))));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
            Assert.IsTrue(
                repository.IsSubtypeOf(
                    TypeIdentity.CreateDefinition(typeof(object)),
                    TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));

            repository.RemovePlugins(new[] { parentFileInfo });
            Assert.IsFalse(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockExportingInterfaceImplementation))));
            Assert.IsTrue(repository.ContainsDefinitionForType(TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
            Assert.IsFalse(
                repository.IsSubtypeOf(
                    TypeIdentity.CreateDefinition(typeof(object)),
                    TypeIdentity.CreateDefinition(typeof(MockChildExportingInterfaceImplementation))));
        }
    }
}
