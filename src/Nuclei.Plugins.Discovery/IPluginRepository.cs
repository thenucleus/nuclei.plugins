//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Apollo.Core.Base.Plugins;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the interface for objects that store all the information about the parts and the part groups.
    /// </summary>
    internal interface IPluginRepository : ISatisfyPluginRequests
    {
        /// <summary>
        /// Returns a collection containing the descriptions of all the known plugins.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known plugins.
        /// </returns>
        IEnumerable<PluginFileInfo> KnownPluginFiles();

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
        void AddPart(PartDefinition part, PluginFileInfo pluginFileInfo);

        /// <summary>
        /// Adds a new part group to the repository.
        /// </summary>
        /// <param name="group">The part group definition.</param>
        /// <param name="pluginFileInfo">The file info of the assembly which owns the group.</param>
        void AddGroup(GroupDefinition group, PluginFileInfo pluginFileInfo);
    }
}
