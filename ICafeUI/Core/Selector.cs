using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ICafeUI.Core
{
    static class Selector
    {
        public static ISelectable CurrentSelected { get; private set; }

        public static void AddSelection(ISelectable item)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
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
    }
}
