//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
    public sealed class TypeIdentityTest : EqualityContractVerifierTest
    {
        private sealed class TypeIdentityEqualityContractVerifier : EqualityContractVerifier<TypeIdentity>
        {
            private readonly TypeIdentity m_First = TypeIdentity.CreateDefinition(typeof(string));

            private readonly TypeIdentity m_Second = TypeIdentity.CreateDefinition(typeof(object));

            protected override TypeIdentity Copy(TypeIdentity original)
            {
                if (original.Equals(typeof(string)))
                {
                    return TypeIdentity.CreateDefinition(typeof(string));
                }

                return TypeIdentity.CreateDefinition(typeof(object));
            }

            protected override TypeIdentity FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override TypeIdentity SecondInstance
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

        private sealed class TypeIdentityHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<TypeIdentity> m_DistinctInstances
                = new List<TypeIdentity> 
                     {
                        TypeIdentity.CreateDefinition(typeof(string)),
                        TypeIdentity.CreateDefinition(typeof(object)),
                        TypeIdentity.CreateDefinition(typeof(int)),
                        TypeIdentity.CreateDefinition(typeof(IComparable)),
                        TypeIdentity.CreateDefinition(typeof(IComparable<>)),
                        TypeIdentity.CreateDefinition(typeof(List<int>)),
                        TypeIdentity.CreateDefinition(typeof(double)),
                        TypeIdentity.CreateDefinition(typeof(void)),
                        TypeIdentity.CreateDefinition(typeof(IEnumerable<>).GetGenericArguments().First()),
                        TypeIdentity.CreateDefinition(typeof(IComparable<>).GetGenericArguments().First()),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly TypeIdentityHashcodeContractVerfier m_HashcodeVerifier = new TypeIdentityHashcodeContractVerfier();

        private readonly TypeIdentityEqualityContractVerifier m_EqualityVerifier = new TypeIdentityEqualityContractVerifier();

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
            var original = TypeIdentity.CreateDefinition(typeof(string));
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = TypeIdentity.CreateDefinition(typeof(List<int>));

            Assert.AreEqual(typeof(List<int>).Name, obj.Name);
            Assert.AreEqual(typeof(List<int>).Namespace, obj.Namespace);
            Assert.AreEqual(typeof(List<int>).FullName, obj.FullName);
            Assert.AreEqual(typeof(List<int>).Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void CreateWithNestedClass()
        {
            var type = typeof(Nested<,>);
            var obj = TypeIdentity.CreateDefinition(type);

            Assert.AreEqual(type.Name, obj.Name);
            Assert.AreEqual(type.Namespace, obj.Namespace);
            Assert.AreEqual(type.FullName, obj.FullName);
            Assert.AreEqual(type.Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void CreateWithInterface()
        {
            var obj = TypeIdentity.CreateDefinition(typeof(IEnumerable<>));

            Assert.AreEqual(typeof(IEnumerable<>).Name, obj.Name);
            Assert.AreEqual(typeof(IEnumerable<>).Namespace, obj.Namespace);
            Assert.AreEqual(typeof(IEnumerable<>).FullName, obj.FullName);
            Assert.AreEqual(typeof(IEnumerable<>).Assembly.GetName().Name, obj.Assembly.Name);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            object second = TypeIdentity.CreateDefinition(typeof(string));

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            object second = TypeIdentity.CreateDefinition(typeof(object));

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = TypeIdentity.CreateDefinition(typeof(string));
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(object));
            var second = typeof(object);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<int>));
            var second = typeof(IEnumerable<int>);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualOpenGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>));
            var second = typeof(IEnumerable<>);

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualGenericTypeParameter()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>).GetGenericArguments().First());
            var second = typeof(IEnumerable<>).GetGenericArguments().First();

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(object));
            var second = typeof(string);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<int>));
            var second = typeof(IEnumerable<float>);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalOpenGenericType()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>));
            var second = typeof(IComparable<>);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalGenericTypeParameter()
        {
            var first = TypeIdentity.CreateDefinition(typeof(IEnumerable<>).GetGenericArguments().First());
            var second = typeof(IComparable<>).GetGenericArguments().First();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
