using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ICafeUI.Core
{
    static class Selector
    {
        public static ISelectable CurrentSelected { get; private set; }

        public static void AddSelection(ISelectable item, bool add)
        {
            if (!add)
                RemoveSelection(CurrentSelected);

            StateContainer.AddSelection(item);
            item.Select();
            CurrentSelected = item;
        }

        public static void RemoveSelection(ISelectable item)
        {
            if (item == null) return;

            StateContainer.RemoveSelection(item);
            item.Deselect();
        }

        public static void RemoveAllSelections()
        {
            for (int i = 0; i < StateContainer.SelectedItems.Count; i++)
                StateContainer.SelectedItems[i].Deselect();
            StateContainer.RemoveAllSelections();
        }

        public static void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Cancel || e.Key == Key.Delete)
            {
                for (int i = 0; i < StateContainer.SelectedItems.Count; i++)
                    StateContainer.SelectedItems[i].Destroy();
                RemoveAllSelections();
            }
        }

        public static void SelectInRect(Point start, Point end)
        {
            Point max = new Point(Math.Max(end.X, start.X), Math.Max(end.Y, start.Y));
            Point min = new Point(Math.Min(end.X, start.X), Math.Min(end.Y, start.Y));

            Point size = new Point(max.X - min.X, max.Y - min.Y);
            var list = StateContainer.GetFiltered<Node>();

            foreach (var item in list)
            {
                Point p = item.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

                if (Contains(p, min, size))
                    AddSelection(item, true);
            }
        }

        static bool Contains(Point p, Point r, Point size)
        {
            if (p.X >= r.X && p.X <= r.X + size.X && p.Y >= r.Y && p.Y <= r.Y + size.Y) return true;
            return false;
        }

        public static bool IsAddKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        public static void MoveAllSelected(Point delta)
        {
            foreach (var item in StateContainer.SelectedItems)
            {
                UIElement elem = item as UIElement;
                TranslateTransform tr = elem.RenderTransform as TranslateTransform;

                elem.RenderTransform = new TranslateTransform(tr.X + delta.X, tr.Y + delta.Y);
            }
        }
    }
}
