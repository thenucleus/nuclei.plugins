//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;
using QuickGraph;

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class ScheduleDefinitionEqualityContractVerifier : EqualityContractVerifier<ScheduleDefinition>
        {
            private readonly ScheduleDefinition m_First = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            private readonly ScheduleDefinition m_Second = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("b"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());

            protected override ScheduleDefinition Copy(ScheduleDefinition original)
            {
                if (original.ContainingGroup.Equals(new GroupRegistrationId("a")))
                {
                    return ScheduleDefinition.CreateDefinition(
                        new GroupRegistrationId("a"),
                        BuildSchedule(),
                        new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                        new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
                }

                return ScheduleDefinition.CreateDefinition(
                    new GroupRegistrationId("b"),
                    BuildSchedule(),
                    new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                    new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            }

            protected override ScheduleDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override ScheduleDefinition SecondInstance
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

        private sealed class ScheduleDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ScheduleDefinition> m_DistinctInstances
                = new List<ScheduleDefinition> 
                     {
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("a"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("b"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("c"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                        ScheduleDefinition.CreateDefinition(
                            new GroupRegistrationId("d"), 
                            BuildSchedule(),
                            new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(), 
                            new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>()),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ScheduleDefinitionHashcodeContractVerfier m_HashcodeVerifier = new ScheduleDefinitionHashcodeContractVerfier();

        private readonly ScheduleDefinitionEqualityContractVerifier m_EqualityVerifier = new ScheduleDefinitionEqualityContractVerifier();

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

        private static ISchedule BuildSchedule()
        {
            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);
            graph.AddEdge(new ScheduleEdge(start, end));

            return new Schedule(graph, start, end);
        }

        [Test]
        public void RoundtripSerialize()
        {
            var original = ScheduleDefinition.CreateDefinition(
                new GroupRegistrationId("a"),
                BuildSchedule(),
                new Dictionary<ScheduleElementId, ScheduleActionRegistrationId>(),
                new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId>());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var groupId = new GroupRegistrationId("a");
            var schedule = BuildSchedule();
            var actions = new Dictionary<ScheduleElementId, ScheduleActionRegistrationId> 
                {
                    { new ScheduleElementId(), new ScheduleActionRegistrationId(typeof(string), 0, "a") }
                };
            var conditions = new Dictionary<ScheduleElementId, ScheduleConditionRegistrationId> 
                { 
                    { new ScheduleElementId(), new ScheduleConditionRegistrationId(typeof(string), 0, "a") }
                };
            var obj = ScheduleDefinition.CreateDefinition(
                groupId,
                schedule,
                actions,
                conditions);

            Assert.AreEqual(groupId, obj.ContainingGroup);
            Assert.AreEqual(schedule, obj.Schedule);
            Assert.That(obj.Actions, Is.EquivalentTo(actions));
            Assert.That(obj.Conditions, Is.EquivalentTo(conditions));
        }
    }
}
