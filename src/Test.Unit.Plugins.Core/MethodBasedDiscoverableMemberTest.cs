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
    public sealed class MethodBasedDiscoverableMemberTest : EqualityContractVerifierTest
    {
        private static MethodInfo GetMethodForInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
        }

        private readonly MethodBasedDiscoverableMemberHashcodeContractVerfier _hashCodeVerifier
            = new MethodBasedDiscoverableMemberHashcodeContractVerfier();

        private readonly MethodBasedDiscoverableMemberEqualityContractVerifier _equalityVerifier
            = new MethodBasedDiscoverableMemberEqualityContractVerifier();

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

            var original = MethodBasedDiscoverableMember.CreateDefinition(GetMethodForInt(), metadata);
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);

            Assert.AreEqual(1, copy.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), copy.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), copy.Metadata.Values.First());
        }

        [Test]
        public void Create()
        {
            var metadata = new Dictionary<string, string>
                {
                    { "A", "B" }
                };

            var obj = MethodBasedDiscoverableMember.CreateDefinition(GetMethodForInt(), metadata);

            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(int)), obj.DeclaringType);
            Assert.AreEqual(MethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);

            Assert.AreEqual(1, obj.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), obj.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), obj.Metadata.Values.First());
        }

        private sealed class MethodBasedDiscoverableMemberEqualityContractVerifier : EqualityContractVerifier<MethodBasedDiscoverableMember>
        {
            private readonly MethodBasedDiscoverableMember _first = MethodBasedDiscoverableMember.CreateDefinition(
                typeof(string).GetMethod("Contains"),
                new Dictionary<string, string>());

            private readonly MethodBasedDiscoverableMember _second = MethodBasedDiscoverableMember.CreateDefinition(
                typeof(int).GetMethod("CompareTo", new[] { typeof(int) }),
                new Dictionary<string, string>());

            protected override MethodBasedDiscoverableMember Copy(MethodBasedDiscoverableMember original)
            {
                if (original.DeclaringType.Equals(TypeIdentity.CreateDefinition(typeof(string))))
                {
                    return MethodBasedDiscoverableMember.CreateDefinition(
                        typeof(string).GetMethod("Contains"),
                        new Dictionary<string, string>());
                }

                return MethodBasedDiscoverableMember.CreateDefinition(
                    typeof(int).GetMethod("CompareTo", new[] { typeof(int) }),
                    new Dictionary<string, string>());
            }

            protected override MethodBasedDiscoverableMember FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override MethodBasedDiscoverableMember SecondInstance
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

        private sealed class MethodBasedDiscoverableMemberHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<MethodBasedDiscoverableMember> _distinctInstances
                = new List<MethodBasedDiscoverableMember>
                     {
                        MethodBasedDiscoverableMember.CreateDefinition(
                            typeof(string).GetMethod("Contains"),
                            new Dictionary<string, string>()),
                        MethodBasedDiscoverableMember.CreateDefinition(
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) }),
                            new Dictionary<string, string>()),
                        MethodBasedDiscoverableMember.CreateDefinition(
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) }),
                            new Dictionary<string, string>()),
                        MethodBasedDiscoverableMember.CreateDefinition(
                            typeof(IComparable).GetMethod("CompareTo"),
                            new Dictionary<string, string>()),
                        MethodBasedDiscoverableMember.CreateDefinition(
                            typeof(IComparable<>).GetMethod("CompareTo"),
                            new Dictionary<string, string>()),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
