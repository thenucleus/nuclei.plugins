//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace Test.Mocks
{
#pragma warning disable SA1649 // File name must match first type name
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class DiscoverableMemberOnMethod
    {
        private IExportingInterface _value = new MockChildExportingInterfaceImplementation();

        [MockDiscoverableMember(Name = "Method")]
        public IExportingInterface DiscoverableMethod()
        {
            return _value;
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class DiscoverableMemberOnProperty
    {
        private IExportingInterface _value = new MockChildExportingInterfaceImplementation();

        [MockDiscoverableMember(Name = "Property")]
        public IExportingInterface DiscoverableProperty
        {
            get
            {
                return _value;
            }
        }
    }

    [MockDiscoverableMember(Name = "Type")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class DiscoverableMemberOnType
    {
        
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnAnotherMethod
    {
        private IExportingInterface _value = new MockExportingInterfaceImplementation();

        [Export]
        public IExportingInterface ExportingMethod()
        {
            return _value;
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMethod
    {
        private IExportingInterface _value = new MockChildExportingInterfaceImplementation();

        [Export]
        public IExportingInterface ExportingMethod()
        {
            return _value;
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMethodWithMultipleParameters
    {
        private IExportingInterface _value = new MockExportingInterfaceImplementation();

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input1",
            Justification = "Parameter is used by reflection")]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input2",
            Justification = "Parameter is used by reflection")]
        [Export]
        public IExportingInterface ExportingMethod(bool input1, bool input2)
        {
            return _value;
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMethodWithName
    {
        private IExportingInterface _value = new MockExportingInterfaceImplementation();

        [Export("OnMethodWithName")]
        public IExportingInterface ExportingMethod()
        {
            return _value;
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnMethodWithType
    {
        private IExportingInterface _value = new MockExportingInterfaceImplementation();

        [Export(typeof(IExportingInterface))]
        public IExportingInterface ExportingMethod()
        {
            return _value;
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnProperty
    {
        private IExportingInterface _value = new MockChildExportingInterfaceImplementation();

        [Export]
        public IExportingInterface ExportingProperty
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnPropertyWithEnumerable
    {
        private IEnumerable<IExportingInterface> _value = new List<IExportingInterface>
            {
                new MockExportingInterfaceImplementation()
            };

        [Export]
        public IEnumerable<IExportingInterface> ExportingProperty
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnPropertyWithName
    {
        private IExportingInterface _value = new MockExportingInterfaceImplementation();

        [Export("OnPropertyWithName")]
        public IExportingInterface ExportingProperty
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }

    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnPropertyWithType
    {
        private IExportingInterface _value = new MockExportingInterfaceImplementation();

        [Export(typeof(IExportingInterface))]
        public IExportingInterface ExportingProperty
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }

    [Export]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnType : IExportingInterface
    {
    }

    [Export("OnTypeWithName")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnTypeWithName : IExportingInterface
    {
    }

    [Export(typeof(IExportingInterface))]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ExportOnTypeWithType : IExportingInterface
    {
    }

    [SuppressMessage(
        "Microsoft.Design",
        "CA1040:AvoidEmptyInterfaces",
        Justification = "Need an exporting interface but we never use any of the members.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public interface IExportingInterface
    {
    }

    [Export(typeof(IExportingInterface))]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class MockChildExportingInterfaceImplementation : MockExportingInterfaceImplementation
    {
    }

    [Export(typeof(IExportingInterface))]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public class MockExportingInterfaceImplementation : IExportingInterface
    {
    }
#pragma warning restore SA1649 // File name must match first type name
}
