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

namespace Apollo.Core.Extensions.Plugins
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class GroupRegistrationIdTest : EqualityContractVerifierTest
    {
        private sealed class GroupRegistrationIdEqualityContractVerifier : EqualityContractVerifier<GroupRegistrationId>
        {
            private readonly GroupRegistrationId m_First = new GroupRegistrationId("a");

            private readonly GroupRegistrationId m_Second = new GroupRegistrationId("b");

            protected override GroupRegistrationId Copy(GroupRegistrationId original)
            {
                return original.Clone();
            }

            protected override GroupRegistrationId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override GroupRegistrationId SecondInstance
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

        private sealed class GroupRegistrationIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<GroupRegistrationId> m_DistinctInstances
                = new List<GroupRegistrationId> 
                     {
                        new GroupRegistrationId("a"),
                        new GroupRegistrationId("b"),
                        new GroupRegistrationId("c"),
                        new GroupRegistrationId("d"),
                        new GroupRegistrationId("e"),
                        new GroupRegistrationId("f"),
                        new GroupRegistrationId("g"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly GroupRegistrationIdHashcodeContractVerfier m_HashcodeVerifier = new GroupRegistrationIdHashcodeContractVerfier();

        private readonly GroupRegistrationIdEqualityContractVerifier m_EqualityVerifier = new GroupRegistrationIdEqualityContractVerifier();

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

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            GroupRegistrationId first = null;
            GroupRegistrationId second = new GroupRegistrationId("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            GroupRegistrationId first = new GroupRegistrationId("a");
            GroupRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            GroupRegistrationId first = null;
            GroupRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new GroupRegistrationId("a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new GroupRegistrationId("b");
            var second = new GroupRegistrationId("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new GroupRegistrationId("a");
            var second = new GroupRegistrationId("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            GroupRegistrationId first = null;
            GroupRegistrationId second = new GroupRegistrationId("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            GroupRegistrationId first = new GroupRegistrationId("a");
            GroupRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            GroupRegistrationId first = null;
            GroupRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new GroupRegistrationId("a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new GroupRegistrationId("b");
            var second = new GroupRegistrationId("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new GroupRegistrationId("a");
            var second = new GroupRegistrationId("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            GroupRegistrationId first = new GroupRegistrationId("a");
            GroupRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            GroupRegistrationId first = new GroupRegistrationId("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new GroupRegistrationId("a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new GroupRegistrationId("b");
            var second = new GroupRegistrationId("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new GroupRegistrationId("a");
            var second = new GroupRegistrationId("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            GroupRegistrationId first = new GroupRegistrationId("a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
