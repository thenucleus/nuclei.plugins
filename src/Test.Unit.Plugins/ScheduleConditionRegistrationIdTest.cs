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
    public sealed class ScheduleConditionRegistrationIdTest : EqualityContractVerifierTest
    {
        private sealed class ScheduleConditionRegistrationIdEqualityContractVerifier : EqualityContractVerifier<ScheduleConditionRegistrationId>
        {
            private readonly ScheduleConditionRegistrationId m_First = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            private readonly ScheduleConditionRegistrationId m_Second = new ScheduleConditionRegistrationId(typeof(int), 0, "a");

            protected override ScheduleConditionRegistrationId Copy(ScheduleConditionRegistrationId original)
            {
                return original.Clone();
            }

            protected override ScheduleConditionRegistrationId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScheduleConditionRegistrationId SecondInstance
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

        private sealed class ScheduleConditionRegistrationIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScheduleConditionRegistrationId> m_DistinctInstances
                = new List<ScheduleConditionRegistrationId> 
                     {
                        new ScheduleConditionRegistrationId(typeof(string), 0, "a"),
                        new ScheduleConditionRegistrationId(typeof(int), 0, "a"),
                        new ScheduleConditionRegistrationId(typeof(string), 1, "a"),
                        new ScheduleConditionRegistrationId(typeof(string), 0, "b"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ScheduleConditionRegistrationIdHashcodeContractVerfier m_HashcodeVerifier 
            = new ScheduleConditionRegistrationIdHashcodeContractVerfier();

        private readonly ScheduleConditionRegistrationIdEqualityContractVerifier m_EqualityVerifier 
            = new ScheduleConditionRegistrationIdEqualityContractVerifier();

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
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            ScheduleConditionRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            ScheduleConditionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ScheduleConditionRegistrationId first = null;
            ScheduleConditionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            ScheduleConditionRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleConditionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ScheduleConditionRegistrationId first = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
