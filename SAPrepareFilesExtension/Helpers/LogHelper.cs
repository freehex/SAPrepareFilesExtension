using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPrepareFilesExtension.Helpers
{
    public static class LogHelper
    {
        public static bool IsEnabled
        {
            get { return GeneralSettings.Default.EnableLogging; }
        }
    }
}
