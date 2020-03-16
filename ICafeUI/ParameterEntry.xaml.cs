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

        public ParameterEntry(ParametersList parent, string content, AssignType assigned)
        {
            InitializeComponent();

            this.parent = parent;
            Label.Text = content;
            EntryName = content;

            SetAssignColor(assigned);
        }

        void SetAssignColor(AssignType assigned)
        {
            switch (assigned)
            {
                case AssignType.Empty: Border.Background = Brushes.White; break;
                case AssignType.Assigned: Border.Background = Brushes.LightGray; break;
                case AssignType.AssignedToSelf: Border.Background = Brushes.LightGray; break;
            }
        }

        private void ParameterEntry_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            parent.SelectEntry(this);
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
