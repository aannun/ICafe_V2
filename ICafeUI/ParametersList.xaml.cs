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

namespace ICafeUI
{
    /// <summary>
    /// Logica di interazione per ParametersList.xaml
    /// </summary>
    public partial class ParametersList : UserControl
    {
        Node node;
        ICafe.Core.Node effect_node;
        Type type;

        float entry_height = 25;
        float final_height;
        float desired_width, desired_height;

        public ParametersList(Node node, ICafe.Core.Node effect_node, Type type, float width, float height)
        {
            InitializeComponent();

            this.node = node;
            this.effect_node = effect_node;
            this.type = type;
            desired_width = width;
            desired_height = height;

            PopulateList();

            MouseLeave += ParametersList_MouseLeave;
            Loaded += ParametersList_Loaded;
        }

        private void ParametersList_Loaded(object sender, RoutedEventArgs e)
        {
            SetDimensions();
        }

        void PopulateList()
        {
            var list = effect_node.GetParameters();
            if (list == null)
                return;

            int count = 0;
            for (int i = 0; i < list.Length; i++)
                if (ICafe.Core.Node.CanBeAssigned(type, list[i].Parameter.ParameterType))
                    count++;

            if (count == 0)
            {
                final_height = 0;
                return;
            }

            final_height = Math.Max(entry_height, desired_height / (float)count);
            for (int i = 0; i < list.Length; i++)
                if (ICafe.Core.Node.CanBeAssigned(type, list[i].Parameter.ParameterType))
                    AddEntry(list[i].Parameter.Name, GetAssignType(list[i], Core.StateContainer.CurrentHandledNode));
        }

        AssignType GetAssignType(ICafe.Core.Node.ParameterData data, Node curr_node)
        {
            if (data.Input == null)
                return AssignType.Empty;
            else if (node.GetNodeFromID(data.Input.Node.ID) == curr_node) return AssignType.AssignedToSelf;
            return AssignType.Assigned;
        }

        void AddEntry(string entry_value, AssignType assigned)
        {
            ParameterEntry entry = new ParameterEntry(this, entry_value, assigned);
            entry.Height = final_height;
            entry.Width = desired_width;

            Stack.Children.Add(entry);
        }

        void SetDimensions()
        {
            int count = Stack.Children.Count;

            if (count > 1)
            {
                (Stack.Children[0] as ParameterEntry)?.SetTop();
                (Stack.Children[count - 1] as ParameterEntry)?.SetBottom();
            }
            else if (count > 0) (Stack.Children[0] as ParameterEntry)?.SetOnlyEntry();

            Width = desired_width;
            Height = final_height == 0 ? 0 : Math.Max(Stack.Children.Count * final_height, desired_height);

            Margin = new Thickness(0, -(node.ActualHeight - Height), 0, 0);
        }

        private void ParametersList_MouseLeave(object sender, MouseEventArgs e)
        {
            node.ReleaseMouse();
        }

        public void SelectEntry(ParameterEntry entry)
        {
            node.RequestConnection(entry.EntryName);
            node.ReleaseMouse();
        }
    }
}
