//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Provides the base class for classes that store information about a MEF export in a serializable form, i.e. without
    /// requiring the owning type to be loaded.
    /// </summary>
    /// <design>
    /// It would be nice if this class would derrive from the <c>System.ComponentModel.Composition.Primitives.ExportDefinition</c> but
    /// that class is not serializable.
    /// </design>
    [Serializable]
    public abstract class SerializableExportDefinition : IEquatable<SerializableExportDefinition>
    {
        /// <summary>
        /// The name of the contract for the export.
        /// </summary>
        private readonly string _contractName;

        /// <summary>
        /// The serialized description of the type that owns the current export.
        /// </summary>
        private readonly TypeIdentity _declaringType;

        /// <summary>
        /// The type identity of the export type as provided by MEF.
        /// </summary>
        private readonly string _exportedTypeIdentityForMef;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableExportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current export.</param>
        /// <param name="exportedTypeIdentityForMef">The type identity that is exported as provided by MEF.</param>
        /// <param name="declaringType">The type that declares the current export.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="declaringType"/> is <see langword="null" />.
        /// </exception>
        protected SerializableExportDefinition(
            string contractName,
            string exportedTypeIdentityForMef,
            TypeIdentity declaringType)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException("declaringType");
            }

            _contractName = contractName;
            _declaringType = declaringType;
            _exportedTypeIdentityForMef = exportedTypeIdentityForMef;
        }

        /// <summary>
        /// Gets the contract name for the export.
        /// </summary>
        public string ContractName
        {
            get
            {
                return _contractName;
            }
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
        /// Determines whether the specified <see cref="SerializableExportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializableExportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializableExportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public abstract bool Equals(SerializableExportDefinition other);

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
        /// Gets the exported type identity as provided by MEF
        /// </summary>
        public string ExportTypeIdentityForMef
        {
            get
            {
                return _exportedTypeIdentityForMef;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
