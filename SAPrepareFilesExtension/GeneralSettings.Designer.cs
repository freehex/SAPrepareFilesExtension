﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAPrepareFilesExtension {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class GeneralSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static GeneralSettings defaultInstance = ((GeneralSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new GeneralSettings())));
        
        public static GeneralSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%ProjectPath%\\..\\ToProduction")]
        public string ResultFolderPath {
            get {
                return ((string)(this["ResultFolderPath"]));
            }
            set {
                this["ResultFolderPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%ProjectPath%\\SQL\\_DeploymentScripts\\OngoingChanges_FullScripts")]
        public string FullScriptsPath {
            get {
                return ((string)(this["FullScriptsPath"]));
            }
            set {
                this["FullScriptsPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%Date%_[%TaskNums%]_fixes")]
        public string DefaultZipName {
            get {
                return ((string)(this["DefaultZipName"]));
            }
            set {
                this["DefaultZipName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CopyMailBodyToClipboard {
            get {
                return ((bool)(this["CopyMailBodyToClipboard"]));
            }
            set {
                this["CopyMailBodyToClipboard"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowStackTrace {
            get {
                return ((bool)(this["ShowStackTrace"]));
            }
            set {
                this["ShowStackTrace"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%Date:yyyyMMdd_HH-mm-ss%")]
        public string ResultSubFoldersName {
            get {
                return ((string)(this["ResultSubFoldersName"]));
            }
            set {
                this["ResultSubFoldersName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AddProjectFolderInZip {
            get {
                return ((bool)(this["AddProjectFolderInZip"]));
            }
            set {
                this["AddProjectFolderInZip"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DeleteTempFilesAfterArchiving {
            get {
                return ((bool)(this["DeleteTempFilesAfterArchiving"]));
            }
            set {
                this["DeleteTempFilesAfterArchiving"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>Please deploy on Production</string>
  <string />
  <string>{Tickets:</string>
  <string>%Tickets%</string>
  <string>}</string>
  <string>{%ProjectName% Files:</string>
  <string>%Files:!SQL%</string>
  <string>}</string>
  <string>{SQL:</string>
  <string>%Files:SQL%</string>
  <string>}</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection EmailBody {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["EmailBody"]));
            }
            set {
                this["EmailBody"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%Date%_[%TaskNums%]_fixes")]
        public string EmailTitle {
            get {
                return ((string)(this["EmailTitle"]));
            }
            set {
                this["EmailTitle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string EmailAddressTo {
            get {
                return ((string)(this["EmailAddressTo"]));
            }
            set {
                this["EmailAddressTo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool OpenResultSubFolderIfNotEmail {
            get {
                return ((bool)(this["OpenResultSubFolderIfNotEmail"]));
            }
            set {
                this["OpenResultSubFolderIfNotEmail"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("%TaskId%    %TaskTitle%")]
        public string TaskItem {
            get {
                return ((string)(this["TaskItem"]));
            }
            set {
                this["TaskItem"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("./%FilePath% %FileChangedType%")]
        public string FileItem {
            get {
                return ((string)(this["FileItem"]));
            }
            set {
                this["FileItem"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IncludeDeletedFilesPathInEmailDescription {
            get {
                return ((bool)(this["IncludeDeletedFilesPathInEmailDescription"]));
            }
            set {
                this["IncludeDeletedFilesPathInEmailDescription"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SQL")]
        public string SqlProjectName {
            get {
                return ((string)(this["SqlProjectName"]));
            }
            set {
                this["SqlProjectName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string EmailAddressFrom {
            get {
                return ((string)(this["EmailAddressFrom"]));
            }
            set {
                this["EmailAddressFrom"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool EnableLogging {
            get {
                return ((bool)(this["EnableLogging"]));
            }
            set {
                this["EnableLogging"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Development")]
        public string RootFolderName {
            get {
                return ((string)(this["RootFolderName"]));
            }
            set {
                this["RootFolderName"] = value;
            }
        }
    }
}
