﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CodeChops.FoodChops.App.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class General {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal General() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CodeChops.FoodChops.App.Resources.General", typeof(General).Assembly);
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
        ///   Looks up a localized string similar to © CodeChops / third parties.
        /// </summary>
        public static string Copyright {
            get {
                return ResourceManager.GetString("Copyright", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Click here to eject all coins..
        /// </summary>
        public static string ExplanationInsertedCoins {
            get {
                return ResourceManager.GetString("ExplanationInsertedCoins", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Press the user&apos;s coins to insert them into the machine..
        /// </summary>
        public static string ExplanationUserCoins {
            get {
                return ResourceManager.GetString("ExplanationUserCoins", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ▼ Scroll down to select the coins ▼.
        /// </summary>
        public static string SeeBelow {
            get {
                return ResourceManager.GetString("SeeBelow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An optimized solution to the &lt;a href=&quot;https://en.wikipedia.org/wiki/Change-making_problem&quot; target=&quot;_blank&quot;&gt;vending machine / change making problem&lt;/a&gt;.&lt;br/&gt;This screen is shared between all users of this app..
        /// </summary>
        public static string Subtitle {
            get {
                return ResourceManager.GetString("Subtitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FoodChops.
        /// </summary>
        public static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
    }
}
