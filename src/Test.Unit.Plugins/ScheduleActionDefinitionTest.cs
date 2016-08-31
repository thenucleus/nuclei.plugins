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
    public sealed class ScheduleActionDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class ScheduleActionDefinitionEqualityContractVerifier : EqualityContractVerifier<ScheduleActionDefinition>
        {
            private readonly ScheduleActionDefinition m_First 
                = ScheduleActionDefinition.CreateDefinition("a", typeof(string).GetMethod("Contains"));

            private readonly ScheduleActionDefinition m_Second 
                = ScheduleActionDefinition.CreateDefinition("b", typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));

            protected override ScheduleActionDefinition Copy(ScheduleActionDefinition original)
            {
                if (original.ContractName.Equals("a"))
                {
                    return ScheduleActionDefinition.CreateDefinition("a", typeof(string).GetMethod("Contains"));
                }

                return ScheduleActionDefinition.CreateDefinition("b", typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));
            }

            protected override ScheduleActionDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScheduleActionDefinition SecondInstance
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

        private sealed class ScheduleActionDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScheduleActionDefinition> m_DistinctInstances
                = new List<ScheduleActionDefinition> 
                     {
                        ScheduleActionDefinition.CreateDefinition("a", typeof(string).GetMethod("Contains")),
                        ScheduleActionDefinition.CreateDefinition("b", typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        ScheduleActionDefinition.CreateDefinition("c", typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        ScheduleActionDefinition.CreateDefinition("d", typeof(IComparable).GetMethod("CompareTo")),
                        ScheduleActionDefinition.CreateDefinition("e", typeof(IComparable<>).GetMethod("CompareTo")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ScheduleActionDefinitionHashcodeContractVerfier m_HashcodeVerifier = new ScheduleActionDefinitionHashcodeContractVerfier();

        private readonly ScheduleActionDefinitionEqualityContractVerifier m_EqualityVerifier = new ScheduleActionDefinitionEqualityContractVerifier();

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
            var original = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = ScheduleActionDefinition.CreateDefinition("a", GetMethodForInt());

            Assert.AreEqual("a", obj.ContractName);
            Assert.AreEqual(MethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }
    }
}
