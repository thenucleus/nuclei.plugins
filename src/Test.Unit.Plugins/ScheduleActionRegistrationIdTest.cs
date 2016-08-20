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
    public sealed class ScheduleActionRegistrationIdTest : EqualityContractVerifierTest
    {
        private sealed class ScheduleActionRegistrationIdEqualityContractVerifier : EqualityContractVerifier<ScheduleActionRegistrationId>
        {
            private readonly ScheduleActionRegistrationId m_First = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            private readonly ScheduleActionRegistrationId m_Second = new ScheduleActionRegistrationId(typeof(int), 0, "a");

            protected override ScheduleActionRegistrationId Copy(ScheduleActionRegistrationId original)
            {
                return original.Clone();
            }

            protected override ScheduleActionRegistrationId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScheduleActionRegistrationId SecondInstance
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

        private sealed class ScheduleActionRegistrationIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScheduleActionRegistrationId> m_DistinctInstances
                = new List<ScheduleActionRegistrationId> 
                     {
                        new ScheduleActionRegistrationId(typeof(string), 0, "a"),
                        new ScheduleActionRegistrationId(typeof(int), 0, "a"),
                        new ScheduleActionRegistrationId(typeof(string), 1, "a"),
                        new ScheduleActionRegistrationId(typeof(string), 0, "b"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ScheduleActionRegistrationIdHashcodeContractVerfier m_HashcodeVerifier 
            = new ScheduleActionRegistrationIdHashcodeContractVerfier();

        private readonly ScheduleActionRegistrationIdEqualityContractVerifier m_EqualityVerifier 
            = new ScheduleActionRegistrationIdEqualityContractVerifier();

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
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            ScheduleActionRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            ScheduleActionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ScheduleActionRegistrationId first = null;
            ScheduleActionRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            ScheduleActionRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "b");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var second = new ScheduleActionRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ScheduleActionRegistrationId first = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
