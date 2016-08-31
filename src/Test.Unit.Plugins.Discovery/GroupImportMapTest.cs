//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class GroupImportMapTest : EqualityContractVerifierTest
    {
        private sealed class GroupImportMapEqualityContractVerifier : EqualityContractVerifier<GroupImportMap>
        {
            private readonly GroupImportMap m_First = new GroupImportMap("a");

            private readonly GroupImportMap m_Second = new GroupImportMap("b", new InsertVertex(1));

            protected override GroupImportMap Copy(GroupImportMap original)
            {
                return new GroupImportMap(original.ContractName, original.InsertPoint, original.ObjectImports);
            }

            protected override GroupImportMap FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override GroupImportMap SecondInstance
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

        private sealed class GroupImportMapHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<GroupImportMap> m_DistinctInstances
                = new List<GroupImportMap> 
                     {
                        new GroupImportMap("a"),
                        new GroupImportMap("b", new InsertVertex(1)),
                        new GroupImportMap(
                            "c", 
                            objectImports: new List<ImportRegistrationId> 
                                { 
                                    new ImportRegistrationId(typeof(string), 0, "aa") 
                                }),
                        new GroupImportMap(
                            "d", 
                            new InsertVertex(1),
                            new List<ImportRegistrationId> 
                                { 
                                    new ImportRegistrationId(typeof(string), 0, "aa") 
                                }),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly GroupImportMapHashcodeContractVerfier m_HashcodeVerifier = new GroupImportMapHashcodeContractVerfier();

        private readonly GroupImportMapEqualityContractVerifier m_EqualityVerifier = new GroupImportMapEqualityContractVerifier();

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
            var original = new GroupImportMap(
                "c",
                new InsertVertex(1),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    });
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void EqualsOperatorWithFirstObjectNull()
        {
            GroupImportMap first = null;
            var second = new GroupImportMap(
                "c",
                new InsertVertex(1),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    });

            Assert.IsFalse(first == second);
        }

        [Test]
        public void Create()
        {
            var name = "a";
            var insertVertex = new InsertVertex(1);
            var imports = new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    };
            var obj = new GroupImportMap(name, insertVertex, imports);

            Assert.AreEqual(name, obj.ContractName);
            Assert.AreEqual(insertVertex, obj.InsertPoint);
            Assert.That(obj.ObjectImports, Is.EquivalentTo(imports));
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var first = new GroupImportMap(
                "c",
                new InsertVertex(1),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    });
            object second = null;

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithEqualObjects()
        {
            var first = new GroupImportMap(
                "c",
                new InsertVertex(1),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    });
            object second = new GroupImportMap(
                "c",
                new InsertVertex(2),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 1, "aa") 
                    });

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjects()
        {
            var first = new GroupImportMap(
                "c",
                new InsertVertex(1),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    });
            object second = new GroupImportMap(
                "d",
                new InsertVertex(1),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    });

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void EqualsWithUnequalObjectTypes()
        {
            var first = new GroupImportMap(
                "c",
                new InsertVertex(1),
                new List<ImportRegistrationId> 
                    { 
                        new ImportRegistrationId(typeof(string), 0, "aa") 
                    });
            var second = new object();

            Assert.IsFalse(first.Equals(second));
        }
    }
}
