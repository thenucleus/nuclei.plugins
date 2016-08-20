//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
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
    public sealed class MethodBasedScheduleConditionDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class MethodBasedScheduleConditionDefinitionEqualityContractVerifier 
            : EqualityContractVerifier<MethodBasedScheduleConditionDefinition>
        {
            private readonly MethodBasedScheduleConditionDefinition m_First = MethodBasedScheduleConditionDefinition.CreateDefinition(
                "a",
                typeof(string).GetMethod("Contains"));

            private readonly MethodBasedScheduleConditionDefinition m_Second = MethodBasedScheduleConditionDefinition.CreateDefinition(
                "b",
                typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));

            protected override MethodBasedScheduleConditionDefinition Copy(MethodBasedScheduleConditionDefinition original)
            {
                if (original.ContractName.Equals("a"))
                {
                    return MethodBasedScheduleConditionDefinition.CreateDefinition(
                        "a",
                        typeof(string).GetMethod("Contains"));
                }

                return MethodBasedScheduleConditionDefinition.CreateDefinition(
                    "b",
                    typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));
            }

            protected override MethodBasedScheduleConditionDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override MethodBasedScheduleConditionDefinition SecondInstance
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

        private sealed class MethodBasedScheduleConditionDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<MethodBasedScheduleConditionDefinition> m_DistinctInstances
                = new List<MethodBasedScheduleConditionDefinition> 
                     {
                        MethodBasedScheduleConditionDefinition.CreateDefinition(
                            "a", 
                            typeof(string).GetMethod("Contains")),
                        MethodBasedScheduleConditionDefinition.CreateDefinition(
                            "b", 
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        MethodBasedScheduleConditionDefinition.CreateDefinition(
                            "c", 
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        MethodBasedScheduleConditionDefinition.CreateDefinition(
                            "d", 
                            typeof(IComparable).GetMethod("CompareTo")),
                        MethodBasedScheduleConditionDefinition.CreateDefinition(
                            "e", 
                            typeof(IComparable<>).GetMethod("CompareTo")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly MethodBasedScheduleConditionDefinitionHashcodeContractVerfier m_HashcodeVerifier 
            = new MethodBasedScheduleConditionDefinitionHashcodeContractVerfier();

        private readonly MethodBasedScheduleConditionDefinitionEqualityContractVerifier m_EqualityVerifier 
            = new MethodBasedScheduleConditionDefinitionEqualityContractVerifier();

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
            var original = MethodBasedScheduleConditionDefinition.CreateDefinition("a", GetMethodForInt());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = MethodBasedScheduleConditionDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.AreEqual("a", obj.ContractName);
            Assert.AreEqual(MethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }
    }
}
