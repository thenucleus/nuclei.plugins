//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Discovery.Container
{
    /// <summary>
    /// Defines the interface for objects that perform scanning of assemblies.
    /// </summary>
    public interface IAssemblyScanner
    {
        /// <summary>
        /// Scans the assemblies for which the given file paths have been provided and
        /// returns the plugin description information.
        /// </summary>
        /// <param name="assemblyFilesToScan">
        /// The collection that maps the file paths of the assemblies that need to be scanned to the plugin container that stores
        /// the assemblies.
        /// </param>
        void Scan(IDictionary<string, PluginOrigin> assemblyFilesToScan);
    }
}
