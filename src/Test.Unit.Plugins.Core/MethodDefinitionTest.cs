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
    public sealed class MethodDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class MethodDefinitionEqualityContractVerifier : EqualityContractVerifier<MethodDefinition>
        {
            private readonly MethodDefinition m_First 
                = MethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains"));

            private readonly MethodDefinition m_Second 
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
                    return m_First;
                }
            }

            protected override MethodDefinition SecondInstance
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

        private sealed class MethodDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<MethodDefinition> m_DistinctInstances
                = new List<MethodDefinition> 
                     {
                        MethodDefinition.CreateDefinition(typeof(string).GetMethod("Contains")),
                        MethodDefinition.CreateDefinition(typeof(int).GetMethod("CompareTo", new[] { typeof(int) })),
                        MethodDefinition.CreateDefinition(typeof(double).GetMethod("CompareTo", new[] { typeof(double) })),
                        MethodDefinition.CreateDefinition(typeof(IComparable).GetMethod("CompareTo")),
                        MethodDefinition.CreateDefinition(typeof(IComparable<>).GetMethod("CompareTo")),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly MethodDefinitionHashcodeContractVerfier m_HashcodeVerifier = new MethodDefinitionHashcodeContractVerfier();

        private readonly MethodDefinitionEqualityContractVerifier m_EqualityVerifier = new MethodDefinitionEqualityContractVerifier();

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

        private static MethodInfo GetMethodForInt()
        {
            return typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
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
    }
}
