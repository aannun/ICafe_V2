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

namespace ICafeUI.Node_xaml
{
    /// <summary>
    /// Logica di interazione per NodePin.xaml
    /// </summary>
    public partial class NodePin : UserControl
    {
        public Action<bool, bool> RemoveConnections;

        public static readonly DependencyProperty AnchorProperty = DependencyProperty.Register("AnchorPoint", typeof(Point), typeof(NodePin), new FrameworkPropertyMetadata(default(Point)));
        public Point AnchorPoint { get { return (Point)this.GetValue(AnchorProperty); } set { this.SetValue(AnchorProperty, value); } }

        public static readonly DependencyProperty AnchorOffsetProperty = DependencyProperty.Register("AnchorOffsetPoint", typeof(Point), typeof(NodePin), new FrameworkPropertyMetadata(default(Point)));
        public Point AnchorOffsetPoint { get { return (Point)this.GetValue(AnchorOffsetProperty); } set { this.SetValue(AnchorOffsetProperty, value); } }

        public bool Hidden;

        bool is_input;
        Type type;
        UIElement parent;

        bool destroyed = false;
        bool is_collection = false;
        bool is_full = false;
        bool is_valid = true;

        Brush ok_color = Brushes.LimeGreen;
        Brush collection_color = Brushes.SkyBlue;
        Brush bad_color = Brushes.Red;
        Brush empty_color = Brushes.White;

        CornerRadius radius = new CornerRadius(10);
        CornerRadius array_radius = new CornerRadius(2);

        public Node Node { get; private set; }
        public string PinName { get; private set; }
        public int Index { get; private set; }

        public NodePin(bool is_input, Type type, string name, string text_name, UIElement parent, Node node, bool is_collection, int index = 0)
        {
            InitializeComponent();

            this.is_input = is_input;
            this.type = type;
            this.parent = parent;
            this.Node = node;
            this.PinName = name;
            this.Index = index;

            Grid.SetColumn(Image, !this.is_input ? 1 : 0);
            Grid.SetColumn(Name, this.is_input ? 1 : 0);

            Container.HorizontalAlignment = this.is_input ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            Name.Text = text_name;

            Hidden = true;

            Core.StateContainer.OnCurrentHandledTypeChanged += OnTypeChanged;
            SetPinState();

            this.is_collection = is_collection;

            Image.CornerRadius = this.is_collection ? array_radius : radius;
            Int_Image.CornerRadius = this.is_collection ? new CornerRadius(array_radius.BottomLeft / 2) : new CornerRadius(radius.BottomLeft / 2);

            //InitConnections();

            SetPinState();
        }

        public void RefreshConnections()
        {
            if (!is_input) return;

            var param = Node.ExecutingNode.GetParameter(PinName);
            if (param == null) return;

            var p = param.Inputs[Index];
            if (p != null)
            {
                var id = p.Node.ID;
                var nodes = Core.StateContainer.GetFiltered<Node>();
                foreach (var item in nodes)
                {
                    if (item.ID == id)
                    {
                        var entries = item.OUT_Container.Children;
                        foreach (var e in entries)
                        {
                            if (((NodePin)e).PinName == p.Field.field.Name)
                            {
                                CreateConnection(this, (NodePin)e, false);
                                return;
                            }
                        }
                        return;
                    }
                }
            }
        }

        private void onLayoutUpdated(object sender, EventArgs e)
        {
            if (destroyed)
                return;

            Size size = Hidden ? parent.RenderSize : RenderSize;
            Point ofs = new Point(is_input ? 0 : size.Width, size.Height / 2);
            UIElement elem = Hidden ? parent : this;

            AnchorPoint = elem.TransformToVisual(Core.StateContainer.View).Transform(ofs);

            float offset = 50;
            AnchorOffsetPoint = new Point(AnchorPoint.X + (is_input ? -offset : offset), AnchorPoint.Y);
        }

        private void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null)
                return;

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                RemoveConnections?.Invoke(is_input, true);
            }
            else
            {
                Core.StateContainer.SetCurrenthandledType(type);
                DragDrop.DoDragDrop(element, new DataObject(this), DragDropEffects.Move);

                Core.StateContainer.SetCurrenthandledType(null);
            }
        }

        private void onDrop(object sender, DragEventArgs args)
        {
            FrameworkElement elem = sender as FrameworkElement;
            if (null == elem)
                return;

            IDataObject data = args.Data;
            if (!data.GetDataPresent(typeof(NodePin)))
                return;

            NodePin node = data.GetData(typeof(NodePin)) as NodePin;
            if (null == node)
                return;

            var input = is_input ? this : node;
            var output = !is_input ? this : node;

            CreateConnection(input, output, true);
        }

        public void CreateConnection(NodePin input, NodePin output, bool create)
        {
            if (input == output || input.is_input == output.is_input)
                return;

            if (create)
            {
                bool removedConnection = false;
                if (!ICafe.Core.Connector.CreateConnection(output.Node.ExecutingNode, output.PinName, input.Node.ExecutingNode, input.PinName, ref removedConnection, input.Index))
                    return;

                if (removedConnection)
                    input.RemoveConnections?.Invoke(is_input, false);
            }

            Node_Line line = new Node_Line(input, output);
            Core.StateContainer.AddControl(line, line, Core.ViewLevel.Background);

            BindingBase sourceBinding = new Binding { Source = input, Path = new PropertyPath(AnchorProperty), Mode = BindingMode.OneWay };
            BindingBase mid1Binding = new Binding { Source = input, Path = new PropertyPath(AnchorOffsetProperty), Mode = BindingMode.OneWay };
            BindingBase mid2Binding = new Binding { Source = output, Path = new PropertyPath(AnchorOffsetProperty), Mode = BindingMode.OneWay };
            BindingBase destinationBinding = new Binding { Source = output, Path = new PropertyPath(AnchorProperty), Mode = BindingMode.OneWay };

            line.BindSourceProperty(sourceBinding);
            line.BindMidPoint1Property(mid1Binding);
            line.BindMidPoint2Property(mid2Binding);
            line.BindDestinationProperty(destinationBinding);

            input.AddConnection(line);
            output.AddConnection(line);
        }

        void SetPinState()
        {
            var color = is_valid ? (is_collection ? collection_color : ok_color) : bad_color;

            Image.Background = color;
            Int_Image.Background = is_full ? color : empty_color;
        }

        void OnTypeChanged(Type type)
        {
            if (type == null)
                is_valid = true;
            else
                is_valid = ICafe.Core.Node.CanBeAssigned(is_input ? type : this.type, !is_input ? type : this.type);

            SetPinState();
        }

        public void AddConnection(Node_Line line)
        {
            RemoveConnections += line.Destroy;
            is_full = is_input ? true : Node.ExecutingNode.GetField(PinName).Inputs.Count > 0;

            SetPinState();
        }

        public void RemoveConnection(Node_Line line)
        {
            RemoveConnections -= line.Destroy;
            is_full = is_input ? false : Node.ExecutingNode.GetField(PinName).Inputs.Count > 0;

            SetPinState();
        }

        public void Destroy()
        {
            Core.StateContainer.OnCurrentHandledTypeChanged -= OnTypeChanged;
            RemoveConnections?.Invoke(is_input, true);
            destroyed = true;
        }
    }
}
