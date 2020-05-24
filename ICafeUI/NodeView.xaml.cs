using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Logica di interazione per NodeView.xaml
    /// </summary>
    public partial class NodeView : UserControl
    {
        public Action<object, object> DragEnd;
        bool open;

        public NodeView()
        {
            InitializeComponent();

            PopulateView();

            Button.Click += Button_Click;
            Main.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            open = !open;
            RefreshVisibility();
        }

        void RefreshVisibility()
        {
            Main.Visibility = open ? Visibility.Visible : Visibility.Collapsed;
        }

        void AddHandler()
        {
            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(HandleClickOutsideOfControl), true);
        }

        private void HandleClickOutsideOfControl(object sender, MouseButtonEventArgs e)
        {
            open = false;
            RefreshVisibility();

            ReleaseMouseCapture();
        }

        void PopulateView()
        {
            var list = ICafe.Core.Node.GetAllNodeTypes();
            for (int i = 0; i < list.Length; i++)
            {
                var name = ICafe.Core.Registry.GetNodeNameFromType(list[i]);
                var b = CreateInteractiveButton(name);
                All.Children.Add(b);
            }
        }

        Button CreateInteractiveButton(string content)
        {
            Button t = new Button { Content = content, Background = Brushes.Transparent, Foreground = Brushes.White, BorderBrush = Brushes.Transparent, HorizontalAlignment = HorizontalAlignment.Left };
            t.MouseDoubleClick += T_MouseDoubleClick;
            t.PreviewMouseLeftButtonDown += T_Click;

            return t;
        }

        private void T_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            DragDrop.DoDragDrop(this, button.Content, DragDropEffects.Move);
            DragEnd?.Invoke(this, button.Content);
        }

        private void T_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Core.StateContainer.AddNode(((Button)sender).Content.ToString(), new Point(0.5f, 0.5f), true);
        }

        private void TxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Visibility visibility = txtSearchBox.Text.Length == 0 ? Visibility.Visible : Visibility.Collapsed;

            txtHint.Visibility = Freq_Exp.Visibility = visibility;
            if (visibility == Visibility.Collapsed) All_Exp.IsExpanded = true;

            RefreshSearch(txtSearchBox.Text);
        }

        void RefreshSearch(string text)
        {
            if (text.Length > 0)
            {
                var txt = text.ToUpper();
                foreach (var item in All.Children)
                {
                    var b = (Button)item;
                    string content = b.Content.ToString();

                    if (content.ToUpper().Contains(txt))
                        b.Visibility = Visibility.Visible;
                    else b.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                foreach (var item in All.Children)
                    ((UIElement)item).Visibility = Visibility.Visible;
            }
        }

        void RefreshFrequent()
        {
        }
    }
}
