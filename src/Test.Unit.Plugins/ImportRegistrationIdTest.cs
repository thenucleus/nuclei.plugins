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
    public sealed class ImportRegistrationIdTest : EqualityContractVerifierTest
    {
        private sealed class ImportRegistrationIdEqualityContractVerifier : EqualityContractVerifier<ImportRegistrationId>
        {
            private readonly ImportRegistrationId m_First = new ImportRegistrationId(typeof(string), 0, "a");

            private readonly ImportRegistrationId m_Second = new ImportRegistrationId(typeof(int), 0, "a");

            protected override ImportRegistrationId Copy(ImportRegistrationId original)
            {
                return original.Clone();
            }

            protected override ImportRegistrationId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ImportRegistrationId SecondInstance
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

        private sealed class ImportRegistrationIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ImportRegistrationId> m_DistinctInstances
                = new List<ImportRegistrationId> 
                     {
                        new ImportRegistrationId(typeof(string), 0, "a"),
                        new ImportRegistrationId(typeof(int), 0, "a"),
                        new ImportRegistrationId(typeof(string), 1, "a"),
                        new ImportRegistrationId(typeof(string), 0, "b"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ImportRegistrationIdHashcodeContractVerfier m_HashcodeVerifier = new ImportRegistrationIdHashcodeContractVerfier();

        private readonly ImportRegistrationIdEqualityContractVerifier m_EqualityVerifier = new ImportRegistrationIdEqualityContractVerifier();

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
            ImportRegistrationId first = null;
            ImportRegistrationId second = new ImportRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            ImportRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            ImportRegistrationId first = null;
            ImportRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "b");
            var second = new ImportRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            var second = new ImportRegistrationId(typeof(string), 0, "b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ImportRegistrationId first = null;
            ImportRegistrationId second = new ImportRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            ImportRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            ImportRegistrationId first = null;
            ImportRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "b");
            var second = new ImportRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            var second = new ImportRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            ImportRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "b");
            var second = new ImportRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            var second = new ImportRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
