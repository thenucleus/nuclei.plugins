//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins.Composition
{
    /// <summary>
    /// Defines helper methods for handling metadata collections.
    /// </summary>
    public static class MetadataServices
    {
        /// <summary>
        /// Defines an empty metadata collection.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "The dictionary is a readonly instance.")]
        public static readonly ReadOnlyDictionary<string, object> EmptyMetadata =
            new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(0));

        /// <summary>
        /// Returns a read only view of the metadata collection.
        /// </summary>
        /// <param name="metadata">The original metadata collection.</param>
        /// <returns>A readonly view of the collection.</returns>
        public static ReadOnlyDictionary<string, object> AsReadOnly(this IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                return EmptyMetadata;
            }

            var readonlyMetadata = metadata as ReadOnlyDictionary<string, object>;
            if (readonlyMetadata != null)
            {
                return readonlyMetadata;
            }

            return new ReadOnlyDictionary<string, object>(metadata);
        }
    }
}
