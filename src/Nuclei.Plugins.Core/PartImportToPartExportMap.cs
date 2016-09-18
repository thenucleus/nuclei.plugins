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
    /// Maps a part import to a part export.
    /// </summary>
    [Serializable]
    public sealed class PartImportToPartExportMap
    {
        /// <summary>
        /// The ID of the import.
        /// </summary>
        private readonly ImportRegistrationId _import;

        /// <summary>
        /// The collection containing the export IDs.
        /// </summary>
        private readonly IEnumerable<ExportRegistrationId> _exports;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartImportToPartExportMap"/> class.
        /// </summary>
        /// <param name="import">The ID of the import.</param>
        /// <param name="exports">The collection containing the export IDs.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="import"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="exports"/> is <see langword="null" />.
        /// </exception>
        public PartImportToPartExportMap(ImportRegistrationId import, IEnumerable<ExportRegistrationId> exports)
        {
            if (import == null)
            {
                throw new ArgumentNullException("import");
            }

            if (exports == null)
            {
                throw new ArgumentNullException("exports");
            }

            _import = import;
            _exports = exports;
        }

        /// <summary>
        /// Gets the ID of the import.
        /// </summary>
        public ImportRegistrationId Import
        {
            get
            {
                return _import;
            }
        }

        /// <summary>
        /// Gets the collection containing the exports.
        /// </summary>
        public IEnumerable<ExportRegistrationId> Exports
        {
            get
            {
                return _exports;
            }
        }
    }
}
