//--------------------------------------------;---------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Diagnostics;
using NuGet.Common;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// Implements the <see cref="ILogger"/> interface and forwards log information
    /// to a <see cref="SystemDiagnostics"/> instance.
    /// </summary>
    internal sealed class LoggerProxy : ILogger
    {
        private SystemDiagnostics _diagnostics;

        public LoggerProxy(SystemDiagnostics diagnostics)
        {
            _diagnostics = diagnostics;
        }

        public void LogDebug(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Debug,
                data);
        }

        public void LogError(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Error,
                data);
        }

        public void LogErrorSummary(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Error,
                data);
        }

        public void LogInformation(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Info,
                data);
        }

        public void LogInformationSummary(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Info,
                data);
        }

        public void LogMinimal(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Info,
                data);
        }

        public void LogVerbose(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Trace,
                data);
        }

        public void LogWarning(string data)
        {
            _diagnostics.Log(
                Diagnostics.Logging.LevelToLog.Warn,
                data);
        }
    }
}
