using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPrepareFilesExtension
{
    public class OptionsPageGrid: DialogPage
    {
        #region Common
        [Category("Common")]
        [DisplayName("Copy Email Body To Clipboard")]
        [Description("Copy email body to clipboard if not email")]
        public bool CopyMailBodyToClipboard
        {
            get { return GeneralSettings.Default.CopyMailBodyToClipboard; }
            set { GeneralSettings.Default.CopyMailBodyToClipboard = value; }
        }

        [Category("Common")]
        [DisplayName("Show Stack Trace")]
        [Description("Show stack trace via exception message")]
        public bool ShowStackTrace
        {
            get { return GeneralSettings.Default.ShowStackTrace; }
            set { GeneralSettings.Default.ShowStackTrace = value; }
        }

        [Category("Common")]
        [DisplayName("Include Project Folder In Zip")]
        [Description("Include root project folder in zip archive")]
        public bool AddProjectFolderInZip
        {
            get { return GeneralSettings.Default.AddProjectFolderInZip; }
            set { GeneralSettings.Default.AddProjectFolderInZip = value; }
        }

        [Category("Common")]
        [DisplayName("Delete Temp Files")]
        [Description("Delete temp files after archiving")]
        public bool DeleteTempFilesAfterArchiving
        {
            get { return GeneralSettings.Default.DeleteTempFilesAfterArchiving; }
            set { GeneralSettings.Default.DeleteTempFilesAfterArchiving = value; }
        }

        [Category("Common")]
        [DisplayName("Open Result Subfolder If Not Email")]
        [Description("Open result subfolder if not email")]
        public bool OpenResultSubFolderIfNotEmail
        {
            get { return GeneralSettings.Default.OpenResultSubFolderIfNotEmail; }
            set { GeneralSettings.Default.OpenResultSubFolderIfNotEmail = value; }
        }

        [Category("Common")]
        [DisplayName("Include Deleted Files Path")]
        [Description("Include deleted files path in email body")]
        public bool IncludeDeletedFilesPathInEmailDescription
        {
            get { return GeneralSettings.Default.IncludeDeletedFilesPathInEmailDescription; }
            set { GeneralSettings.Default.IncludeDeletedFilesPathInEmailDescription = value; }
        }

        [Category("Common")]
        [DisplayName("SQL Project Name")]
        [Description("SQL project name")]
        public string SqlProjectName
        {
            get { return GeneralSettings.Default.SqlProjectName; }
            set { GeneralSettings.Default.SqlProjectName = value; }
        }

        [Category("Common")]
        [DisplayName("Enable Logging")]
        [Description("Enable/Disable logging")]
        public bool EnableLogging
        {
            get { return GeneralSettings.Default.EnableLogging; }
            set { GeneralSettings.Default.EnableLogging = value; }
        }
        #endregion Common

        #region Files
        [Category("Files")]
        [DisplayName("Result Folder Path")]
        [Description("Result folder path")]
        public string ResultFolderPath
        {
            get { return GeneralSettings.Default.ResultFolderPath; }
            set { GeneralSettings.Default.ResultFolderPath = value; }
        }

        [Category("Files")]
        [DisplayName("SQLFullScripts Path")]
        [Description("SQL full scripts folder path")]
        public string FullScriptsPath
        {
            get { return GeneralSettings.Default.FullScriptsPath; }
            set { GeneralSettings.Default.FullScriptsPath = value; }
        }

        [Category("Files")]
        [DisplayName("Zip File Name")]
        [Description("Zip name template")]
        public string DefaultZipName
        {
            get { return GeneralSettings.Default.DefaultZipName; }
            set { GeneralSettings.Default.DefaultZipName = value; }
        }

        [Category("Files")]
        [DisplayName("Result SubFolders Name")]
        [Description("Result subFolders name template")]
        public string ResultSubFoldersName
        {
            get { return GeneralSettings.Default.ResultSubFoldersName; }
            set { GeneralSettings.Default.ResultSubFoldersName = value; }
        }
        #endregion Files

        #region Email
        [Category("Email")]
        [DisplayName("Body")]
        [Description("Email body template")]
        public string[] EmailBody
        {
            get { return GeneralSettings.Default.EmailBody.Cast<string>().ToArray(); }
            set {
                var list = new StringCollection();
                list.AddRange(value);
                GeneralSettings.Default.EmailBody = list;
            }
        }

        [Category("Email")]
        [DisplayName("Title")]
        [Description("Email title template")]
        public string EmailTitle
        {
            get { return GeneralSettings.Default.EmailTitle; }
            set { GeneralSettings.Default.EmailTitle = value; }
        }

        [Category("Email")]
        [DisplayName("AddressTo")]
        [Description("Email AddressTo")]
        public string EmailAddressTo
        {
            get { return GeneralSettings.Default.EmailAddressTo; }
            set { GeneralSettings.Default.EmailAddressTo = value; }
        }

        [Category("Email")]
        [DisplayName("Task Item")]
        [Description("Email task item template")]
        public string TaskItem
        {
            get { return GeneralSettings.Default.TaskItem; }
            set { GeneralSettings.Default.TaskItem = value; }
        }

        [Category("Email")]
        [DisplayName("File Item")]
        [Description("Email file item template")]
        public string FileItem
        {
            get { return GeneralSettings.Default.FileItem; }
            set { GeneralSettings.Default.FileItem = value; }
        }

        [Category("Email")]
        [DisplayName("AddressFrom")]
        [Description("Email AddressFrom")]
        public string EmailAddressFrom
        {
            get { return GeneralSettings.Default.EmailAddressFrom; }
            set { GeneralSettings.Default.EmailAddressFrom = value; }
        }
        #endregion Email

        public OptionsPageGrid()
        {
            GeneralSettings.Default.Save();
            GeneralSettings.Default.PropertyChanged += GeneralSettings_PropertyChanged;
        }

        private void GeneralSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GeneralSettings.Default.Save();
        }
    }
}
