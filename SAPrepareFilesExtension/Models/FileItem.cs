using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPrepareFilesExtension
{
    public class FileItem
    {
        public string LocalPath { get; set; }
        public string ServerPath { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsAdd { get; set; }
        public bool IsDelete { get; set; }
    }
}
