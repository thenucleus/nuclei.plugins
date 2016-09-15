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

namespace Nuclei.Plugins.Instantiation
{
    // Note that it is not possible to use the Gallio Comparison contract verifiers because they require that the
    // class implements the overloaded operators directly which ID derivative classes do not do (and could only do if we
    // move all the overloads of Equals(object) and GetHashCode() to the ID derivative class).
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class PartInstanceIdTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<PartInstanceId>
        {
            private readonly PartInstanceId m_First = new PartInstanceId();

            private readonly PartInstanceId m_Second = new PartInstanceId();

            protected override PartInstanceId Copy(PartInstanceId original)
            {
                return original.Clone();
            }

            protected override PartInstanceId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override PartInstanceId SecondInstance
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
            private readonly IEnumerable<PartInstanceId> m_DistinctInstances
                = new List<PartInstanceId> 
                     {
                        new PartInstanceId(),
                        new PartInstanceId(),
                        new PartInstanceId(),
                        new PartInstanceId(),
                        new PartInstanceId(),
                        new PartInstanceId(),
                        new PartInstanceId(),
                        new PartInstanceId(),
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

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            PartInstanceId first = null;
            PartInstanceId second = new PartInstanceId();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            PartInstanceId first = new PartInstanceId();
            PartInstanceId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            PartInstanceId first = null;
            PartInstanceId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new PartInstanceId();
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            PartInstanceId first = null;
            PartInstanceId second = new PartInstanceId();

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            PartInstanceId first = new PartInstanceId();
            PartInstanceId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            PartInstanceId first = null;
            PartInstanceId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new PartInstanceId();
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            PartInstanceId first = new PartInstanceId();
            PartInstanceId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            PartInstanceId first = new PartInstanceId();
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new PartInstanceId();
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) > 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) < 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var firstGuid = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();

            var first = (firstGuid.CompareTo(secondGuid) < 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);
            var second = (firstGuid.CompareTo(secondGuid) > 0) ? new PartInstanceId(firstGuid) : new PartInstanceId(secondGuid);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            PartInstanceId first = new PartInstanceId();
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }
    }
}
