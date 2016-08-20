//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
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
    public sealed class ConstructorDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class ConstructorDefinitionEqualityContractVerifier : EqualityContractVerifier<ConstructorDefinition>
        {
            private readonly ConstructorDefinition m_First = ConstructorDefinition.CreateDefinition(typeof(object).GetConstructor(new Type[0]));

            private readonly ConstructorDefinition m_Second = ConstructorDefinition.CreateDefinition(typeof(List<int>).GetConstructor(new Type[0]));

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
                    return m_First;
                }
            }

            protected override ConstructorDefinition SecondInstance
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

        private sealed class ConstructorDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ConstructorDefinition> m_DistinctInstances
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

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private static ConstructorInfo GetConstructorForString()
        {
            return typeof(string).GetConstructor(new[] { typeof(char[]) });
        }

        private readonly ConstructorDefinitionHashcodeContractVerfier m_HashcodeVerifier = new ConstructorDefinitionHashcodeContractVerfier();

        private readonly ConstructorDefinitionEqualityContractVerifier m_EqualityVerifier = new ConstructorDefinitionEqualityContractVerifier();

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

        [Test]
        public void RoundtripSerialize()
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
                Is.EquivalentTo(constructor.GetParameters().Select(p => ParameterDefinition.CreateDefinition(p))));
            Assert.AreEqual(TypeIdentity.CreateDefinition(constructor.DeclaringType), obj.DeclaringType);
        }
    }
}
