//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class TypeDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<TypeDefinition>
        {
            private readonly TypeDefinition m_First = TypeDefinition.CreateDefinition(typeof(string), TypeIdentity.CreateDefinition);

            private readonly TypeDefinition m_Second = TypeDefinition.CreateDefinition(typeof(object), TypeIdentity.CreateDefinition);

            protected override TypeDefinition Copy(TypeDefinition original)
            {
                if (original.Identity.Equals(typeof(string)))
                {
                    return TypeDefinition.CreateDefinition(typeof(string), TypeIdentity.CreateDefinition);
                }

                return TypeDefinition.CreateDefinition(typeof(object), TypeIdentity.CreateDefinition);
            }

            protected override TypeDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override TypeDefinition SecondInstance
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
            private readonly IEnumerable<TypeDefinition> m_DistinctInstances
                = new List<TypeDefinition> 
                     {
                        TypeDefinition.CreateDefinition(typeof(string), TypeIdentity.CreateDefinition),
                        TypeDefinition.CreateDefinition(typeof(object), TypeIdentity.CreateDefinition),
                        TypeDefinition.CreateDefinition(typeof(int), TypeIdentity.CreateDefinition),
                        TypeDefinition.CreateDefinition(typeof(IComparable), TypeIdentity.CreateDefinition),
                        TypeDefinition.CreateDefinition(typeof(IComparable<>), TypeIdentity.CreateDefinition),
                        TypeDefinition.CreateDefinition(typeof(List<int>), TypeIdentity.CreateDefinition),
                        TypeDefinition.CreateDefinition(typeof(double), TypeIdentity.CreateDefinition),
                        TypeDefinition.CreateDefinition(typeof(void), TypeIdentity.CreateDefinition),
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

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Type has to be public for it to be used for reflection.")]
        public sealed class Nested<TKey, TValue>
        { 
        }

        [Test]
        public void RoundtripSerialize()
        {
            var original = TypeDefinition.CreateDefinition(typeof(string), TypeIdentity.CreateDefinition);
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeDefinition.CreateDefinition(typeof(List<int>), TypeIdentity.CreateDefinition);

            Assert.AreEqual(typeof(List<int>).FullName, obj.Identity.FullName);
            Assert.AreEqual(typeof(List<int>).IsClass, obj.IsClass);
            Assert.AreEqual(typeof(List<int>).IsInterface, obj.IsInterface);

            Assert.AreEqual(typeof(List<int>).BaseType.FullName, obj.BaseType.FullName);

            var interfaces = new[]
                {
                    TypeIdentity.CreateDefinition(typeof(IList<int>)),
                    TypeIdentity.CreateDefinition(typeof(ICollection<int>)),
                    TypeIdentity.CreateDefinition(typeof(IEnumerable<int>)),
                    TypeIdentity.CreateDefinition(typeof(IReadOnlyCollection<int>)),
                    TypeIdentity.CreateDefinition(typeof(IReadOnlyList<int>)),
                    TypeIdentity.CreateDefinition(typeof(IList)),
                    TypeIdentity.CreateDefinition(typeof(ICollection)),
                    TypeIdentity.CreateDefinition(typeof(IEnumerable)),
                };
            Assert.That(obj.BaseInterfaces, Is.EquivalentTo(interfaces));
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeDefinition.CreateDefinition(type, TypeIdentity.CreateDefinition);

            Assert.AreEqual(type.FullName, obj.Identity.FullName);
            Assert.AreEqual(type.IsClass, obj.IsClass);
            Assert.AreEqual(type.IsInterface, obj.IsInterface);

            Assert.AreEqual(type.BaseType.FullName, obj.BaseType.FullName);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeDefinition.CreateDefinition(typeof(IEnumerable<>), TypeIdentity.CreateDefinition);

            Assert.AreEqual(typeof(IEnumerable<>).FullName, obj.Identity.FullName);
            Assert.AreEqual(typeof(IEnumerable<>).IsClass, obj.IsClass);
            Assert.AreEqual(typeof(IEnumerable<>).IsInterface, obj.IsInterface);
            Assert.IsNull(obj.BaseType);
            Assert.That(obj.BaseInterfaces, Is.EquivalentTo(new[] { TypeIdentity.CreateDefinition(typeof(IEnumerable)) }));
        }
    }
}
