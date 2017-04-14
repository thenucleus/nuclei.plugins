//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using NuGet.Packaging.Core;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// A delegate that is used to execute a post-install action when a new NuGet package is installed.
    /// </summary>
    /// <param name="rootDirectory">The directory into which all packages are installed.</param>
    /// <param name="packageDirectory">The directory for the newly installed package.</param>
    /// <param name="id">The identity of the package.</param>
    public delegate void PackagePostInstall(string rootDirectory, string packageDirectory, PackageIdentity id);
}
