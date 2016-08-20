//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Apollo.Core.Dataset.Properties;
using Nuclei;

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Defines an ID number for an instance of a part.
    /// </summary>
    [Serializable]
    internal sealed class PartInstanceId : Id<PartInstanceId, Guid>
    {
        /// <summary>
        /// Defines the ID number for an invalid dataset ID.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Guid s_InvalidId = new Guid("{9C5C17FA-53A1-44C2-AB65-2F155D249081}");

        /// <summary>
        /// Returns the next integer that can be used for an ID number.
        /// </summary>
        /// <returns>
        /// The next unused ID value.
        /// </returns>
        private static Guid NextIdValue()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartInstanceId"/> class.
        /// </summary>
        public PartInstanceId()
            : this(NextIdValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartInstanceId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        internal PartInstanceId(Guid id)
            : base(id)
        {
            if (id.Equals(s_InvalidId))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_PartInstanceIdCannotBeTheInvalidId,
                    "id");
            }
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override PartInstanceId Clone(Guid value)
        {
            var result = new PartInstanceId(value);
            return result;
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
                "PartInstanceId: [{0}]",
                InternalValue);
        }
    }
}
