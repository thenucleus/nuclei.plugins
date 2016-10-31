//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nuclei.Plugins.Core.NuGet.Properties;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// This class represents a <see cref="NuGetProject"/> based on a folder such as packages folder like the
    /// <see cref="FolderNuGetProject"/> but with the additional ability to perform a post-install action.
    /// </summary>
    internal class PluginNuGetProject : NuGetProject
    {
        /// <summary>
        /// The object that handles installing NuGet packages into a folder.
        /// </summary>
        private readonly FolderNuGetProject _folderProject;

        /// <summary>
        /// The object that is used to resolve installed package paths.
        /// </summary>
        private readonly PackagePathResolver _packagePathResolver;

        /// <summary>
        /// The action that is executed after a package is installed.
        /// </summary>
        private readonly PackagePostInstall _postInstallAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNuGetProject"/> class.
        /// </summary>
        /// <param name="root">The base directory into which all packages are installed.</param>
        /// <param name="packagePathResolver">The object that is used to resolve package install paths.</param>
        /// <param name="postInstallAction">The action that is executed after a package is installed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="root"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="root"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="packagePathResolver"/> is <see langword="null" />.
        /// </exception>
        public PluginNuGetProject(
            string root,
            PackagePathResolver packagePathResolver,
            PackagePostInstall postInstallAction = null)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (string.IsNullOrEmpty(root))
            {
                throw new ArgumentException(Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString, "root");
            }

            if (packagePathResolver == null)
            {
                throw new ArgumentNullException("packagePathResolver");
            }

            _folderProject = new FolderNuGetProject(root, packagePathResolver);
            _packagePathResolver = packagePathResolver;
            _postInstallAction = postInstallAction;
        }

        /// <summary>
        /// Is used by Dependency Resolver and more
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the work to be done.</returns>
        public override Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(CancellationToken token)
        {
            return _folderProject.GetInstalledPackagesAsync(token);
        }

        /// <summary>
        /// This installs a package into the NuGetProject.
        /// </summary>
        /// <param name="packageIdentity">The identity of the package.</param>
        /// <param name="downloadResourceResult">The result of a download resource.</param>
        /// <param name="nuGetProjectContext">The project context for NuGet.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// Returns false if the package was already present in the NuGetProject. On successful installation, returns true
        /// </returns>
        public override Task<bool> InstallPackageAsync(
            PackageIdentity packageIdentity,
            DownloadResourceResult downloadResourceResult,
            INuGetProjectContext nuGetProjectContext,
            CancellationToken token)
        {
            return _folderProject.InstallPackageAsync(packageIdentity, downloadResourceResult, nuGetProjectContext, token)
                .ContinueWith(
                    t =>
                    {
                        _postInstallAction?.Invoke(_folderProject.Root, _packagePathResolver.GetInstallPath(packageIdentity), packageIdentity);
                        return t.Result;
                    });
        }

        /// <summary>
        /// A package is considered to exist in FileSystemNuGetProject, if the 'nupkg' file is present where expected
        /// </summary>
        /// <param name="packageIdentity">The identity of the package.</param>
        /// <returns>
        ///     <see langword="true"/> if the package exists in the exists; otherwise returns <see langword="false" />.
        /// </returns>
        public bool PackageExists(PackageIdentity packageIdentity)
        {
            return _folderProject.PackageExists(packageIdentity);
        }

        /// <summary>
        /// Uninstalls the package from the project.
        /// </summary>
        /// <param name="packageIdentity">The identity of the package.</param>
        /// <param name="nuGetProjectContext">The project context for NuGet.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// Returns false if the package was not found. On successful uninstallation, returns true
        /// </returns>
        public override Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, INuGetProjectContext nuGetProjectContext, CancellationToken token)
        {
            return _folderProject.UninstallPackageAsync(packageIdentity, nuGetProjectContext, token);
        }
    }
}
