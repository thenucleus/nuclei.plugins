//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Nuclei.Plugins.Core.Properties;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// An exception thrown if an invalid <see cref="PluginOrigin"/> type is passed to a <see cref="ILoadTypesFromPlugins"/> instance.
    /// </summary>
    [Serializable]
    public sealed class InvalidPluginOriginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPluginOriginException"/> class.
        /// </summary>
        public InvalidPluginOriginException()
            : this(Resources.Exceptions_Messages_InvalidPluginOrigin)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPluginOriginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidPluginOriginException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPluginOriginException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidPluginOriginException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPluginOriginException"/> class.
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
        private InvalidPluginOriginException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
