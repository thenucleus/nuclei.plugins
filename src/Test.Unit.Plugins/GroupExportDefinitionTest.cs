//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Extensions.Plugins;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class GroupExportDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class GroupExportDefinitionEqualityContractVerifier : EqualityContractVerifier<GroupExportDefinition>
        {
            private readonly GroupExportDefinition m_First = GroupExportDefinition.CreateDefinition(
                "a",
                new GroupRegistrationId("b"),
                Enumerable.Empty<ExportRegistrationId>());

            private readonly GroupExportDefinition m_Second = GroupExportDefinition.CreateDefinition(
                "c",
                new GroupRegistrationId("d"),
                Enumerable.Empty<ExportRegistrationId>());

            protected override GroupExportDefinition Copy(GroupExportDefinition original)
            {
                return GroupExportDefinition.CreateDefinition(original.ContractName, original.ContainingGroup, original.ProvidedExports);
            }

            protected override GroupExportDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override GroupExportDefinition SecondInstance
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

        private sealed class GroupExportDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<GroupExportDefinition> m_DistinctInstances
                = new List<GroupExportDefinition> 
                     {
                        GroupExportDefinition.CreateDefinition(
                            "a", 
                            new GroupRegistrationId("b"), 
                            Enumerable.Empty<ExportRegistrationId>()),
                        GroupExportDefinition.CreateDefinition(
                            "c", 
                            new GroupRegistrationId("d"), 
                            Enumerable.Empty<ExportRegistrationId>()),
                        GroupExportDefinition.CreateDefinition(
                            "e", 
                            new GroupRegistrationId("f"), 
                            Enumerable.Empty<ExportRegistrationId>()),
                        GroupExportDefinition.CreateDefinition(
                            "g", 
                            new GroupRegistrationId("h"), 
                            Enumerable.Empty<ExportRegistrationId>()),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly GroupExportDefinitionHashcodeContractVerfier m_HashcodeVerifier = new GroupExportDefinitionHashcodeContractVerfier();

        private readonly GroupExportDefinitionEqualityContractVerifier m_EqualityVerifier = new GroupExportDefinitionEqualityContractVerifier();

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
            var original = GroupExportDefinition.CreateDefinition(
                "b",
                new GroupRegistrationId("a"),
                new List<ExportRegistrationId> { new ExportRegistrationId(typeof(string), 1, "a") });
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var groupId = new GroupRegistrationId("a");
            var contractName = "b";
            var imports = new List<ExportRegistrationId> { new ExportRegistrationId(typeof(string), 0, "a") };
            var obj = GroupExportDefinition.CreateDefinition(contractName, groupId, imports);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(contractName, obj.ContractName);
            Assert.That(obj.ProvidedExports, Is.EquivalentTo(imports));
        }
    }
}
