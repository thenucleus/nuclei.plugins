//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Core.Assembly
{
    /// <summary>
    /// Loads plugin types from an assembly.
    /// </summary>
    public sealed class PluginAssemblyTypeLoader : ILoadTypesFromPlugins
    {
        /// <summary>
        /// Loads the type defined by the <paramref name="assemblyFullyQualifiedTypeName"/> from the <paramref name="origin"/>.
        /// </summary>
        /// <param name="origin">The object that indicates where the assembly containing the desired type can be found.</param>
        /// <param name="assemblyFullyQualifiedTypeName">The assembly fully qualified name of the type.</param>
        /// <returns>The requested <see cref="Type"/>.</returns>
        public Type Load(PluginOrigin origin, string assemblyFullyQualifiedTypeName)
        {
            var assemblyOrigin = origin as PluginAssemblyOrigin;
            if (assemblyOrigin == null)
            {
                throw new InvalidPluginOriginException();
            }

            return TypeLoader.FromFullyQualifiedName(assemblyFullyQualifiedTypeName);
        }

        /// <summary>
        /// Gets the type of the <see cref="PluginOrigin"/> that the current type loader can use.
        /// </summary>
        public Type ValidOriginType
        {
            get
            {
                return typeof(PluginAssemblyOrigin);
            }
        }
    }
}
