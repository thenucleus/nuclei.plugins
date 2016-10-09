//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Plugins.Core;

namespace Test.Mocks
{
#pragma warning disable SA1649 // File name must match first type name
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class MockDiscoverableMemberAttribute : DiscoverableMemberAttribute
    {
        public string Name
        {
            get;
            set;
        }

        public override IDictionary<string, string> Metadata()
        {
            return new Dictionary<string, string>
            {
                { "Name", Name }
            };
        }
    }
#pragma warning restore SA1649 // File name must match first type name
}
