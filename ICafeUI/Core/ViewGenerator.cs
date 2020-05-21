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

namespace ICafeUI.Core
{
    static class ViewGenerator
    {
        class ENUM { }
        class STRUCT { }
        class OTHER { }

        static Dictionary<Type, Func<ICafe.Core.ReactiveField, UIElement>> Converters;
        static float separator_height = 2;

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
            Converters.Add(typeof(OTHER), OtherField);
        }

        public static Window GenerateViewWindow(ICafe.Core.Node node)
        {
            Window win = WindowFactory.GetNewWindow();
            win.Background = Brushes.DarkGray;
            win.SizeToContent = SizeToContent.Height;
            win.Width = 400;
            //win.Height = 500;

            var scrollview = new ScrollViewer();
            var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            scrollview.Content = stackPanel;
            win.Content = scrollview;

            Border border = new Border { CornerRadius = new CornerRadius(5), BorderThickness = new Thickness(1), BorderBrush = Brushes.Black, Width = 250, Margin = new Thickness(0, 0, 0, 20), Background = Brushes.White };
            border.Child = new Label { Content = node.NodeName, HorizontalAlignment = HorizontalAlignment.Center };
            stackPanel.Children.Add(border);

            var fields = node.GetFields();
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    UIElement elem = CreateField(fields[i].Field, node);
                    if (elem != null)
                    {
                        stackPanel.Children.Add(elem);
                        stackPanel.Children.Add(new Separator { Height = separator_height });
                    }
                }
            }

            //if (node.GetActiveField() != null)
            //    node.GetActiveField().Active = true;
            win.Show();
            return win;
        }

        static UIElement CreateField(ICafe.Core.ReactiveField element, ICafe.Core.Node node)
        {
            Type t = GetConvertedType(element.field.FieldType);

            if (!Converters.ContainsKey(t))
                return null;

            UIElement el = Converters[t](element);
            //if (el == null)
            //    return null;

            Border border = new Border { Background = Brushes.White, BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), VerticalAlignment = VerticalAlignment.Top };
            border.Child = new Label { Content = element.field.Name };

            Grid grid = new Grid();

            for (int i = 0; i < 3; i++)
            {
                ColumnDefinition gridCol1 = new ColumnDefinition();
                ColumnDefinition gridCol2 = new ColumnDefinition();
                gridCol2.Width = new GridLength(separator_height);

                grid.ColumnDefinitions.Add(gridCol1);
                grid.ColumnDefinitions.Add(gridCol2);
            }
            grid.ColumnDefinitions[0].MaxWidth = 20;
            grid.ColumnDefinitions[2].MaxWidth = 200;

            Grid.SetColumn(border, 2);
            grid.Children.Add(border);

            if (el != null)
            {
                Grid.SetColumn(el, 4);
                grid.Children.Add(el);
            }

            if (node != null)
            {
                CheckBox box = new CheckBox { VerticalAlignment = VerticalAlignment.Top };
                Binding b = new Binding("Active");
                b.Source = element;
                b.Mode = BindingMode.TwoWay;
                box.SetBinding(CheckBox.IsCheckedProperty, b);

                Grid.SetColumn(box, 0);
                grid.Children.Add(box);
            }

            return grid;
        }

        private static void Box_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        static Type GetConvertedType(Type type)
        {
            if (type.IsEnum)
                return typeof(ENUM);
            if (type.IsValueType && !type.IsPrimitive)
                return typeof(STRUCT);
            if (ICafe.Core.Node.IsTypeValid(type))
                return type;
            return typeof(OTHER);
        }

        static void Bind(ICafe.Core.ReactiveField element, Control ui, DependencyProperty property)
        {
            Binding b = new Binding("Property");
            b.Source = element;
            b.Mode = BindingMode.TwoWay;
            ui.SetBinding(property, b);
        }

        static UIElement TextField(ICafe.Core.ReactiveField element)
        {
            TextBox text = new TextBox();
            text.VerticalContentAlignment = VerticalAlignment.Center;
            Bind(element, text, TextBox.TextProperty);
            text.Text = element.Property != null ? element.Property.ToString() : null;
            return text;
        }

        static UIElement NumberField(ICafe.Core.ReactiveField element)
        {
            NumberBox text = new NumberBox(element.field.FieldType);
            text.VerticalContentAlignment = VerticalAlignment.Stretch;
            Bind(element, text, NumberBox.ValueProperty);
            text.Text.Text = element.Property != null ? element.Property.ToString() : null;
            return text;
        }

        static UIElement BoolField(ICafe.Core.ReactiveField element)
        {
            CheckBox check = new CheckBox();
            check.VerticalAlignment = VerticalAlignment.Center;
            Bind(element, check, CheckBox.IsCheckedProperty);
            check.IsChecked = (bool)element.Property;
            return check;
        }

        static UIElement EnumField(ICafe.Core.ReactiveField element)
        {
            ComboBox box = new ComboBox();
            Bind(element, box, ComboBox.SelectedIndexProperty);
            string[] names = Enum.GetNames(element.field.FieldType);
            box.ItemsSource = names;
            box.SelectedIndex = (int)element.Property;
            return box;
        }

        static UIElement StructField(ICafe.Core.ReactiveField element)
        {
            Expander exp = new Expander();
            StackPanel panel = new StackPanel();
            exp.Content = panel;

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

        static UIElement OtherField(ICafe.Core.ReactiveField element)
        {
            return null;
        }
    }
}
