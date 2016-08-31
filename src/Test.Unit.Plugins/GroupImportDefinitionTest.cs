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

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class GroupImportDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<GroupImportDefinition>
        {
            private readonly GroupImportDefinition m_First = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            private readonly GroupImportDefinition m_Second = GroupImportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("d"),
                null,
                Enumerable.Empty<ImportRegistrationId>());

            protected override GroupImportDefinition Copy(GroupImportDefinition original)
            {
                if (original.ContractName.Equals("a"))
                {
                    return GroupImportDefinition.CreateDefinition(
                        "a",
                        new GroupRegistrationId("b"),
                        null,
                        Enumerable.Empty<ImportRegistrationId>());
                }

                return GroupImportDefinition.CreateDefinition(
                    "c",
                    new GroupRegistrationId("d"),
                    null,
                    Enumerable.Empty<ImportRegistrationId>());
            }

            protected override GroupImportDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override GroupImportDefinition SecondInstance
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
            private readonly IEnumerable<GroupImportDefinition> m_DistinctInstances
                = new List<GroupImportDefinition> 
                     {
                        GroupImportDefinition.CreateDefinition(
                            "a", 
                            new GroupRegistrationId("b"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        GroupImportDefinition.CreateDefinition(
                            "c", 
                            new GroupRegistrationId("d"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        GroupImportDefinition.CreateDefinition(
                            "e", 
                            new GroupRegistrationId("f"), 
                            null, 
                            Enumerable.Empty<ImportRegistrationId>()),
                        GroupImportDefinition.CreateDefinition(
                            "g", 
                            new GroupRegistrationId("h"), 
                            new InsertVertex(0, 1), 
                            Enumerable.Empty<ImportRegistrationId>()),
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
        public void RoundtripSerialize()
        {
            var original = GroupImportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                null,
                new List<ImportRegistrationId> { new ImportRegistrationId(typeof(string), 0, "a") });
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var groupId = new GroupRegistrationId("a");
            var contractName = "b";
            var vertex = new InsertVertex(0, 1);
            var imports = new List<ImportRegistrationId> { new ImportRegistrationId(typeof(string), 0, "a") };
            var obj = GroupImportDefinition.CreateDefinition(contractName, groupId, vertex, imports);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(contractName, obj.ContractName);
            Assert.AreEqual(vertex, obj.ScheduleInsertPosition);
            Assert.That(obj.ImportsToMatch, Is.EquivalentTo(imports));
        }
    }
}
