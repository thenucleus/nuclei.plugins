//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Provides the base class for classes that store information about a discoverable member, as marked by a
    /// <see cref="DiscoverableMemberAttribute"/> in a serializable form, i.e. without requiring the owning type to be loaded.
    /// </summary>
    [Serializable]
    public abstract class SerializableDiscoverableMemberDefinition : IEquatable<SerializableDiscoverableMemberDefinition>
    {
        /// <summary>
        /// The serialized description of the type that owns the current member.
        /// </summary>
        private readonly TypeIdentity _declaringType;

        /// <summary>
        /// The collection of metadata attached to the member.
        /// </summary>
        private readonly ReadOnlyDictionary<string, string> _metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDiscoverableMemberDefinition"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares the current export.</param>
        /// <param name="metadata">The collection of metadata attached to the member.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="declaringType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="metadata"/> is <see langword="null" />.
        /// </exception>
        protected SerializableDiscoverableMemberDefinition(
            TypeIdentity declaringType,
            IDictionary<string, string> metadata)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException("declaringType");
            }

            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            _declaringType = declaringType;
            _metadata = new ReadOnlyDictionary<string, string>(metadata);
        }

        /// <summary>
        /// Gets the serialized definition of the type that declares the export.
        /// </summary>
        public TypeIdentity DeclaringType
        {
            get
            {
                return _declaringType;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializableDiscoverableMemberDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializableDiscoverableMemberDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializableDiscoverableMemberDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public abstract bool Equals(SerializableDiscoverableMemberDefinition other);

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public abstract override bool Equals(object obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Gets the collection of metadata for the member.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata
        {
            get
            {
                return _metadata;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
