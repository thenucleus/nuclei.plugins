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
    public sealed class MethodDefinitionTest : EqualityContractVerifierTest
    {
        private static MethodInfo GetMethodForInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
        }

        private readonly MethodDefinitionHashcodeContractVerfier _hashCodeVerifier = new MethodDefinitionHashcodeContractVerfier();

        private readonly MethodDefinitionEqualityContractVerifier _equalityVerifier = new MethodDefinitionEqualityContractVerifier();

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
            var original = MethodDefinition.CreateDefinition(GetMethodForInt());
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void CreateWithClass()
        {
            var obj = MethodDefinition.CreateDefinition(GetMethodForInt());
            var method = GetMethodForInt();

            Assert.AreEqual(method.Name, obj.MethodName);
            Assert.AreEqual(TypeIdentity.CreateDefinition(method.ReturnType), obj.ReturnType);
            Assert.That(
                obj.Parameters,
                Is.EquivalentTo(method.GetParameters().Select(p => ParameterDefinition.CreateDefinition(p))));
            Assert.AreEqual(TypeIdentity.CreateDefinition(method.DeclaringType), obj.DeclaringType);
        }

        private sealed class MethodDefinitionEqualityContractVerifier : EqualityContractVerifier<MethodDefinition>
        {
            private readonly MethodDefinition _first
                = MethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains"));

            private readonly MethodDefinition _second
                = MethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));

            protected override MethodDefinition Copy(MethodDefinition original)
            {
                if (original.DeclaringType.Equals(typeof(string)))
                {
                    return MethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains"));
                }

                return MethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) }));
            }

            protected override MethodDefinition FirstInstance
            {
                get
                {
                    return _first;
                }
            }

            protected override MethodDefinition SecondInstance
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

        private sealed class MethodDefinitionHashcodeContractVerfier : HashCodeContractVerifier
        {
            private readonly IEnumerable<MethodDefinition> _distinctInstances
                = new List<MethodDefinition>
                     {
                        MethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                        MethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        MethodDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        MethodDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        MethodDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                     };

            protected override IEnumerable<int> GetHashCodes()
            {
                return _distinctInstances.Select(i => i.GetHashCode());
            }
        }
    }
}
