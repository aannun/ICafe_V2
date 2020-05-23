using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class MainWindow : Window
    {
        Core.TopBar top_bar;

        bool mouseDown = false;
        bool receivedDrop = false;
        Point drop_position, mouseDownPos;

        public MainWindow()
        {
            InitializeComponent();

            Closed += MainWindow_Closed;
            Loaded += MainWindow_Loaded;

            top_bar = new Core.TopBar();
            top_bar.Player.StateChanged += UpdatePlayState;
            View.Drop += View_Drop;
            NodeCreator.DragEnd += View_ReceiveDrop;

            Core.StateContainer.Init(Container);
        }

        private void View_Drop(object sender, DragEventArgs e)
        {
            drop_position = e.GetPosition(Container);
            receivedDrop = true;
        }

        private void View_ReceiveDrop(object sender, object e)
        {
            if (receivedDrop)
            {
                var name = e.ToString();
                var pos = drop_position;
                Core.StateContainer.AddNode(name, new Point(pos.X, pos.Y));
                receivedDrop = false;
            }
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
            top_bar.Stop();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            top_bar.Play();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            top_bar.Stop();
        }

        void UpdatePlayState()
        {
            bool stop = !top_bar.Player.IsPlaying;

            Stop.Visibility = stop ? Visibility.Collapsed : Visibility.Visible;
            Play.Visibility = !stop ? Visibility.Collapsed : Visibility.Visible;
            View.Background = stop ? Brushes.LightGray : Brushes.LightBlue;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            top_bar.SaveAs();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            top_bar.SaveAs();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            top_bar.Open();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            top_bar.New();
        }

        private void View_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                // When the mouse is held down, reposition the drag selection box.

                Point mousePos = e.GetPosition(View);

                if (mouseDownPos.X < mousePos.X)
                {
                    Canvas.SetLeft(selectionBox, mouseDownPos.X);
                    selectionBox.Width = mousePos.X - mouseDownPos.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, mousePos.X);
                    selectionBox.Width = mouseDownPos.X - mousePos.X;
                }

                if (mouseDownPos.Y < mousePos.Y)
                {
                    Canvas.SetTop(selectionBox, mouseDownPos.Y);
                    selectionBox.Height = mousePos.Y - mouseDownPos.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, mousePos.Y);
                    selectionBox.Height = mouseDownPos.Y - mousePos.Y;
                }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Core.Selector.RemoveAllSelections();

            mouseDown = true;
            mouseDownPos = e.GetPosition(View);
            Container.CaptureMouse();

            // Initial placement of the drag selection box.         
            Canvas.SetLeft(selectionBox, mouseDownPos.X);
            Canvas.SetTop(selectionBox, mouseDownPos.Y);
            selectionBox.Width = 0;
            selectionBox.Height = 0;

            // Make the drag selection box visible.
            selectionBox.Visibility = Visibility.Visible;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!mouseDown) return;

            mouseDown = false;
            Container.ReleaseMouseCapture();

            // Hide the drag selection box.
            selectionBox.Visibility = Visibility.Collapsed;

            Point mouseUpPos = e.GetPosition(View);
            Core.Selector.SelectInRect(mouseDownPos, mouseUpPos);
        }
    }
}
