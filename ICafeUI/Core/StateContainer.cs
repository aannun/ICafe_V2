using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafeUI.Core
{
    public static class StateContainer
    {
        public static Action<Node> OnCurrentHandledChanged;

        public static Node CurrentHandledNode { get; private set; }
        public static List<ISelectable> SelectedItems { get; private set; }

        static StateContainer()
        {
            SelectedItems = new List<ISelectable>();
        }

        public static void SetCurrenthandledNode(Node node)
        {
            CurrentHandledNode = node;
            OnCurrentHandledChanged?.Invoke(CurrentHandledNode);
        }

        public static void AddSelection(ISelectable item, bool override_current = true)
        {
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
    }
}
