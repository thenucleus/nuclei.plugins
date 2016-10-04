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

namespace Nuclei.Plugins.Core
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PropertyBasedImportDefinitionTest : EqualityContractVerifierTest
    {
        private static PropertyInfo GetPropertyForString()
        {
            return typeof(string).GetProperty("Length");
        }

        private readonly PropertyBasedImportDefinitionHashcodeContractVerfier _hashCodeVerifier
            = new PropertyBasedImportDefinitionHashcodeContractVerfier();

        private readonly PropertyBasedImportDefinitionEqualityContractVerifier _equalityVerifier
            = new PropertyBasedImportDefinitionEqualityContractVerifier();

        protected override HashCodeContractVerifier HashContract
        {
            get
            {
                return _hashCodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return _equalityVerifier;
            }
        }

        [Test]
        public void RoundTripSerialize()
        {
            var original = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(int)),
                "System.Int32",
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
                "System.Int32",
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

        private sealed class PropertyBasedImportDefinitionEqualityContractVerifier : EqualityContractVerifier<PropertyBasedImportDefinition>
        {
            private readonly PropertyBasedImportDefinition _first = PropertyBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(int)),
                "System.Int32",
                ImportCardinality.ExactlyOne,
                true,
                CreationPolicy.NonShared,
                typeof(string).GetProperty("Length"));

            private readonly PropertyBasedImportDefinition _second = PropertyBasedImportDefinition.CreateDefinition(
                "B",
                TypeIdentity.CreateDefinition(typeof(int)),
                "System.Int32",
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
                        "System.Int32",
                        ImportCardinality.ExactlyOne,
                        true,
                        CreationPolicy.NonShared,
                        typeof(string).GetProperty("Length"));
                }

                return PropertyBasedImportDefinition.CreateDefinition(
                    "B",
                    TypeIdentity.CreateDefinition(typeof(int)),
                    "System.Int32",
                    ImportCardinality.ExactlyOne,
                    true,
                    CreationPolicy.NonShared,
                    typeof(Version).GetProperty("Build"));
            }

            protected override PropertyBasedImportDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PropertyBasedImportDefinition SecondInstance
            {
                get
                {
                    return _second;
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

        private sealed class PropertyBasedImportDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PropertyBasedImportDefinition> _distinctInstances
                = new List<PropertyBasedImportDefinition>
                     {
                        PropertyBasedImportDefinition.CreateDefinition(
                            "A",
                            TypeIdentity.CreateDefinition(typeof(int)),
                            "System.Int32",
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(string).GetProperty("Length")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "B",
                            TypeIdentity.CreateDefinition(typeof(int)),
                            "System.Int32",
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(Version).GetProperty("Build")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "C",
                            TypeIdentity.CreateDefinition(typeof(int)),
                            "System.Int32",
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(List<int>).GetProperty("Count")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "D",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            "System.String",
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedImportDefinition.CreateDefinition(
                            "E",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            "System.String",
                            ImportCardinality.ExactlyOne,
                            true,
                            CreationPolicy.NonShared,
                            typeof(TimeZoneInfo).GetProperty("StandardName")),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
