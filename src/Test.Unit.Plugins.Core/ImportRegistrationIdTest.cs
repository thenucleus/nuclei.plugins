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
    public sealed class ImportRegistrationIdTest : EqualityContractVerifierTest
    {
        private readonly ImportRegistrationIdHashcodeContractVerfier _hashCodeVerifier = new ImportRegistrationIdHashcodeContractVerfier();

        private readonly ImportRegistrationIdEqualityContractVerifier _equalityVerifier = new ImportRegistrationIdEqualityContractVerifier();

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
        public void Clone()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            ImportRegistrationId second = first.Clone();

            Assert.AreEqual(first, second);
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
        public void CompareToWithNullObject()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
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

        [Test]
        public void Create()
        {
            var id = new ImportRegistrationId(typeof(string), 0, typeof(double));

            Assert.IsNotNull(id);
            Assert.AreEqual(typeof(double).FullName, id.ContractName);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.ImportRegistrationId",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithEmptyContractName()
        {
            Assert.Throws<ArgumentException>(() => new ImportRegistrationId(typeof(string), 0, string.Empty));
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
        public void LargerThanOperatorWithFirstObjectNull()
        {
            ImportRegistrationId first = null;
            ImportRegistrationId second = new ImportRegistrationId(typeof(string), 0, "a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            var second = new ImportRegistrationId(typeof(string), 0, "b");

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
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            ImportRegistrationId first = null;
            ImportRegistrationId second = new ImportRegistrationId(typeof(string), 0, "a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmaller()
        {
            var first = new ImportRegistrationId(typeof(string), 0, "a");
            var second = new ImportRegistrationId(typeof(string), 0, "b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            ImportRegistrationId first = new ImportRegistrationId(typeof(string), 0, "a");
            ImportRegistrationId second = null;

            Assert.IsFalse(first < second);
        }

        private sealed class ImportRegistrationIdEqualityContractVerifier : EqualityContractVerifier<ImportRegistrationId>
        {
            private readonly ImportRegistrationId _first = new ImportRegistrationId(typeof(string), 0, "a");

            private readonly ImportRegistrationId _second = new ImportRegistrationId(typeof(int), 0, "a");

            protected override ImportRegistrationId Copy(ImportRegistrationId original)
            {
                return original.Clone();
            }

            protected override ImportRegistrationId FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ImportRegistrationId SecondInstance
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

        private sealed class ImportRegistrationIdHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<ImportRegistrationId> _distinctInstances
                = new List<ImportRegistrationId>
                     {
                        new ImportRegistrationId(typeof(string), 0, "a"),
                        new ImportRegistrationId(typeof(int), 0, "a"),
                        new ImportRegistrationId(typeof(string), 1, "a"),
                        new ImportRegistrationId(typeof(string), 0, "b"),
                        new ImportRegistrationId(typeof(string), 0, typeof(int)),
                        new ImportRegistrationId(typeof(string), 0, typeof(double)),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
