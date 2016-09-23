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
    public sealed class PartRegistrationIdTest : EqualityContractVerifierTest
    {
        private readonly PartRegistrationIdHashcodeContractVerfier _hashCodeVerifier = new PartRegistrationIdHashcodeContractVerfier();

        private readonly PartRegistrationIdEqualityContractVerifier _equalityVerifier = new PartRegistrationIdEqualityContractVerifier();

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
        public void LargerThanOperatorWithFirstObjectNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            PartRegistrationId second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLarger()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 1);
            var second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = new PartRegistrationId(typeof(string).FullName, 1);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            PartRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            PartRegistrationId first = null;
            PartRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLarger()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 1);
            var second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = new PartRegistrationId(typeof(string).FullName, 1);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void Clone()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            PartRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            object second = first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObject()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 1);
            var second = new PartRegistrationId(typeof(string).FullName, 0);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObject()
        {
            var first = new PartRegistrationId(typeof(string).FullName, 0);
            var second = new PartRegistrationId(typeof(string).FullName, 1);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            PartRegistrationId first = new PartRegistrationId(typeof(string).FullName, 0);
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }

        private sealed class PartRegistrationIdEqualityContractVerifier : EqualityContractVerifier<PartRegistrationId>
        {
            private readonly PartRegistrationId _first = new PartRegistrationId(typeof(string).FullName, 0);

            private readonly PartRegistrationId _second = new PartRegistrationId(typeof(int).FullName, 0);

            protected override PartRegistrationId Copy(PartRegistrationId original)
            {
                return original.Clone();
            }

            protected override PartRegistrationId FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PartRegistrationId SecondInstance
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

        private sealed class PartRegistrationIdHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PartRegistrationId> _distinctInstances
                = new List<PartRegistrationId>
                     {
                        new PartRegistrationId(typeof(string).FullName, 0),
                        new PartRegistrationId(typeof(int).FullName, 0),
                        new PartRegistrationId(typeof(string).FullName, 1),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
