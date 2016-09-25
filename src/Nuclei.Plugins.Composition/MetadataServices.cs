﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public static readonly IDictionary<string, object> EmptyMetadata =
            new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(0));

        /// <summary>
        /// Returns a read only view of the metadata collection.
        /// </summary>
        /// <param name="metadata">The original metadata collection.</param>
        /// <returns>A readonly view of the collection.</returns>
        public static IDictionary<string, object> AsReadOnly(this IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                return EmptyMetadata;
            }

            if (metadata is ReadOnlyDictionary<string, object>)
            {
                return metadata;
            }

            return new ReadOnlyDictionary<string, object>(metadata);
        }

        public static T GetValue<T>(this IDictionary<string, object> metadata, string key)
        {
            Assumes.NotNull<IDictionary<string, object>, string>(metadata, "metadata");
            object obj = (object)null;
            if (!metadata.TryGetValue(key, out obj))
                return default(T);
            if (obj is T)
                return (T)obj;
            return default(T);
        }
    }
}
