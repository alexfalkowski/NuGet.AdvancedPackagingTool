﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuGet.Enterprise.Core.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuGet.Enterprise.Core.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Could not get directory name from path &apos;{0}&apos;..
        /// </summary>
        internal static string DirectoryNameErrorMessage {
            get {
                return ResourceManager.GetString("DirectoryNameErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find &apos;{0}&apos; in the tools folder of the package..
        /// </summary>
        internal static string FileNameDoesNotExistInToolsFolder {
            get {
                return ResourceManager.GetString("FileNameDoesNotExistInToolsFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get file name name from path &apos;{0}&apos;..
        /// </summary>
        internal static string FileNameErrorMessage {
            get {
                return ResourceManager.GetString("FileNameErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid execution of the package manager..
        /// </summary>
        internal static string InvalidExecutionErrorMessage {
            get {
                return ResourceManager.GetString("InvalidExecutionErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid execution of the package manager, please check the logs..
        /// </summary>
        internal static string InvalidExecutionExceptionMessage {
            get {
                return ResourceManager.GetString("InvalidExecutionExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package &apos;{0}&apos; does not contain a projectUrl in the spec file. This is used as the destination folder..
        /// </summary>
        internal static string InvalidInstallationFolder {
            get {
                return ResourceManager.GetString("InvalidInstallationFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Install or uninstall was not specified..
        /// </summary>
        internal static string InvalidInstallUninstallFlag {
            get {
                return ResourceManager.GetString("InvalidInstallUninstallFlag", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package &apos;{0}&apos; cannot be found in repository &apos;{1}&apos;..
        /// </summary>
        internal static string InvalidPackage {
            get {
                return ResourceManager.GetString("InvalidPackage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package &apos;{0}&apos; with version &apos;{1}&apos; cannot be found in repository &apos;{2}&apos;..
        /// </summary>
        internal static string InvalidPackageWithVersion {
            get {
                return ResourceManager.GetString("InvalidPackageWithVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package was not specified..
        /// </summary>
        internal static string PackageNotSpecified {
            get {
                return ResourceManager.GetString("PackageNotSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The package source file &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string PackagesSourceFileDoesNotExixst {
            get {
                return ResourceManager.GetString("PackagesSourceFileDoesNotExixst", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The script &apos;{0}&apos; contained the following errors &apos;{1}&apos;..
        /// </summary>
        internal static string PowershellErrorMessage {
            get {
                return ResourceManager.GetString("PowershellErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The source was not specified..
        /// </summary>
        internal static string SourceNotSpecified {
            get {
                return ResourceManager.GetString("SourceNotSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The spec file does not exist..
        /// </summary>
        internal static string SpecFileNotFound {
            get {
                return ResourceManager.GetString("SpecFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get target framework from path &apos;{0}&apos;..
        /// </summary>
        internal static string TargetFrameworkErrorMessage {
            get {
                return ResourceManager.GetString("TargetFrameworkErrorMessage", resourceCulture);
            }
        }
    }
}
