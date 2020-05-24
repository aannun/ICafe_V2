using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logica di interazione per NodeData.xaml
    /// </summary>
    public partial class NodeData : UserControl
    {
        ICafe.Core.Node node;

        Dictionary<Control, DependencyProperty> bindings;

        public NodeData(ICafe.Core.Node node)
        {
            InitializeComponent();
            this.node = node;

            Loaded += NodeData_Loaded;

            bindings = new Dictionary<Control, DependencyProperty>();
            BindNameField();
        }

        private void NodeData_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closing += NodeData_Closing;
        }

        private void NodeData_Closing(object sender, CancelEventArgs e)
        {
            Window.GetWindow(this).Closing -= NodeData_Closing;

            UnbindNameFIeld();
            UnbindAll();
        }

        public void UpdateName(object sender, PropertyChangedEventArgs args)
        {
            NodeName_Label.Content = node.GetNameField().Field.Property;
        }

        private void TxtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHint.Visibility = txtSearchBox.Text.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
            RefreshSearch(txtSearchBox.Text);
        }

        void RefreshSearch(string text)
        {
            if (text.Length > 0)
            {
                var txt = text.ToUpper();
                foreach (var item in Data.Children)
                {
                    var b = (NodeDataEntry)item;
                    string content = b.GetDataName();

                    if (content.ToUpper().Contains(txt))
                        b.Visibility = Visibility.Visible;
                    else b.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                foreach (var item in Data.Children)
                    ((UIElement)item).Visibility = Visibility.Visible;
            }
        }

        public void AddElement(UIElement element)
        {
            Data.Children.Add(element);
        }

        void BindNameField()
        {
            var name_field = node.GetNameField();
            name_field.Field.PropertyChanged += UpdateName;
            NodeName_Label.Content = node.NodeName;
        }

        void UnbindNameFIeld()
        {
            var name_field = node.GetNameField();
            name_field.Field.PropertyChanged -= UpdateName;
        }

        public void Bind(ICafe.Core.ReactiveField element, Control ui, DependencyProperty property)
        {
            Binding b = new Binding("Property");
            b.Source = element;
            b.Mode = BindingMode.TwoWay;
            ui.SetBinding(property, b);

            bindings.Add(ui, property);
        }

        void Unbind(Control ui, DependencyProperty property)
        {
            BindingOperations.ClearBinding(ui, property);

            bindings.Remove(ui);
        }

        void UnbindAll()
        {
            var list = bindings.Keys.ToArray();
            for (int i = list.Length - 1; i >= 0; i--)
            {
                Unbind(list[i], bindings[list[i]]);
            }
        }
    }
}
