using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ICafeUI.Core
{
    class TopBar
    {
        const string File_Extension = ".icf";
        const string File_Filter = "ICafe files (*" + File_Extension + ")|*" + File_Extension + "";

        public ICafe.Core.Player Player { get; private set; }
        public FileManager FileManager { get; private set; }

        public TopBar()
        {
            Player = new ICafe.Core.Player(null);
            FileManager = new FileManager();
        }

        public void Play()
        {
            var list = new List<ICafe.Core.Node>();
            var nodes = StateContainer.GetFiltered<Node>();

            foreach (var item in nodes)
                list.Add(item.ExecutingNode);

            Player.InitNodes(list);
            Player.Start();
        }

        public void Stop()
        {
            if (Player != null)
                Player.Stop();
        }

        string OpenFileDialog()
        {
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = File_Filter;
            if (f.ShowDialog() == true)
                return f.FileName;
            return null;
        }

        string OpenFolderDialog()
        {
            CommonOpenFileDialog f = new CommonOpenFileDialog();
            f.IsFolderPicker = true;
            if (f.ShowDialog() == CommonFileDialogResult.Ok)
                return f.FileName;
            return null;
        }

        string GetEncodedFile()
        {
            var nodes = StateContainer.GetFiltered<Node>();
            if (nodes.Count == 0) return null;

            List<Point> positions = new List<Point>(nodes.Count);
            List<ICafe.Core.Node> ns = new List<ICafe.Core.Node>(nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                positions.Add(nodes[i].GetPosition());
                ns.Add(nodes[i].ExecutingNode);
            }
            return ICafe.Core.Encoder.Encode(ns, positions);
        }

        string OpenSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                return saveFileDialog.FileName;
            return null;
        }

        public void SaveAs()
        {
            string path = OpenSaveFileDialog();
            if (path == null) return;

            string s = GetEncodedFile();
            if (s == null) return;

            FileManager.Save(s, Path.HasExtension(path) ? path : path + File_Extension);
        }

        public void Save()
        {
            if (FileManager.current_file != null)
            {
                string s = GetEncodedFile();
                if (s == null) return;

                FileManager.SaveCurrentFile(s);
            }
            else SaveAs();
        }

        public void Open()
        {
            string path = OpenFileDialog();
            if (path == null) return;

            StateContainer.DestroyAllNodes();
            string s = FileManager.Load(path);

            //decode
            List<ICafe.Core.Node> nds;
            Dictionary<ICafe.Core.Node, Point> pss;
            if (ICafe.Core.Encoder.Decode(s, out nds, out pss) == 0)
            {
                for (int i = 0; i < nds.Count; i++)
                    StateContainer.AddNode(nds[i], pss[nds[i]]);

                var list = StateContainer.GetFiltered<Node>();
                foreach (var item in list)
                    item.RefreshConnections();
            }
        }

        MessageBoxResult OpenWantToSaveDialog()
        {
            string messageBoxText = "Do you want to save changes?";
            string caption = "ICafe";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            return MessageBox.Show(messageBoxText, caption, button, icon);
        }

        public void New()
        {
            if (FileManager.current_file != null)
            {
                var result = OpenWantToSaveDialog();
                switch (result)
                {
                    case MessageBoxResult.Cancel: return;
                    case MessageBoxResult.Yes: Save(); break;
                    case MessageBoxResult.No: break;
                }
            }

            StateContainer.DestroyAllNodes();
        }
    }
}
