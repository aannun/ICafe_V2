using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICafeUI.Core
{
    public static class ConnectionViewGenerator
    {
        struct NodePair
        {
            public ICafe.Core.Node A, B;
        }

        struct WindowData
        {
            public Window window;
            public ConnectionView cv;
        }

        static Dictionary<NodePair, WindowData> active_windows;

        static ConnectionViewGenerator()
        {
            active_windows = new Dictionary<NodePair, WindowData>();
        }

        public static Window GenerateViewWindow(ICafe.Core.Node node_input, ICafe.Core.Node node_output)
        {
            Window win = null;
            if (WindowExists(ref win, node_input, node_output))
            {
                win.Focus();
                return win;
            }

            win = WindowFactory.GetNewWindow();
            win.Background = Brushes.DarkGray;
            win.SizeToContent = SizeToContent.Height;
            win.Width = 500;

            var scrollview = new ScrollViewer();
            ConnectionView view = new ConnectionView(node_input, node_output, win);
            scrollview.Content = view;
            win.Content = scrollview;

            win.Closed += Win_Closed;
            active_windows.Add(new NodePair { A = node_input, B = node_output }, new WindowData { window = win, cv = view });

            win.Show();

            return win;
        }

        private static void Win_Closed(object sender, EventArgs e)
        {
            foreach (KeyValuePair<NodePair, WindowData> item in active_windows)
            {
                if (item.Value.window == sender)
                {
                    active_windows.Remove(item.Key);
                    return;
                }
            }
        }

        static bool WindowExists(ref Window win, ICafe.Core.Node A, ICafe.Core.Node B)
        {
            foreach (KeyValuePair<NodePair, WindowData> item in active_windows)
            {
                if ((item.Key.A == A && item.Key.B == B) || (item.Key.B == A && item.Key.A == B))
                {
                    win = item.Value.window;
                    return true;
                }
            }
            return false;
        }

        static bool ConnectionExists(ref ConnectionView view, ICafe.Core.Node A, ICafe.Core.Node B)
        {
            foreach (KeyValuePair<NodePair, WindowData> item in active_windows)
            {
                if ((item.Key.A == A && item.Key.B == B) || (item.Key.B == A && item.Key.A == B))
                {
                    view = item.Value.cv;
                    return true;
                }
            }
            return false;
        }

        public static void CloseConnection(ICafe.Core.Node A, ICafe.Core.Node B)
        {
            Window win = null;
            if (WindowExists(ref win, A, B))
                win.Close();
        }

        public static void CloseConnection(ICafe.Core.Node node)
        {
            List<Window> to_close = new List<Window>();
            foreach (KeyValuePair<NodePair, WindowData> item in active_windows)
                if (item.Key.A == node || item.Key.B == node)
                    to_close.Add(item.Value.window);

            for (int i = 0; i < to_close.Count; i++)
                to_close[i].Close();
        }

        public static void RefreshConnection(ICafe.Core.Node sender, ICafe.Core.Node other)
        {
            ConnectionView cv = null;
            if (ConnectionExists(ref cv, sender, other))
                cv.RefreshConnections();
        }

        public static void RefreshConnections(ICafe.Core.Node node)
        {
            List<ConnectionView> cv = new List<ConnectionView>();
            foreach (KeyValuePair<NodePair, WindowData> item in active_windows)
                if (item.Key.A == node || item.Key.B == node)
                    cv.Add(item.Value.cv);

            for (int i = 0; i < cv.Count; i++)
                cv[i].RefreshConnections();
        }
    }
}
