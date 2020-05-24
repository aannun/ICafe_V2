using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ICafeUI
{
    /// <summary>
    /// Logica di interazione per NodeDataEntry.xaml
    /// </summary>
    public partial class NodeDataEntry : UserControl
    {
        public NodeDataEntry(string Name, UIElement content)
        {
            InitializeComponent();

            EntryName.Content = Name;
            Data.Child = content;
        }

        public string GetDataName()
        {
            return EntryName.Content.ToString();
        }
    }
}
