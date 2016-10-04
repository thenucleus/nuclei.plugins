//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Nuclei.Plugins.Composition
{
    [TestFixture]
    public sealed class MetadataServicesTest
    {
        [Test]
        public void AsReadOnly()
        {
            var collection = new Dictionary<string, object>
                {
                    { "a", 10 }
                };

            var newCollection = collection.AsReadOnly();

            Assert.AreEqual(collection.First().Value, newCollection.First().Value);
        }
    }
}
