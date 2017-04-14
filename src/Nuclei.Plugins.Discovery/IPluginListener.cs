//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the interface for objects that listen for changes in plugin files.
    /// </summary>
    public interface IPluginListener
    {
        /// <summary>
        /// Disables the uploading of packages.
        /// </summary>
        void Disable();

        /// <summary>
        /// Enables the uploading of packages.
        /// </summary>
        void Enable();
    }
}
