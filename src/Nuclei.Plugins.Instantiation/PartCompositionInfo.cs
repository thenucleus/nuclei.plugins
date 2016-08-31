﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Apollo.Core.Base.Plugins;

namespace Nuclei.Plugins.Instantiation
{
    /// <summary>
    /// Stores information about a composed part.
    /// </summary>
    internal sealed class PartCompositionInfo
    {
        /// <summary>
        /// The definition of the part.
        /// </summary>
        private readonly GroupPartDefinition m_Definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartCompositionInfo"/> class.
        /// </summary>
        /// <param name="definition">The definition of the part.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="definition"/> is <see langword="null" />.
        /// </exception>
        public PartCompositionInfo(GroupPartDefinition definition)
        {
            {
                Lokad.Enforce.Argument(() => definition);
            }

            m_Definition = definition;
        }

        /// <summary>
        /// Gets the definition of the part.
        /// </summary>
        public GroupPartDefinition Definition
        {
            get
            {
                return m_Definition;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the instance of the current part.
        /// </summary>
        public PartInstanceId Instance
        {
            get;
            set;
        }
    }
}
