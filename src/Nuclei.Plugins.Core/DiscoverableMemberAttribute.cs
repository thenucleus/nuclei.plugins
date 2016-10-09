//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines an attribute that indicates that a methods is an action method.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = true)]
    public abstract class DiscoverableMemberAttribute : Attribute
    {
        /// <summary>
        /// Returns the metadata for the current attribute.
        /// </summary>
        /// <returns>A collection containing the metadata for the current attribute.</returns>
        public abstract IDictionary<string, string> Metadata();
    }
}
