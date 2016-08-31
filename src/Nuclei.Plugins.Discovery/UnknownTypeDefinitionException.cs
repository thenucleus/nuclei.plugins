﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Apollo.Core.Host.Properties;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// An exception thrown when the user tries to get a type definition from the repository with a type identity that does not exist.
    /// </summary>
    [Serializable]
    public sealed class UnknownTypeDefinitionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTypeDefinitionException"/> class.
        /// </summary>
        public UnknownTypeDefinitionException()
            : this(Resources.Exceptions_Messages_UnknownTypeDefinition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTypeDefinitionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownTypeDefinitionException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTypeDefinitionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownTypeDefinitionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownTypeDefinitionException"/> class.
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
        private UnknownTypeDefinitionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
