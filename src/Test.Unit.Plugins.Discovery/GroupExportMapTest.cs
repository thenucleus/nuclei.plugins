//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class GroupExportMapTest : EqualityContractVerifierTest
    {
        private sealed class GroupExportMapEqualityContractVerifier : EqualityContractVerifier<GroupExportMap>
        {
            private readonly GroupExportMap m_First = new GroupExportMap("a");

            private readonly GroupExportMap m_Second = new GroupExportMap("b");

            protected override GroupExportMap Copy(GroupExportMap original)
            {
                return new GroupExportMap(original.ContractName);
            }

            protected override GroupExportMap FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override GroupExportMap SecondInstance
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

        private sealed class GroupExportMapHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<GroupExportMap> m_DistinctInstances
                = new List<GroupExportMap> 
                     {
                        new GroupExportMap("a"),
                        new GroupExportMap("b"),
                        new GroupExportMap("c"),
                        new GroupExportMap("d"),
                        new GroupExportMap("e"),
                        new GroupExportMap("f"),
                        new GroupExportMap("g"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly GroupExportMapHashcodeContractVerfier m_HashcodeVerifier = new GroupExportMapHashcodeContractVerfier();

        private readonly GroupExportMapEqualityContractVerifier m_EqualityVerifier = new GroupExportMapEqualityContractVerifier();

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
        public void RoundtripSerialize()
        {
            var original = new GroupExportMap("a");
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = new GroupExportMap("a");

            Assert.AreEqual("a", obj.ContractName);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new GroupExportMap("a");
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new GroupExportMap("a");
            object second = new GroupExportMap("a");

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new GroupExportMap("a");
            object second = new GroupExportMap("b");

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new GroupExportMap("a");
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
