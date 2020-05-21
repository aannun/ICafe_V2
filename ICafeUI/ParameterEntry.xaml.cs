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
    public enum AssignType { Empty, Assigned, AssignedToSelf }

    /// <summary>
    /// Logica di interazione per ParameterEntry.xaml
    /// </summary>
    public partial class ParameterEntry : UserControl
    {
        public string EntryName { get; private set; }
        ParametersList parent;
        MouseButton button;
        AssignType assigned;

        public ParameterEntry(ParametersList parent, string content, AssignType assigned, MouseButton button)
        {
            InitializeComponent();

            this.parent = parent;
            Label.Text = content;
            EntryName = content;
            this.button = button;

            this.assigned = assigned;
            SetAssignColor(assigned);

            MouseUp += ParameterEntry_Select;
            MouseEnter += ParameterEntry_MouseEnter;
            MouseLeave += ParameterEntry_MouseLeave;
        }

        private void ParameterEntry_MouseLeave(object sender, MouseEventArgs e)
        {
            SetAssignColor(assigned);
        }

        private void ParameterEntry_MouseEnter(object sender, MouseEventArgs e)
        {
            Border.Background = Brushes.LightGray;
        }

        private void ParameterEntry_Select(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == button)
                parent.SelectEntry(this);
        }

        void SetAssignColor(AssignType assigned)
        {
            switch (assigned)
            {
                case AssignType.Empty: Border.Background = Brushes.White; break;
                case AssignType.Assigned: Border.Background = Brushes.Gray; break;
                case AssignType.AssignedToSelf: Border.Background = Brushes.LightGreen; break;
            }
        }

        public void SetTop()
        {
            Border.CornerRadius = new CornerRadius(10, 10, 0, 0);
        }

        public void SetBottom()
        {
            Border.CornerRadius = new CornerRadius(0, 0, 10, 10);
        }

        public void SetOnlyEntry()
        {
            Border.CornerRadius = new CornerRadius(10, 10, 10, 10);
        }
    }
}
