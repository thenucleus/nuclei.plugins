//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Apollo.Core.Base.Plugins;
using Apollo.Core.Extensions.Plugins;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Plugins.Instantiation
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class PartCompositionIdTest : EqualityContractVerifierTest
    {
        private sealed class EndpointIdEqualityContractVerifier : EqualityContractVerifier<PartCompositionId>
        {
            private readonly PartCompositionId m_First = new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("a", 0));

            private readonly PartCompositionId m_Second = new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("b", 1));

            protected override PartCompositionId Copy(PartCompositionId original)
            {
                return new PartCompositionId(original.Group, original.Part);
            }

            protected override PartCompositionId FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override PartCompositionId SecondInstance
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

        private sealed class EndpointIdHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<PartCompositionId> m_DistinctInstances
                = new List<PartCompositionId> 
                     {
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("a", 0)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("b", 1)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("c", 2)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("d", 3)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("e", 4)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("f", 5)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("g", 6)),
                        new PartCompositionId(new GroupCompositionId(), new PartRegistrationId("h", 7)),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly EndpointIdHashcodeContractVerfier m_HashcodeVerifier = new EndpointIdHashcodeContractVerfier();

        private readonly EndpointIdEqualityContractVerifier m_EqualityVerifier = new EndpointIdEqualityContractVerifier();

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
        public void Create()
        {
            var group = new GroupCompositionId();
            var part = new PartRegistrationId("a", 1);
            var id = new PartCompositionId(group, part);

            Assert.AreEqual(group, id.Group);
            Assert.AreEqual(part, id.Part);
        }
    }
}
