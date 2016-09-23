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
    public sealed class ExportRegistrationIdTest : EqualityContractVerifierTest
    {
        private readonly ExportRegistrationIdHashcodeContractVerfier _hashCodeVerifier = new ExportRegistrationIdHashcodeContractVerfier();

        private readonly ExportRegistrationIdEqualityContractVerifier _equalityVerifier = new ExportRegistrationIdEqualityContractVerifier();

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

        private sealed class ExportRegistrationIdEqualityContractVerifier : EqualityContractVerifier<ExportRegistrationId>
        {
            private readonly ExportRegistrationId _first = new ExportRegistrationId(typeof(string), 0, "a");

            private readonly ExportRegistrationId _second = new ExportRegistrationId(typeof(int), 0, "a");

            protected override ExportRegistrationId Copy(ExportRegistrationId original)
            {
                return original.Clone();
            }

            protected override ExportRegistrationId FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ExportRegistrationId SecondInstance
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

        private sealed class ExportRegistrationIdHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<ExportRegistrationId> _distinctInstances
                = new List<ExportRegistrationId>
                     {
                        new ExportRegistrationId(typeof(string), 0, "a"),
                        new ExportRegistrationId(typeof(int), 0, "a"),
                        new ExportRegistrationId(typeof(string), 1, "a"),
                        new ExportRegistrationId(typeof(string), 0, "b"),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
