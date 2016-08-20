//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Moq;
using NUnit.Framework;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class PartImportEngineTest
    {
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "input",
            Justification = "This parameter is used by reflection")]
        public static void MockInput(string input)
        {
            // Do nothing
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "first",
            Justification = "This parameter is used by reflection")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "second",
            Justification = "This parameter is used by reflection")]
        public static void MockInput(string first, string second)
        {
            // Do nothing
        }

        private static Func<Type, TypeIdentity> IdentityFactory(List<TypeDefinition> typeStorage, IDictionary<Type, TypeIdentity> currentlyBuilding)
        {
            // Fake out the compiler because we need the function inside the function itself
            Func<Type, TypeIdentity> createTypeIdentity = null;
            createTypeIdentity =
                t =>
                {
                    // First make sure we're not already creating a definition for this type. If so then we just
                    // return the identity because at some point we'll get the definition being added.
                    // This is necessary because if we don't check this there is a good possibility that
                    // we end-up in an infinite loop. e.g. trying to handle
                    // System.Boolean means we have to process System.IComparable<System.Boolean> which means ....
                    if (currentlyBuilding.ContainsKey(t))
                    {
                        return currentlyBuilding[t];
                    }

                    if (typeStorage.Find(typeDef => typeDef.Identity.Equals(t)) == null)
                    {
                        try
                        {
                            // Create a local version of the TypeIdentity and store that
                            var typeIdentity = TypeIdentity.CreateDefinition(t);
                            currentlyBuilding.Add(t, typeIdentity);

                            var typeDefinition = TypeDefinition.CreateDefinition(t, createTypeIdentity);
                            typeStorage.Add(typeDefinition);
                        }
                        finally
                        {
                            // Once we add the real definition then we can just remove the local copy
                            // from the stack.
                            currentlyBuilding.Remove(t);
                        }
                    }

                    return typeStorage.Find(typeDef => typeDef.Identity.Equals(t)).Identity;
                };

            return createTypeIdentity;
        }

        private static bool IsSubTypeOf(List<TypeDefinition> types, TypeIdentity parent, TypeIdentity child)
        {
            var childDef = types.Find(t => t.Identity.Equals(child));

            var directParent = childDef.BaseType;
            while (directParent != null)
            {
                if (parent.Equals(directParent))
                {
                    return true;
                }

                var directParentDef = types.Find(t => t.Identity.Equals(directParent));
                directParent = directParentDef.BaseType;
            }

            foreach (var baseInterface in childDef.BaseInterfaces)
            {
                if (parent.Equals(baseInterface))
                {
                    return true;
                }
            }

            return false;
        }

        [Test]
        public void AcceptsWithNonmatchingContractName()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(int)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = PropertyBasedExportDefinition.CreateDefinition(
                "B",
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            Assert.IsFalse(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithExportTypeMatchesImportType()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(int)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = PropertyBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetProperty("Length"),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithImportIsEnumerable()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(IEnumerable<int>)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = PropertyBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetProperty("Length"),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithImportIsEnumerableLazy()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(IEnumerable<Lazy<int>>)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = PropertyBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetProperty("Length"),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithImportIsLazy()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(Lazy<int>)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = PropertyBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetProperty("Length"),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithImportIsSingleArgumentFunc()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(Func<int>)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = PropertyBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetProperty("Length"),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithImportIsMultipleArgumentFunc()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(Func<string, string, bool>)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = MethodBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) }),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithImportIsSingleArgumentAction()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(Action<string>)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = MethodBasedExportDefinition.CreateDefinition(
                "A",
                this.GetType().GetMethod("MockInput", new[] { typeof(string) }),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithImportIsMultipleArgumentAction()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(Action<string, string>)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = MethodBasedExportDefinition.CreateDefinition(
                "A",
                this.GetType().GetMethod("MockInput", new[] { typeof(string), typeof(string) }),
                createTypeIdentity);

            Assert.IsTrue(importEngine.Accepts(importDefinition, exportDefinition));
        }

        [Test]
        public void AcceptsWithNonmatchingImportAndExport()
        {
            var types = new List<TypeDefinition>();
            var createTypeIdentity = IdentityFactory(types, new Dictionary<Type, TypeIdentity>());

            var repository = new Mock<ISatisfyPluginRequests>();
            {
                repository.Setup(r => r.IdentityByName(It.IsAny<string>()))
                    .Returns<string>(n => types.Where(t => t.Identity.AssemblyQualifiedName.Equals(n)).Select(t => t.Identity).FirstOrDefault());
                repository.Setup(r => r.TypeByIdentity(It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity>(id => types.Where(t => t.Identity.Equals(id)).FirstOrDefault());
                repository.Setup(r => r.IsSubTypeOf(It.IsAny<TypeIdentity>(), It.IsAny<TypeIdentity>()))
                    .Returns<TypeIdentity, TypeIdentity>((parent, child) => IsSubTypeOf(types, parent, child));
            }

            var importEngine = new PartImportEngine(repository.Object);

            var importDefinition = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                createTypeIdentity(typeof(string)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"),
                createTypeIdentity);
            var exportDefinition = MethodBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) }),
                createTypeIdentity);

            Assert.IsFalse(importEngine.Accepts(importDefinition, exportDefinition));
        }
    }
}
