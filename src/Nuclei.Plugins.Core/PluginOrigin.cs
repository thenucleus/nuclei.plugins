//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the origin for a plugin.
    /// </summary>
    [Serializable]
    public abstract class PluginOrigin : Id<PluginOrigin, PluginOriginData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginOrigin"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected PluginOrigin(PluginOriginData value)
            : base(value)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override abstract PluginOrigin Clone(PluginOriginData value);
    }
}
