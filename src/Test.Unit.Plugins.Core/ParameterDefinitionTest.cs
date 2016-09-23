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
    public sealed class ParameterDefinitionTest : EqualityContractVerifierTest
    {
        private static ParameterInfo ParameterFromInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First();
        }

        private readonly ParameterDefinitionHashcodeContractVerfier _hashCodeVerifier = new ParameterDefinitionHashcodeContractVerfier();

        private readonly ParameterDefinitionEqualityContractVerifier _equalityVerifier = new ParameterDefinitionEqualityContractVerifier();

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
        public void RoundtripSerialize()
        {
            var original = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = ParameterDefinition.CreateDefinition(ParameterFromInt());
            var parameter = ParameterFromInt();

            Assert.AreEqual(parameter.Name, obj.Name);
            Assert.AreEqual(TypeIdentity.CreateDefinition(parameter.ParameterType), obj.Identity);
        }

        private sealed class ParameterDefinitionEqualityContractVerifier : EqualityContractVerifier<ParameterDefinition>
        {
            private readonly ParameterDefinition _first = ParameterDefinition.CreateDefinition(
                typeof(string).GetMethod("Contains").GetParameters().First());

            private readonly ParameterDefinition _second = ParameterDefinition.CreateDefinition(
                typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First());

            protected override ParameterDefinition Copy(ParameterDefinition original)
            {
                if (original.Identity.Equals(typeof(string)))
                {
                    return ParameterDefinition.CreateDefinition(
                        typeof(string).GetMethod("Contains").GetParameters().First());
                }

                return ParameterDefinition.CreateDefinition(
                    typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First());
            }

            protected override ParameterDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ParameterDefinition SecondInstance
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

        private sealed class ParameterDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<ParameterDefinition> _distinctInstances
                = new List<ParameterDefinition>
                     {
                        ParameterDefinition.CreateDefinition(
                            typeof(string).GetMethod("Contains").GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(double).GetMethod("CompareTo", new[] { typeof(double) }).GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(IComparable).GetMethod("CompareTo").GetParameters().First()),
                        ParameterDefinition.CreateDefinition(
                            typeof(IComparable<>).GetMethod("CompareTo").GetParameters().First()),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
