//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins
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
        /// The name of the contract for the import.
        /// </summary>
        private readonly string m_ContractName;

        /// <summary>
        /// The type identity of the export type expected.
        /// </summary>
        private readonly TypeIdentity m_RequiredTypeIdentity;

        /// <summary>
        /// The import cardinality for the import.
        /// </summary>
        private readonly ImportCardinality m_Cardinality;

        /// <summary>
        /// A flag indicating if it is possible to provide values for the import multiple times during the lifetime of 
        /// the part.
        /// </summary>
        private readonly bool m_IsRecomposable;

        /// <summary>
        /// A flag indicating if the import has to be satisfied before the creation of the part.
        /// </summary>
        private readonly bool m_IsPreRequisite;

        /// <summary>
        /// The creation policy for the import.
        /// </summary>
        private readonly CreationPolicy m_CreationPolicy;

        /// <summary>
        /// The serialized description of the type that declares the current import.
        /// </summary>
        private readonly TypeIdentity m_DeclaringType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableImportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name that is used to identify the current import.</param>
        /// <param name="requiredTypeIdentity">The type identity of the export type expected.</param>
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
        /// <param name="creationPolicy">
        ///     A value that indicates that the importer requires a specific creation policy for the exports used to satisfy this import.
        /// </param>
        /// <param name="declaringType">The type that declares the current import.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="declaringType"/> is <see langword="null" />.
        /// </exception>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Recomposable",
            Justification = "MEF uses the same term, so we're not going to make up some other one.")]
        protected SerializableImportDefinition(
            string contractName,
            TypeIdentity requiredTypeIdentity,
            ImportCardinality cardinality, 
            bool isRecomposable, 
            bool isPrerequisite,
            CreationPolicy creationPolicy,
            TypeIdentity declaringType)
        {
            {
                Lokad.Enforce.Argument(() => declaringType);
            }

            m_ContractName = contractName;
            m_RequiredTypeIdentity = requiredTypeIdentity;
            m_Cardinality = cardinality;
            m_IsRecomposable = isRecomposable;
            m_IsPreRequisite = isPrerequisite;
            m_CreationPolicy = creationPolicy;
            m_DeclaringType = declaringType;
        }

        /// <summary>
        /// Gets the contract name for the import.
        /// </summary>
        public string ContractName
        {
            get
            {
                return m_ContractName;
            }
        }

        /// <summary>
        /// Gets the type identity of the export type expected.
        /// </summary>
        public TypeIdentity RequiredTypeIdentity
        {
            get 
            {
                return m_RequiredTypeIdentity;
            }
        }

        /// <summary>
        /// Gets the cardinality of the import.
        /// </summary>
        public ImportCardinality Cardinality
        {
            get
            {
                return m_Cardinality;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the import can be satisfied multiple times during the lifetime of
        /// a part.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Recomposable",
            Justification = "MEF uses the same term.")]
        public bool IsRecomposable
        {
            get
            {
                return m_IsRecomposable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the import should be satisfied before parts can be produced.
        /// </summary>
        public bool IsPrerequisite
        {
            get
            {
                return m_IsPreRequisite;
            }
        }

        /// <summary>
        /// Gets the creation policy for the import.
        /// </summary>
        public CreationPolicy RequiredCreationPolicy
        {
            get
            {
                return m_CreationPolicy;
            }
        }

        /// <summary>
        /// Gets the serialized definition of the type that declares the import.
        /// </summary>
        public TypeIdentity DeclaringType
        {
            get
            {
                return m_DeclaringType;
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
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public abstract bool Equals(SerializableImportDefinition other);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
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
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
