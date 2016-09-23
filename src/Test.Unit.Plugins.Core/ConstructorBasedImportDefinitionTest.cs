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
    public sealed class ConstructorBasedImportDefinitionTest : EqualityContractVerifierTest
    {
        private static ConstructorInfo GetConstructorForString()
        {
            return typeof(string).GetConstructor(new[] { typeof(char[]) });
        }

        private readonly ConstructorBasedImportDefinitionHashcodeContractVerfier _hashCodeVerifier
            = new ConstructorBasedImportDefinitionHashcodeContractVerfier();

        private readonly ConstructorBasedImportDefinitionEqualityContractVerifier _equalityVerifier
            = new ConstructorBasedImportDefinitionEqualityContractVerifier();

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
        public void RoundtripSerialize()
        {
            var original = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                GetConstructorForString().GetParameters().First());
            var constructor = GetConstructorForString();
            var parameter = constructor.GetParameters().First();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(char[])), obj.RequiredTypeIdentity);
            Assert.AreEqual(ImportCardinality.ExactlyOne, obj.Cardinality);
            Assert.IsFalse(obj.IsRecomposable);
            Assert.IsTrue(obj.IsPrerequisite);
            Assert.AreEqual(ConstructorDefinition.CreateDefinition(constructor), obj.Constructor);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(string)), obj.DeclaringType);
            Assert.AreEqual(ParameterDefinition.CreateDefinition(parameter), obj.Parameter);
        }

        private sealed class ConstructorBasedImportDefinitionEqualityContractVerifier : EqualityContractVerifier<ConstructorBasedImportDefinition>
        {
            private readonly ConstructorBasedImportDefinition _first = ConstructorBasedImportDefinition.CreateDefinition(
                "A",
                TypeIdentity.CreateDefinition(typeof(char[])),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                typeof(string).GetConstructor(
                    new[]
                    {
                        typeof(char[])
                    }).GetParameters().First());

            private readonly ConstructorBasedImportDefinition _second = ConstructorBasedImportDefinition.CreateDefinition(
                "B",
                TypeIdentity.CreateDefinition(typeof(string)),
                ImportCardinality.ExactlyOne,
                CreationPolicy.NonShared,
                typeof(Uri).GetConstructor(
                    new[]
                    {
                        typeof(string)
                    }).GetParameters().First());

            protected override ConstructorBasedImportDefinition Copy(ConstructorBasedImportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return ConstructorBasedImportDefinition.CreateDefinition(
                        "A",
                        TypeIdentity.CreateDefinition(typeof(char[])),
                        ImportCardinality.ExactlyOne,
                        CreationPolicy.NonShared,
                        typeof(string).GetConstructor(
                            new[]
                            {
                                typeof(char[])
                            }).GetParameters().First());
                }

                return ConstructorBasedImportDefinition.CreateDefinition(
                    "B",
                    TypeIdentity.CreateDefinition(typeof(string)),
                    ImportCardinality.ExactlyOne,
                    CreationPolicy.NonShared,
                    typeof(Uri).GetConstructor(
                        new[]
                        {
                            typeof(string)
                        }).GetParameters().First());
            }

            protected override ConstructorBasedImportDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ConstructorBasedImportDefinition SecondInstance
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

        private sealed class ConstructorBasedImportDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<ConstructorBasedImportDefinition> _distinctInstances
                = new List<ConstructorBasedImportDefinition>
                     {
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "A",
                            TypeIdentity.CreateDefinition(typeof(char[])),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(string).GetConstructor(
                                new[]
                                {
                                    typeof(char[])
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "B",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(Uri).GetConstructor(
                                new[]
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "C",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(Version).GetConstructor(
                                new[]
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                        ConstructorBasedImportDefinition.CreateDefinition(
                            "D",
                            TypeIdentity.CreateDefinition(typeof(string)),
                            ImportCardinality.ExactlyOne,
                            CreationPolicy.NonShared,
                            typeof(NotImplementedException).GetConstructor(
                                new[]
                                {
                                    typeof(string)
                                }).GetParameters().First()),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
