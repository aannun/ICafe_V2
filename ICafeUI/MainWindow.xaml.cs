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
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IControlInterface
    {
        Dictionary<Guid, Node> nodes;
        ContextMenu cm;

        public MainWindow()
        {
            InitializeComponent();
            InitializeContextMenu();

            nodes = new Dictionary<Guid, Node>();

            ICafe.Core.Connector.OnConnectionDeleted += OnConnectionDeleted;
            Closed += MainWindow_Closed;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Core.Selector.OnKeyDown(e);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Core.WindowFactory.CloseAllWindows();
        }

        void OnConnectionDeleted(Guid idA, Guid idB)
        {
            if (nodes.ContainsKey(idA))
                nodes[idA].RefreshConnections();
            if (nodes.ContainsKey(idB))
                nodes[idB].RefreshConnections();
        }

        public void AddControl(Control control)
        {
            Grid.Children.Add(control);
        }

        public void RemoveControl(Control control)
        {
            Grid.Children.Remove(control);
        }

        void InitializeContextMenu()
        {
            cm = this.FindResource("ContextMenu") as ContextMenu;
            MenuItem item = cm.Items[0] as MenuItem;

            var list = ICafe.Core.Node.GetAllNodeTypes();

            for (int i = 0; i < list.Length; i++)
            {
                MenuItem t = new MenuItem { Header = list[i].Name, Name = list[i].Name };
                item.Items.Add(t);
                t.Click += SelectNode;
            }
        }

        private void SelectNode(object sender, RoutedEventArgs e)
        {
            MenuItem t = sender as MenuItem;

            ICafe.Core.Node n = ICafe.Core.Node.CreateNodeFromTypeName(t.Name) as ICafe.Core.Node;
            if (n == null) return;

            n.Initialize();
            Node node = new Node(this, n);

            Point p = Mouse.GetPosition(this);
            node.RenderTransform = new TranslateTransform(p.X - ActualWidth * 0.5f, p.Y - ActualHeight * 0.5f);

            node.OnDestroy += DestroyNode;
            Grid.Children.Add(node);
            nodes.Add(node.ID, node);
        }

        public void DestroyNode(Node n)
        {
            if (nodes.ContainsKey(n.ID))
            {
                Grid.Children.Remove(n);
                nodes.Remove(n.ID);
            }
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            cm.IsOpen = true;
        }

        public Node GetNode(Guid id)
        {
            if (nodes.ContainsKey(id))
                return nodes[id];
            return null;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.Selector.RemoveAllSelections();
        }
    }
}
