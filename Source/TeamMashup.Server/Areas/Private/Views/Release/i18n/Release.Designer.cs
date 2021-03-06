﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TeamMashup.Server.Areas.Private.Views.Release.i18n
{


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
    public class Release {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Release() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TeamMashup.Server.Areas.Private.Views.Release.i18n.Release", typeof(Release).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mark this Release as Completed.
        /// </summary>
        public static string MarkThisReleaseAsCompleted {
            get {
                return ResourceManager.GetString("MarkThisReleaseAsCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no Active Release!.
        /// </summary>
        public static string NoActiveRelease {
            get {
                return ResourceManager.GetString("NoActiveRelease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There is no Active Iteration in this Release!.
        /// </summary>
        public static string ReleasePlanNoActiveIterationForRelease {
            get {
                return ResourceManager.GetString("ReleasePlanNoActiveIterationForRelease", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No user stories selected. Drag and drop here!.
        /// </summary>
        public static string ReleasePlanNoIssuesSelected {
            get {
                return ResourceManager.GetString("ReleasePlanNoIssuesSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All the issues in the iteration will be marked as &quot;Commited&quot;.
        /// </summary>
        public static string StartIterationAllIssuesCommited {
            get {
                return ResourceManager.GetString("StartIterationAllIssuesCommited", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All members of the project will be notified.
        /// </summary>
        public static string StartIterationAllMembersNotified {
            get {
                return ResourceManager.GetString("StartIterationAllMembersNotified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to start.
        /// </summary>
        public static string StartIterationAreYouSure {
            get {
                return ResourceManager.GetString("StartIterationAreYouSure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Take into account that if you decide to close the iteration prematurely, the change will be reflected in reports.
        /// </summary>
        public static string StartIterationWarningFooter {
            get {
                return ResourceManager.GetString("StartIterationWarningFooter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you start the Iteration, the following will occur.
        /// </summary>
        public static string StartIterationWarningHeader {
            get {
                return ResourceManager.GetString("StartIterationWarningHeader", resourceCulture);
            }
        }
    }
}
