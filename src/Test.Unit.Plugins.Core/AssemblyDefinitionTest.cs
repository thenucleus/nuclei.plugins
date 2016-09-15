//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class AssemblyDefinitionTest : EqualityContractVerifierTest
    {
        private sealed class AssemblyDefinitionEqualityContractVerifier : EqualityContractVerifier<AssemblyDefinition>
        {
            private readonly AssemblyDefinition m_First = AssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            private readonly AssemblyDefinition m_Second = AssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly);

            protected override AssemblyDefinition Copy(AssemblyDefinition original)
            {
                if (original.FullName.Equals(typeof(string).Assembly.FullName))
                {
                    return AssemblyDefinition.CreateDefinition(typeof(string).Assembly);
                }

                return AssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly);
            }

            protected override AssemblyDefinition FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override AssemblyDefinition SecondInstance
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

        private sealed class AssemblyDefinitionHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<AssemblyDefinition> m_DistinctInstances
                = new List<AssemblyDefinition> 
                     {
                        AssemblyDefinition.CreateDefinition(typeof(string).Assembly),
                        AssemblyDefinition.CreateDefinition(typeof(ExportAttribute).Assembly),
                        AssemblyDefinition.CreateDefinition(typeof(TestFixtureAttribute).Assembly),
                        AssemblyDefinition.CreateDefinition(typeof(BigInteger).Assembly),
                        AssemblyDefinition.CreateDefinition(typeof(AssemblyDefinition).Assembly),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly AssemblyDefinitionHashcodeContractVerfier m_HashcodeVerifier = new AssemblyDefinitionHashcodeContractVerfier();

        private readonly AssemblyDefinitionEqualityContractVerifier m_EqualityVerifier = new AssemblyDefinitionEqualityContractVerifier();

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
            var original = AssemblyDefinition.CreateDefinition(typeof(string).Assembly);
            var copy = AssertExtensions.RoundTripSerialize(original);

            Assert.AreEqual(original, copy);
        }

        [Test]
        public void Create()
        {
            var obj = AssemblyDefinition.CreateDefinition(typeof(string).Assembly);

            Assert.AreEqual(typeof(string).Assembly.GetName().Name, obj.Name);
            Assert.AreEqual(typeof(string).Assembly.GetName().Version, obj.Version);
            Assert.AreEqual(typeof(string).Assembly.GetName().CultureInfo, obj.Culture);

            var bits = typeof(string).Assembly.GetName().GetPublicKeyToken();
            var token = new StringBuilder();
            foreach (var bit in bits)
            {
                token.Append(bit.ToString("x2", CultureInfo.InvariantCulture));
            }

            Assert.AreEqual(token.ToString(), obj.PublicKeyToken);
        }
    }
}
