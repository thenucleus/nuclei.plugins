//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Plugins.Core.NuGet;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using NUnit.Framework;

namespace Nuclei.Plugins.Core.Assembly
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class PluginNuGetTypeLoaderTest
    {
        [Test]
        public void Load()
        {
            var type = typeof(TypeLoader);
            var origin = new PluginAssemblyOrigin(@"c:\temp\myassembly.dll");

            var loader = new PluginAssemblyTypeLoader();
            var loadedType = loader.Load(origin, type.AssemblyQualifiedName);
            Assert.AreEqual(type, loadedType);
        }

        [Test]
        public void LoadWithInvalidOriginType()
        {
            var type = typeof(TypeLoader);
            var origin = new PluginNuGetOrigin(new PackageIdentity("a", new NuGetVersion(1, 2, 3)));

            var loader = new PluginAssemblyTypeLoader();
            Assert.Throws<InvalidPluginOriginException>(() => loader.Load(origin, type.AssemblyQualifiedName));
        }

        [Test]
        public void LoadWithNullOrigin()
        {
            var type = typeof(TypeLoader);

            var loader = new PluginAssemblyTypeLoader();
            Assert.Throws<InvalidPluginOriginException>(() => loader.Load(null, type.AssemblyQualifiedName));
        }

        [Test]
        public void ValidOriginType()
        {
            var loader = new PluginAssemblyTypeLoader();
            Assert.AreEqual(typeof(PluginAssemblyOrigin), loader.ValidOriginType);
        }
    }
}
