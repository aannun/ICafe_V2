using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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

        public static string OpenFileDialog(string filter)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = filter;
            if (f.ShowDialog() == true)
                return f.FileName;
            return null;
        }

        public static string OpenFolderDialog()
        {
            CommonOpenFileDialog f = new CommonOpenFileDialog();
            f.IsFolderPicker = true;
            if (f.ShowDialog() == CommonFileDialogResult.Ok)
                return f.FileName;
            return null;
        }

        public static string OpenSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                return saveFileDialog.FileName;
            return null;
        }

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
