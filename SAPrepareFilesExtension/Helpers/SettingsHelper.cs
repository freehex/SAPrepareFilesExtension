using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SAPrepareFilesExtension.Helpers
{
    public static class SettingsHelper
    {
        public static string ProjectPath { get; set; }
        public static IEnumerable<TaskItem> Tasks { get; set; }
        public static IEnumerable<TaskItem> Files { get; set; }
        public static string ProjectName { get; set; }
        private static IEnumerable<string> FilesPathList { get; set; }
        private static IEnumerable<string> WorkItemsList { get; set; }

        // slowly, but simple
        public static string GetValue(string parameter)
        {
            LogHelper.Begin(new { parameter });

            var next = Regex.Replace(
                parameter, 
                "%date.*?%", 
                x => {
                    var id = x.Value.IndexOf(':');
                    return id >= 0 ?
                            DateTime.Now.ToString(x.Value.Substring(id + 1, x.Value.Length - id - 2))
                            : DateTime.Now.ToString("yyyyMMdd");
                },
                RegexOptions.IgnoreCase
            );

            LogHelper.Trace("after %date% replaced", new { next });

            next = Regex.Replace(next, 
                "%tasknums.*?%",
                x => {
                    if (Tasks?.Count() > 0)
                    {
                        var id = x.Value.IndexOf(':');
                        return id >= 0 ?
                                string.Join(x.Value.Substring(id + 1, x.Value.Length - id - 2), Tasks.Select(t => t.Id))
                                : string.Join(",", Tasks.Select(t => t.Id));
                    }
                    return "";
                },
                RegexOptions.IgnoreCase
            );

            LogHelper.Trace("after %tasknums% replaced", new { next });

            next = Regex.Replace(
                next,
                "%projectpath%",
                ProjectPath.TrimEnd('\\'),
                RegexOptions.IgnoreCase
                );

            LogHelper.End(new { next });

            return next;
        }

        public static string GetEmailBodyValue(IEnumerable<Tuple<string, IEnumerable<string>>> filesGroupText, IEnumerable<string> taskItemsText)
        {
            LogHelper.Begin(new { filesGroupText, taskItemsText });

            var tasksText = string.Join(Environment.NewLine, taskItemsText);
            var next = string.Join(Environment.NewLine, GeneralSettings.Default.EmailBody.Cast<string>());

            LogHelper.Trace(new { tasksText, next });

            next = Regex.Replace(
                next,
                "{.*?%tickets%.*?}",
                x => Regex.Replace(x.Value.Substring(1, x.Value.Length - 2), "%tickets%", tasksText, RegexOptions.IgnoreCase), //without curly braces
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            LogHelper.Trace("after %tickets% replaced", new { next });

            next = Regex.Replace( 
                next,
                "{.*?%files.*?%.*?}",
                x =>
                {
                    var paramsMatch = Regex.Match(x.Value, "%files:.*?%", RegexOptions.IgnoreCase);
                    var sb = new StringBuilder();
                    var filledValue = GetValue(x.Value.Substring(1, x.Value.Length - 2)); //without curly braces

                    LogHelper.Trace(new { paramsMatch, filledValue });

                    if (!string.IsNullOrEmpty(paramsMatch.Value))
                    {
                        var filesParams = paramsMatch.Value.Substring(7, paramsMatch.Value.Length - 8); //without "%files:" and "%" parts
                        var isIncluded = filesParams[0] != '!';
                        var parameters = filesParams.ToLower().Split(',').Select(p => p.TrimStart('!'));

                        LogHelper.Trace("for specific projects", new { filesParams, isIncluded, parameters });

                        if (isIncluded)
                        {
                            foreach (var group in filesGroupText.Where(g => parameters.Contains(g.Item1.ToLower())))
                            {
                                sb.Append(
                                    Regex.Replace(GetProjectValue(filledValue, group.Item1), "%files.*?%", string.Join(Environment.NewLine, group.Item2.Select(l => "  " + l)), RegexOptions.IgnoreCase)
                                );
                            }
                        }
                        else
                        {
                            foreach (var group in filesGroupText.Where(g => !parameters.Contains(g.Item1.ToLower())))
                            
                                sb.Append(
                                    Regex.Replace(GetProjectValue(filledValue, group.Item1), "%files.*?%", string.Join(Environment.NewLine, group.Item2.Select(l => "  " + l)), RegexOptions.IgnoreCase)
                                );
                        }
                    }
                    else // no specific projects
                    {
                        foreach (var group in filesGroupText)
                        {
                            sb.Append(
                                Regex.Replace(GetProjectValue(filledValue, group.Item1), "%files.*?%", string.Join(Environment.NewLine, group.Item2.Select(l => "  " + l)), RegexOptions.IgnoreCase)
                            );
                        }
                    }
                    return sb.ToString();
                },
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            LogHelper.End(new { next });

            return next;
        }

        public static string GetProjectValue(string parameter, string projectName)
        {
            LogHelper.Begin(new { parameter, projectName });

            var result = Regex.Replace(
                parameter,
                "%projectname%",
                projectName,
                RegexOptions.IgnoreCase
                );

            LogHelper.End(new { result });

            return result;
        }

        public static IEnumerable<Tuple<string, IEnumerable<string>>> GetFileItemsText(IEnumerable<FileItem> files, string rootServerPath)
        {
            LogHelper.Begin(new { files, rootServerPath });

            var fileItemTemplate = GetValue(GeneralSettings.Default.FileItem);

            LogHelper.Trace(new { fileItemTemplate });

            var result = files.Where(x => !x.IsDirectory && (!x.IsDelete || GeneralSettings.Default.IncludeDeletedFilesPathInEmailDescription))
                        .GroupBy(x => GetProjectName(rootServerPath, x.ServerPath))
                        .Select(g =>
                                {
                                    var sqlProjectName = !string.IsNullOrEmpty(GeneralSettings.Default.SqlProjectName) ? GeneralSettings.Default.SqlProjectName : "sql";
                                    var isSql = string.Compare(g.Key, sqlProjectName, StringComparison.OrdinalIgnoreCase) == 0;
                                    var sqlFullScriptsPath = isSql ?
                                                                GeneralSettings.Default.FullScriptsPath
                                                                        .Substring(GeneralSettings.Default.FullScriptsPath.IndexOf($"{sqlProjectName}\\", StringComparison.OrdinalIgnoreCase) + sqlProjectName.Length + 1)
                                                                        .Replace("\\", "/")
                                                                : null;

                                    LogHelper.Trace(new { sqlProjectName, isSql, sqlFullScriptsPath });

                                    return new Tuple<string, IEnumerable<string>>
                                    (
                                        g.Key,
                                        g.OrderBy(x => x.IsDelete)
                                        .ThenBy(x => x.ServerPath)
                                        .ThenByDescending(x => x.ServerPath.Length)
                                        .Select(x =>
                                        {
                                            if (!isSql)
                                            {
                                                var next = Regex.Replace(fileItemTemplate, "%FilePath%", GetPathWithoutRootPart(rootServerPath, x.ServerPath, g.Key), RegexOptions.IgnoreCase);

                                                LogHelper.Trace("after %FilePath% replaced", new { next });

                                                next = Regex.Replace(
                                                                next, 
                                                                "%FileChangedType%", 
                                                                v => x.IsAdd ? "[Added]" : (x.IsDelete ? "[Deleted]" : ""), 
                                                                RegexOptions.IgnoreCase
                                                            );

                                                LogHelper.Trace("after %FileChangedType% replaced", new { next });

                                                return next;
                                            }
                                            else
                                            {
                                                if (x.ServerPath.IndexOf(sqlFullScriptsPath) >= 0)
                                                {
                                                    var sqlPath = GetPathWithoutRootPart(sqlFullScriptsPath, x.ServerPath, g.Key);

                                                    LogHelper.Trace(new { sqlPath });

                                                    return sqlPath;
                                                }
                                                return null;
                                            }
                                        })
                                        .Where(x => !string.IsNullOrEmpty(x))
                                    );
                                }
                        );

            LogHelper.End(new { result });

            return result;
        }

        public static IEnumerable<string> GetTaskItemsText(IEnumerable<TaskItem> tasks)
        {
            LogHelper.Begin(new { tasks });

            var taskItemTemplate = GetValue(GeneralSettings.Default.TaskItem);
            
            var result = tasks.OrderBy(x => x.Id)
                        .Select(x => 
                        {
                            var next = Regex.Replace(taskItemTemplate, "%TaskId%", x.Id.ToString(), RegexOptions.IgnoreCase);

                            LogHelper.Trace("after %TaskId% replaced", new { next });

                            next = Regex.Replace(next, "%TaskTitle%", x.Title, RegexOptions.IgnoreCase);

                            LogHelper.Trace("after %TaskTitle% replaced", new { next });

                            next = Regex.Replace(next, "%TaskType%", x.TypeName, RegexOptions.IgnoreCase);

                            LogHelper.Trace("after %TaskType% replaced", new { next });

                            return next;
                        }
            );

            LogHelper.End(new { result });

            return result;
        }

        public static string GetPathWithoutRootPart(string rootPath, string itemPath, string projectName)
        {
            LogHelper.Begin(new { rootPath, itemPath, projectName });

            var sqlProjectName = !string.IsNullOrEmpty(GeneralSettings.Default.SqlProjectName) ? GeneralSettings.Default.SqlProjectName : "sql";

            var result = string.Compare(projectName, sqlProjectName, StringComparison.OrdinalIgnoreCase) != 0 ?
                            itemPath.Substring(rootPath.Length + projectName.Length + 2)
                            :
                            itemPath.Substring(itemPath.IndexOf(rootPath) + rootPath.Length + 1);

            LogHelper.End(new { sqlProjectName, result });

            return result;
        }

        public static string GetProjectName(string rootPath, string itemPath)
        {
            LogHelper.Begin(new { rootPath, itemPath });

            var result = Regex.Match(itemPath.Substring(rootPath.Length), ".+?[\\/]")?.Value?.Trim(new char[] { '\\', '/' });

            LogHelper.End(new { result });

            return result;
        }

        public static RootFolder GetRootFolders(string serverRootPath, string localRootPath, IEnumerable<string> serverItems)
        {
            LogHelper.Begin(new { serverRootPath, localRootPath, serverItems, GeneralSettings.Default.RootFolderName });

            RootFolder result = null;

            if (!string.IsNullOrEmpty(serverRootPath) && !string.IsNullOrEmpty(localRootPath))
            {
                if (string.IsNullOrEmpty(GeneralSettings.Default.RootFolderName) || serverItems?.Count() < 1)
                {
                    result = new RootFolder()
                    {
                        ServerItem = serverRootPath,
                        LocalItem = localRootPath
                    };
                }
                else
                {
                    var serverItem = serverItems.First(x => x?.IndexOf(GeneralSettings.Default.RootFolderName, StringComparison.OrdinalIgnoreCase) >= 0);
                    serverItem = serverItem.Substring(0, serverItem.IndexOf(GeneralSettings.Default.RootFolderName, StringComparison.OrdinalIgnoreCase) + GeneralSettings.Default.RootFolderName.Length);

                    result = new RootFolder()
                    {
                        ServerItem = serverItem,
                        LocalItem = serverItem.Replace(serverRootPath, localRootPath.TrimEnd('\\') + "\\").Replace("/", "\\").TrimEnd('\\')
                    };
                }
            }

            LogHelper.End(new { result });

            return result;
        }
    }
}
