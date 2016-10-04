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
using System.Reflection;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins.Core
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MethodBasedExportDefinitionTest : EqualityContractVerifierTest
    {
        private static MethodInfo GetMethodForInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
        }

        private readonly MethodBasedExportDefinitionHashcodeContractVerfier _hashCodeVerifier
            = new MethodBasedExportDefinitionHashcodeContractVerfier();

        private readonly MethodBasedExportDefinitionEqualityContractVerifier _equalityVerifier
            = new MethodBasedExportDefinitionEqualityContractVerifier();

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
        public void RoundTripSerialize()
        {
            var original = MethodBasedExportDefinition.CreateDefinition("B", "C", GetMethodForInt());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = MethodBasedExportDefinition.CreateDefinition("B", "C", GetMethodForInt());

            Assert.AreEqual("B", obj.ContractName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(typeof(int)), obj.DeclaringType);
            Assert.AreEqual(MethodDefinition.CreateDefinition(GetMethodForInt()), obj.Method);
        }

        private sealed class MethodBasedExportDefinitionEqualityContractVerifier : EqualityContractVerifier<MethodBasedExportDefinition>
        {
            private readonly MethodBasedExportDefinition _first = MethodBasedExportDefinition.CreateDefinition(
                "A",
                "B",
                typeof(string).GetMethod("Contains"));

            private readonly MethodBasedExportDefinition _second = MethodBasedExportDefinition.CreateDefinition(
                "C",
                "D",
                typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));

            protected override MethodBasedExportDefinition Copy(MethodBasedExportDefinition original)
            {
                if (original.ContractName.Equals("A"))
                {
                    return MethodBasedExportDefinition.CreateDefinition(
                        "A",
                        "B",
                        typeof(string).GetMethod("Contains"));
                }

                return MethodBasedExportDefinition.CreateDefinition(
                    "C",
                    "D",
                    typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));
            }

            protected override MethodBasedExportDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override MethodBasedExportDefinition SecondInstance
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

        private sealed class MethodBasedExportDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<MethodBasedExportDefinition> _distinctInstances
                = new List<MethodBasedExportDefinition>
                     {
                        MethodBasedExportDefinition.CreateDefinition(
                            "A",
                            "B",
                            typeof(string).GetMethod("Contains")),
                        MethodBasedExportDefinition.CreateDefinition(
                            "C",
                            "D",
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        MethodBasedExportDefinition.CreateDefinition(
                            "E",
                            "F",
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        MethodBasedExportDefinition.CreateDefinition(
                            "G",
                            "H",
                            typeof(IComparable).GetMethod("CompareTo")),
                        MethodBasedExportDefinition.CreateDefinition(
                            "I",
                            "J",
                            typeof(IComparable<>).GetMethod("CompareTo")),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
