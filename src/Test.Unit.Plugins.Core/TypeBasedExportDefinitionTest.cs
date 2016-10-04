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
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins.Core
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class TypeBasedExportDefinitionTest : EqualityContractVerifierTest
    {
        private readonly TypeBasedExportDefinitionHashcodeContractVerfier _hashCodeVerifier
            = new TypeBasedExportDefinitionHashcodeContractVerfier();

        private readonly TypeBasedExportDefinitionEqualityContractVerifier _equalityVerifier
            = new TypeBasedExportDefinitionEqualityContractVerifier();

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

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Type has to be public for it to be used for reflection.")]
        public sealed class Nested<TKey, TValue>
        {
        }

        [Test]
        public void RoundTripSerialize()
        {
            var original = TypeBasedExportDefinition.CreateDefinition("A", "B", typeof(string));
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeBasedExportDefinition.CreateDefinition("A", "B", typeof(List<int>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(List<int>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeBasedExportDefinition.CreateDefinition("A", "B", type);

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Nested<,>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeBasedExportDefinition.CreateDefinition("A", "B", typeof(IEnumerable<>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<>)), obj.DeclaringType);
        }

        private sealed class TypeBasedExportDefinitionEqualityContractVerifier : EqualityContractVerifier<TypeBasedExportDefinition>
        {
            private readonly TypeBasedExportDefinition _first = TypeBasedExportDefinition.CreateDefinition("A", "B", typeof(string));

            private readonly TypeBasedExportDefinition _second = TypeBasedExportDefinition.CreateDefinition("C", "D", typeof(object));

            protected override TypeBasedExportDefinition Copy(TypeBasedExportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return TypeBasedExportDefinition.CreateDefinition("A", "B", typeof(string));
                }

                return TypeBasedExportDefinition.CreateDefinition("C", "D", typeof(object));
            }

            protected override TypeBasedExportDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override TypeBasedExportDefinition SecondInstance
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

        private sealed class TypeBasedExportDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<TypeBasedExportDefinition> _distinctInstances
                = new List<TypeBasedExportDefinition>
                     {
                        TypeBasedExportDefinition.CreateDefinition("A", "B", typeof(string)),
                        TypeBasedExportDefinition.CreateDefinition("C", "D", typeof(object)),
                        TypeBasedExportDefinition.CreateDefinition("E", "F", typeof(int)),
                        TypeBasedExportDefinition.CreateDefinition("G", "H", typeof(IComparable)),
                        TypeBasedExportDefinition.CreateDefinition("I", "J", typeof(IComparable<>)),
                        TypeBasedExportDefinition.CreateDefinition("K", "L", typeof(List<int>)),
                        TypeBasedExportDefinition.CreateDefinition("M", "N", typeof(double)),
                        TypeBasedExportDefinition.CreateDefinition("O", "P", typeof(void)),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
