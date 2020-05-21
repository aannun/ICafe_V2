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
        public Action<ParametersList> OnDeactivate;
        public Action<ParametersList, ParameterEntry> OnSelect;

        Node node;

        EntryData[] entries;
        MouseButton button;

        float entry_height = 25;
        float final_height;
        float desired_width, desired_height;

        public struct EntryData
        {
            public string name;
            public AssignType type;
        }

        public ParametersList(Node node, EntryData[] entries, MouseButton button, float width, float height)
        {
            InitializeComponent();

            this.node = node;
            desired_width = width;
            desired_height = height;
            this.button = button;

            this.entries = entries;

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
            if (entries == null) return;

            final_height = Math.Max(entry_height, desired_height / (float)entries.Length);
            for (int i = 0; i < entries.Length; i++)
                    AddEntry(entries[i].name, entries[i].type);
        }

        void AddEntry(string entry_value, AssignType assigned)
        {
            ParameterEntry entry = new ParameterEntry(this, entry_value, assigned, button);
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
        }

        private void ParametersList_MouseLeave(object sender, MouseEventArgs e)
        {
            OnDeactivate?.Invoke(this);
            
            //node.ReleaseMouse();
        }

        public void SelectEntry(ParameterEntry entry)
        {
            OnSelect?.Invoke(this, entry);
            OnDeactivate?.Invoke(this);

            //node.RequestConnection(entry.EntryName);
            //node.ReleaseMouse();
        }
    }
}
