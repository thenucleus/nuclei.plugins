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
using System.Reflection;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class MethodBasedExportDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class MethodBasedExportDefinitionEqualityContractVerifier : EqualityContractVerifier<MethodBasedExportDefinition>
        {
            private readonly MethodBasedExportDefinition m_First = MethodBasedExportDefinition.CreateDefinition(
                "A",
                typeof(string).GetMethod("Contains"));

            private readonly MethodBasedExportDefinition m_Second = MethodBasedExportDefinition.CreateDefinition(
                "B",
                typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));

            protected override MethodBasedExportDefinition Copy(MethodBasedExportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return MethodBasedExportDefinition.CreateDefinition(
                        "A",
                        typeof(string).GetMethod("Contains"));
                }

                return MethodBasedExportDefinition.CreateDefinition(
                    "B",
                    typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));
            }

            protected override MethodBasedExportDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override MethodBasedExportDefinition SecondInstance
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

        private sealed class MethodBasedExportDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<MethodBasedExportDefinition> m_DistinctInstances
                = new List<MethodBasedExportDefinition> 
                     {
                        MethodBasedExportDefinition.CreateDefinition(
                            "A", 
                            typeof(string).GetMethod("Contains")),
                        MethodBasedExportDefinition.CreateDefinition(
                            "B", 
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        MethodBasedExportDefinition.CreateDefinition(
                            "C", 
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        MethodBasedExportDefinition.CreateDefinition(
                            "D", 
                            typeof(IComparable).GetMethod("CompareTo")),
                        MethodBasedExportDefinition.CreateDefinition(
                            "E", 
                            typeof(IComparable<>).GetMethod("CompareTo")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly MethodBasedExportDefinitionHashcodeContractVerfier m_HashcodeVerifier 
            = new MethodBasedExportDefinitionHashcodeContractVerfier();

        private readonly MethodBasedExportDefinitionEqualityContractVerifier m_EqualityVerifier 
            = new MethodBasedExportDefinitionEqualityContractVerifier();

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

        private static MethodInfo GetMethodForInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
        }

        [Test]
        public void RoundtripSerialize()
        {
            var original = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = MethodBasedExportDefinition.CreateDefinition("B", GetMethodForInt());

            Assert.AreEqual("B", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(int)), obj.DeclaringType);
            Assert.AreEqual(MethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }
    }
}
