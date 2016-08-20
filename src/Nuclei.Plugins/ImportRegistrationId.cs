//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Apollo.Utilities;
using Nuclei;

namespace Apollo.Core.Extensions.Plugins
{
    /// <summary>
    /// Defines the ID of a registration of an import.
    /// </summary>
    [Serializable]
    public sealed class ImportRegistrationId : Id<ImportRegistrationId, string>
    {
        /// <summary>
        /// The contract name for the export.
        /// </summary>
        private readonly string m_ContractName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRegistrationId"/> class.
        /// </summary>
        /// <param name="id">The ID of the import.</param>
        /// <param name="contractName">The contract name for the import.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="id"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="id"/> is an empty string.
        /// </exception>
        private ImportRegistrationId(string id, string contractName)
            : base(id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => id, Lokad.Rules.StringIs.NotEmpty);
            }

            m_ContractName = contractName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRegistrationId"/> class.
        /// </summary>
        /// <param name="owner">The type that owns the import.</param>
        /// <param name="objectIndex">The index of the object in the group.</param>
        /// <param name="contractName">The contract name for the import.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="contractName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods",
            Justification = "There is no way to validate these values before using them. We'll rely on the CLR here.")]
        public ImportRegistrationId(Type owner, int objectIndex, string contractName)
            : base(string.Format(CultureInfo.InvariantCulture, "[{0}]-[{1}]-[{2}]", owner.AssemblyQualifiedName, objectIndex, contractName))
        {
            {
                Lokad.Enforce.Argument(() => contractName);
                Lokad.Enforce.Argument(() => contractName, Lokad.Rules.StringIs.NotEmpty);
            }

            m_ContractName = contractName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRegistrationId"/> class.
        /// </summary>
        /// <param name="owner">The type that owns the import.</param>
        /// <param name="objectIndex">The index of the object in the group.</param>
        /// <param name="contractType">The contract type for the import.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods",
            Justification = "There is no way to validate these values before using them. We'll rely on the CLR here.")]
        public ImportRegistrationId(Type owner, int objectIndex, Type contractType)
            : this(owner, objectIndex, contractType.FullName)
        {
            m_ContractName = contractType.FullName;
        }

        /// <summary>
        /// Gets the contract name for the current import.
        /// </summary>
        public string ContractName
        {
            get
            {
                return m_ContractName;
            }
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override ImportRegistrationId Clone(string value)
        {
            return new ImportRegistrationId(value, m_ContractName);
        }

        /// <summary>
        /// Compares the values.
        /// </summary>
        /// <param name="ourValue">The value of the current object.</param>
        /// <param name="theirValue">The value of the object with which the current object is being compared.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="ourValue"/> is less than <paramref name="theirValue"/>.
        /// Zero
        /// <paramref name="ourValue"/> is equal to <paramref name="theirValue"/>.
        /// Greater than zero
        /// <paramref name="ourValue"/> is greater than <paramref name="theirValue"/>.
        /// </returns>
        protected override int CompareValues(string ourValue, string theirValue)
        {
            return string.Compare(ourValue, theirValue, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, @"Import registration: {0}", InternalValue);
        }
    }
}
