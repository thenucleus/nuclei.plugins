//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Plugins.Discovery.Origin.FileSystem;

namespace Nuclei.Plugins.Discovery.Container
{
    /// <summary>
    /// Defines the interface for objects that perform scanning of plugin assemblies.
    /// </summary>
    public interface IAssemblyScanner
    {
        /// <summary>
        /// Scans the assemblies for which the given file paths have been provided and
        /// returns the plugin description information.
        /// </summary>
        /// <param name="assemblyFilesToScan">
        /// The collection that contains the file paths to all the assemblies to be scanned.
        /// </param>
        void Scan(IEnumerable<PluginFileOrigin> assemblyFilesToScan);
    }
}
