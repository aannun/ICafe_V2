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
        public event EventHandler OnDestroy;
        public Action<bool> OnHideChanged;

        public ICafe.Core.Node ExecutingNode { get; private set; }
        public Guid ID { get; private set; }

        Window window;

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

        bool state_open, force_state;

        public Node(ICafe.Core.Node node)
        {
            InitializeComponent();
            DataContext = this;

            ExecutingNode = node;
            ID = ExecutingNode.ID;

            BindName();

            state_open = true;
            SetLock(true);
            SetState(state_open);

            InitINContainerData();
            InitOUTContainerData();

            Loaded += Node_Loaded;
        }

        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closing += Node_Closing;
        }

        private void Node_Closing(object sender, CancelEventArgs e)
        {
            UnbindName();
        }

        void BindName()
        {
            var name_field = ExecutingNode.GetNameField();
            name_field.Field.PropertyChanged += UpdateName;
            Name.Text = ExecutingNode.NodeName;
        }

        void UnbindName()
        {
            var name_field = ExecutingNode.GetNameField();
            name_field.Field.PropertyChanged -= UpdateName;
        }

        void UpdateName(object sender, PropertyChangedEventArgs args)
        {
            Name.Text = ExecutingNode.NodeName;
        }

        public void RefreshConnections()
        {
            foreach (var item in IN_Container.Children)
                ((Node_xaml.NodePin)item).RefreshConnections();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AdornerLayer.GetAdornerLayer(this).Add(new Dragger(this));
        }

        void ResetCurrentNode()
        {
            Core.StateContainer.SetCurrenthandledNode(null);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (Core.StateContainer.CurrentHandledNode != null && Core.StateContainer.CurrentHandledNode != this) return;

            if (window == null)
            {
                window = Core.ViewGenerator.GenerateViewWindow(ExecutingNode);
                window.Closed += Window_Closed;
            }
            else window.Focus();

            e.Handled = true;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            SetState(true);

        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            SetState(false);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            SetState(true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            SetState(false);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            window.Closed -= Window_Closed;
            window = null;
        }

        public void Destroy()
        {

            foreach (var item in IN_Container.Children)
            {
                OnHideChanged -= (x) => ((Node_xaml.NodePin)item).Hidden = x;
                ((Node_xaml.NodePin)item).Destroy();
            }
            foreach (var item in OUT_Container.Children)
            {
                OnHideChanged -= (x) => ((Node_xaml.NodePin)item).Hidden = x;
                ((Node_xaml.NodePin)item).Destroy();
            }

            IN_Container.Children.Clear();
            OUT_Container.Children.Clear();

            OnDestroy?.Invoke(this, null);
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

            bool should_add = Core.Selector.IsAddKeyDown() ? true : (Core.Selector.SelectedCount > 1);
            Core.Selector.AddSelection(this, should_add);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetLock(!force_state);
        }

        void SetLock(bool state)
        {
            force_state = state;
            LockButton.Text = force_state ? "UNLOCK" : "LOCK";
        }

        void SetState(bool state)
        {
            state_open = state;

            if (force_state == true && !state_open)
                return;

            DataPanel.Visibility = state_open ? Visibility.Visible : Visibility.Collapsed;

            OnHideChanged?.Invoke(!state_open);
        }
        
        void InitINContainerData()
        {
            if (ExecutingNode == null) return;

            var list = ExecutingNode.GetParameters();
            for (int i = 0; i < list.Length; i++)
            {
                for (int j = 0; j < list[i].Lenght; j++)
                {
                    Node_xaml.NodePin pin = new Node_xaml.NodePin(true, list[i].Parameter.ParameterType, list[i].Parameter.Name, list[i].GetName(j), Name, this, list[i].Lenght > 1, j);
                    IN_Container.Children.Add(pin);
                    OnHideChanged += (x) => pin.Hidden = x;
                }
            }
        }

        void InitOUTContainerData()
        {
            if (ExecutingNode == null) return;

            var list = ExecutingNode.GetFields();
            for (int i = 0; i < list.Length; i++)
            {
                if (!list[i].IsOut) continue;

                Node_xaml.NodePin pin = new Node_xaml.NodePin(false, list[i].Field.field.FieldType, list[i].Field.field.Name, list[i].Field.field.Name, Name, this, false);
                OUT_Container.Children.Add(pin);
                OnHideChanged += (x) => pin.Hidden = x;
            }
        }

        public Point GetPosition()
        {
            var parent = VisualTreeHelper.GetParent(this) as UIElement;
            Point p = TranslatePoint(new Point(RenderSize.Width * 0.5f, RenderSize.Height * 0.5f), parent);
            return new Point(p.X - parent.RenderSize.Width / 2, p.Y - parent.RenderSize.Height / 2);
        }
    }
}
