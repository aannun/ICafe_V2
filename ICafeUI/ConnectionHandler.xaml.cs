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
    /// Logica di interazione per ConnectionHandler.xaml
    /// </summary>
    public partial class ConnectionHandler : UserControl
    {
        public Action OnDeactivate;

        public ConnectionHandler(UIElement parent, UIElement view, Point offset)
        {
            InitializeComponent();

            this.IsHitTestVisible = false;

            view.MouseUp += Element_MouseUp;
            view.MouseLeave += Element_MouseLeave;
            view.MouseMove += Element_MouseMove;
            parent.MouseUp += Element_MouseUp;

            Line.X1 = parent.RenderSize.Width * 0.5f + offset.X;
            Line.Y1 = parent.RenderSize.Height * 0.5f + offset.Y;

            Activate();
            Line.X2 = Line.X1;
            Line.Y2 = Line.Y1;
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateLocation();
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            Reset();
        }

        private void Element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        void UpdateLocation()
        {
            Point point = Mouse.GetPosition(this);

            Line.X2 = point.X;
            Line.Y2 = point.Y;
        }

        public void Reset()
        {
            this.Visibility = Visibility.Collapsed;
            OnDeactivate?.Invoke();
        }

        public void Activate()
        {
            this.Visibility = Visibility.Visible;
            UpdateLocation();
        }
    }
}
