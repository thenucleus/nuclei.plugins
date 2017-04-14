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
    public sealed class PropertyBasedDiscoverableMemberTest : EqualityContractVerifierTest
    {
        private static PropertyInfo GetPropertyForString()
        {
            return typeof(string).GetProperty("Length");
        }

        private readonly PropertyBasedDiscoverableMemberHashcodeContractVerfier _hashCodeVerifier
            = new PropertyBasedDiscoverableMemberHashcodeContractVerfier();

        private readonly PropertyBasedDiscoverableMemberEqualityContractVerifier _equalityVerifier
            = new PropertyBasedDiscoverableMemberEqualityContractVerifier();

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
            var metadata = new Dictionary<string, string>
                {
                    { "A", "B" }
                };

            var original = PropertyBasedDiscoverableMember.CreateDefinition(GetPropertyForString(), metadata);
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);

            Assert.AreEqual(1, copy.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), copy.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), copy.Metadata.Values.First());
        }

        [Test]
        public void CreateWithProperty()
        {
            var metadata = new Dictionary<string, string>
                {
                    { "A", "B" }
                };

            var obj = PropertyBasedDiscoverableMember.CreateDefinition(GetPropertyForString(), metadata);
            var property = GetPropertyForString();

            Assert.AreEqual(TypeIdentity.CreateDefinition(property.DeclaringType), obj.DeclaringType);
            Assert.AreEqual(PropertyDefinition.CreateDefinition(GetPropertyForString()), obj.Property);

            Assert.AreEqual(1, obj.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), obj.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), obj.Metadata.Values.First());
        }

        private sealed class PropertyBasedDiscoverableMemberEqualityContractVerifier : EqualityContractVerifier<PropertyBasedDiscoverableMember>
        {
            private readonly PropertyBasedDiscoverableMember _first
                = PropertyBasedDiscoverableMember.CreateDefinition(typeof(string).GetProperty("Length"), new Dictionary<string, string>());

            private readonly PropertyBasedDiscoverableMember _second
                = PropertyBasedDiscoverableMember.CreateDefinition(typeof(Version).GetProperty("Build"), new Dictionary<string, string>());

            protected override PropertyBasedDiscoverableMember Copy(PropertyBasedDiscoverableMember original)
            {
                if (original.DeclaringType.Equals(TypeIdentity.CreateDefinition(typeof(string))))
                {
                    return PropertyBasedDiscoverableMember.CreateDefinition(typeof(string).GetProperty("Length"), new Dictionary<string, string>());
                }

                return PropertyBasedDiscoverableMember.CreateDefinition(typeof(Version).GetProperty("Build"), new Dictionary<string, string>());
            }

            protected override PropertyBasedDiscoverableMember FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PropertyBasedDiscoverableMember SecondInstance
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

        private sealed class PropertyBasedDiscoverableMemberHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PropertyBasedDiscoverableMember> _distinctInstances
                = new List<PropertyBasedDiscoverableMember>
                     {
                        PropertyBasedDiscoverableMember.CreateDefinition(typeof(string).GetProperty("Length"), new Dictionary<string, string>()),
                        PropertyBasedDiscoverableMember.CreateDefinition(typeof(Version).GetProperty("Build"), new Dictionary<string, string>()),
                        PropertyBasedDiscoverableMember.CreateDefinition(typeof(List<int>).GetProperty("Count"), new Dictionary<string, string>()),
                        PropertyBasedDiscoverableMember.CreateDefinition(typeof(TimeZone).GetProperty("StandardName"), new Dictionary<string, string>()),
                        PropertyBasedDiscoverableMember.CreateDefinition(typeof(TimeZoneInfo).GetProperty("StandardName"), new Dictionary<string, string>()),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
