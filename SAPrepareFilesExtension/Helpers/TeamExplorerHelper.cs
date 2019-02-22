using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SAPrepareFilesExtension.Helpers
{
    public static class TeamExplorerHelper
    {
        public static IList<PendingChange> GetIncludedChanges(ITeamExplorer teamExplorer)
        {
            var errorMessage = "";

            if (teamExplorer != null)
            {
                var teamExplorerPage = (TeamExplorerPageBase)teamExplorer.CurrentPage;

                var model = teamExplorerPage.Model;
                var prInfo = model.GetType().GetProperty("DataProvider", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                var dataProvider = prInfo?.GetValue(model);

                if (dataProvider != null)
                {
                    var dataProviderType = dataProvider.GetType();

                    prInfo = dataProviderType.GetProperty("IncludedChanges", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    var getResult = prInfo.GetMethod.CreateDelegate(typeof(Func<IList<PendingChange>>), dataProvider) as Func<IList<PendingChange>>;
                    return getResult();
                }
                else
                    errorMessage = "Can't get DataProvider. Please make sure the correct Team Explorer page is loaded and not empty";
            }
            else
                errorMessage = "Can't get Team Explorer. Please make sure the Team Explorer is loaded and connected to TFS";

            throw new Exception(errorMessage);
        }

        public static IList<WorkItemCheckedInfo> GetWorkItemInfo(ITeamExplorer teamExplorer)
        {
            var errorMessage = "";

            if (teamExplorer != null)
            {
                var teamExplorerPage = (TeamExplorerPageBase)teamExplorer.CurrentPage;

                var model = teamExplorerPage.Model;
                var prInfo = model.GetType().GetProperty("DataProvider", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                var dataProvider = prInfo?.GetValue(model);

                if (dataProvider != null)
                {
                    var dataProviderType = dataProvider.GetType();

                    prInfo = dataProviderType.GetProperty("WorkItemInfo", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    var getResult = prInfo.GetMethod.CreateDelegate(typeof(Func<IList<WorkItemCheckedInfo>>), dataProvider) as Func<IList<WorkItemCheckedInfo>>;
                    return getResult();
                }
                else
                    errorMessage = "Can't get DataProvider. Please make sure the correct Team Explorer page is loaded and not empty";
            }
            else
                errorMessage = "Can't get Team Explorer. Please make sure the Team Explorer is loaded and connected to TFS";

            throw new Exception(errorMessage);
        }

        public static IList<WorkItemCheckinInfo> GetWorkItems(ITeamExplorer teamExplorer)
        {
            var errorMessage = "";

            if (teamExplorer != null)
            {
                var teamExplorerPage = (TeamExplorerPageBase)teamExplorer.CurrentPage;
                
                if (teamExplorerPage != null)
                {
                    var model = teamExplorerPage.Model;
                    var modelType = model.GetType();
                    var method = modelType.GetMethod("Microsoft.TeamFoundation.VersionControl.Client.IPendingCheckinWorkItems.get_CheckedWorkItems", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    var getResult = method.CreateDelegate(typeof(Func<IList<WorkItemCheckinInfo>>), model) as Func<IList<WorkItemCheckinInfo>>;
                    return getResult();
                }
                else
                    errorMessage = "Can't get Team Explorer page. Please make sure the correct Team Explorer page is loaded and not empty";
            }
            else
                errorMessage = "Can't get Team Explorer. Please make sure the Team Explorer is loaded and connected to TFS";

            throw new Exception(errorMessage);
        }

        public static Workspace GetWorkspace(ITeamExplorer teamExplorer)
        {
            var errorMessage = "";

            if (teamExplorer != null)
            {
                var teamExplorerPage = (TeamExplorerPageBase)teamExplorer.CurrentPage;

                if (teamExplorerPage != null)
                {
                    var model = teamExplorerPage.Model;
                    var modelType = model.GetType();
                    var method = modelType.GetMethod("get_Workspace", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    
                    var getResult = method.CreateDelegate(typeof(Func<Workspace>), model) as Func<Workspace>;
                    return getResult();
                }
                else
                    errorMessage = "Can't get Team Explorer page. Please make sure the correct Team Explorer page is loaded and not empty";
            }
            else
                errorMessage = "Can't get Team Explorer. Please make sure the Team Explorer is loaded and connected to TFS";

            throw new Exception(errorMessage);
        }
    }
}
