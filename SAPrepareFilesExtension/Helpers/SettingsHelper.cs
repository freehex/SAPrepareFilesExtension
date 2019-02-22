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
            next = Regex.Replace(
                next,
                "%projectpath%",
                ProjectPath,
                RegexOptions.IgnoreCase
                );
            
            return next;
        }

        public static string GetEmailBodyValue(IEnumerable<Tuple<string, IEnumerable<string>>> filesGroupText, IEnumerable<string> taskItemsText)
        {
            var tasksText = string.Join(Environment.NewLine, taskItemsText);
            var next = string.Join(Environment.NewLine, GeneralSettings.Default.EmailBody.Cast<string>());

            next = Regex.Replace(
                next,
                "{.*?%tickets%.*?}",
                x => Regex.Replace(x.Value.Substring(1, x.Value.Length - 2), "%tickets%", tasksText, RegexOptions.IgnoreCase),
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            next = Regex.Replace( 
                next,
                "{.*?%files.*?%.*?}",
                x =>
                {
                    var paramsMatch = Regex.Match(x.Value, "%files:.*?%", RegexOptions.IgnoreCase);
                    var sb = new StringBuilder();
                    var filledValue = GetValue(x.Value.Substring(1, x.Value.Length - 2));

                    if (!string.IsNullOrEmpty(paramsMatch.Value))
                    {
                        var filesParams = paramsMatch.Value.Substring(7, paramsMatch.Value.Length - 8);
                        var isIncluded = filesParams[0] != '!';
                        var parameters = filesParams.ToLower().Split(',').Select(p => p.TrimStart('!'));

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
            
            return next;
        }

        public static string GetProjectValue(string parameter, string projectName)
        {
            return Regex.Replace(
                parameter,
                "%projectname%",
                projectName,
                RegexOptions.IgnoreCase
                );
        }

        public static IEnumerable<Tuple<string, IEnumerable<string>>> GetFileItemsText(IEnumerable<FileItem> files, string rootServerPath)
        {
            var fileItemTemplate = GetValue(GeneralSettings.Default.FileItem);

            return files.Where(x => !x.IsDirectory && (!x.IsDelete || GeneralSettings.Default.IncludeDeletedFilesPathInEmailDescription))
                        .GroupBy(x => GetProjectName(rootServerPath, x.ServerPath))
                        .Select(g =>
                                {
                                    var isSql = string.CompareOrdinal(g.Key.ToLower(), "sql") == 0;
                                    var sqlFullScriptsPath = isSql ?
                                                                GeneralSettings.Default.FullScriptsPath
                                                                        .Substring(GeneralSettings.Default.FullScriptsPath.ToLower().IndexOf("sql\\") + 4)
                                                                        .Replace("\\", "/")
                                                                : null;
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
                                                return next = Regex.Replace(
                                                                next, 
                                                                "%FileChangedType%", 
                                                                v => x.IsAdd ? "[Added]" : (x.IsDelete ? "[Deleted]" : ""), 
                                                                RegexOptions.IgnoreCase
                                                            );
                                            }
                                            else
                                            {
                                                if (x.ServerPath.IndexOf(sqlFullScriptsPath) >= 0)
                                                    return GetPathWithoutRootPart(sqlFullScriptsPath, x.ServerPath, g.Key);
                                                return null;
                                            }
                                        })
                                        .Where(x => !string.IsNullOrEmpty(x))
                                    );
                                }
                        );
        }

        public static IEnumerable<string> GetTaskItemsText(IEnumerable<TaskItem> tasks)
        {
            var taskItemTemplate = GetValue(GeneralSettings.Default.TaskItem);
            
            return tasks.OrderBy(x => x.Id)
                        .Select(x => 
                        {
                            var next = Regex.Replace(taskItemTemplate, "%TaskId%", x.Id.ToString(), RegexOptions.IgnoreCase);
                            next = Regex.Replace(next, "%TaskTitle%", x.Title, RegexOptions.IgnoreCase);
                            return next = Regex.Replace(next, "%TaskType%", x.TypeName, RegexOptions.IgnoreCase);
                        }
            );
        }

        public static string GetPathWithoutRootPart(string rootPath, string itemPath, string projectName)
        {
            return string.CompareOrdinal(projectName?.ToLower(), "sql") != 0 ?
                            itemPath.Substring(rootPath.Length + projectName.Length + 2)
                            :
                            itemPath.Substring(itemPath.IndexOf(rootPath) + rootPath.Length + 1);
        }

        public static string GetProjectName(string rootPath, string itemPath)
        {
            return Regex.Match(itemPath.Substring(rootPath.Length + 1), ".*?[\\/]")?.Value?.TrimEnd(new char[] { '\\', '/' });
        }
    }
}
