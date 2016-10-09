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
    /// An exception thrown when the user tries to add a part definition to the repository that is already registered.
    /// </summary>
    [Serializable]
    public sealed class DuplicateDiscoverableMemberException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDiscoverableMemberException"/> class.
        /// </summary>
        public DuplicateDiscoverableMemberException()
            : this(Resources.Exceptions_Messages_DuplicateDiscoverableMember)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDiscoverableMemberException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DuplicateDiscoverableMemberException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDiscoverableMemberException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DuplicateDiscoverableMemberException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateDiscoverableMemberException"/> class.
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
        private DuplicateDiscoverableMemberException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
