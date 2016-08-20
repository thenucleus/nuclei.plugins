//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Nuclei.Properties;
using Nuclei;

namespace Nuclei.Plugins
{
    /// <summary>
    /// Defines an ID number for a part group that is included in a composition graph.
    /// </summary>
    [Serializable]
    public sealed class GroupCompositionId : Id<GroupCompositionId, Guid>
    {
        /// <summary>
        /// Defines the ID number for an invalid dataset ID.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Guid s_InvalidId = new Guid("{E4FBEA47-5E38-4A3D-9556-4EA7AF55B974}");

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
        /// Initializes a new instance of the <see cref="GroupCompositionId"/> class.
        /// </summary>
        public GroupCompositionId()
            : this(NextIdValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCompositionId"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        internal GroupCompositionId(Guid id)
            : base(id)
        {
            if (id.Equals(s_InvalidId))
            {
                throw new ArgumentException(
                    Resources.Exceptions_Messages_GroupCompositionIdCannotBeTheInvalidId,
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
        protected override GroupCompositionId Clone(Guid value)
        {
            var result = new GroupCompositionId(value);
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
                "GroupCompositionId: [{0}]",
                InternalValue);
        }
    }
}
