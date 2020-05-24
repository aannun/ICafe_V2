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
    /// Logica di interazione per FileBox.xaml
    /// </summary>
    public partial class FileBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Value", typeof(string), typeof(FileBox), new FrameworkPropertyMetadata(null));
        public string Value { get { return (string)this.GetValue(TextProperty); } set { this.SetValue(TextProperty, value); Text.Text = value; } }

        string filter;
        bool is_folder;

        public FileBox(string filter, bool is_folder)
        {
            InitializeComponent();

            this.filter = filter;
            this.is_folder = is_folder;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string path = null;

            if (is_folder)
                path = Core.FileManager.OpenFolderDialog();
            else path = Core.FileManager.OpenFileDialog(filter);

            if (path != null)
                Value = path;
        }
    }
}
