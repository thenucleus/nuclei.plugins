//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using NUnit.Framework;
using Test.Mocks;

namespace Nuclei.Plugins.Instantiation
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class InstanceLayerTest
    {
        // ExportOnType - PartContract1
        private static GroupPartDefinition CreateTypeExportingDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ExportOnType)),
                0,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>
                    {
                        { 
                            new ExportRegistrationId(typeof(ExportOnType), 0, "PartContract1"),
                            TypeBasedExportDefinition.CreateDefinition("PartContract1", typeof(ExportOnType))
                        }
                    },
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ExportOnProperty - PartContract2
        private static GroupPartDefinition CreatePropertyExportingDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ExportOnProperty)),
                1,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition> 
                    { 
                        {
                            new ExportRegistrationId(typeof(ExportOnProperty), 1, "PartContract2"),
                            PropertyBasedExportDefinition.CreateDefinition(
                                "PartContract2",
                                typeof(ExportOnProperty).GetProperty("ExportingProperty"))
                        }
                    },
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ExportOnMethod - PartContract3
        private static GroupPartDefinition CreateMethodExportingDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ExportOnMethod)),
                2,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition> 
                    { 
                        {
                            new ExportRegistrationId(typeof(ExportOnMethod), 2, "PartContract3"),
                            MethodBasedExportDefinition.CreateDefinition(
                                "PartContract3",
                                typeof(ExportOnMethod).GetMethod("ExportingMethod"))
                        }
                    },
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ExportOnMultiParameterMethod - PartContract4
        private static GroupPartDefinition CreateMultiParameterMethodExportingDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ExportOnMethodWithMultipleParameters)),
                3,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition> 
                    { 
                        {
                            new ExportRegistrationId(typeof(ExportOnMethodWithMultipleParameters), 3, "PartContract4"),
                            MethodBasedExportDefinition.CreateDefinition(
                                "PartContract4",
                                typeof(ExportOnMethodWithMultipleParameters).GetMethod("ExportingMethod"))
                        }
                    },
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>(),
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnProperty - PartContract1
        private static GroupPartDefinition CreatePropertyImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnProperty)),
                1,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnProperty), 1, "PartContract1"),
                            PropertyBasedImportDefinition.CreateDefinition(
                                "PartContract1", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                false,
                                CreationPolicy.Any,
                                typeof(ImportOnProperty).GetProperty("ImportingProperty"))
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnPropertyWithLazy - PartContract2
        private static GroupPartDefinition CreateLazyPropertyImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithLazy)),
                2,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnPropertyWithLazy), 2, "PartContract2"),
                            PropertyBasedImportDefinition.CreateDefinition(
                                "PartContract2", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                false,
                                CreationPolicy.Any,
                                typeof(ImportOnPropertyWithLazy).GetProperty("ImportingProperty"))
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnPropertyWithEnumerable - PartContract3
        private static GroupPartDefinition CreateEnumerablePropertyImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithEnumerable)),
                3,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnPropertyWithEnumerable), 3, "PartContract3"),
                            PropertyBasedImportDefinition.CreateDefinition(
                                "PartContract3", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                false,
                                CreationPolicy.Any,
                                typeof(ImportOnPropertyWithEnumerable).GetProperty("ImportingProperty"))
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnPropertyWithFunc - PartContract4
        private static GroupPartDefinition CreateFuncPropertyImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithFunc)),
                4,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnPropertyWithFunc), 4, "PartContract4"),
                            PropertyBasedImportDefinition.CreateDefinition(
                                "PartContract4", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                false,
                                CreationPolicy.Any,
                                typeof(ImportOnPropertyWithFunc).GetProperty("ImportingProperty"))
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnPropertyWithMultiParameterFunc - PartContract5
        private static GroupPartDefinition CreateMultiParameterFuncPropertyImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnPropertyWithFuncWithMultipleParameters)),
                5,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnPropertyWithFuncWithMultipleParameters), 5, "PartContract5"),
                            PropertyBasedImportDefinition.CreateDefinition(
                                "PartContract5", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                false,
                                CreationPolicy.Any,
                                typeof(ImportOnPropertyWithFuncWithMultipleParameters).GetProperty("ImportingProperty"))
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnConstructorWithType - PartContract6
        private static GroupPartDefinition CreateConstructorImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithType)),
                6,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnConstructorWithType), 6, "PartContract6"),
                            ConstructorBasedImportDefinition.CreateDefinition(
                                "PartContract6", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                CreationPolicy.Any,
                                typeof(ImportOnConstructorWithType).GetConstructor(
                                    new[] 
                                    { 
                                        typeof(IExportingInterface) 
                                    }).GetParameters().First())
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnConstructorWithLazy - PartContract7
        private static GroupPartDefinition CreateLazyConstructorImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithLazy)),
                7,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnConstructorWithLazy), 7, "PartContract7"),
                            ConstructorBasedImportDefinition.CreateDefinition(
                                "PartContract7", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                CreationPolicy.Any,
                                typeof(ImportOnConstructorWithLazy).GetConstructor(
                                    new[] 
                                    { 
                                        typeof(Lazy<IExportingInterface>) 
                                    }).GetParameters().First())
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnConstructorWithEnumerable - PartContract8
        private static GroupPartDefinition CreateEnumerableConstructorImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithEnumerable)),
                8,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnConstructorWithEnumerable), 8, "PartContract8"),
                            ConstructorBasedImportDefinition.CreateDefinition(
                                "PartContract8", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                CreationPolicy.Any,
                                typeof(ImportOnConstructorWithEnumerable).GetConstructor(
                                    new[] 
                                    { 
                                        typeof(IEnumerable<IExportingInterface>) 
                                    }).GetParameters().First())
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnConstructorWithFunc - PartContract9
        private static GroupPartDefinition CreateFuncConstructorImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithFunc)),
                9,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnConstructorWithFunc), 9, "PartContract9"),
                            ConstructorBasedImportDefinition.CreateDefinition(
                                "PartContract9", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                CreationPolicy.Any,
                                typeof(ImportOnConstructorWithFunc).GetConstructor(
                                    new[] 
                                    { 
                                        typeof(Func<IExportingInterface>) 
                                    }).GetParameters().First())
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnConstructorWithMultiParameterFunc - PartContract10
        private static GroupPartDefinition CreateMultiParameterFuncConstructorImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithFuncWithMultipleParameters)),
                10,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnConstructorWithFuncWithMultipleParameters), 10, "PartContract10"),
                            ConstructorBasedImportDefinition.CreateDefinition(
                                "PartContract10", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                CreationPolicy.Any,
                                typeof(ImportOnConstructorWithFuncWithMultipleParameters).GetConstructor(
                                    new[] 
                                    { 
                                        typeof(Func<IExportingInterface, bool, bool>) 
                                    }).GetParameters().First())
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnConstructorWithCollectionOfLazy - PartContract11
        private static GroupPartDefinition CreateEnumerableLazyConstructorImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithCollectionOfLazy)),
                11,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnConstructorWithCollectionOfLazy), 11, "PartContract11"),
                            ConstructorBasedImportDefinition.CreateDefinition(
                                "PartContract11", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                CreationPolicy.Any,
                                typeof(ImportOnConstructorWithCollectionOfLazy).GetConstructor(
                                    new[] 
                                    { 
                                        typeof(IEnumerable<Lazy<IExportingInterface>>) 
                                    }).GetParameters().First())
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        // ImportOnConstructorWithCollectionOfFunc - PartContract12
        private static GroupPartDefinition CreateEnumerableFuncConstructorImportDefinition()
        {
            return new GroupPartDefinition(
                TypeIdentity.CreateDefinition(typeof(ImportOnConstructorWithCollectionOfFunc)),
                12,
                new Dictionary<ExportRegistrationId, SerializableExportDefinition>(),
                new Dictionary<ImportRegistrationId, SerializableImportDefinition>
                    {
                        { 
                            new ImportRegistrationId(typeof(ImportOnConstructorWithCollectionOfFunc), 12, "PartContract12"),
                            ConstructorBasedImportDefinition.CreateDefinition(
                                "PartContract12", 
                                TypeIdentity.CreateDefinition(typeof(IExportingInterface)),
                                ImportCardinality.ExactlyOne,
                                CreationPolicy.Any,
                                typeof(ImportOnConstructorWithCollectionOfFunc).GetConstructor(
                                    new[] 
                                    { 
                                        typeof(IEnumerable<Func<IExportingInterface>>) 
                                    }).GetParameters().First())
                        }
                    },
                new Dictionary<ScheduleActionRegistrationId, ScheduleActionDefinition>(),
                new Dictionary<ScheduleConditionRegistrationId, ScheduleConditionDefinition>());
        }

        [Test]
        public void ConstructWithNullPartDefinition()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            Assert.Throws<ArgumentNullException>(
                () => layer.Construct(null, Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>()));
        }

        [Test]
        public void ConstructWithMissingConnections()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var definition = CreateConstructorImportDefinition();

            Assert.Throws<ConstructionOfPluginTypeFailedException>(
                () => layer.Construct(definition, Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>()));
        }

        [Test]
        public void ConstructWithoutConnections()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var definition = CreateTypeExportingDefinition();

            var id = layer.Construct(definition, Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(id);
            Assert.IsTrue(layer.HasInstanceFor(id));
        }

        [Test]
        public void ConstructSecondInstance()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var definition = CreateTypeExportingDefinition();

            var id = layer.Construct(definition, Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(id);
            Assert.IsTrue(layer.HasInstanceFor(id));

            var otherId = layer.Construct(definition, Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(otherId);
            Assert.IsTrue(layer.HasInstanceFor(otherId));
            Assert.AreNotEqual(id, otherId);
        }

        [Test]
        public void ConstructWithConstructorImports()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithPropertyImports()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreatePropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithLazyConstructorImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateLazyConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithLazyPropertyImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateLazyPropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithLazyConstructorImportAndPropertyExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreatePropertyExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateLazyConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithLazyPropertyImportAndPropertyExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreatePropertyExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateLazyPropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithLazyConstructorImportAndMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateLazyConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithLazyPropertyImportAndMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateLazyPropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorCollectionImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerableConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithPropertyCollectionImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerablePropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorCollectionImportAndPropertyExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreatePropertyExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerableConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithPropertyCollectionImportAndPropertyExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreatePropertyExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerablePropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorCollectionImportAndMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerableConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithPropertyCollectionImportAndMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerablePropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorWithLazyCollectionImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerableLazyConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorWithFuncCollectionImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateEnumerableFuncConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorWithParameterlessFunctionImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithPropertyWithParameterlessFunctionImportAndTypeExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncPropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorWithParameterlessFunctionImportAndPropertyExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreatePropertyExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithPropertyWithParameterlessFunctionImportAndPropertyExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreatePropertyExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncPropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorWithParameterlessFunctionImportAndMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithPropertyWithParameterlessFunctionImportAndMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncPropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);
        }

        [Test]
        public void ConstructWithConstructorWithParameterlessFunctionImportAndMultipleParameterMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMultiParameterMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateMultiParameterFuncConstructorImportDefinition();
            Assert.Throws<ConstructionOfPluginTypeFailedException>(() => layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    }));
        }

        [Test]
        public void ConstructWithPropertyWithParameterlessFunctionImportAndMultipleParameterMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMultiParameterMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateMultiParameterFuncPropertyImportDefinition();
            Assert.Throws<ConstructionOfPluginTypeFailedException>(() => layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    }));
        }

        [Test]
        public void ConstructWithConstructorWithParameterlessFunctionImportAndNonmatchingMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMultiParameterMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncConstructorImportDefinition();
            Assert.Throws<ConstructionOfPluginTypeFailedException>(
                () => layer.Construct(
                    importingDefinition,
                    new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                        { 
                            new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                                importingDefinition.RegisteredImports.First(),
                                exportingId,
                                exportingDefinition.RegisteredExports.First()),
                        }));
        }

        [Test]
        public void ConstructWithPropertyWithParameterlessFunctionImportAndNonmatchingMethodExport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateMultiParameterMethodExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateFuncPropertyImportDefinition();
            Assert.Throws<ConstructionOfPluginTypeFailedException>(
                () => layer.Construct(
                    importingDefinition,
                    new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                        { 
                            new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                                importingDefinition.RegisteredImports.First(),
                                exportingId,
                                exportingDefinition.RegisteredExports.First()),
                        }));
        }

        [Test]
        public void UpdateIfRequiredWithCurrentConnectionsOnConstructorImport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);

            var changes = layer.UpdateIfRequired(
                importingId,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });

            Assert.AreEqual(0, changes.Count());
            Assert.IsTrue(layer.HasInstanceFor(exportingId));
            Assert.IsTrue(layer.HasInstanceFor(importingId));
        }

        [Test]
        public void UpdateIfRequiredWithCurrentConnectionsOnPropertyImport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreatePropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);

            var changes = layer.UpdateIfRequired(
                importingId,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });

            Assert.AreEqual(0, changes.Count());
            Assert.IsTrue(layer.HasInstanceFor(exportingId));
            Assert.IsTrue(layer.HasInstanceFor(importingId));
        }

        [Test]
        public void UpdateIfRequiredWithConstructorUpdateConnections()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);

            var otherExportingDefinition = CreatePropertyExportingDefinition();
            var otherExportingId = layer.Construct(
                otherExportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(otherExportingId);
            Assert.IsTrue(layer.HasInstanceFor(otherExportingId));

            var changes = layer.UpdateIfRequired(
                importingId,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            otherExportingId,
                            otherExportingDefinition.RegisteredExports.First()),
                    });

            Assert.AreEqual(1, changes.Count());
            Assert.That(
                changes,
                Is.EquivalentTo(
                    new List<InstanceUpdate>
                    {
                        new InstanceUpdate 
                            {
                                Instance = importingId,
                                Change = InstanceChange.Reconstructed,
                            },
                    }));

            Assert.IsTrue(layer.HasInstanceFor(exportingId));
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.IsTrue(layer.HasInstanceFor(otherExportingId));
        }

        [Test]
        public void UpdateIfRequiredWithPropertyUpdateConnections()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreatePropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);

            var otherExportingDefinition = CreatePropertyExportingDefinition();
            var otherExportingId = layer.Construct(
                otherExportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(otherExportingId);
            Assert.IsTrue(layer.HasInstanceFor(otherExportingId));

            var changes = layer.UpdateIfRequired(
                importingId,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            otherExportingId,
                            otherExportingDefinition.RegisteredExports.First()),
                    });

            Assert.AreEqual(1, changes.Count());
            Assert.That(
                changes,
                Is.EquivalentTo(
                    new List<InstanceUpdate>
                    {
                        new InstanceUpdate 
                            {
                                Instance = importingId,
                                Change = InstanceChange.Updated,
                            },
                    }));

            Assert.IsTrue(layer.HasInstanceFor(exportingId));
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.IsTrue(layer.HasInstanceFor(otherExportingId));
        }

        [Test]
        public void ReleaseWithConstructorImport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreateConstructorImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);

            var changes = layer.Release(exportingId);

            Assert.AreEqual(2, changes.Count());
            Assert.That(
                changes,
                Is.EquivalentTo(
                    new List<InstanceUpdate>
                    {
                        new InstanceUpdate 
                            {
                                Instance = exportingId,
                                Change = InstanceChange.Removed,
                            },
                        new InstanceUpdate
                            {
                                Instance = importingId,
                                Change = InstanceChange.Removed,
                            }
                    }));

            Assert.IsFalse(layer.HasInstanceFor(exportingId));
            Assert.IsFalse(layer.HasInstanceFor(importingId));
        }

        [Test]
        public void ReleaseWithPropertyImport()
        {
            var layer = InstanceLayer.CreateInstanceWithoutTimeline();
            var exportingDefinition = CreateTypeExportingDefinition();

            var exportingId = layer.Construct(
                exportingDefinition, 
                Enumerable.Empty<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>>());
            Assert.IsNotNull(exportingId);
            Assert.IsTrue(layer.HasInstanceFor(exportingId));

            var importingDefinition = CreatePropertyImportDefinition();
            var importingId = layer.Construct(
                importingDefinition,
                new List<Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>> 
                    { 
                        new Tuple<ImportRegistrationId, PartInstanceId, ExportRegistrationId>(
                            importingDefinition.RegisteredImports.First(),
                            exportingId,
                            exportingDefinition.RegisteredExports.First()),
                    });
            Assert.IsNotNull(importingId);
            Assert.IsTrue(layer.HasInstanceFor(importingId));
            Assert.AreNotEqual(exportingId, importingId);

            var changes = layer.Release(exportingId);

            Assert.AreEqual(2, changes.Count());
            Assert.That(
                changes,
                Is.EquivalentTo(
                    new List<InstanceUpdate>
                    {
                        new InstanceUpdate 
                            {
                                Instance = exportingId,
                                Change = InstanceChange.Removed,
                            },
                        new InstanceUpdate
                            {
                                Instance = importingId,
                                Change = InstanceChange.Updated,
                            }
                    }));

            Assert.IsFalse(layer.HasInstanceFor(exportingId));
            Assert.IsTrue(layer.HasInstanceFor(importingId));
        }
    }
}
