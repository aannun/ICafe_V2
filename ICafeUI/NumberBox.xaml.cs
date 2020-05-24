using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logica di interazione per NumberBox.xaml
    /// </summary>
    public partial class NumberBox : UserControl
    {
        private static readonly Regex regex = new Regex("[^0-9.-]+");

        Type type;
        MethodInfo parse;

        public object Value
        {
            get
            {
                return this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
                Text.Text = value.ToString();
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(NumberBox));

        public NumberBox(Type type)
        {
            InitializeComponent();

            this.type = type;

            MethodInfo[] ms = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < ms.Length; i++)
            {
                if (ms[i].Name == "Parse")
                {
                    ParameterInfo[] pms = ms[i].GetParameters();
                    if (pms.Length == 1 && pms[0].ParameterType == typeof(string))
                    {
                        parse = ms[i];
                        break;
                    }
                }
            }

            if (parse != null)
            {
                Text.PreviewTextInput += Text_PreviewTextInput;
                Text.PreviewKeyDown += Text_PreviewKeyDown;
                Text.TextChanged += Text_TextChanged;
                //Text.LostFocus += Text_LostFocus;
            }
        }

        //private void Text_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    Text.Text = Value.ToString();
        //}

        private void Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            object result;
            if (TryParse(Text.Text, type, out result)) Value = result;
        }

        private void Text_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
            else if (e.Key == Key.Return || e.Key == Key.Enter) { Keyboard.ClearFocus(); }
            //Text_LostFocus(null, null); }
        }

        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (regex.IsMatch(e.Text)) e.Handled = true;
        }

        bool TryParse(string text, Type type, out object result)
        {
            result = null;
            if (parse != null)
            {
                try
                {
                    result = parse.Invoke(null, new object[] { text });
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
