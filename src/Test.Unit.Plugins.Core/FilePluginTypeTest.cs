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
using Moq;
using Nuclei.Nunit.Extensions;
using Nuclei.Plugins.Core.Assembly;
using Nuclei.Plugins.Discovery.Assembly;
using NUnit.Framework;

namespace Nuclei.Plugins.Core
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class FilePluginTypeTest : EqualityContractVerifierTest
    {
        private readonly FilePluginTypeHashcodeContractVerfier _hashCodeVerifier = new FilePluginTypeHashcodeContractVerfier();

        private readonly FilePluginTypeEqualityContractVerifier _equalityVerifier = new FilePluginTypeEqualityContractVerifier();

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
        public void Create()
        {
            var extension = "a";

            var origin = new PluginAssemblyOrigin("a");
            Func<string, PluginOrigin> builder = s => origin;
            var type = new FilePluginType(extension, builder);

            Assert.IsNotNull(type);
            Assert.AreSame(origin, type.Origin("b"));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.FilePluginType",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithEmptyExtension()
        {
            Assert.Throws<ArgumentException>(() => new FilePluginType(string.Empty, s => null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.FilePluginType",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullExtension()
        {
            Assert.Throws<ArgumentNullException>(() => new FilePluginType(null, s => null));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.FilePluginType",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullOriginBuilder()
        {
            Assert.Throws<ArgumentNullException>(() => new FilePluginType("a", null));
        }

        private sealed class FilePluginTypeEqualityContractVerifier : EqualityContractVerifier<FilePluginType>
        {
            private readonly FilePluginType _first = new FilePluginType("a", s => null);

            private readonly FilePluginType _second = new FilePluginType("b", s => null);

            protected override FilePluginType Copy(FilePluginType original)
            {
                return (FilePluginType)original.Clone();
            }

            protected override FilePluginType FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override FilePluginType SecondInstance
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

        private sealed class FilePluginTypeHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<FilePluginType> _distinctInstances
                = new List<FilePluginType>
                     {
                        new FilePluginType("a", s => null),
                        new FilePluginType("b", s => null),
                        new FilePluginType("c", s => null),
                        new FilePluginType("d", s => null),
                        new FilePluginType("e", s => null),
                        new FilePluginType("f", s => null),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
