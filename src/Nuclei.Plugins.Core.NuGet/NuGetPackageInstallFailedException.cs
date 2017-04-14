//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using Nuclei.Plugins.Core.NuGet.Properties;
using NuGet.Packaging.Core;

namespace Nuclei.Plugins.Core.NuGet
{
    /// <summary>
    /// An exception thrown when a NuGet package fails to install.
    /// </summary>
    [Serializable]
    public sealed class NuGetPackageInstallFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstallFailedException"/> class.
        /// </summary>
        public NuGetPackageInstallFailedException()
            : base(Resources.Exceptions_Messages_NuGetPackageFailedToInstall)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstallFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NuGetPackageInstallFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstallFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NuGetPackageInstallFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstallFailedException"/> class.
        /// </summary>
        /// <param name="identity">The identity of the package.</param>
        /// <param name="innerException">The inner exception.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "There is no way to validate this before we use it.")]
        public NuGetPackageInstallFailedException(PackageIdentity identity, Exception innerException)
            : base(
                  string.Format(
                      CultureInfo.InvariantCulture,
                      Resources.Exceptions_Messages_NuGetPackageFailedToInstall_WithId,
                      identity.Id,
                      identity.Version),
                  innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstallFailedException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
        ///     about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private NuGetPackageInstallFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
