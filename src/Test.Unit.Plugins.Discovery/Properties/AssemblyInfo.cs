﻿//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using Nuclei.Build;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Test.Unit.Plugins.Discovery")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyProduct("Test.Unit.Plugins.Discovery")]

[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("Copyright ©  2013")]

[assembly: AssemblyCulture("")]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTrademark("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Indicate that the assembly is CLS compliant.
[assembly: CLSCompliant(true)]

[assembly: AssemblyConfiguration("Release")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]

// The time the assembly was build
[assembly: AssemblyBuildTime(buildTime: "1900-01-01T00:00:00.0000000+00:00")]

// The version from which the assembly was build
[module: SuppressMessage(
    "Microsoft.Usage",
    "CA2243:AttributeStringLiteralsShouldParseCorrectly",
    Justification = "It's a VCS revision, not a version")]
[assembly: AssemblyBuildInformation(buildNumber: 0, versionControlInformation: "1234567890123456789012345678901234567890")]
