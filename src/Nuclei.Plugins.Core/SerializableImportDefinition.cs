//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Provides the base class for classes that store information about a MEF import in a serializable form, i.e. without
    /// requiring the owning type to be loaded.
    /// </summary>
    /// <design>
    /// It would be nice if this class would derrive from the <c>System.ComponentModel.Composition.Primitives.ImportDefinition</c> but
    /// that class is not serializable.
    /// </design>
    [Serializable]
    public abstract class SerializableImportDefinition : IEquatable<SerializableImportDefinition>
    {
        /// <summary>
        /// The import cardinality for the import.
        /// </summary>
        private readonly ImportCardinality _cardinality;

        /// <summary>
        /// The name of the contract for the import.
        /// </summary>
        private readonly string _contractName;

        /// <summary>
        /// The creation policy for the import.
        /// </summary>
        private readonly CreationPolicy _creationPolicy;

        /// <summary>
        /// The serialized description of the type that declares the current import.
        /// </summary>
        private readonly TypeIdentity _declaringType;

        /// <summary>
        /// A flag that indicates if the import is an export factory.
        /// </summary>
        private readonly bool _isExportFactory;

        /// <summary>
        /// A flag indicating if the import has to be satisfied before the creation of the part.
        /// </summary>
        private readonly bool _isPreRequisite;

        /// <summary>
        /// A flag indicating if it is possible to provide values for the import multiple times during the lifetime of
        /// the part.
        /// </summary>
        private readonly bool _isRecomposable;

        /// <summary>
        /// The type identity of the export type expected.
        /// </summary>
        private readonly TypeIdentity _requiredTypeIdentity;

        /// <summary>
        /// The type identity of the export type as expected by MEF.
        /// </summary>
        private readonly string _requiredTypeIdentityForMef;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableImportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
        /// <param name="requiredTypeIdentityForMef">The type identity of the export type as expected by MEF.</param>
        /// <param name="cardinality">
        ///     One of the enumeration values that indicates the cardinality of the export object required by the import definition.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true" /> to specify that the import definition can be satisfied multiple times throughout the lifetime of a parts;
        ///     otherwise, <see langword="false" />.
        /// </param>
        /// <param name="isPrerequisite">
        ///     <see langword="true" /> to specify that the import definition is required to be satisfied before a part can start producing exported
        ///     objects; otherwise, <see langword="false" />.
        /// </param>
        /// <param name="isExportFactory">
        ///     <see langword="true" /> to specify that the import definition requires an <see cref="ExportFactory{T}"/> or
        ///     <see cref="ExportFactory{T, TMetadata}"/> instance; otherwise, <see langword="false" />.
        /// </param>
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="declaringType">The type that declares the current import.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="declaringType"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Recomposable",
            Justification = "MEF uses the same term, so we're not going to make up some other one.")]
        protected SerializableImportDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            string requiredTypeIdentityForMef,
            ImportCardinality cardinality,
            bool isRecomposable,
            bool isPrerequisite,
            bool isExportFactory,
            CreationPolicy creationPolicy,
            TypeIdentity declaringType)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException("declaringType");
            }

            _cardinality = cardinality;
            _contractName = contractName;
            _creationPolicy = creationPolicy;
            _declaringType = declaringType;
            _isExportFactory = isExportFactory;
            _isPreRequisite = isPrerequisite;
            _isRecomposable = isRecomposable;
            _requiredTypeIdentity = requiredTypeIdentity;
            _requiredTypeIdentityForMef = requiredTypeIdentityForMef;
        }

        /// <summary>
        /// Gets the cardinality of the import.
        /// </summary>
        public ImportCardinality Cardinality
        {
            get
            {
                return _cardinality;
            }
        }

        /// <summary>
        /// Gets the contract name for the import.
        /// </summary>
        public string ContractName
        {
            get
            {
                return _contractName;
            }
        }

        /// <summary>
        /// Gets the serialized definition of the type that declares the import.
        /// </summary>
        public TypeIdentity DeclaringType
        {
            get
            {
                return _declaringType;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SerializableImportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SerializableImportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="SerializableImportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public abstract bool Equals(SerializableImportDefinition other);

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
        /// Gets a value indicating whether the import requires an <see cref="ExportFactory{T}"/> or <see cref="ExportFactory{T, TMetadata}"/>
        /// instance.
        /// </summary>
        public bool IsExportFactory
        {
            get
            {
                return _isExportFactory;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the import should be satisfied before parts can be produced.
        /// </summary>
        public bool IsPrerequisite
        {
            get
            {
                return _isPreRequisite;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the import can be satisfied multiple times during the lifetime of
        /// a part.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Recomposable",
            Justification = "MEF uses the same term.")]
        public bool IsRecomposable
        {
            get
            {
                return _isRecomposable;
            }
        }

        /// <summary>
        /// Gets the creation policy for the import.
        /// </summary>
        public CreationPolicy RequiredCreationPolicy
        {
            get
            {
                return _creationPolicy;
            }
        }

        /// <summary>
        /// Gets the type identity of the export type expected.
        /// </summary>
        public TypeIdentity RequiredTypeIdentity
        {
            get
            {
                return _requiredTypeIdentity;
            }
        }

        /// <summary>
        /// Gets the type identify of the export as determined by MEF.
        /// </summary>
        public string RequiredTypeIdentityForMef
        {
            get
            {
                return _requiredTypeIdentityForMef;
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
