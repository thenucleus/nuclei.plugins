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
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NUnit.Framework;

namespace Nuclei.Plugins.Discovery.NuGet
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PluginNuGetOriginTest : EqualityContractVerifierTest
    {
        private readonly PluginNuGetOriginHashcodeContractVerfier _hashCodeVerifier = new PluginNuGetOriginHashcodeContractVerfier();

        private readonly PluginNuGetOriginEqualityContractVerifier _equalityVerifier = new PluginNuGetOriginEqualityContractVerifier();

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
            PluginNuGetOrigin first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            PluginNuGetOrigin second = (PluginNuGetOrigin)first.Clone();

            Assert.AreEqual(first, second);
        }

        [Test]
        public void CompareToOperatorWithEqualObjects()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            object second = (PluginNuGetOrigin)first.Clone();

            Assert.AreEqual(0, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnId()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithLargerFirstObjectBasedOnVersion()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(4, 5, 6)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [Test]
        public void CompareToWithNullObject()
        {
            PluginNuGetOrigin first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            object second = null;

            Assert.AreEqual(1, first.CompareTo(second));
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnId()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(1, 2, 3)));

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithSmallerFirstObjectBasedOnVersion()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(4, 5, 6)));

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [Test]
        public void CompareToWithUnequalObjectTypes()
        {
            PluginNuGetOrigin first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            object second = new object();

            Assert.Throws<ArgumentException>(() => first.CompareTo(second));
        }

        [Test]
        public void Create()
        {
            var identity = new PackageIdentity("a", new NuGetVersion(1, 2, 3));
            var origin = new PluginNuGetOrigin(identity);

            Assert.IsNotNull(origin);
            Assert.AreEqual(identity, origin.Identity);
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Discovery.NuGet.PluginNuGetOrigin",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullIdentity()
        {
            Assert.Throws<ArgumentNullException>(() => new PluginNuGetOrigin(null));
        }

        [Test]
        public void LargerThanOperatorWithBothObjectsNull()
        {
            PluginNuGetOrigin first = null;
            PluginNuGetOrigin second = null;

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithEqualObjects()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = (PluginNuGetOrigin)first.Clone();

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnId()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectLargerBasedOnVersion()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(4, 5, 6)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsTrue(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectNull()
        {
            PluginNuGetOrigin first = null;
            PluginNuGetOrigin second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnId()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(1, 2, 3)));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithFirstObjectSmallerBasedOnVersion()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(4, 5, 6)));

            Assert.IsFalse(first > second);
        }

        [Test]
        public void LargerThanOperatorWithSecondObjectNull()
        {
            PluginNuGetOrigin first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            PluginNuGetOrigin second = null;

            Assert.IsTrue(first > second);
        }

        [Test]
        public void SmallerThanOperatorWithBothObjectsNull()
        {
            PluginNuGetOrigin first = null;
            PluginNuGetOrigin second = null;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithEqualObjects()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = (PluginNuGetOrigin)first.Clone();

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnId()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectLargerBasedOnVersion()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(4, 5, 6)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsFalse(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectNull()
        {
            PluginNuGetOrigin first = null;
            PluginNuGetOrigin second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnId()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(1, 2, 3)));

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithFirstObjectSmallerBasedOnVersion()
        {
            var first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            var second = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(4, 5, 6)));

            Assert.IsTrue(first < second);
        }

        [Test]
        public void SmallerThanOperatorWithSecondObjectNull()
        {
            PluginNuGetOrigin first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));
            PluginNuGetOrigin second = null;

            Assert.IsFalse(first < second);
        }

        private sealed class PluginNuGetOriginEqualityContractVerifier : EqualityContractVerifier<PluginNuGetOrigin>
        {
            private readonly PluginNuGetOrigin _first = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            private readonly PluginNuGetOrigin _second = new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(4, 5, 6)));

            protected override PluginNuGetOrigin Copy(PluginNuGetOrigin original)
            {
                return (PluginNuGetOrigin)original.Clone();
            }

            protected override PluginNuGetOrigin FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override PluginNuGetOrigin SecondInstance
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

        private sealed class PluginNuGetOriginHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<PluginNuGetOrigin> _distinctInstances
                = new List<PluginNuGetOrigin>
                     {
                        new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3))),
                        new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(4, 5, 6))),
                        new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(1, 2, 3))),
                        new PluginNuGetOrigin(new PackageIdentity("b", new NuGetVersion(4, 5, 6))),
                        new PluginNuGetOrigin(new PackageIdentity("c", new NuGetVersion(1, 2, 3))),
                        new PluginNuGetOrigin(new PackageIdentity("c", new NuGetVersion(4, 5, 6))),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
