using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using SAPrepareFilesExtension.Helpers;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SAPrepareFilesExtension
{
    internal class SAPrepareFilesBaseCommand
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        protected Package package;

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        protected IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        protected static ITeamExplorer _teamExplorer;
        protected IList<PendingChange> _pendingChanges;
        protected IList<WorkItemCheckinInfo> _workItems;
        protected Workspace _workspace;

        protected void OnClick(object sender, EventArgs e)
        {
            try
            {
                _pendingChanges = TeamExplorerHelper.GetIncludedChanges(_teamExplorer);
                _workItems = TeamExplorerHelper.GetWorkItems(_teamExplorer);
                _workspace = TeamExplorerHelper.GetWorkspace(_teamExplorer);

                var workingFolder = _workspace?.Folders?.FirstOrDefault(x => _pendingChanges.Any(pc => pc.ServerItem.IndexOf(x.ServerItem) >= 0));
                var files = _pendingChanges.Select(x => new FileItem() {
                                            LocalPath = x.LocalOrServerItem.Replace(workingFolder.ServerItem, workingFolder?.LocalItem).Replace("/", "\\"),
                                            ServerPath = x.ServerItem,
                                            IsDirectory = x.IsDelete ? false : File.GetAttributes(x.LocalOrServerItem.Replace(workingFolder.ServerItem, workingFolder?.LocalItem).Replace("/", "\\")).HasFlag(FileAttributes.Directory),
                                            IsAdd = x.IsAdd,
                                            IsDelete= x.IsDelete
                });
                var tasks = _workItems?.Where(x => x.WorkItem.Type?.Name?.IndexOf("Code Review") < 0)
                                        .Select(x => new TaskItem() {
                                            Id = x.WorkItem.Id,
                                            Title = x.WorkItem.Title,
                                            TypeName = x.WorkItem.Type?.Name
                                        });
                
                if (files?.Count() > 0)
                {
                    SettingsHelper.ProjectPath = workingFolder?.LocalItem;
                    SettingsHelper.Tasks = tasks;

                    var pathToBeSent = FileHelper.CopyFilesToFolder(files.Where(x => !x.IsDelete));

                    var filesGroupText = SettingsHelper.GetFileItemsText(files, workingFolder?.ServerItem);
                    var tasksText = SettingsHelper.GetTaskItemsText(tasks);

                    if (ShowMessage("The archive containing changed files is prepared. Do you want to generate a letter?", OLEMSGBUTTON.OLEMSGBUTTON_YESNO) == 6)
                    {
                        EmailHelper.SendMessage(
                            GeneralSettings.Default.EmailTitle,
                            SettingsHelper.GetEmailBodyValue(filesGroupText, tasksText),
                            pathToBeSent
                        );
                    }
                    else
                    {
                        if (GeneralSettings.Default.OpenResultSubFolderIfNotEmail && pathToBeSent?.Count() > 0)
                        {
                            Process.Start("explorer.exe", Path.GetDirectoryName(pathToBeSent.ElementAt(0)));
                        }
                        if (GeneralSettings.Default.CopyMailBodyToClipboard)
                        {
                            Clipboard.SetText(SettingsHelper.GetEmailBodyValue(filesGroupText, tasksText));
                        }
                    }
                }
                else
                {
                    ShowMessage("You don't have any modified files to check in. Please add changes and ensure that changed files are placed in the \"Included changes\" section.");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Exception: {ex.Message}" + (GeneralSettings.Default.ShowStackTrace ? $"{Environment.NewLine}StackTrace: {ex.StackTrace}" : ""));
            }
        }

        protected int ShowMessage(string text, OLEMSGBUTTON buttonType = OLEMSGBUTTON.OLEMSGBUTTON_OK)
        {
            return VsShellUtilities.ShowMessageBox(
                    this.ServiceProvider,
                    text ?? String.Empty,
                    "SAPrepareFilesExtension",
                    OLEMSGICON.OLEMSGICON_INFO,
                    buttonType,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
