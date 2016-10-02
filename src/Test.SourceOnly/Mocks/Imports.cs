﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace Test.Mocks
{
#pragma warning disable SA1649 // File name must match first type name
    [Export]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.MaintainabilityRules",
        "SA1402:FileMayOnlyContainASingleClass",
        Justification = "These classes are only here for testing purposes so there's little point in having them in a separate file each.")]
    public sealed class ImportOnConstructorWithName
    {
        private readonly IExportingInterface _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithName([Import("ImportOnConstructor")]IExportingInterface input)
        {
            _import = input;
        }

        public IExportingInterface Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructorWithType
    {
        private readonly IExportingInterface _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithType([Import(typeof(IExportingInterface))]IExportingInterface input)
        {
            _import = input;
        }

        public IExportingInterface Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructor
    {
        private readonly IExportingInterface _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructor([Import]IExportingInterface input)
        {
            _import = input;
        }

        public IExportingInterface Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructorWithEnumerable
    {
        private readonly IEnumerable<IExportingInterface> _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithEnumerable([Import("ContractName")]IEnumerable<IExportingInterface> input)
        {
            _import = input;
        }

        public IEnumerable<IExportingInterface> Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructorWithMany
    {
        private readonly IEnumerable<IExportingInterface> _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithMany([ImportMany("ContractName")]IEnumerable<IExportingInterface> input)
        {
            _import = input;
        }

        public IEnumerable<IExportingInterface> Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructorWithLazy
    {
        private readonly Lazy<IExportingInterface> _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithLazy([Import("ContractName")]Lazy<IExportingInterface> input)
        {
            _import = input;
        }

        public Lazy<IExportingInterface> Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructorWithFunc
    {
        private readonly Func<IExportingInterface> _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithFunc([Import("ContractName")]Func<IExportingInterface> input)
        {
            _import = input;
        }

        public Func<IExportingInterface> Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructorWithFuncWithMultipleParameters
    {
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithFuncWithMultipleParameters([Import("ContractName")]Func<IExportingInterface, bool, bool> input)
        {
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
    public sealed class ImportOnConstructorWithCollectionOfLazy
    {
        private readonly IEnumerable<Lazy<IExportingInterface>> _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithCollectionOfLazy([ImportMany]IEnumerable<Lazy<IExportingInterface>> input)
        {
            _import = input;
        }

        public IEnumerable<Lazy<IExportingInterface>> Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnConstructorWithCollectionOfFunc
    {
        private readonly IEnumerable<Func<IExportingInterface>> _import;

        [SuppressMessage(
            "Microsoft.Usage",
            "CA1801:ReviewUnusedParameters",
            MessageId = "input",
            Justification = "Parameter is used by reflection")]
        [ImportingConstructor]
        public ImportOnConstructorWithCollectionOfFunc([ImportMany]IEnumerable<Func<IExportingInterface>> input)
        {
            _import = input;
        }

        public IEnumerable<Func<IExportingInterface>> Import
        {
            get
            {
                return _import;
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
    public sealed class ImportOnPropertyWithName
    {
        [Import("ImportOnProperty")]
        public IExportingInterface ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithType
    {
        [Import(typeof(IExportingInterface))]
        public IExportingInterface ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnProperty
    {
        [Import]
        public IExportingInterface ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithEnumerable
    {
        [Import]
        public IEnumerable<IExportingInterface> ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithEnumerableFromMany
    {
        [ImportMany]
        public IEnumerable<IExportingInterface> ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithCollectionOfFunc
    {
        [ImportMany]
        public IEnumerable<Func<IExportingInterface>> ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithCollectionOfLazy
    {
        [ImportMany]
        public IEnumerable<Lazy<IExportingInterface>> ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithLazy
    {
        [Import]
        public Lazy<IExportingInterface> ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithFunc
    {
        [Import]
        public Func<IExportingInterface> ImportingProperty
        {
            get;
            set;
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
    public sealed class ImportOnPropertyWithFuncWithMultipleParameters
    {
        [Import]
        public Func<IExportingInterface, bool, bool> ImportingProperty
        {
            get;
            set;
        }
    }
#pragma warning restore SA1649 // File name must match first type name
}
