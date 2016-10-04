//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Plugins.Discovery
{
    /// <summary>
    /// Defines the signature of a function that will create a new <see cref="AppDomain"/>.
    /// </summary>
    /// <param name="name">The name of the appdomain.</param>
    /// <param name="assemblyDirectories">
    ///     The collection of directory paths that should be included in the assembly search path.
    /// </param>
    /// <returns>
    ///     The newly created <see cref="AppDomain"/>.
    /// </returns>
    public delegate AppDomain CreateAppDomain(string name, string[] assemblyDirectories);
}
