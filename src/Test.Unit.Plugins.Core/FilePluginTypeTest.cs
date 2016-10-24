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
using Nuclei.Nunit.Extensions;
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
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.FilePluginType",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithEmptyExtension()
        {
            Assert.Throws<ArgumentException>(() => new FilePluginType(string.Empty));
        }

        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Nuclei.Plugins.Core.FilePluginType",
            Justification = "Testing that the constructor throws an exception.")]
        public void CreateWithNullExtension()
        {
            Assert.Throws<ArgumentNullException>(() => new FilePluginType(null));
        }

        private sealed class FilePluginTypeEqualityContractVerifier : EqualityContractVerifier<FilePluginType>
        {
            private readonly FilePluginType _first = new FilePluginType("a");

            private readonly FilePluginType _second = new FilePluginType("b");

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
                        new FilePluginType("a"),
                        new FilePluginType("b"),
                        new FilePluginType("c"),
                        new FilePluginType("d"),
                        new FilePluginType("e"),
                        new FilePluginType("f"),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
