//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Client;
using NuGet.Packaging.Core;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Defines the interface for objects that install NuGet packages from a given feed.
    /// </summary>
    public interface IInstallPackages
    {
        /// <summary>
        /// Installs a given version of a package and its dependencies.
        /// </summary>
        /// <param name="name">The ID of the package.</param>
        /// <param name="outputLocation">The full path of the directory where the packages should be installed.</param>
        /// <param name="postInstallAction">
        /// An action that is run after each package is installed. The input values are the <paramref name="outputLocation"/>,
        /// the path to the installed package and the package ID.
        /// </param>
        void Install(
            PackageIdentity name,
            string outputLocation,
            Action<string, string, PackageIdentity> postInstallAction = null);
    }
}
