using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPrepareFilesExtension.Helpers
{
    public static class FileHelper
    {
        public static IEnumerable<string> CopyFilesToFolder(IEnumerable<FileItem> files)
        {
            LogHelper.Begin(new { files });

            var result = new List<string>();
            var targetPath = Path.GetFullPath(
                            SettingsHelper.GetValue(GeneralSettings.Default.ResultFolderPath)
                            + "\\"
                            + SettingsHelper.GetValue(GeneralSettings.Default.ResultSubFoldersName)
                            );
            var sqlFullScriptsPath = Path.GetFullPath(SettingsHelper.GetValue(GeneralSettings.Default.FullScriptsPath));

            LogHelper.Trace(new { targetPath, sqlFullScriptsPath });

            Directory.CreateDirectory(targetPath); // create if not exists
            
            foreach(var file in files)
            {
                if (!IsSqlPath(SettingsHelper.ProjectPath, file.LocalPath))
                {
                    var destFilePath = Path.GetFullPath(file.LocalPath.Replace(SettingsHelper.ProjectPath.TrimEnd('\\'), targetPath));
                    Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));

                    LogHelper.Trace(new { destFilePath, file.LocalPath });

                    if (!file.IsDirectory)
                    {
                        File.Copy(file.LocalPath, destFilePath);
                    }
                }
                // for SQL project we ll copy sqripts from OngoingChanges_FullScripts folder only
                else if (file.LocalPath.IndexOf(sqlFullScriptsPath) >= 0)
                {
                    var destFilePath = Path.GetFullPath(file.LocalPath.Replace(sqlFullScriptsPath, targetPath));
                    Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));

                    LogHelper.Trace("for SQL", new { destFilePath, file.LocalPath });

                    if (!file.IsDirectory)
                    {
                        File.Copy(file.LocalPath, destFilePath);
                    }
                }
            }

            var projectRootFolders = Directory.GetDirectories(targetPath, "*", SearchOption.TopDirectoryOnly);
            
            foreach (var projDir in projectRootFolders)
            {
                var resultZipPath = projDir
                                + $"\\..\\{(projectRootFolders.Count() > 1 ? new DirectoryInfo(projDir).Name + "_" : "")}"
                                + $"{SettingsHelper.GetValue(GeneralSettings.Default.DefaultZipName)}.zip";

                LogHelper.Trace(new { projDir, resultZipPath });

                ZipFile.CreateFromDirectory(
                    projDir,
                    resultZipPath,
                    CompressionLevel.Optimal,
                    GeneralSettings.Default.AddProjectFolderInZip
                    );

                result.Add(resultZipPath);
            }

            var sqlFilesPath = Directory.GetFiles(targetPath, "*.sql", SearchOption.TopDirectoryOnly);
            LogHelper.Trace(new { sqlFilesPath });

            foreach (var sqlFilePath in sqlFilesPath)
            {
                result.Add(sqlFilePath);
            }

            if (GeneralSettings.Default.DeleteTempFilesAfterArchiving)
            {
                foreach (var projDir in projectRootFolders)
                {
                    Directory.Delete(projDir, true);
                }
            }

            LogHelper.End(new { result });

            return result;
        }

        private static bool IsSqlPath(string rootPath, string path)
        {
            LogHelper.Begin(new { rootPath, path });

            var result = path.IndexOf(Path.GetFullPath($"{rootPath.TrimEnd('\\')}\\{GeneralSettings.Default.SqlProjectName.TrimEnd('\\')}\\")) >= 0;

            LogHelper.End(new { result });

            return result;
        }

        public static string GetEmailFilePath(string dirPath)
        {
            LogHelper.Begin(new { dirPath });

            var result = Directory.GetFiles(Path.GetDirectoryName(dirPath), "*.eml", SearchOption.TopDirectoryOnly).FirstOrDefault();

            LogHelper.End(new { result });

            return result;
        }
    }
}
