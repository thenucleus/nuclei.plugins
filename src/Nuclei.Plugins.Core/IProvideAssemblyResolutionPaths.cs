//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines methods for updating and interrogating the paths from which the assembly loader can load assemblies.
    /// </summary>
    public interface IProvideAssemblyResolutionPaths
    {
        /// <summary>
        /// Adds a new directory path to the assembly resolution path list.
        /// </summary>
        /// <param name="directory">The full path to the directory from which assemblies should be resolved.</param>
        void Add(string directory);

        /// <summary>
        /// Returns a value indicating whether the given directory path is on the list of directories from which
        /// assemblies can be resolved.
        /// </summary>
        /// <param name="directory">The full path to the directory</param>
        /// <returns>
        ///     <see langword="true" /> if the path is on the resolution list; otherwise returns <see langword="false" />.
        /// </returns>
        bool IsOnResolutionList(string directory);
    }
}
