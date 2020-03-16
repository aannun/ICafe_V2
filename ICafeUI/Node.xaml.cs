using ICafeUI.Adorners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logica di interazione per Node.xaml
    /// </summary>
    public partial class Node : UserControl, Core.ISelectable
    {
        public Action<Node> OnDestroy;
        public Action<bool> OnListToggle;
        public UIElement View { get; private set; }
        public ICafe.Core.Node ExecutingNode { get; private set; }
        public Guid ID { get; private set; }

        Window window;
        ParametersList list;

        public Point LeftPoint
        {
            get
            {
                return new Point(0, ActualHeight * 0.5f);
            }
        }
        public Point RightPoint
        {
            get
            {
                return new Point(ActualWidth, ActualHeight * 0.5f);
            }
        }

        public List<Node> Inputs { get; private set; }
        public List<Node> Outputs { get; private set; }

        ConnectionHandler con_handler;

        bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                if (value != selected)
                {
                    selected = value;
                    Border.BorderBrush = selected ? Brushes.OrangeRed : Brushes.Black;
                }
            }
        }

        public Node(UIElement view, ICafe.Core.Node node)
        {
            InitializeComponent();
            DataContext = this;

            this.View = view;

            ExecutingNode = node;
            ID = ExecutingNode.ID;

            Name.Text = ExecutingNode.NodeName;

            LeftConnector.Init(true, this);
            RightConnector.Init(false, this);
            LeftConnector.SetColor(Brushes.Red);
            RightConnector.SetColor(Brushes.LimeGreen);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dragger r = new Dragger(this, View);
            r.OnDrag += OnStartDrag;
            r.MoveDrag += OnMoveDrag;
            r.OnDragEnd += OnEndDrag;
            AdornerLayer.GetAdornerLayer(this).Add(new Dragger(this, r));
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (con_handler == null)
            {
                con_handler = new ConnectionHandler(this, View, new Point(0, 0));
                Handler.Children.Add(con_handler);
                con_handler.OnDeactivate += ResetCurrentNode;
            }
            else con_handler.Activate();

            Core.StateContainer.SetCurrenthandledNode(this);
            e.Handled = true;
        }

        void ResetCurrentNode()
        {
            Core.StateContainer.SetCurrenthandledNode(null);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (Core.StateContainer.CurrentHandledNode != null && Core.StateContainer.CurrentHandledNode != this) return;

            if (window == null)
            {
                window = Core.ViewGenerator.GenerateViewWindow(ExecutingNode);
                window.Closed += Window_Closed;
            }
            else window.Focus();

            e.Handled = true;

            if (con_handler != null) con_handler.Reset();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed && Core.StateContainer.CurrentHandledNode != null && Core.StateContainer.CurrentHandledNode != this)
                CatchMouse();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            window = null;
        }

        public void CatchMouse()
        {
            var field = Core.StateContainer.CurrentHandledNode.ExecutingNode.GetActiveField();
            if (field == null) return;

            list = new ParametersList(this, ExecutingNode, field.field.FieldType, (float)Width, (float)Height);

            Point p = this.RenderTransform.Transform(new Point(0, 0));
            list.RenderTransform = new TranslateTransform(p.X, p.Y);

            ((IControlInterface)View).AddControl(list);
            OnListToggle?.Invoke(true);
        }

        public void ReleaseMouse()
        {
            ((IControlInterface)View).RemoveControl(list);
            OnListToggle?.Invoke(false);

            list = null;
        }

        public Node GetNodeFromID(Guid id)
        {
            if (View == null) return null;
            return ((IControlInterface)View).GetNode(id);
        }

        List<Node> GetInputNodes()
        {
            List<Node> nodes = new List<Node>();

            var list = ExecutingNode.GetParameters();
            if (list == null)
                return nodes;

            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Input != null)
                {
                    Node n = GetNodeFromID(list[i].Input.Node.ID);
                    nodes.Add(n);
                }
            }

            return nodes;
        }

        List<Node> GetOutputNodes()
        {
            List<Node> nodes = new List<Node>();

            var list = ExecutingNode.GetFields();
            if (list == null)
                return nodes;

            for (int i = 0; i < list.Length; i++)
            {
                for (int j = 0; j < list[i].Inputs.Count; j++)
                {
                    Node n = GetNodeFromID(list[i].Inputs[j].Node.ID);
                    if (n != null)
                        nodes.Add(n);
                }
            }

            return nodes;
        }

        public void RefreshConnections()
        {
            Inputs = GetInputNodes();
            Outputs = GetOutputNodes();

            LeftConnector.Refresh();
            RightConnector.Refresh();

            Core.ConnectionViewGenerator.RefreshConnections(ExecutingNode);
        }

        public void IgnoreConnection(Node ignore)
        {
            if (Inputs != null && Inputs.Contains(ignore))
                Inputs.Remove(ignore);
            if (Outputs != null && Outputs.Contains(ignore))
                Outputs.Remove(ignore);

            LeftConnector.IgnoreConnection(ignore);
            RightConnector.IgnoreConnection(ignore);
        }

        void OnStartDrag(UIElement element)
        {
            RefreshConnections();

            for (int i = 0; i < Inputs.Count; i++)
                Inputs[i].IgnoreConnection(this);
            for (int i = 0; i < Outputs.Count; i++)
                Outputs[i].IgnoreConnection(this);
        }

        void OnMoveDrag(UIElement element)
        {
            LeftConnector.UpdateConnections();
            RightConnector.UpdateConnections();
        }

        void OnEndDrag(UIElement element)
        {
            LeftConnector.UpdateConnections();
            RightConnector.UpdateConnections();
        }

        public void RequestConnection(string parameter_name)
        {
            if (Core.StateContainer.CurrentHandledNode != null)
                Core.StateContainer.CurrentHandledNode.Connect(this, parameter_name);
        }

        public void Connect(Node node, string parameter)
        {
            var hnd = new ICafe.Core.Connector.ConnectionData();

            var field = Core.StateContainer.CurrentHandledNode.ExecutingNode.GetActiveField();
            ICafe.Core.Connector.CreateConnection(ref hnd, this.ExecutingNode, field != null ? field.field.Name : null, node.ExecutingNode, parameter);
            RefreshConnections();
        }

        public void Destroy()
        {
            ICafe.Core.Connector.RemoveAllConnections(ExecutingNode);
            if (window != null)
                window.Close();
            Core.ConnectionViewGenerator.CloseConnection(this.ExecutingNode);

            OnDestroy?.Invoke(this);
        }

        public void Select()
        {
            Selected = true;
        }

        public void Deselect()
        {
            Selected = false;
        }

        public bool IsSelected()
        {
            return Selected;
        }

        public void Refresh()
        { }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Core.Selector.AddSelection(this);
        }
    }
}
