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
    public sealed class PropertyBasedScheduleConditionDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class PropertyBasedScheduleConditionDefinitionEqualityContractVerifier 
            : EqualityContractVerifier<PropertyBasedScheduleConditionDefinition>
        {
            private readonly PropertyBasedScheduleConditionDefinition m_First
                = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", typeof(string).GetProperty("Length"));

            private readonly PropertyBasedScheduleConditionDefinition m_Second
                = PropertyBasedScheduleConditionDefinition.CreateDefinition("b", typeof(Version).GetProperty("Build"));

            protected override PropertyBasedScheduleConditionDefinition Copy(PropertyBasedScheduleConditionDefinition original)
            {
                if (original.ContractName.Equals("a"))
                {
                    return PropertyBasedScheduleConditionDefinition.CreateDefinition("a", typeof(string).GetProperty("Length"));
                }

                return PropertyBasedScheduleConditionDefinition.CreateDefinition("b", typeof(Version).GetProperty("Build"));
            }

            protected override PropertyBasedScheduleConditionDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override PropertyBasedScheduleConditionDefinition SecondInstance
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

        private sealed class PropertyBasedScheduleConditionDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<PropertyBasedScheduleConditionDefinition> m_DistinctInstances
                = new List<PropertyBasedScheduleConditionDefinition> 
                     {
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("a", typeof(string).GetProperty("Length")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("b", typeof(Version).GetProperty("Build")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("c", typeof(List<int>).GetProperty("Count")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("d", typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedScheduleConditionDefinition.CreateDefinition("e", typeof(TimeZoneInfo).GetProperty("StandardName")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly PropertyBasedScheduleConditionDefinitionHashcodeContractVerfier m_HashcodeVerifier 
            = new PropertyBasedScheduleConditionDefinitionHashcodeContractVerfier();

        private readonly PropertyBasedScheduleConditionDefinitionEqualityContractVerifier m_EqualityVerifier 
            = new PropertyBasedScheduleConditionDefinitionEqualityContractVerifier();

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
            var original = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = PropertyBasedScheduleConditionDefinition.CreateDefinition("a", GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("a", obj.ContractName);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(property), obj.Property);
        }
    }
}
