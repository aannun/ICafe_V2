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
using static ICafe.Core.Node;

namespace ICafeUI
{
    /// <summary>
    /// Logica di interazione per ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        ICafe.Core.Node input, output;
        Window view;

        FieldData current_field;
        List<DependencyObject> results;
        ContextMenu cm;

        float separator_height = 2;
        ConnectionHandler handler;
        bool handler_active;

        struct DestroyConnectionData
        {
            public bool isInput;
            public string field_name;
        }
        DestroyConnectionData curr_data;

        public ConnectionView(ICafe.Core.Node Input, ICafe.Core.Node Output, Window View)
        {
            InitializeComponent();

            input = Input;
            output = Output;
            view = View;

            InputName.Content = Input.NodeName;
            OutputName.Content = Output.NodeName;

            PopulateInputList();
            PopulateOutputList();

            results = new List<DependencyObject>();

            view.Activated += View_Activated;
            view.SizeChanged += View_SizeChanged;

            InitializeContextMenu();
        }

        private void View_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshConnections();
        }

        private void View_Activated(object sender, EventArgs e)
        {
            RefreshConnections();
        }

        void InitConnectionHandler(UIElement parent)
        {
            handler = new ConnectionHandler(parent, view, parent.TranslatePoint(new Point(0, 0), HandlerSlot));
            handler.OnDeactivate += HandlerDeactivated;
            if (!HandlerSlot.Children.Contains(handler))
                HandlerSlot.Children.Add(handler);
        }

        UIElement AddRow(Grid grid, string value, bool insert_separator = true)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            Border border = new Border { Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
            border.Child = new Label { Content = value, HorizontalAlignment = HorizontalAlignment.Center, IsHitTestVisible = false };
            Grid.SetRow(border, grid.RowDefinitions.Count - 1);
            grid.Children.Add(border);

            //separator
            if (!insert_separator) return border;

            grid.RowDefinitions.Add(new RowDefinition());
            Separator s = new Separator { Height = separator_height };
            Grid.SetRow(s, grid.RowDefinitions.Count - 1);
            grid.Children.Add(s);

            return border;
        }

        void PopulateInputList()
        {
            var list = input.GetParameters();
            for (int i = 0; i < list.Length; i++)
            {
                UIElement element = AddRow(Inputs, list[i].Parameter.Name);
                element.MouseRightButtonDown += Element_MouseRightButtonDown;
            }
        }

        void PopulateOutputList()
        {
            var list = output.GetFields();
            for (int i = 0; i < list.Length; i++)
            {
                UIElement element = AddRow(Outputs, list[i].Field.field.Name);
                element.MouseLeftButtonDown += ConnectionView_MouseButtonDown;
                element.MouseRightButtonDown += Element_MouseRightButtonDown;
            }
        }

        private void Element_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            string value = ((sender as Border).Child as Label).Content.ToString();

            if (Inputs.Children.Contains(sender as UIElement))
            {
                var v = input.GetParameter(value);
                OpenContextMenu(false, value, null);
            }
            else
            {
                //var v = output.GetField(value);
                //OpenContextMenu(true, value, v.Inputs);
            }
        }

        private void HandlerDeactivated()
        {
            if (handler_active)
            {
                handler_active = false;
                SetColorByType(Brushes.White, Brushes.White, current_field.Field.field.FieldType);
                RefreshInputColors();
                DragEnd();
                current_field = null;
            }
        }

        string GetElementValue(Border border)
        {
            return (border.Child as Label).Content.ToString();
        }

        private void ConnectionView_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            handler_active = true;
            InitConnectionHandler(sender as UIElement);
            current_field = output.GetField(GetElementValue(sender as Border));
            SetColorByType(Brushes.LightGreen, Brushes.LightCoral, current_field.Field.field.FieldType);
        }

        private void DragEnd()
        {
            results.Clear();
            Point p = Mouse.GetPosition(view);
            VisualTreeHelper.HitTest(view, null, new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(p));

            for (int i = 0; i < results.Count; i++)
            {
                Border elem = results[i] as Border;
                if (elem == null) continue;

                if (Outputs.Children.Contains(elem)) continue;

                Label label = elem.Child as Label;
                if (label == null) continue;

                var param = input.GetParameter(label.Content.ToString());
               // ICafe.Core.Connector.CreateConnection(output, current_field.Field.field.Name, input, param.Parameter.Name);

                break;
            }

            current_field = null;
            RefreshConnections();
        }

        void SetColorByType(Brush color, Brush fail_color, Type type)
        {
            var list = input.GetParameters();
            for (int i = 0; i < list.Length; i++)
            {
                Border border = Inputs.Children[i * 2] as Border;
                if (border == null) continue;

                border.Background = ICafe.Core.Node.CanBeAssigned(type, list[i].Parameter.ParameterType) ? color : fail_color;
            }
        }

        public void RefreshConnections()
        {
            var list = input.GetParameters();
            Connections.Children.Clear();

            int count = 0;
            for (int i = 0; i < list.Length; i++)
            {
                //if (!list[i].IsAssigned()) continue;

                ParameterData pd = list[i];
                for (int j = 0; j < pd.Count; j++)
                {

                    if (pd.Inputs[j].Node != output) continue;

                    Border left = GetElementFromGrid(Outputs, pd.Inputs[j].Field.field.Name);
                    Border right = GetElementFromGrid(Inputs, pd.Parameter.Name);

                    if (left == null || right == null) continue;

                    Point start = left.TranslatePoint(new Point(0, -left.ActualHeight * 2), HandlerSlot);
                    Point end = right.TranslatePoint(new Point(-right.ActualWidth, -right.ActualHeight * 2), HandlerSlot);

                    Line l = new Line { Stroke = Brushes.Black, StrokeThickness = 2 };
                    l.X1 = start.X;
                    l.X2 = end.X;
                    l.Y1 = start.Y;
                    l.Y2 = end.Y;

                    Connections.Children.Add(l);
                    count++;
                }
            }

            RefreshInputColors();

            if (count == 0)
                view.Close();
        }

        public void RefreshInputColors()
        {
            var list = input.GetParameters();
            for (int i = 0; i < list.Length; i++)
            {
                var elem = GetElementFromGrid(Inputs, list[i].Parameter.Name);
                //if (list[i].IsFullyAssigned())
                //    elem.Background = Brushes.Gray;
                //else elem.Background = Brushes.White;
            }
        }

        Border GetElementFromGrid(Grid grid, string label_value)
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                Border elem = grid.Children[i] as Border;
                if (elem == null) continue;

                if (GetElementValue(elem) == label_value) return elem;
            }
            return null;
        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            results.Add(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }

        void InitializeContextMenu()
        {
            cm = Grid.FindResource("ContextMenu") as ContextMenu;
            (cm.Items[0] as MenuItem).Click += ConnectionView_Click;
        }

        void OpenContextMenu(bool DestoySingleConnection, string name, List<ParameterData> parameters)
        {
            bool is_input = true;
            if (DestoySingleConnection)
            {
                MenuItem m = (cm.Items[1] as MenuItem);
                m.Visibility = Visibility.Visible;
                m.Items.Clear();

                for (int i = 0; i < parameters.Count; i++)
                {
                    MenuItem item = new MenuItem { Header = parameters[i].Parameter.Name };
                    item.Click += Item_Click;
                    m.Items.Add(item);
                }

                is_input = false;
            }
            else (cm.Items[1] as MenuItem).Visibility = Visibility.Collapsed;

            cm.IsOpen = true;
            curr_data = new DestroyConnectionData { isInput = is_input, field_name = name };
        }

        private void ConnectionView_Click(object sender, RoutedEventArgs e)
        {
            //if (curr_data.isInput)
            //    ICafe.Core.Connector.RemoveConnection(input, curr_data.field_name, output);
            //else
            //    ICafe.Core.Connector.RemoveConnections(output, curr_data.field_name);

            RefreshConnections();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var list = input.GetParameters();
            //for (int i = 0; i < list.Length; i++)
            //    ICafe.Core.Connector.RemoveConnection(input, list[i].Parameter.Name, output);

            RefreshConnections();
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            string value = (sender as MenuItem).Header.ToString();

            //ICafe.Core.Connector.RemoveConnection(input, value, curr_data.field_name);
            //RefreshConnections();
        }
    }
}
