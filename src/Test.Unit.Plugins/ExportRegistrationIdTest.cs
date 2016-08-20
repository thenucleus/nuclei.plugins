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
    public sealed class ExportRegistrationIdTest : EqualityContractVerifierTest
    {
        private sealed class ExportRegistrationIdEqualityContractVerifier : EqualityContractVerifier<ExportRegistrationId>
        {
            private readonly ExportRegistrationId m_First = new ExportRegistrationId(typeof(string), 0, "a");

            private readonly ExportRegistrationId m_Second = new ExportRegistrationId(typeof(int), 0, "a");

            protected override ExportRegistrationId Copy(ExportRegistrationId original)
            {
                return original.Clone();
            }

            protected override ExportRegistrationId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ExportRegistrationId SecondInstance
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

        private sealed class ExportRegistrationIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ExportRegistrationId> m_DistinctInstances
                = new List<ExportRegistrationId> 
                     {
                        new ExportRegistrationId(typeof(string), 0, "a"),
                        new ExportRegistrationId(typeof(int), 0, "a"),
                        new ExportRegistrationId(typeof(string), 1, "a"),
                        new ExportRegistrationId(typeof(string), 0, "b"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ExportRegistrationIdHashcodeContractVerfier m_HashcodeVerifier = new ExportRegistrationIdHashcodeContractVerfier();

        private readonly ExportRegistrationIdEqualityContractVerifier m_EqualityVerifier = new ExportRegistrationIdEqualityContractVerifier();

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
            ExportRegistrationId first = null;
            ExportRegistrationId second = new ExportRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ExportRegistrationId first = new ExportRegistrationId(typeof(string), 0, "a");
            ExportRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ExportRegistrationId first = null;
            ExportRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "b");
            var second = new ExportRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "a");
            var second = new ExportRegistrationId(typeof(string), 0, "b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ExportRegistrationId first = null;
            ExportRegistrationId second = new ExportRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ExportRegistrationId first = new ExportRegistrationId(typeof(string), 0, "a");
            ExportRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ExportRegistrationId first = null;
            ExportRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "b");
            var second = new ExportRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "a");
            var second = new ExportRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ExportRegistrationId first = new ExportRegistrationId(typeof(string), 0, "a");
            ExportRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ExportRegistrationId first = new ExportRegistrationId(typeof(string), 0, "a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "b");
            var second = new ExportRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ExportRegistrationId(typeof(string), 0, "a");
            var second = new ExportRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ExportRegistrationId first = new ExportRegistrationId(typeof(string), 0, "a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
