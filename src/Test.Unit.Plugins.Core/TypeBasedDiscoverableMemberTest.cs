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

namespace Nuclei.Plugins.Core
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class TypeBasedDiscoverableMemberTest : EqualityContractVerifierTest
    {
        private readonly TypeBasedDiscoverableMemberHashcodeContractVerfier _hashCodeVerifier
            = new TypeBasedDiscoverableMemberHashcodeContractVerfier();

        private readonly TypeBasedDiscoverableMemberEqualityContractVerifier _equalityVerifier
            = new TypeBasedDiscoverableMemberEqualityContractVerifier();

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
            var original = TypeBasedDiscoverableMember.CreateDefinition(typeof(string), metadata);
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
            Assert.AreEqual(1, copy.Metadata.Count);
            Assert.AreEqual(metadata.Keys.First(), copy.Metadata.Keys.First());
            Assert.AreEqual(metadata.Values.First(), copy.Metadata.Values.First());
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeBasedDiscoverableMember.CreateDefinition(typeof(List<int>), new Dictionary<string, string>());

            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(List<int>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeBasedDiscoverableMember.CreateDefinition(type, new Dictionary<string, string>());

            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(Nested<,>)), obj.DeclaringType);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeBasedDiscoverableMember.CreateDefinition(typeof(IEnumerable<>), new Dictionary<string, string>());

            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(IEnumerable<>)), obj.DeclaringType);
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Type has to be public for it to be used for reflection.")]
        public sealed class Nested<TKey, TValue>
        {
        }

        private sealed class TypeBasedDiscoverableMemberEqualityContractVerifier : EqualityContractVerifier<TypeBasedDiscoverableMember>
        {
            private readonly TypeBasedDiscoverableMember _first = TypeBasedDiscoverableMember.CreateDefinition(typeof(string), new Dictionary<string, string>());

            private readonly TypeBasedDiscoverableMember _second = TypeBasedDiscoverableMember.CreateDefinition(typeof(object), new Dictionary<string, string>());

            protected override TypeBasedDiscoverableMember Copy(TypeBasedDiscoverableMember original)
            {
                if (original.DeclaringType.Equals(TypeIdentity.CreateDefinition(typeof(string))))
                {
                    return TypeBasedDiscoverableMember.CreateDefinition(typeof(string), new Dictionary<string, string>());
                }

                return TypeBasedDiscoverableMember.CreateDefinition(typeof(object), new Dictionary<string, string>());
            }

            protected override TypeBasedDiscoverableMember FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override TypeBasedDiscoverableMember SecondInstance
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

        private sealed class TypeBasedDiscoverableMemberHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<TypeBasedDiscoverableMember> _distinctInstances
                = new List<TypeBasedDiscoverableMember>
                     {
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(string), new Dictionary<string, string>()),
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(object), new Dictionary<string, string>()),
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(int), new Dictionary<string, string>()),
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(IComparable), new Dictionary<string, string>()),
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(IComparable<>), new Dictionary<string, string>()),
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(List<int>), new Dictionary<string, string>()),
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(double), new Dictionary<string, string>()),
                        TypeBasedDiscoverableMember.CreateDefinition(typeof(void), new Dictionary<string, string>()),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
