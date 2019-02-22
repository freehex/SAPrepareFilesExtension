using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;
using SAPrepareFilesExtension.Helpers;
using System.IO;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace SAPrepareFilesExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SAChangesetDetailsCommand: SAPrepareFilesBaseCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("e2dac92d-3716-4219-a7e1-c2037284aa6e");

        /// <summary>
        /// Initializes a new instance of the <see cref="SAChangesetDetailsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private SAChangesetDetailsCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SAChangesetDetailsCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new SAChangesetDetailsCommand(package);

            if (package is SAPrepareFilesPackage)
                _teamExplorer = (package as SAPrepareFilesPackage).GetService(typeof(ITeamExplorer)) as ITeamExplorer;
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            base.OnClick(sender, e);
            //try
            //{
            //    _pendingChanges = TeamExplorerHelper.GetIncludedChanges(_teamExplorer);
            //    _workItems = TeamExplorerHelper.GetWorkItems(_teamExplorer);
            //    _workspace = TeamExplorerHelper.GetWorkspace(_teamExplorer);

            //    var workingFolder = _workspace?.Folders?.FirstOrDefault(x => _pendingChanges.Any(pc => pc.ServerItem.IndexOf(x.ServerItem) >= 0));
            //    var filesList = _pendingChanges.Where(x => !x.IsDelete).Select(x => x.LocalOrServerItem.Replace(workingFolder.ServerItem, workingFolder?.LocalItem).Replace("/","\\"));
            //    var taskNums = _workItems?.Where(x => x.WorkItem.Type?.Name?.IndexOf("Code Review") < 0).Select(x => x.WorkItem?.Id.ToString());

            //    FileHelper.CopyFilesToFolder(filesList, workingFolder?.LocalItem, taskNums);

            //    ShowMessage(Path.GetPathRoot(filesList.ElementAt(0)) + " :: " + filesList.ElementAt(0));

            //    ShowMessage(
            //        _pendingChanges?.Count > 0
            //        ?
            //        _pendingChanges.Select(x => $"LocalOrServerItem: {x.LocalOrServerItem}, IsDelete: {x.IsDelete}, IsAdd: {x.IsAdd}, LocalItem: {x.LocalItem}, LocalOrServerFolder: {x.LocalOrServerFolder}, ServerItem: {x.ServerItem}, SourceLocalItem: {x.SourceLocalItem}, SourceServerItem: {x.SourceServerItem}, SourceVersionFrom: {x.SourceVersionFrom}, ChangeTypeName: {x.ChangeTypeName}")
            //        .Aggregate((x, y) => x + Environment.NewLine + y)
            //        :
            //        "Changes list has no elements"
            //        );

            //    ShowMessage(
            //        _workItems?.Count > 0
            //        ?
            //        _workItems.Select(x => $"Id: {x.WorkItem?.Id}, Title: {x.WorkItem?.Title}, Type: {x.WorkItem?.Type.Name}")
            //        .Aggregate((x, y) => x + Environment.NewLine + y)
            //        :
            //        "Work item list has no elements"
            //        );
            //}
            //catch (Exception ex)
            //{
            //    ShowMessage(
            //        $"Exception: {ex.Message}" + (GeneralSettings.Default.ShowStackTrace ? $"{Environment.NewLine}StackTrace: {ex.StackTrace}" : ""));
            //}
        }

        //private void ShowMessage(string text)
        //{
        //    VsShellUtilities.ShowMessageBox(
        //        this.ServiceProvider,
        //        text ?? String.Empty,
        //        "SAPrepareFilesExtension",
        //        OLEMSGICON.OLEMSGICON_INFO,
        //        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //}
        //private void MenuItemCallback(object sender, EventArgs e)
        //{
        //    string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
        //    string title = "SAChangesetDetailsCommand";

        //    if (_teamExplorer != null)
        //    {
        //        message = "NOT null" + _teamExplorer.ToString();

        //        var changesetDetailsPage = (TeamExplorerPageBase)_teamExplorer.CurrentPage;//.NavigateToPage(new Guid(TeamExplorerPageIds.ChangesetDetails), null);

        //        if (changesetDetailsPage != null)
        //        {
        //            message = "changesetDetailsPage is NOT null " + changesetDetailsPage.ToString();

        //            // Show a message box to prove we were here
        //            VsShellUtilities.ShowMessageBox(
        //                this.ServiceProvider,
        //                message,
        //                title,
        //                OLEMSGICON.OLEMSGICON_INFO,
        //                OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //        }
        //        else
        //        {
        //            message = "changesetDetailsPage is null ";

        //            // Show a message box to prove we were here
        //            VsShellUtilities.ShowMessageBox(
        //                this.ServiceProvider,
        //                message,
        //                title,
        //                OLEMSGICON.OLEMSGICON_INFO,
        //                OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //        }



        //            var cdModel = changesetDetailsPage.Model;
        //        var cd = cdModel.GetType().GetProperty("DataProvider", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        //        var dataProvider2 = cd.GetValue(cdModel); // IPendingChangesDataProvider is internal;

        //        if (dataProvider2 != null)
        //        {
        //            message = "dataProvider2 is NOT null " + dataProvider2.ToString();

        //            // Show a message box to prove we were here
        //            VsShellUtilities.ShowMessageBox(
        //                this.ServiceProvider,
        //                message,
        //                title,
        //                OLEMSGICON.OLEMSGICON_INFO,
        //                OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //            var dataProviderType = dataProvider2.GetType();

        //            var ss = dataProviderType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        //            message = ss.Select(x => x.Name).Aggregate((x, y) => x + Environment.NewLine + y);
        //            VsShellUtilities.ShowMessageBox(
        //                this.ServiceProvider,
        //                message,
        //                title,
        //                OLEMSGICON.OLEMSGICON_INFO,
        //                OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //            var cdxModelType = changesetDetailsPage.Model.GetType();
        //            var sss = cdxModelType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        //            message = sss.Select(x => x.Name).Aggregate((x, y) => x + Environment.NewLine + y);
        //            VsShellUtilities.ShowMessageBox(
        //                this.ServiceProvider,
        //                message,
        //                title,
        //                OLEMSGICON.OLEMSGICON_INFO,
        //                OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //            cd = dataProviderType.GetProperty("IncludedChanges", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        //            if (cd != null)
        //            {
        //                message = "cd is NOT null " + cd.ToString();

        //                // Show a message box to prove we were here
        //                VsShellUtilities.ShowMessageBox(
        //                    this.ServiceProvider,
        //                    message,
        //                    title,
        //                    OLEMSGICON.OLEMSGICON_INFO,
        //                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //                var m = cd.GetMethod;

        //                if (m != null)
        //                {
        //                    message = "m is NOT null " + cd.ToString();

        //                    // Show a message box to prove we were here
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //                    var _getIncludedChanges2 = (Func<IList<PendingChange>>)m.CreateDelegate(typeof(Func<IList<PendingChange>>), dataProvider2);

        //                    var xx = _getIncludedChanges2();

        //                    message = xx.Select(x => $"LocalOrServerItem: {x.LocalOrServerItem}, IsDelete: {x.IsDelete}, IsAdd: {x.IsAdd}, ChangeType: {x.ChangeType}, "
        //                    + $"PropertyValues: {(x.PropertyValues == null ? "null" : x.PropertyValues.Count.ToString())}") //?.Select(y => $"PropertyName: {y?.PropertyName}, Value: {y?.Value}")?.Aggregate((ii, jj) => $"{ii}; {jj}") ?? "none"}")
        //                    .Aggregate((x, y) => x + Environment.NewLine + y);
        //                }
        //                else
        //                    message = "cd is null ";


        //                // Show a message box to prove we were here
        //                VsShellUtilities.ShowMessageBox(
        //                    this.ServiceProvider,
        //                    message,
        //                    title,
        //                    OLEMSGICON.OLEMSGICON_INFO,
        //                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);



        //                }
        //            else
        //            {
        //                message = "cd is null ";

        //                // Show a message box to prove we were here
        //                VsShellUtilities.ShowMessageBox(
        //                    this.ServiceProvider,
        //                    message,
        //                    title,
        //                    OLEMSGICON.OLEMSGICON_INFO,
        //                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //            }

        //            //-------------------------------------------------------------------------

        //            cd = dataProviderType.GetProperty("WorkItemInfo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        //            if (cd != null)
        //            {
        //                message = "cd is NOT null " + cd.ToString();

        //                // Show a message box to prove we were here
        //                VsShellUtilities.ShowMessageBox(
        //                    this.ServiceProvider,
        //                    message,
        //                    title,
        //                    OLEMSGICON.OLEMSGICON_INFO,
        //                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //                var m = cd.GetMethod;

        //                if (m != null)
        //                {
        //                    message = "m is NOT null " + cd.ToString();

        //                    // Show a message box to prove we were here
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //                    message = JsonConvert.SerializeObject(m);
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //                    var tt5 = cdxModelType.GetMethod("Microsoft.TeamFoundation.VersionControl.Client.IPendingCheckinWorkItems.get_CheckedWorkItems", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        //                    var tt5_delegate = (Func<WorkItemCheckinInfo[]>)tt5.CreateDelegate(typeof(Func<WorkItemCheckinInfo[]>), cdModel);
        //                    var tt5_list = tt5_delegate();
        //                    message = tt5_list.Select(x => $"Id: {x.WorkItem?.Id}, Title: {x.WorkItem?.Title}, Type: {x.WorkItem?.Type}").Aggregate((x, y) => x + Environment.NewLine + y);
        //                    VsShellUtilities.ShowMessageBox(
        //                       this.ServiceProvider,
        //                       message,
        //                       title,
        //                       OLEMSGICON.OLEMSGICON_INFO,
        //                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //                    var t = cdxModelType.GetMethod("FindWorkItem", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        //                    var tt = cdxModelType.GetMethod("FindWorkItems", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        //                    var tt2 = cdxModelType.GetMethod("Microsoft.TeamFoundation.VersionControl.Client.IPendingCheckin.get_WorkItems", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        //                    var tt3 = cdxModelType.GetMethod("get_WorkItemsListProvider", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        //                    var tt4 = cdxModelType.GetMethod("get_WorkItemsListAvailable", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        //                    message = JsonConvert.SerializeObject(t);
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //                    message = JsonConvert.SerializeObject(tt);
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //                    message = JsonConvert.SerializeObject(tt2);
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //                    message = JsonConvert.SerializeObject(tt3);
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //                    message = JsonConvert.SerializeObject(tt4);
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //                    message = JsonConvert.SerializeObject(tt5);
        //                    VsShellUtilities.ShowMessageBox(
        //                        this.ServiceProvider,
        //                        message,
        //                        title,
        //                        OLEMSGICON.OLEMSGICON_INFO,
        //                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //                    var _getIncludedChanges2 = (Func<IList<WorkItemCheckedInfo>>)m.CreateDelegate(typeof(Func<IList<WorkItemCheckedInfo>>), dataProvider2);

        //                    var xx = _getIncludedChanges2();

        //                    message = xx.Select(x => $"Id: {x.Id}, Title: {JsonConvert.SerializeObject(x)}").Aggregate((x, y) => x + Environment.NewLine + y);
        //                }
        //                else
        //                    message = "cd is null ";

        //                // Show a message box to prove we were here
        //                VsShellUtilities.ShowMessageBox(
        //                    this.ServiceProvider,
        //                    message,
        //                    title,
        //                    OLEMSGICON.OLEMSGICON_INFO,
        //                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        //            }
        //            else
        //            {
        //                message = "cd is null ";

        //                // Show a message box to prove we were here
        //                VsShellUtilities.ShowMessageBox(
        //                    this.ServiceProvider,
        //                    message,
        //                    title,
        //                    OLEMSGICON.OLEMSGICON_INFO,
        //                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //            }


        //        }
        //        else
        //            message = "dataProvider2 IS null";
        //    }
        //    else
        //        message = "IS null";

        //    //PendingChangesInclusion(_teamExplorer);




        //    // Show a message box to prove we were here
        //    VsShellUtilities.ShowMessageBox(
        //        this.ServiceProvider,
        //        message,
        //        title,
        //        OLEMSGICON.OLEMSGICON_INFO,
        //        OLEMSGBUTTON.OLEMSGBUTTON_OK,
        //        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        //}
    }
}
