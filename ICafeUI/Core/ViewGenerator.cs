using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ICafeUI.Core
{
    static class ViewGenerator
    {
        class ENUM { }
        class STRUCT { }
        class FILE { }
        class OTHER { }

        static Dictionary<Type, Func<ICafe.Core.ReactiveField, UIElement>> Converters;
        static NodeData current_data;

        static ViewGenerator()
        {
            Converters = new Dictionary<Type, Func<ICafe.Core.ReactiveField, UIElement>>();
            Converters.Add(typeof(int), NumberField);
            Converters.Add(typeof(float), NumberField);
            Converters.Add(typeof(double), NumberField);
            Converters.Add(typeof(string), TextField);
            Converters.Add(typeof(bool), BoolField);
            Converters.Add(typeof(ENUM), EnumField);
            Converters.Add(typeof(STRUCT), StructField);
            Converters.Add(typeof(FILE), FileField);
            Converters.Add(typeof(OTHER), OtherField);
        }

        public static Window GenerateViewWindow(ICafe.Core.Node node)
        {
            Window win = WindowFactory.GetNewWindow();
            win.SizeToContent = SizeToContent.Height;
            win.Width = 400;

            var data = new NodeData(node);
            current_data = data;
            win.Content = data;

            var fields = node.GetFields();
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    UIElement elem = CreateField(fields[i].Field, node);
                    if (elem != null)
                        data.AddElement(elem);
                }
            }

            current_data = null;
            win.Show();
            return win;
        }

        static UIElement CreateField(ICafe.Core.ReactiveField element, ICafe.Core.Node node)
        {
            Type t = GetConvertedType(element, element.field.FieldType);

            if (!Converters.ContainsKey(t))
                return null;

            UIElement el = Converters[t](element);
            if (el == null) return null;

            var nodeData = new NodeDataEntry(element.field.Name, el);
            return nodeData;
        }

        static Type GetConvertedType(ICafe.Core.ReactiveField field, Type type)
        {
            if (type.IsEnum)
                return typeof(ENUM);
            if (type.IsValueType && !type.IsPrimitive)
                return typeof(STRUCT);
            if (type == typeof(string) && field.field.GetCustomAttribute(typeof(FileField)) != null)
                return typeof(FILE);
            if (ICafe.Core.Node.IsTypeValid(type))
                return type;
            return typeof(OTHER);
        }

        static void Bind(ICafe.Core.ReactiveField element, Control ui, DependencyProperty property)
        {
            if (current_data != null)
                current_data.Bind(element, ui, property);
        }

        static UIElement TextField(ICafe.Core.ReactiveField element)
        {
            DynamicTextBox text = new DynamicTextBox() { VerticalAlignment = VerticalAlignment.Center };
            Bind(element, text, DynamicTextBox.ValueProperty);
            text.Value = element.Property != null ? element.Property.ToString() : null;
            return text;
        }

        static UIElement NumberField(ICafe.Core.ReactiveField element)
        {
            NumberBox text = new NumberBox(element.field.FieldType) { VerticalContentAlignment = VerticalAlignment.Center };
            Bind(element, text, NumberBox.ValueProperty);
            text.Value = element.Property != null ? element.Property : null;
            return text;
        }

        static UIElement BoolField(ICafe.Core.ReactiveField element)
        {
            CheckBox check = new CheckBox() { VerticalAlignment = VerticalAlignment.Center };
            Bind(element, check, CheckBox.IsCheckedProperty);
            check.IsChecked = (bool)element.Property;
            return check;
        }

        static UIElement EnumField(ICafe.Core.ReactiveField element)
        {
            string[] names = Enum.GetNames(element.field.FieldType);
            ComboBox box = new ComboBox() { ItemsSource = names, SelectedIndex = (int)element.Property };
            Bind(element, box, ComboBox.SelectedIndexProperty);
            box.SelectedIndex = (int)element.Property;
            return box;
        }

        static UIElement StructField(ICafe.Core.ReactiveField element)
        {
            StackPanel panel = new StackPanel();
            Expander exp = new Expander() { Content = panel };

            if (element.internal_fields != null)
            {
                for (int i = 0; i < element.internal_fields.Count; i++)
                {
                    UIElement elem = CreateField(element.internal_fields[i], null);
                    if (elem != null)
                        panel.Children.Add(elem);
                }
            }

            return exp;
        }

        static UIElement FileField(ICafe.Core.ReactiveField element)
        {
            FileField f = element.field.GetCustomAttribute(typeof(FileField)) as FileField;

            FileBox box = new FileBox(f.filter, f.is_folder) { VerticalAlignment = VerticalAlignment.Center };
            Bind(element, box, FileBox.TextProperty);
            box.Value = element.Property != null ? element.Property.ToString() : null;
            return box;
        }

        static UIElement OtherField(ICafe.Core.ReactiveField element)
        {
            return null;
        }
    }
}
