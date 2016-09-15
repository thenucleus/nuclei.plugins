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

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class ParameterDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class ParameterDefinitionEqualityContractVerifier : EqualityContractVerifier<ParameterDefinition>
        {
            private readonly ParameterDefinition m_First = ParameterDefinition.CreateDefinition(
                typeof(string).GetMethod("Contains").GetParameters().First());

            private readonly ParameterDefinition m_Second = ParameterDefinition.CreateDefinition(
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
                    return m_First;
                }
            }

            protected override ParameterDefinition SecondInstance
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

        private sealed class ParameterDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ParameterDefinition> m_DistinctInstances
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

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly ParameterDefinitionHashcodeContractVerfier m_HashcodeVerifier = new ParameterDefinitionHashcodeContractVerfier();

        private readonly ParameterDefinitionEqualityContractVerifier m_EqualityVerifier = new ParameterDefinitionEqualityContractVerifier();

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

        private static ParameterInfo ParameterFromInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) }).GetParameters().First();
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
    }
}
