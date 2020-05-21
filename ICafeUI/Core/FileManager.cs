using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafeUI.Core
{
    class FileManager
    {
        public string current_file { get; private set; }

        public void Save(string data, string path)
        {
            File.WriteAllText(path, data);
        }

        public void SaveCurrentFile(string data)
        {
            Save(data, current_file);
        }

        public string Load(string path)
        {
            current_file = path;
            return File.ReadAllText(path);
        }
    }
}
