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
    /// Logica di interazione per DynamicTextBox.xaml
    /// </summary>
    public partial class DynamicTextBox : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(DynamicTextBox));
        public string Value
        {
            get
            {
                return (string)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
                Text.Text = value;
            }
        }

        public DynamicTextBox()
        {
            InitializeComponent();

            Text.PreviewKeyDown += Text_PreviewKeyDown;
            Text.TextChanged += Text_TextChanged;
        }

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Text.Text;
        }

        private void Text_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter) { Keyboard.ClearFocus(); }
        }
    }
}
