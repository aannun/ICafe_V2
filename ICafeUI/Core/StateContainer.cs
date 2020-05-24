using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ICafeUI.Core
{
    public enum ViewLevel { Background, Foreground, UI }

    public static class StateContainer
    {
        public static Action<Node> OnCurrentHandledChanged;
        public static Action<Type> OnCurrentHandledTypeChanged;

        public static Type CurrentHandledType { get; private set; }

        public static Node CurrentHandledNode { get; private set; }
        public static List<ISelectable> SelectedItems { get; private set; }
        public static List<ISelectable> Items { get; private set; }

        public static Canvas View { get; private set; }

        static StateContainer()
        {
            SelectedItems = new List<ISelectable>();
            Items = new List<ISelectable>();
        }

        public static void Init(Canvas view)
        {
            View = view;
        }

        public static void SetCurrenthandledNode(Node node)
        {
            CurrentHandledNode = node;
            OnCurrentHandledChanged?.Invoke(CurrentHandledNode);
        }

        public static void SetCurrenthandledType(Type type)
        {
            CurrentHandledType = type;
            OnCurrentHandledTypeChanged?.Invoke(CurrentHandledType);
        }

        public static void AddSelection(ISelectable item, bool override_current = true)
        {
            if (!SelectedItems.Contains(item))
                SelectedItems.Add(item);
        }

        public static void RemoveSelection(ISelectable item)
        {
            if (SelectedItems.Contains(item))
                SelectedItems.Remove(item);
        }

        public static void RemoveAllSelections()
        {
            SelectedItems.Clear();
        }

        public static void AddControl(UserControl control, ISelectable selectable, ViewLevel level)
        {
            Canvas.SetZIndex(control, (int)level);
            View.Children.Add(control);
            Items.Add(selectable);

            selectable.OnDestroy += (sender, e) => RemoveControl(sender as UserControl, level);
        }

        public static void RemoveControl(UserControl control, ViewLevel level)
        {
            View.Children.Remove(control);
            Items.Remove((ISelectable)control);

            ((ISelectable)control).OnDestroy -= (sender, e) => RemoveControl(sender as UserControl, level);
        }

        public static Point ConvertPoint(Point p)
        {
            p = new Point(p.X * View.RenderSize.Width, p.Y * View.RenderSize.Height);
            Point halsSize = new Point(0.5 * View.RenderSize.Width, 0.5 * View.RenderSize.Height);
            ScaleTransform st = (ScaleTransform)((TransformGroup)View.RenderTransform).Children.First(tr => tr is ScaleTransform);

            p = View.TransformToAncestor(Application.Current.MainWindow).Transform(p);
            return new Point(halsSize.X + (halsSize.X - p.X ) / st.ScaleX, halsSize.Y + (halsSize.Y - p.Y) / st.ScaleY);
        }

        public static void AddNode(ICafe.Core.Node n, Point position, bool convert_position = false)
        {
            if (convert_position)
                position = ConvertPoint(position);

            Node node = new Node(n);
            node.RenderTransform = new TranslateTransform(position.X, position.Y);

            AddControl(node, node, ViewLevel.Foreground);
        }

        public static void AddNode(string type_name, Point position, bool convert_position = false)
        {
            ICafe.Core.Node n = ICafe.Core.Node.CreateNodeFromUserTypeName(type_name) as ICafe.Core.Node;
            if (n == null) return;

            n.Initialize();
            AddNode(n, position, convert_position);
        }

        public static void AddNode(Type type, Point position, bool convert_position = false)
        {
            AddNode(type.Name, position, convert_position);
        }

        public static void DestroyNode(Node n)
        {
            n.Destroy();
        }

        public static void DestroyAllNodes()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
                Items[i].Destroy();
        }

        public static List<T> GetFiltered<T>()
        {
            var list = new List<T>();
            foreach (var item in Items)
            {
                if (item.GetType() == typeof(T))
                    list.Add((T)item);
            }
            return list;
        }
    }
}
