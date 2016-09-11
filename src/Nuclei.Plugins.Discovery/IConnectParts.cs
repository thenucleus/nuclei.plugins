﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the interface for objects that assist in connecting parts.
    /// </summary>
    internal interface IConnectParts
    {
        /// <summary>
        /// Returns a value indicating if the given import would accept the given export.
        /// </summary>
        /// <param name="importDefinition">The import definition.</param>
        /// <param name="exportDefinition">The export definition.</param>
        /// <returns>
        ///     <see langword="true" /> if the given import would accept the given export; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool Accepts(SerializableImportDefinition importDefinition, SerializableExportDefinition exportDefinition);
    }
}
