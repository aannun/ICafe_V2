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
    /// Logica di interazione per Node_Line.xaml
    /// </summary>
    public partial class Node_Line : UserControl, Core.ISelectable
    {
        public event EventHandler OnDestroy;

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Point), typeof(Node_Line), new FrameworkPropertyMetadata(default(Point)));
        public Point Source { get { return (Point)this.GetValue(SourceProperty); } set { this.SetValue(SourceProperty, value); } }

        public static readonly DependencyProperty MidPoint1Property = DependencyProperty.Register("MidPoint1", typeof(Point), typeof(Node_Line), new FrameworkPropertyMetadata(default(Point)));
        public Point MidPoint1 { get { return (Point)this.GetValue(MidPoint1Property); } set { this.SetValue(MidPoint1Property, value); } }

        public static readonly DependencyProperty MidPoint2Property = DependencyProperty.Register("MidPoint2", typeof(Point), typeof(Node_Line), new FrameworkPropertyMetadata(default(Point)));
        public Point MidPoint2 { get { return (Point)this.GetValue(MidPoint2Property); } set { this.SetValue(MidPoint2Property, value); } }

        public static readonly DependencyProperty DestinationProperty = DependencyProperty.Register("Destination", typeof(Point), typeof(Node_Line), new FrameworkPropertyMetadata(default(Point)));
        public Point Destination { get { return (Point)this.GetValue(DestinationProperty); } set { this.SetValue(DestinationProperty, value); } }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(Node_Line), new FrameworkPropertyMetadata(default(Brush)));
        public Brush Color { get { return (Brush)this.GetValue(ColorProperty); } set { this.SetValue(ColorProperty, value); } }

        NodePin source_node, destination_node;
        bool selected;

        public Node_Line(NodePin source, NodePin destination)
        {
            //InitializeComponent();

            source_node = source;
            destination_node = destination;

            BezierSegment bezier = new BezierSegment(default(Point), default(Point), default(Point), true);

            PathFigure figure = new PathFigure(default(Point), new[] { bezier }, false);
            PathGeometry geometry = new PathGeometry(new[] { figure });

            BindingBase sourceBinding = new Binding { Source = this, Path = new PropertyPath(SourceProperty) };
            BindingBase mid1Binding = new Binding { Source = this, Path = new PropertyPath(MidPoint1Property) };
            BindingBase mid2Binding = new Binding { Source = this, Path = new PropertyPath(MidPoint2Property) };
            BindingBase destinationBinding = new Binding { Source = this, Path = new PropertyPath(DestinationProperty) };

            BindingOperations.SetBinding(figure, PathFigure.StartPointProperty, sourceBinding);
            BindingOperations.SetBinding(bezier, BezierSegment.Point1Property, mid1Binding);
            BindingOperations.SetBinding(bezier, BezierSegment.Point2Property, mid2Binding);
            BindingOperations.SetBinding(bezier, BezierSegment.Point3Property, destinationBinding);

            Content = new Path
            {
                Data = geometry,
                StrokeThickness = 5,
                Stroke = Brushes.Red,
                MinWidth = 1,
                MinHeight = 1,
            };

            BindingBase colorBinding = new Binding { Source = this, Path = new PropertyPath(ColorProperty) };
            BindingOperations.SetBinding(Content as Path, Path.StrokeProperty, colorBinding);

            Color = Brushes.Black;
            selected = false;
        }

        public void BindSourceProperty(BindingBase binding)
        {
            BindingOperations.SetBinding(this, SourceProperty, binding);
        }

        public void BindMidPoint1Property(BindingBase binding)
        {
            BindingOperations.SetBinding(this, MidPoint1Property, binding);
        }

        public void BindMidPoint2Property(BindingBase binding)
        {
            BindingOperations.SetBinding(this, MidPoint2Property, binding);
        }

        public void BindDestinationProperty(BindingBase binding)
        {
            BindingOperations.SetBinding(this, DestinationProperty, binding);
        }

        //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonDown(e);
        //    Core.Selector.AddSelection(this, Core.Selector.IsAddKeyDown());

        //    e.Handled = true;
        //}

        public void Select()
        {
            Color = Brushes.Orange;
            selected = true;
        }

        public void Deselect()
        {
            Color = Brushes.Black;
            selected = false;
        }

        public void Destroy(bool is_input, bool destroy_connection)
        {
            if (destroy_connection)
            {
                if (is_input)
                    ICafe.Core.Connector.RemoveConnection_Parameter(source_node.Node.ExecutingNode, source_node.PinName, source_node.Index);
                else
                    ICafe.Core.Connector.RemoveConnection_Field(destination_node.Node.ExecutingNode, destination_node.PinName);
            }

            source_node.RemoveConnection(this);
            destination_node.RemoveConnection(this);

            OnDestroy?.Invoke(this, null);
        }

        public void Destroy()
        {
            Destroy(false, true);
        }

        public void Refresh()
        {
        }

        public bool IsSelected()
        {
            return selected;
        }
    }
}
