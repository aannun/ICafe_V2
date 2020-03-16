using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ICafeUI.Core
{
    public static class WindowFactory
    {
        static List<Window> windows;

        static WindowFactory()
        {
            windows = new List<Window>();
        }

        public static Window GetNewWindow()
        {
            Window w = new Window();
            w.Closed += W_Closed;
            windows.Add(w);

            return w;
        }

        private static void W_Closed(object sender, EventArgs e)
        {
            windows.Remove(sender as Window);
        }

        public static void CloseAllWindows()
        {
            for (int i = 0; i < windows.Count; i++)
                windows[i].Close();
            windows.Clear();
        }
    }
}
