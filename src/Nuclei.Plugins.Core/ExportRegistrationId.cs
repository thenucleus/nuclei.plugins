//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Plugins.Core.Properties;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the ID of a registration of an export.
    /// </summary>
    [Serializable]
    public sealed class ExportRegistrationId : Id<ExportRegistrationId, string>
    {
        /// <summary>
        /// The contract name for the export.
        /// </summary>
        private readonly string _contractName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRegistrationId"/> class.
        /// </summary>
        /// <param name="id">The ID of the export.</param>
        /// <param name="contractName">The contract name for the export.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="id"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="id"/> is an empty string.
        /// </exception>
        private ExportRegistrationId(string id, string contractName)
            : base(id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "id");
            }

            _contractName = contractName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRegistrationId"/> class.
        /// </summary>
        /// <param name="owner">The type that owns the export.</param>
        /// <param name="objectIndex">The index of the object in the group.</param>
        /// <param name="contractName">The contract name for the export.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="contractName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            Justification = "There is no way to validate these values before using them. We'll rely on the CLR here.")]
        public ExportRegistrationId(Type owner, int objectIndex, string contractName)
            : base(string.Format(CultureInfo.InvariantCulture, "[{0}]-[{1}]-[{2}]", owner.AssemblyQualifiedName, objectIndex, contractName))
        {
            if (contractName == null)
            {
                throw new ArgumentNullException("contractName");
            }

            if (string.IsNullOrWhiteSpace(contractName))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_ParameterShouldNotBeAnEmptyString,
                    "contractName");
            }

            _contractName = contractName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRegistrationId"/> class.
        /// </summary>
        /// <param name="owner">The type that owns the export.</param>
        /// <param name="objectIndex">The index of the object in the group.</param>
        /// <param name="contractType">The contract type for the export.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            Justification = "There is no way to validate these values before using them. We'll rely on the CLR here.")]
        public ExportRegistrationId(Type owner, int objectIndex, Type contractType)
            : this(owner, objectIndex, contractType.FullName)
        {
            _contractName = contractType.FullName;
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override ExportRegistrationId Clone(string value)
        {
            return new ExportRegistrationId(value, _contractName);
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
        /// Gets the contract name for the current export.
        /// </summary>
        public string ContractName
        {
            get
            {
                return _contractName;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, @"Export registration: {0}", InternalValue);
        }
    }
}
