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
        /// Adds a new discoverable member to the repository.
        /// </summary>
        /// <param name="member">The member that should be added.</param>
        /// <param name="pluginOrigin">The origin of the assembly that owns the discoverable member</param>
        void AddDiscoverableMember(SerializableDiscoverableMemberDefinition member, PluginOrigin pluginOrigin);

        /// <summary>
        /// Adds a new part to the repository.
        /// </summary>
        /// <param name="part">The part definition.</param>
        /// <param name="pluginOrigin">The origin of the assembly which owns the part.</param>
        void AddPart(PartDefinition part, PluginOrigin pluginOrigin);

        /// <summary>
        /// Adds a new type definition to the repository.
        /// </summary>
        /// <param name="type">The type definition.</param>
        void AddType(TypeDefinition type);

        /// <summary>
        /// Returns a collection containing the descriptions of all the known plugins.
        /// </summary>
        /// <returns>
        /// A collection containing the descriptions of all the known plugins.
        /// </returns>
        IEnumerable<PluginOrigin> KnownPluginOrigins();

        /// <summary>
        /// Removes all the plugins related to the given plugin origins.
        /// </summary>
        /// <param name="deletedPlugins">The collection of plugins that were removed.</param>
        void RemovePlugins(IEnumerable<PluginOrigin> deletedPlugins);
    }
}
