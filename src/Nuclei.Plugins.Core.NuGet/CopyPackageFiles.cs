//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using NuGet.Packaging.Core;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Defines the signature for methods that copy files from a NuGet package to a given location.
    /// </summary>
    /// <param name="id">The ID of the package.</param>
    /// <param name="fileSearchPattern">The file pattern that indicates which files should be copied.</param>
    /// <param name="installPath">The full path to the installed location of the package.</param>
    /// <param name="destinationPath">The full path to the destination folder where the files should be copied to.</param>
    /// <returns>
    ///     A collection containing the full file paths of all the copied files.
    /// </returns>
    public delegate IEnumerable<string> CopyPackageFiles(PackageIdentity id, string fileSearchPattern, string installPath, string destinationPath);
}
