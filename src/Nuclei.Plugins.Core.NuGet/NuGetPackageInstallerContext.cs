//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Xml.Linq;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using NuGet.Packaging;
using NuGet.ProjectManagement;

namespace Nuclei.Plugins.Core.NuGet
{
    internal sealed class NuGetPackageInstallerContext : INuGetProjectContext
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstallerContext"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public NuGetPackageInstallerContext(SystemDiagnostics diagnostics)
        {
            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            _diagnostics = diagnostics;
        }

        public NuGetActionType ActionType
        {
            get;
            set;
        }

        public ExecutionContext ExecutionContext
        {
            get
            {
                return null;
            }
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            if ((args == null) || (args.Length > 0))
            {
                message = string.Format(CultureInfo.CurrentCulture, message, args);
            }

            switch (level)
            {
                case MessageLevel.Info:
                    _diagnostics.Log(LevelToLog.Info, message);
                    break;
                case MessageLevel.Warning:
                    _diagnostics.Log(LevelToLog.Warn, message);
                    break;
                case MessageLevel.Debug:
                    _diagnostics.Log(LevelToLog.Debug, message);
                    break;
                case MessageLevel.Error:
                    _diagnostics.Log(LevelToLog.Error, message);
                    break;
                default:
                    _diagnostics.Log(LevelToLog.Info, message);
                    break;
            }
        }

        public XDocument OriginalPackagesConfig
        {
            get;
            set;
        }

        public PackageExtractionContext PackageExtractionContext
        {
            get;
            set;
        }

        public void ReportError(string message)
        {
            _diagnostics.Log(LevelToLog.Error, message);
        }

        public FileConflictAction ResolveFileConflict(string message)
        {
            return FileConflictAction.IgnoreAll;
        }

        public ISourceControlManagerProvider SourceControlManagerProvider
        {
            get
            {
                return null;
            }
        }
    }
}
