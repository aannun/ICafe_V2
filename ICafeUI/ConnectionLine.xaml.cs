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
    /// Logica di interazione per ConnectionLine.xaml
    /// </summary>
    public partial class ConnectionLine : UserControl
    {
        public Node NodeInput { get; private set; }
        public Node NodeOutput { get; private set; }

        float width, height;

        public ConnectionLine(float width, float height)
        {
            InitializeComponent();

            this.width = width;
            this.height = height;

            Loaded += ConnectionLine_Loaded;
            Core.StateContainer.OnCurrentHandledChanged += HandledChanged;
            MouseLeftButtonDown += ConnectionLine_MouseButtonDown;
            MouseRightButtonDown += ConnectionLine_MouseButtonDown;
        }

        private void ConnectionLine_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void HandledChanged(Node n)
        {
            IsHitTestVisible = n == null;
        }

        private void ConnectionLine_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCenter();
        }

        public void SetLineStart(double x, double y)
        {
            Line.X1 = x;
            Line.Y1 = y;

            UpdateCenter();
        }

        public void SetLineEnd(double x, double y)
        {
            Line.X2 = x;
            Line.Y2 = y;

            UpdateCenter();
        }

        public void SetNodes(Node input, Node Output)
        {
            NodeInput = input;
            NodeOutput = Output;
        }

        void UpdateCenter()
        {
            Center.RenderTransform = new TranslateTransform((Line.X2 + Line.X1) * 0.5f - ActualWidth * 0.5f, (Line.Y2 + Line.Y1) * 0.5f - ActualHeight * 0.5f);
            Center.Width = width;
            Center.Height = height;
        }

        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.ConnectionViewGenerator.GenerateViewWindow(NodeInput.ExecutingNode, NodeOutput.ExecutingNode);
            e.Handled = true;
        }
    }
}
