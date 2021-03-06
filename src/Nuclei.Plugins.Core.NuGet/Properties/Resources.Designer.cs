﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nuclei.Plugins.Core.NuGet.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Nuclei.Plugins.Core.NuGet.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot compare these two objects, their types do not match [this: {0}; other: {1}].
        /// </summary>
        internal static string Exceptions_Messages_CompareArgument_WithTypes {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CompareArgument_WithTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given NuGet package failed to install..
        /// </summary>
        internal static string Exceptions_Messages_NuGetPackageFailedToInstall {
            get {
                return ResourceManager.GetString("Exceptions_Messages_NuGetPackageFailedToInstall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given NuGet package {0} [{1}] failed to install..
        /// </summary>
        internal static string Exceptions_Messages_NuGetPackageFailedToInstall_WithId {
            get {
                return ResourceManager.GetString("Exceptions_Messages_NuGetPackageFailedToInstall_WithId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter should not be an empty string..
        /// </summary>
        internal static string Exceptions_Messages_ParameterShouldNotBeAnEmptyString {
            get {
                return ResourceManager.GetString("Exceptions_Messages_ParameterShouldNotBeAnEmptyString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installing package {0} from sources
        ///{1}.
        /// </summary>
        internal static string LogMessage_NuGetPackageInstaller_InstallingPackageFromSources_WithPackageIdAndSources {
            get {
                return ResourceManager.GetString("LogMessage_NuGetPackageInstaller_InstallingPackageFromSources_WithPackageIdAndSou" +
                        "rces", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Package {0} is already present in the output directory..
        /// </summary>
        internal static string LogMessage_NuGetPackageInstaller_PackageExists_WithPackageId {
            get {
                return ResourceManager.GetString("LogMessage_NuGetPackageInstaller_PackageExists_WithPackageId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File in package {0} [{1}] at {2} already exists in destination at {3}..
        /// </summary>
        internal static string LogMessage_PackageUtilities_AssemblyFileAlreadyExistsAtDestination_WithPackageIdAndVersionAndOriginAndDestination {
            get {
                return ResourceManager.GetString("LogMessage_PackageUtilities_AssemblyFileAlreadyExistsAtDestination_WithPackageIdA" +
                        "ndVersionAndOriginAndDestination", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Copying file in package {0} [{1}] from {2} to {3}..
        /// </summary>
        internal static string LogMessage_PackageUtilities_CopyingAssemblyFile_WithPackageIdAndVersionAndOriginAndDestination {
            get {
                return ResourceManager.GetString("LogMessage_PackageUtilities_CopyingAssemblyFile_WithPackageIdAndVersionAndOriginA" +
                        "ndDestination", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Adding the package destination folder at {0} to the assembly resolution list..
        /// </summary>
        internal static string LogMessage_PluginNuGetTypeLoader_AddingDestinationToAssemblyResolutionFolderList_WithDestination {
            get {
                return ResourceManager.GetString("LogMessage_PluginNuGetTypeLoader_AddingDestinationToAssemblyResolutionFolderList_" +
                        "WithDestination", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to remove the package destination directory at {0}. Error was: {1}.
        /// </summary>
        internal static string LogMessage_PluginNuGetTypeLoader_FailedToDeleteDestination_WithDestinationAndException {
            get {
                return ResourceManager.GetString("LogMessage_PluginNuGetTypeLoader_FailedToDeleteDestination_WithDestinationAndExce" +
                        "ption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to install package {0} [{1}] to {2}. Error was: {3}..
        /// </summary>
        internal static string LogMessage_PluginNuGetTypeLoader_FailedToInstallPackage_WithPackageNameAndVersionAndDestinationAndException {
            get {
                return ResourceManager.GetString("LogMessage_PluginNuGetTypeLoader_FailedToInstallPackage_WithPackageNameAndVersion" +
                        "AndDestinationAndException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Installing package {0} [{1}] and copying binaries to {2}..
        /// </summary>
        internal static string LogMessage_PluginNuGetTypeLoader_InstallingPackage_WithPackageNameAndVersionAndDestination {
            get {
                return ResourceManager.GetString("LogMessage_PluginNuGetTypeLoader_InstallingPackage_WithPackageNameAndVersionAndDe" +
                        "stination", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removing the package destination directory at {0}..
        /// </summary>
        internal static string LogMessage_PluginNuGetTypeLoader_RemovingDestination_WithDestination {
            get {
                return ResourceManager.GetString("LogMessage_PluginNuGetTypeLoader_RemovingDestination_WithDestination", resourceCulture);
            }
        }
    }
}
