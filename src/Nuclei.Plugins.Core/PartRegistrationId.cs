//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the ID for the registration of an object in an object group.
    /// </summary>
    [Serializable]
    public sealed class PartRegistrationId : Id<PartRegistrationId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartRegistrationId"/> class.
        /// </summary>
        /// <param name="id">The text value that forms the ID.</param>
        private PartRegistrationId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartRegistrationId"/> class.
        /// </summary>
        /// <param name="typeName">The type of object for which this ID is valid.</param>
        /// <param name="number">The index of the object in the owning group.</param>
        public PartRegistrationId(string typeName, int number)
            : this(string.Format(CultureInfo.InvariantCulture, "{0}-{1}", typeName, number))
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override PartRegistrationId Clone(string value)
        {
            return new PartRegistrationId(value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Object registered with Id: [{0}]",
                InternalValue);
        }
    }
}
