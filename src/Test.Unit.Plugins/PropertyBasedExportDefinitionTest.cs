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
    public sealed class PropertyBasedExportDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<PropertyBasedExportDefinition>
        {
            private readonly PropertyBasedExportDefinition m_First 
                = PropertyBasedExportDefinition.CreateDefinition("A", typeof(string).GetProperty("Length"));

            private readonly PropertyBasedExportDefinition m_Second 
                = PropertyBasedExportDefinition.CreateDefinition("B", typeof(Version).GetProperty("Build"));

            protected override PropertyBasedExportDefinition Copy(PropertyBasedExportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return PropertyBasedExportDefinition.CreateDefinition("A", typeof(string).GetProperty("Length"));
                }

                return PropertyBasedExportDefinition.CreateDefinition("B", typeof(Version).GetProperty("Build"));
            }

            protected override PropertyBasedExportDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override PropertyBasedExportDefinition SecondInstance
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

        private sealed class EndpointIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<PropertyBasedExportDefinition> m_DistinctInstances
                = new List<PropertyBasedExportDefinition> 
                     {
                        PropertyBasedExportDefinition.CreateDefinition("A", typeof(string).GetProperty("Length")),
                        PropertyBasedExportDefinition.CreateDefinition("B", typeof(Version).GetProperty("Build")),
                        PropertyBasedExportDefinition.CreateDefinition("C", typeof(List<int>).GetProperty("Count")),
                        PropertyBasedExportDefinition.CreateDefinition("D", typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedExportDefinition.CreateDefinition("E", typeof(TimeZoneInfo).GetProperty("StandardName")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly EndpointIdHashcodeContractVerfier m_HashcodeVerifier = new EndpointIdHashcodeContractVerfier();

        private readonly EndpointIdEqualityContractVerifier m_EqualityVerifier = new EndpointIdEqualityContractVerifier();

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
            var original = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = PropertyBasedExportDefinition.CreateDefinition("A", GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(GetPropertyForString()), obj.Property);
        }
    }
}
