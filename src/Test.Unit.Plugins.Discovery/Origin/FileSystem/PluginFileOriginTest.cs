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

namespace Nuclei.Plugins.Discovery.Origin.FileSystem
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PluginFileOriginTest : EqualityContractVerifierTest
    {
        private readonly PluginFileOriginHashcodeContractVerfier _hashCodeVerifier = new PluginFileOriginHashcodeContractVerfier();

        private readonly PluginFileOriginEqualityContractVerifier _equalityVerifier = new PluginFileOriginEqualityContractVerifier();

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
            PluginFileOrigin first = new PluginFileOrigin("a");
            PluginFileOrigin second = (PluginFileOrigin)first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new PluginFileOrigin("a");
            object second = (PluginFileOrigin)first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now + TimeSpan.FromHours(2), now);
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now, now + TimeSpan.FromHours(2));
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnPath()
        {
            var first = new PluginFileOrigin("b");
            var second = new PluginFileOrigin("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            PluginFileOrigin first = new PluginFileOrigin("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now - TimeSpan.FromHours(2), now);
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnModifiedTimed()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now, now - TimeSpan.FromHours(2));
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnPath()
        {
            var first = new PluginFileOrigin("a");
            var second = new PluginFileOrigin("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            PluginFileOrigin first = new PluginFileOrigin("a");
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }

        [Test]
        public void Create()
        {
            var path = "a";
            var now = DateTimeOffset.Now;
            var creation = now;
            var modified = now + TimeSpan.FromHours(2);
            var origin = new PluginFileOrigin(path, creation, modified);

            Assert.IsNotNull(origin);
            Assert.AreEqual(path, origin.FilePath);
            Assert.AreEqual(creation, origin.CreationTimeUtc);
            Assert.AreEqual(modified, origin.LastWriteTimeUtc);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Discovery.Origin.FileSystem.PluginFileOrigin",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithEmptyFilePath()
        {
            Assert.Throws<ArgumentException>(() => new PluginFileOrigin(string.Empty));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Discovery.Origin.FileSystem.PluginFileOrigin",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullFilePath()
        {
            Assert.Throws<ArgumentException>(() => new PluginFileOrigin(null));
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            PluginFileOrigin first = null;
            PluginFileOrigin second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new PluginFileOrigin("a");
            var second = (PluginFileOrigin)first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now + TimeSpan.FromHours(2), now);
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now, now + TimeSpan.FromHours(2));
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnPath()
        {
            var first = new PluginFileOrigin("b");
            var second = new PluginFileOrigin("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            PluginFileOrigin first = null;
            PluginFileOrigin second = new PluginFileOrigin("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now - TimeSpan.FromHours(2), now);
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now, now - TimeSpan.FromHours(2));
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnPath()
        {
            var first = new PluginFileOrigin("a");
            var second = new PluginFileOrigin("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            PluginFileOrigin first = new PluginFileOrigin("a");
            PluginFileOrigin second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            PluginFileOrigin first = null;
            PluginFileOrigin second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new PluginFileOrigin("a");
            var second = (PluginFileOrigin)first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now + TimeSpan.FromHours(2), now);
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now, now + TimeSpan.FromHours(2));
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnPath()
        {
            var first = new PluginFileOrigin("b");
            var second = new PluginFileOrigin("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            PluginFileOrigin first = null;
            PluginFileOrigin second = new PluginFileOrigin("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now - TimeSpan.FromHours(2), now);
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginFileOrigin("a", now, now - TimeSpan.FromHours(2));
            var second = new PluginFileOrigin("a", now, now);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnPath()
        {
            var first = new PluginFileOrigin("a");
            var second = new PluginFileOrigin("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            PluginFileOrigin first = new PluginFileOrigin("a");
            PluginFileOrigin second = null;

            Assert.IsFalse(first < second);
        }

        private sealed class PluginFileOriginEqualityContractVerifier : EqualityContractVerifier<PluginFileOrigin>
        {
            private readonly PluginFileOrigin _first = new PluginFileOrigin("a");

            private readonly PluginFileOrigin _second = new PluginFileOrigin("b");

            protected override PluginFileOrigin Copy(PluginFileOrigin original)
            {
                return (PluginFileOrigin)original.Clone();
            }

            protected override PluginFileOrigin FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PluginFileOrigin SecondInstance
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

        private sealed class PluginFileOriginHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PluginFileOrigin> _distinctInstances
                = new List<PluginFileOrigin>
                     {
                        new PluginFileOrigin("a"),
                        new PluginFileOrigin("b"),
                        new PluginFileOrigin("c"),
                        new PluginFileOrigin("d"),
                        new PluginFileOrigin("e"),
                        new PluginFileOrigin("f"),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
