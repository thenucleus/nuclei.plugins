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

namespace Nuclei.Plugins.Core.Assembly
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PluginAssemblyOriginTest : EqualityContractVerifierTest
    {
        private readonly PluginAssemblyOriginHashcodeContractVerfier _hashCodeVerifier = new PluginAssemblyOriginHashcodeContractVerfier();

        private readonly PluginAssemblyOriginEqualityContractVerifier _equalityVerifier = new PluginAssemblyOriginEqualityContractVerifier();

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
            PluginAssemblyOrigin first = new PluginAssemblyOrigin("a");
            PluginAssemblyOrigin second = (PluginAssemblyOrigin)first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new PluginAssemblyOrigin("a");
            object second = (PluginAssemblyOrigin)first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now + TimeSpan.FromHours(2), now);
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now, now + TimeSpan.FromHours(2));
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnPath()
        {
            var first = new PluginAssemblyOrigin("b");
            var second = new PluginAssemblyOrigin("a");

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            PluginAssemblyOrigin first = new PluginAssemblyOrigin("a");
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now - TimeSpan.FromHours(2), now);
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnModifiedTimed()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now, now - TimeSpan.FromHours(2));
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnPath()
        {
            var first = new PluginAssemblyOrigin("a");
            var second = new PluginAssemblyOrigin("b");

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            PluginAssemblyOrigin first = new PluginAssemblyOrigin("a");
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
            var origin = new PluginAssemblyOrigin(path, creation, modified);

            Assert.IsNotNull(origin);
            Assert.AreEqual(path, origin.FilePath);
            Assert.AreEqual(creation, origin.CreationTimeUtc);
            Assert.AreEqual(modified, origin.LastWriteTimeUtc);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Discovery.Assembly.PluginAssemblyOrigin",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithEmptyFilePath()
        {
            Assert.Throws<ArgumentException>(() => new PluginAssemblyOrigin(string.Empty));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Discovery.Assembly.PluginAssemblyOrigin",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullFilePath()
        {
            Assert.Throws<ArgumentNullException>(() => new PluginAssemblyOrigin(null));
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            PluginAssemblyOrigin first = null;
            PluginAssemblyOrigin second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new PluginAssemblyOrigin("a");
            var second = (PluginAssemblyOrigin)first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now + TimeSpan.FromHours(2), now);
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now, now + TimeSpan.FromHours(2));
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnPath()
        {
            var first = new PluginAssemblyOrigin("b");
            var second = new PluginAssemblyOrigin("a");

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            PluginAssemblyOrigin first = null;
            PluginAssemblyOrigin second = new PluginAssemblyOrigin("a");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now - TimeSpan.FromHours(2), now);
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now, now - TimeSpan.FromHours(2));
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnPath()
        {
            var first = new PluginAssemblyOrigin("a");
            var second = new PluginAssemblyOrigin("b");

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            PluginAssemblyOrigin first = new PluginAssemblyOrigin("a");
            PluginAssemblyOrigin second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            PluginAssemblyOrigin first = null;
            PluginAssemblyOrigin second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new PluginAssemblyOrigin("a");
            var second = (PluginAssemblyOrigin)first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now + TimeSpan.FromHours(2), now);
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now, now + TimeSpan.FromHours(2));
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnPath()
        {
            var first = new PluginAssemblyOrigin("b");
            var second = new PluginAssemblyOrigin("a");

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            PluginAssemblyOrigin first = null;
            PluginAssemblyOrigin second = new PluginAssemblyOrigin("a");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnCreationTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now - TimeSpan.FromHours(2), now);
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnModifiedTime()
        {
            var now = DateTimeOffset.Now;
            var first = new PluginAssemblyOrigin("a", now, now - TimeSpan.FromHours(2));
            var second = new PluginAssemblyOrigin("a", now, now);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnPath()
        {
            var first = new PluginAssemblyOrigin("a");
            var second = new PluginAssemblyOrigin("b");

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            PluginAssemblyOrigin first = new PluginAssemblyOrigin("a");
            PluginAssemblyOrigin second = null;

            Assert.IsFalse(first < second);
        }

        private sealed class PluginAssemblyOriginEqualityContractVerifier : EqualityContractVerifier<PluginAssemblyOrigin>
        {
            private readonly PluginAssemblyOrigin _first = new PluginAssemblyOrigin("a");

            private readonly PluginAssemblyOrigin _second = new PluginAssemblyOrigin("b");

            protected override PluginAssemblyOrigin Copy(PluginAssemblyOrigin original)
            {
                return (PluginAssemblyOrigin)original.Clone();
            }

            protected override PluginAssemblyOrigin FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PluginAssemblyOrigin SecondInstance
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

        private sealed class PluginAssemblyOriginHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PluginAssemblyOrigin> _distinctInstances
                = new List<PluginAssemblyOrigin>
                     {
                        new PluginAssemblyOrigin("a"),
                        new PluginAssemblyOrigin("b"),
                        new PluginAssemblyOrigin("c"),
                        new PluginAssemblyOrigin("d"),
                        new PluginAssemblyOrigin("e"),
                        new PluginAssemblyOrigin("f"),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
