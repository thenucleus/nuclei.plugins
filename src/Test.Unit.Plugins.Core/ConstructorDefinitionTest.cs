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
    public sealed class ConstructorDefinitionTest : EqualityContractVerifierTest
    {
        private static ConstructorInfo GetConstructorForString()
        {
            return typeof(string).GetConstructor(new[] { typeof(char[]) });
        }

        private readonly ConstructorDefinitionHashcodeContractVerfier _hashCodeVerifier = new ConstructorDefinitionHashcodeContractVerfier();

        private readonly ConstructorDefinitionEqualityContractVerifier _equalityVerifier = new ConstructorDefinitionEqualityContractVerifier();

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
            var original = ConstructorDefinition.CreateDefinition(GetConstructorForString());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = ConstructorDefinition.CreateDefinition(GetConstructorForString());
            var constructor = GetConstructorForString();

            Assert.That(
                obj.Parameters,
                Is.EquivalentTo(constructor.GetParameters().Select(p => ParameterDefinition.CreateDefinition(p, t => TypeIdentity.CreateDefinition(t)))));
            Assert.AreEqual(TypeIdentity.CreateDefinition(constructor.DeclaringType), obj.DeclaringType);
        }

        private sealed class ConstructorDefinitionEqualityContractVerifier : EqualityContractVerifier<ConstructorDefinition>
        {
            private readonly ConstructorDefinition _first = ConstructorDefinition.CreateDefinition(typeof(object).GetConstructor(new Type[0]));

            private readonly ConstructorDefinition _second = ConstructorDefinition.CreateDefinition(typeof(List<int>).GetConstructor(new Type[0]));

            protected override ConstructorDefinition Copy(ConstructorDefinition original)
            {
                if (original.DeclaringType.Equals(typeof(object)))
                {
                    return ConstructorDefinition.CreateDefinition(typeof(object).GetConstructor(new Type[0]));
                }

                return ConstructorDefinition.CreateDefinition(typeof(List<int>).GetConstructor(new Type[0]));
            }

            protected override ConstructorDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override ConstructorDefinition SecondInstance
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

        private sealed class ConstructorDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<ConstructorDefinition> _distinctInstances
                = new List<ConstructorDefinition>
                     {
                        ConstructorDefinition.CreateDefinition(
                            typeof(string).GetConstructor(new[]
                                {
                                    typeof(char[])
                                })),
                        ConstructorDefinition.CreateDefinition(typeof(object).GetConstructor(new Type[0])),
                        ConstructorDefinition.CreateDefinition(typeof(List<int>).GetConstructor(new Type[0])),
                        ConstructorDefinition.CreateDefinition(
                            typeof(Uri).GetConstructor(new[]
                            {
                                typeof(string)
                            })),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
