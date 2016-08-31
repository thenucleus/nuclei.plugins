//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Defines the different change types a part instance can be put through.
    /// </summary>
    internal enum InstanceChange
    {
        /// <summary>
        /// There was no change to the instance.
        /// </summary>
        None,

        /// <summary>
        /// A new object was created for the instance and all imports were updated.
        /// </summary>
        Reconstructed,

        /// <summary>
        /// One or more imports on the instance were changed without creating a new object for the instance.
        /// </summary>
        Updated,

        /// <summary>
        /// The instance was removed.
        /// </summary>
        Removed
    }
}
