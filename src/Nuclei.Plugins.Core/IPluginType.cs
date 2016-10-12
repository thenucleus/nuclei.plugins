//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Core
{
    /// <summary>
    /// Defines the interface for classes that define a specific type of plugin, e.g. a plugin file or a given type.
    /// </summary>
    public interface IPluginType : IEquatable<IPluginType>
    {
    }
}
