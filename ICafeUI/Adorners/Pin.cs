using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;

namespace ICafeUI
{
    class Pin : Adorner
    {
        Node element;
        Brush brush;
        float radius;
        bool input;

        public Pin(Node element, bool input, float radius, Brush brush) : base(element)
        {
            this.element = element;
            this.radius = radius;
            this.brush = brush;
            this.input = input;

            element.OnListToggle += ToggleVisibility;
        }

        void ToggleVisibility(bool visible)
        {
            this.Visibility = visible ? Visibility.Hidden : Visibility.Visible;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = Brushes.Red;
            ellipse.Fill = Brushes.White;

            drawingContext.DrawEllipse(brush, null, input ? element.LeftPoint : element.RightPoint, radius, radius);
        }
    }
}
