//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Defines constants for the NuGet side of Nuclei.Plugins.
    /// </summary>
    public static class NuGetConstants
    {
        /// <summary>
        /// Defines the extension for a NuGet package.
        /// </summary>
        public const string NuGetPackageExtension = ".nupkg";

        /// <summary>
        /// The default location where the application will copy any assemblies from NuGet packages so that they
        /// can be used.
        /// </summary>
        private static readonly string _defaultAssemblyCacheLocation
            = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        /// <summary>
        /// The default location where the application installs any NuGet packages.
        /// </summary>
        private static readonly string _defaultInstallLocation
            = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        /// <summary>
        /// Gets the default location where the application will copy assemblies from NuGet packages in order to use them.
        /// </summary>
        public static string DefaultAssemblyCacheLocation
        {
            get
            {
                return _defaultAssemblyCacheLocation;
            }
        }

        /// <summary>
        /// Gets the default location where the application installs any NuGet packages.
        /// </summary>
        public static string DefaultInstallLocation
        {
            get
            {
                return _defaultInstallLocation;
            }
        }
    }
}
