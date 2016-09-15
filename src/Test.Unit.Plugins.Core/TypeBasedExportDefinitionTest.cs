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

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeBasedExportDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class TypeBasedExportDefinitionEqualityContractVerifier : EqualityContractVerifier<TypeBasedExportDefinition>
        {
            private readonly TypeBasedExportDefinition m_First = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));

            private readonly TypeBasedExportDefinition m_Second = TypeBasedExportDefinition.CreateDefinition("B", typeof(object));

            protected override TypeBasedExportDefinition Copy(TypeBasedExportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
                }

                return TypeBasedExportDefinition.CreateDefinition("B", typeof(object));
            }

            protected override TypeBasedExportDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override TypeBasedExportDefinition SecondInstance
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

        private sealed class TypeBasedExportDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<TypeBasedExportDefinition> m_DistinctInstances
                = new List<TypeBasedExportDefinition> 
                     {
                        TypeBasedExportDefinition.CreateDefinition("A", typeof(string)),
                        TypeBasedExportDefinition.CreateDefinition("B", typeof(object)),
                        TypeBasedExportDefinition.CreateDefinition("C", typeof(int)),
                        TypeBasedExportDefinition.CreateDefinition("D", typeof(IComparable)),
                        TypeBasedExportDefinition.CreateDefinition("E", typeof(IComparable<>)),
                        TypeBasedExportDefinition.CreateDefinition("F", typeof(List<int>)),
                        TypeBasedExportDefinition.CreateDefinition("G", typeof(double)),
                        TypeBasedExportDefinition.CreateDefinition("H", typeof(void)),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly TypeBasedExportDefinitionHashcodeContractVerfier m_HashcodeVerifier 
            = new TypeBasedExportDefinitionHashcodeContractVerfier();

        private readonly TypeBasedExportDefinitionEqualityContractVerifier m_EqualityVerifier 
            = new TypeBasedExportDefinitionEqualityContractVerifier();

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

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Type has to be public for it to be used for reflection.")]
        public sealed class Nested<TKey, TValue>
        {
        }

        [Test]
        public void RoundtripSerialize()
        {
            var original = TypeBasedExportDefinition.CreateDefinition("A", typeof(string));
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeBasedExportDefinition.CreateDefinition("A", typeof(List<int>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(List<int>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeBasedExportDefinition.CreateDefinition("A", type);

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Nested<,>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeBasedExportDefinition.CreateDefinition("A", typeof(IEnumerable<>));

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<>)), obj.DeclaringType);
        }
    }
}
