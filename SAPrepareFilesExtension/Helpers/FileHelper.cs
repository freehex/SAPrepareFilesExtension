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
            var result = new List<string>();
            var targetPath = Path.GetFullPath(
                            SettingsHelper.GetValue(GeneralSettings.Default.ResultFolderPath)
                            + "\\"
                            + SettingsHelper.GetValue(GeneralSettings.Default.ResultSubFoldersName)
                            );
            var sqlFullScriptsPath = Path.GetFullPath(SettingsHelper.GetValue(GeneralSettings.Default.FullScriptsPath));

            Directory.CreateDirectory(targetPath); // create if not exists
            
            foreach(var file in files)
            {
                if (!IsSqlPath(SettingsHelper.ProjectPath, file.LocalPath))
                {
                    var destFilePath = Path.GetFullPath(file.LocalPath.Replace(SettingsHelper.ProjectPath, targetPath));
                    Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));

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

                ZipFile.CreateFromDirectory(
                    projDir,
                    resultZipPath,
                    CompressionLevel.Optimal,
                    GeneralSettings.Default.AddProjectFolderInZip
                    );

                result.Add(resultZipPath);
            }

            var sqlFilesPath = Directory.GetFiles(targetPath, "*.sql", SearchOption.TopDirectoryOnly);
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

            return result;
        }

        private static bool IsSqlPath(string rootPath, string path)
        {
            return path.IndexOf(Path.GetFullPath($"{rootPath}\\{GeneralSettings.Default.SqlProjectName}\\")) >= 0;
        }

        public static string GetEmailFilePath(string dirPath)
        {
            return Directory.GetFiles(Path.GetDirectoryName(dirPath), "*.eml", SearchOption.TopDirectoryOnly).FirstOrDefault();
        }
    }
}
