﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CliverBotCustomization.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    public sealed partial class Proxy : global::System.Configuration.ApplicationSettingsBase {
        
        private static Proxy defaultInstance = ((Proxy)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Proxy())));
        
        public static Proxy Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int MaxAttemptCountWithNewProxy {
            get {
                return ((int)(this["MaxAttemptCountWithNewProxy"]));
            }
            set {
                this["MaxAttemptCountWithNewProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ProxiesFileUri {
            get {
                return ((string)(this["ProxiesFileUri"]));
            }
            set {
                this["ProxiesFileUri"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ProxyLogin {
            get {
                return ((string)(this["ProxyLogin"]));
            }
            set {
                this["ProxyLogin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ProxyPassword {
            get {
                return ((string)(this["ProxyPassword"]));
            }
            set {
                this["ProxyPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HTTP")]
        public global::Cliver.ProxyType ProxyType {
            get {
                return ((global::Cliver.ProxyType)(this["ProxyType"]));
            }
            set {
                this["ProxyType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int ReloadProxyFileInSeconds {
            get {
                return ((int)(this["ReloadProxyFileInSeconds"]));
            }
            set {
                this["ReloadProxyFileInSeconds"] = value;
            }
        }
    }
}
