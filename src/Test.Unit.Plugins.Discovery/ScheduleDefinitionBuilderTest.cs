//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Apollo.Core.Base.Scheduling;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;
using Moq;
using NUnit.Framework;
using QuickGraph;

namespace Nuclei.Plugins.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ScheduleDefinitionBuilderTest
    {
        [Test]
        public void AddAction()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            ScheduleElementId id = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddExecutingAction(It.IsAny<ScheduleElementId>()))
                    .Callback<ScheduleElementId>(s => id = s)
                    .Returns<ScheduleElementId>(s => new ExecutingActionVertex(0, s));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var registrationId = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var vertex = builder.AddExecutingAction(registrationId);

            Assert.IsNotNull(vertex);
            Assert.IsNotNull(id);
        }

        [Test]
        public void AddActionWithExistingAction()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            ScheduleElementId id = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddExecutingAction(It.IsAny<ScheduleElementId>()))
                    .Callback<ScheduleElementId>(s => id = s)
                    .Returns<ScheduleElementId>(s => new ExecutingActionVertex(0, s));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var firstRegistration = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var secondRegistration = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var firstVertex = builder.AddExecutingAction(firstRegistration);

            var first = id;
            id = null;
            var secondVertex = builder.AddExecutingAction(secondRegistration);
            Assert.IsNotNull(firstVertex);
            Assert.IsNotNull(secondVertex);
            Assert.AreEqual(first, id);
        }

        [Test]
        public void AddSubSchedule()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            var schedule = new ScheduleId();
            ScheduleId storedId = null;
            var scheduleVertex = new SubScheduleVertex(0, schedule);
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddSubSchedule(It.IsAny<ScheduleId>()))
                    .Callback<ScheduleId>(s => storedId = s)
                    .Returns(scheduleVertex);
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);
            var vertex = builder.AddSubSchedule(schedule);

            Assert.AreSame(scheduleVertex, vertex);
            Assert.AreSame(schedule, storedId);
        }

        [Test]
        public void AddSynchronizationStartPoint()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            IEnumerable<IScheduleVariable> variables = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddSynchronizationStart(It.IsAny<IEnumerable<IScheduleVariable>>()))
                    .Callback<IEnumerable<IScheduleVariable>>(s => variables = s)
                    .Returns<IEnumerable<IScheduleVariable>>(s => new SynchronizationStartVertex(0, s));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var synchronizationVariables = new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object };
            var vertex = builder.AddSynchronizationStart(synchronizationVariables);

            Assert.IsNotNull(vertex);
            Assert.That(variables, Is.EquivalentTo(synchronizationVariables));
        }

        [Test]
        public void AddSynchronizationEndPoint()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            SynchronizationStartVertex startVertex = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddSynchronizationEnd(It.IsAny<SynchronizationStartVertex>()))
                    .Callback<SynchronizationStartVertex>(s => startVertex = s)
                    .Returns<SynchronizationStartVertex>(s => new SynchronizationEndVertex(0));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var inputVertex = new SynchronizationStartVertex(
                0, 
                new List<IScheduleVariable> { new Mock<IScheduleVariable>().Object });
            var vertex = builder.AddSynchronizationEnd(inputVertex);

            Assert.IsNotNull(vertex);
            Assert.AreSame(inputVertex, startVertex);
        }

        [Test]
        public void AddHistoryMarkingPoint()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            MarkHistoryVertex startVertex = new MarkHistoryVertex(0);
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddHistoryMarkingPoint())
                    .Returns(startVertex);
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);
            var vertex = builder.AddHistoryMarkingPoint();

            Assert.AreSame(startVertex, vertex);
        }

        [Test]
        public void AddInsertPoint()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            var insertVertex = new InsertVertex(0);
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddInsertPoint())
                    .Returns(insertVertex);
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);
            var vertex = builder.AddInsertPoint();

            Assert.AreSame(insertVertex, vertex);
        }

        [Test]
        public void AddInsertPointWithCount()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.AddInsertPoint(It.IsAny<int>()))
                    .Returns<int>(i => new InsertVertex(0, i));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);
            var vertex = builder.AddInsertPoint(10);
            Assert.AreEqual(10, vertex.RemainingInserts);
        }

        [Test]
        public void LinkToWithoutCondition()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(
                        s => s.LinkTo(
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<ScheduleElementId>()))
                    .Callback<IScheduleVertex, IScheduleVertex, ScheduleElementId>(
                        (s, e, c) => Assert.IsNull(c));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var start = new MarkHistoryVertex(0);
            var end = new InsertVertex(1);
            builder.LinkTo(start, end);
        }

        [Test]
        public void LinkToWithCondition()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            ScheduleElementId id = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(
                        s => s.LinkTo(
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<ScheduleElementId>()))
                    .Callback<IScheduleVertex, IScheduleVertex, ScheduleElementId>(
                        (s, e, c) => id = c);
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var start = new MarkHistoryVertex(0);
            var end = new InsertVertex(1);
            var condition = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            builder.LinkTo(start, end, condition);

            Assert.IsNotNull(id);
        }

        [Test]
        public void LinkToWithExistingCondition()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            ScheduleElementId id = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(
                        s => s.LinkTo(
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<ScheduleElementId>()))
                    .Callback<IScheduleVertex, IScheduleVertex, ScheduleElementId>(
                        (s, e, c) => id = c);
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var condition = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            builder.LinkTo(new MarkHistoryVertex(0), new InsertVertex(1), condition);
            
            Assert.IsNotNull(id);
            
            var firstId = id;
            builder.LinkTo(new MarkHistoryVertex(2), new InsertVertex(3), condition);

            Assert.IsNotNull(id);
            Assert.AreSame(firstId, id);
        }

        [Test]
        public void LinkFromStartWithoutCondition()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(
                        s => s.LinkFromStart(
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<ScheduleElementId>()))
                    .Callback<IScheduleVertex, ScheduleElementId>(
                        (e, c) => Assert.IsNull(c));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);
            builder.LinkFromStart(new MarkHistoryVertex(0));
        }

        [Test]
        public void LinkFromStartWithCondition()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            ScheduleElementId id = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(
                        s => s.LinkFromStart(
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<ScheduleElementId>()))
                    .Callback<IScheduleVertex, ScheduleElementId>(
                        (e, c) => id = c);
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var condition = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            builder.LinkFromStart(new InsertVertex(1), condition);

            Assert.IsNotNull(id);
        }

        [Test]
        public void LinkToEndWithoutCondition()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(
                        s => s.LinkToEnd(
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<ScheduleElementId>()))
                    .Callback<IScheduleVertex, ScheduleElementId>(
                        (s, c) => Assert.IsNull(c));
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);
            builder.LinkToEnd(new MarkHistoryVertex(0));
        }

        [Test]
        public void LinkToEndWithCondition()
        {
            var owner = new Mock<IOwnScheduleDefinitions>();

            ScheduleElementId id = null;
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(
                        s => s.LinkToEnd(
                            It.IsAny<IScheduleVertex>(),
                            It.IsAny<ScheduleElementId>()))
                    .Callback<IScheduleVertex, ScheduleElementId>(
                        (s, c) => id = c);
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);

            var condition = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            builder.LinkToEnd(new InsertVertex(1), condition);

            Assert.IsNotNull(id);
        }

        [Test]
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2100:CodeLineMustNotBeLongerThan",
            Justification = "Either it is a long line or we get complaints that the opening bracket should be on the same line as the method name")]
        public void Register()
        {
            var graph = new BidirectionalGraph<IScheduleVertex, ScheduleEdge>();

            var start = new StartVertex(1);
            graph.AddVertex(start);

            var end = new EndVertex(2);
            graph.AddVertex(end);
            graph.AddEdge(new ScheduleEdge(start, end));

            var schedule = new Schedule(graph, start, end);
            var scheduleBuilder = new Mock<IBuildFixedSchedules>();
            {
                scheduleBuilder.Setup(s => s.Build())
                    .Returns(schedule);
                scheduleBuilder.Setup(s => s.AddExecutingAction(It.IsAny<ScheduleElementId>()))
                    .Returns<ScheduleElementId>(s => new ExecutingActionVertex(0, s));
            }

            var actionId = new ScheduleActionRegistrationId(typeof(string), 0, "a");
            var conditionId = new ScheduleConditionRegistrationId(typeof(string), 0, "a");
            var owner = new Mock<IOwnScheduleDefinitions>();
            {
                owner.Setup(
                    o => o.StoreSchedule(
                        It.IsAny<ISchedule>(),
                        It.IsAny<Dictionary<ScheduleActionRegistrationId, ScheduleElementId>>(),
                        It.IsAny<Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>>()))
                    .Callback<ISchedule, Dictionary<ScheduleActionRegistrationId, ScheduleElementId>, Dictionary<ScheduleConditionRegistrationId, ScheduleElementId>>(
                        (s, a, c) => 
                        {
                            Assert.That(a.Keys, Is.EquivalentTo(new List<ScheduleActionRegistrationId> { actionId }));
                            Assert.That(c.Keys, Is.EquivalentTo(new List<ScheduleConditionRegistrationId> { conditionId }));
                        });
            }

            var builder = new ScheduleDefinitionBuilder(owner.Object, scheduleBuilder.Object);
            var vertex = builder.AddExecutingAction(actionId);
            builder.LinkToEnd(vertex, conditionId);
            builder.Register();
        }
    }
}
