//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Apollo.Core.Extensions.Plugins;
using Apollo.Core.Extensions.Scheduling;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Stores information about an export for a component group in serialized form, i.e. without requiring the
    /// assembly which defines the group to be loaded.
    /// </summary>
    [Serializable]
    public sealed class GroupExportDefinition : IEquatable<GroupExportDefinition>
    {
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(GroupExportDefinition first, GroupExportDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return true;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first object.</param>
        /// <param name="second">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(GroupExportDefinition first, GroupExportDefinition second)
        {
            // Check if first is a null reference by using ReferenceEquals because
            // we overload the == operator. If first isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            {
                return false;
            }

            var nonNullObject = first;
            var possibleNullObject = second;
            if (ReferenceEquals(first, null))
            {
                nonNullObject = second;
                possibleNullObject = first;
            }

            return !nonNullObject.Equals(possibleNullObject);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="GroupExportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name for the import.</param>
        /// <param name="containingGroup">The ID of the group that has registered the import.</param>
        /// <param name="providedExports">The object exports that are provided with the current export.</param>
        /// <returns>The serialized export definition for the group.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="contractName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="containingGroup"/> is <see langword="null" />.
        /// </exception>
        public static GroupExportDefinition CreateDefinition(
            string contractName, 
            GroupRegistrationId containingGroup, 
            IEnumerable<ExportRegistrationId> providedExports)
        {
            {
                Lokad.Enforce.Argument(() => contractName);
                Lokad.Enforce.Argument(() => contractName, Lokad.Rules.StringIs.NotEmpty);
                Lokad.Enforce.Argument(() => containingGroup);
            }

            return new GroupExportDefinition(
                contractName, 
                containingGroup, 
                providedExports ?? Enumerable.Empty<ExportRegistrationId>());
        }

        /// <summary>
        /// The ID of the group that has registered the export.
        /// </summary>
        private readonly GroupRegistrationId m_Id;

        /// <summary>
        /// The contract name for the export.
        /// </summary>
        private readonly string m_ContractName;

        /// <summary>
        /// The object exports that can be used with the current export.
        /// </summary>
        private readonly IEnumerable<ExportRegistrationId> m_ProvidedExports;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupExportDefinition"/> class.
        /// </summary>
        /// <param name="contractName">The contract name for the import.</param>
        /// <param name="containingGroup">The ID of the group that has registered the import.</param>
        /// <param name="providedExports">The object exports that are provided with the current export.</param>
        private GroupExportDefinition(
            string contractName, 
            GroupRegistrationId containingGroup, 
            IEnumerable<ExportRegistrationId> providedExports)
        {
            {
                Debug.Assert(containingGroup != null, "The ID of the group registering the export should not be null.");
                Debug.Assert(!string.IsNullOrEmpty(contractName), "The contract name for the export should not be empty.");
                Debug.Assert(providedExports != null, "The collection of object exports should not be null.");
            }

            m_ContractName = contractName;
            m_Id = containingGroup;
            m_ProvidedExports = providedExports;
        }

        /// <summary>
        /// Gets the contract name for the export.
        /// </summary>
        public string ContractName
        {
            get
            {
                return m_ContractName;
            }
        }

        /// <summary>
        /// Gets the ID of the group that has registered the export.
        /// </summary>
        public GroupRegistrationId ContainingGroup
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the object exports that can be used with the current export.
        /// </summary>
        public IEnumerable<ExportRegistrationId> ProvidedExports
        {
            get
            {
                return m_ProvidedExports;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="GroupExportDefinition"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="GroupExportDefinition"/> to compare with this instance.</param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="GroupExportDefinition"/> is equal to this instance;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Equals(GroupExportDefinition other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Check if other is a null reference by using ReferenceEquals because
            // we overload the == operator. If other isn't actually null then
            // we get an infinite loop where we're constantly trying to compare to null.
            return !ReferenceEquals(other, null) && string.Equals(ContractName, other.ContractName, StringComparison.OrdinalIgnoreCase);
        }

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
        public sealed override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var id = obj as GroupExportDefinition;
            return Equals(id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            // As obtained from the Jon Skeet answer to:
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // And adapted towards the Modified Bernstein (shown here: http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx)
            //
            // Overflow is fine, just wrap
            unchecked
            {
                // Pick a random prime number
                int hash = 17;

                // Mash the hash together with yet another random prime number
                hash = (hash * 23) ^ ContractName.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Exporting [{0}] on {1}",
                ContractName,
                ContainingGroup);
        }
    }
}
