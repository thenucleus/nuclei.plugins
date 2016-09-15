//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Stores the serialized information for a given <see cref="Identity"/>.
    /// </summary>
    [Serializable]
    public sealed class PartDefinition
    {
        /// <summary>
        /// Gets or sets the serialized type info.
        /// </summary>
        public TypeIdentity Identity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of imports for the current type.
        /// </summary>
        public IEnumerable<SerializableImportDefinition> Imports
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of exports for the current type.
        /// </summary>
        public IEnumerable<SerializableExportDefinition> Exports
        {
            get;
            set;
        }
    }
}
