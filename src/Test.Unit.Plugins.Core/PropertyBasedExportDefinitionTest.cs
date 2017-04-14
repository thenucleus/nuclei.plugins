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

namespace Nuclei.Plugins.Core
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PropertyBasedExportDefinitionTest : EqualityContractVerifierTest
    {
        private static PropertyInfo GetPropertyForString()
        {
            return typeof(string).GetProperty("Length");
        }

        private readonly EndpointIdHashcodeContractVerfier _hashCodeVerifier = new EndpointIdHashcodeContractVerfier();

        private readonly EndpointIdEqualityContractVerifier _equalityVerifier = new EndpointIdEqualityContractVerifier();

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
            var original = PropertyBasedExportDefinition.CreateDefinition("A", "B", GetPropertyForString());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = PropertyBasedExportDefinition.CreateDefinition("A", "B", GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual("A", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(GetPropertyForString()), obj.Property);
        }

        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<PropertyBasedExportDefinition>
        {
            private readonly PropertyBasedExportDefinition _first
                = PropertyBasedExportDefinition.CreateDefinition("A", "B", typeof(string).GetProperty("Length"));

            private readonly PropertyBasedExportDefinition _second
                = PropertyBasedExportDefinition.CreateDefinition("C", "D", typeof(Version).GetProperty("Build"));

            protected override PropertyBasedExportDefinition Copy(PropertyBasedExportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return PropertyBasedExportDefinition.CreateDefinition("A", "B", typeof(string).GetProperty("Length"));
                }

                return PropertyBasedExportDefinition.CreateDefinition("C", "D", typeof(Version).GetProperty("Build"));
            }

            protected override PropertyBasedExportDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PropertyBasedExportDefinition SecondInstance
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

        private sealed class EndpointIdHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PropertyBasedExportDefinition> _distinctInstances
                = new List<PropertyBasedExportDefinition>
                     {
                        PropertyBasedExportDefinition.CreateDefinition("A", "B", typeof(string).GetProperty("Length")),
                        PropertyBasedExportDefinition.CreateDefinition("C", "D", typeof(Version).GetProperty("Build")),
                        PropertyBasedExportDefinition.CreateDefinition("E", "F", typeof(List<int>).GetProperty("Count")),
                        PropertyBasedExportDefinition.CreateDefinition("G", "H", typeof(TimeZone).GetProperty("StandardName")),
                        PropertyBasedExportDefinition.CreateDefinition("I", "J", typeof(TimeZoneInfo).GetProperty("StandardName")),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
