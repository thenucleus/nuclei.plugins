//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class PropertyBasedImportDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class PropertyBasedImportDefinitionEqualityContractVerifier : EqualityContractVerifier<PropertyBasedImportDefinition>
        {
            private readonly PropertyBasedImportDefinition m_First = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(int)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"));

            private readonly PropertyBasedImportDefinition m_Second = PropertyBasedImportDefinition.CreateDefinition(
                "B",
                TypeIdentity.CreateDefinition(typeof(int)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(Version).GetProperty("Build"));

            protected override PropertyBasedImportDefinition Copy(PropertyBasedImportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return PropertyBasedImportDefinition.CreateDefinition(
                        "A",
                        TypeIdentity.CreateDefinition(typeof(int)),
                        ImportCardinality.ExactlyOne,
                        true,
                        CreationPolicy.NonShared,
                        typeof(string).GetProperty("Length"));
                }

                return PropertyBasedImportDefinition.CreateDefinition(
                    "B",
                    TypeIdentity.CreateDefinition(typeof(int)),
                    ImportCardinality.ExactlyOne,
                    true,
                    CreationPolicy.NonShared,
                    typeof(Version).GetProperty("Build"));
            }

            protected override PropertyBasedImportDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override PropertyBasedImportDefinition SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class PropertyBasedImportDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<PropertyBasedImportDefinition> m_DistinctInstances
                = new List<PropertyBasedImportDefinition> 
                     {
                        PropertyBasedImportDefinition.CreateDefinition(
                            "A", 
                            TypeIdentity.CreateDefinition(typeof(int)),
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(string).GetProperty("Length")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "B", 
                            TypeIdentity.CreateDefinition(typeof(int)),
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(Version).GetProperty("Build")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "C", 
                            TypeIdentity.CreateDefinition(typeof(int)),
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(List<int>).GetProperty("Count")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "D", 
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "E", 
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(TimeZoneInfo).GetProperty("StandardName")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly PropertyBasedImportDefinitionHashcodeContractVerfier m_HashcodeVerifier 
            = new PropertyBasedImportDefinitionHashcodeContractVerfier();

        private readonly PropertyBasedImportDefinitionEqualityContractVerifier m_EqualityVerifier 
            = new PropertyBasedImportDefinitionEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }

        private static PropertyInfo GetPropertyForString()
        {
            return typeof(string).GetProperty("Length");
        }

        [Test]
        public void RoundtripSerialize()
        {
            var original = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(int)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"));
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(int)),
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"));
            var property = GetPropertyForString();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(int)), obj.RequiredTypeIdentity);
            Assert.AreEqual(ImportCardinality.ExactlyOne, obj.Cardinality);
            Assert.IsTrue(obj.IsRecomposable);
            Assert.IsFalse(obj.IsPrerequisite);
            Assert.AreEqual(CreationPolicy.NonShared, obj.RequiredCreationPolicy);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(property), obj.Property);
        }
    }
}
