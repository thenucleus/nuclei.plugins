//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using Apollo.Utilities;
using Nuclei;

namespace Apollo.Core.Extensions.Plugins
{
    /// <summary>
    /// Defines the ID of a registration of a group of components.
    /// </summary>
    [Serializable]
    public sealed class GroupRegistrationId : Id<GroupRegistrationId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRegistrationId"/> class.
        /// </summary>
        /// <param name="id">The ID of the group.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="id"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="id"/> is an empty string.
        /// </exception>
        public GroupRegistrationId(string id)
            : base(id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => id, Lokad.Rules.StringIs.NotEmpty);
            }
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override GroupRegistrationId Clone(string value)
        {
            return new GroupRegistrationId(value);
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
            return string.Format(CultureInfo.InvariantCulture, @"Group registration: {0}", InternalValue);
        }
    }
}
