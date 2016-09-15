//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the interface for objects that store all the information about the parts and the part groups.
    /// </summary>
    public interface IPluginRepository : ISatisfyPluginRequests
    {
        /// <summary>
        /// Returns a collection containing the descriptions of all the known plugins.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known plugins.
        /// </returns>
        IEnumerable<PluginOrigin> KnownPluginFiles();

        /// <summary>
        /// Removes all the plugins related to the given plugin files.
        /// </summary>
        /// <param name="deletedFiles">The collection of plugin file paths that were removed.</param>
        void RemovePlugins(IEnumerable<string> deletedFiles);

        /// <summary>
        /// Adds a new type definition to the repository.
        /// </summary>
        /// <param name="type">The type definition.</param>
        void AddType(TypeDefinition type);

        /// <summary>
        /// Adds a new part to the repository.
        /// </summary>
        /// <param name="part">The part definition.</param>
        /// <param name="pluginFileInfo">The file info of the assembly which owns the part.</param>
        void AddPart(PartDefinition part, PluginOrigin pluginFileInfo);
    }
}
