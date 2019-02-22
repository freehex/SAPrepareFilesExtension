using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

namespace SAPrepareFilesExtension.Helpers
{
    public static class LogHelper
    {
        public static bool IsEnabled
        {
            get { return GeneralSettings.Default.EnableLogging; }
        }
        public static string LogFilePath
        {
            get { return $"{Path.GetTempPath()}\\SAPrepareFilesExtension\\Logs\\{DateTime.Now.ToString("yyyyMMdd")}.log"; }
        }

        public static void Begin(dynamic parameters = null, [CallerFilePath] string methodPath = "", [CallerMemberName] string methodName = "")
        {
            SaveMessage("Trace", "BEGIN", parameters, methodPath, methodName);
        }
        public static void End(dynamic parameters = null, [CallerFilePath] string methodPath = "", [CallerMemberName] string methodName = "")
        {
            SaveMessage("Trace", "END", parameters, methodPath, methodName);
        }
        public static void Trace(dynamic parameters = null, [CallerFilePath] string methodPath = "", [CallerMemberName] string methodName = "")
        {
            SaveMessage("Trace", "", parameters, methodPath, methodName);
        }
        public static void Trace(string message, dynamic parameters = null, [CallerFilePath] string methodPath = "", [CallerMemberName] string methodName = "")
        {
            SaveMessage("Trace", message, parameters, methodPath, methodName);
        }

        public static void Fatal(Exception ex)
        {
            if (ex != null)
            {
                Fatal($"Exception:\"{ex.Message}\" | StackTrace:\"{ex.StackTrace}\"");
            }
        }

        public static void Fatal(string message, dynamic parameters = null, [CallerFilePath] string methodPath = "", [CallerMemberName] string methodName = "")
        {
            SaveMessage("Fatal", message, parameters, methodPath, methodName);
        }

        private static string GetCallerParams(dynamic list = null)
        {
            var sb = new StringBuilder();

            try
            {
                if (list != null)
                {
                    foreach (PropertyInfo pi in list?.GetType().GetProperties())
                    {
                        sb.Append(
                            string.Format("{0}:{1} | ",
                                            pi.Name,
                                            JsonConvert.SerializeObject(pi.GetValue(list, null),
                                            new JsonSerializerSettings
                                            {
                                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                PreserveReferencesHandling = PreserveReferencesHandling.Objects
                                            }))
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                SaveMessage($"Inner log Fatal || GetCallerParams || Exception:\"{ex.Message}\" | StackTrace:\"{ex.StackTrace}\"");
            }

            return sb.ToString();
        }

        private static void SaveMessage(string recordType, string message, dynamic parameters = null, string methodPath = "", string methodName = "")
        {
            try
            {
                if (IsEnabled)
                {
                    SaveMessage($"{recordType} || {Path.GetFileNameWithoutExtension(methodPath)}.{methodName} || {message} || {GetCallerParams(parameters)}");
                }
            }
            catch (Exception ex)
            { }
        }

        private static void SaveMessage(string message)
        {
            try
            {
                if (IsEnabled)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));

                    using (TextWriter tsw = new StreamWriter(LogFilePath, true))
                    {
                        tsw.WriteLine($"{DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.ff")} || {message}");
                    }
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
