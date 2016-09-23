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
    public sealed class PropertyDefinitionTest : EqualityContractVerifierTest
    {
        private readonly PropertyDefinitionHashcodeContractVerfier _hashCodeVerifier = new PropertyDefinitionHashcodeContractVerfier();

        private readonly PropertyDefinitionEqualityContractVerifier _equalityVerifier = new PropertyDefinitionEqualityContractVerifier();

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

        private static PropertyInfo GetPropertyForString()
        {
            return typeof(string).GetProperty("Length");
        }

        [Test]
        public void RoundtripSerialize()
        {
            var original = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = PropertyDefinition.CreateDefinition(GetPropertyForString());
            var property = GetPropertyForString();

            Assert.AreEqual(property.Name, obj.PropertyName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.PropertyType), obj.PropertyType);
            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
        }

        private sealed class PropertyDefinitionEqualityContractVerifier : EqualityContractVerifier<PropertyDefinition>
        {
            private readonly PropertyDefinition _first
                = PropertyDefinition.CreateDefinition(typeof(string).GetProperty("Length"));

            private readonly PropertyDefinition _second
                = PropertyDefinition.CreateDefinition(typeof(Version).GetProperty("Build"));

            protected override PropertyDefinition Copy(PropertyDefinition original)
            {
                if (original.DeclaringType.Equals(typeof(string)))
                {
                    return PropertyDefinition.CreateDefinition(typeof(string).GetProperty("Length"));
                }

                return PropertyDefinition.CreateDefinition(typeof(Version).GetProperty("Build"));
            }

            protected override PropertyDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PropertyDefinition SecondInstance
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

        private sealed class PropertyDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PropertyDefinition> _distinctInstances
                = new List<PropertyDefinition>
                     {
                        PropertyDefinition.CreateDefinition(typeof(string).GetProperty("Length")),
                        PropertyDefinition.CreateDefinition(typeof(Version).GetProperty("Build")),
                        PropertyDefinition.CreateDefinition(typeof(List<int>).GetProperty("Count")),
                        PropertyDefinition.CreateDefinition(typeof(TimeZone).GetProperty("StandardName")),
                        PropertyDefinition.CreateDefinition(typeof(TimeZoneInfo).GetProperty("StandardName")),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
