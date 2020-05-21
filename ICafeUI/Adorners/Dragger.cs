using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace ICafeUI
{
    class Dragger : Adorner
    {
        public Action<UIElement> OnDrag, OnDragEnd;
        public Action<Point> MoveDrag;

        UIElement element;
        bool is_moving;
        TranslateTransform move_start;
        Point move_delta;

        public Dragger(UIElement element) : base(element)
        {
            this.element = element;

            element.MouseUp += Element_MouseUp;
            element.MouseDown += Element_MouseDown;
            element.MouseMove += Element_MouseMove;

            move_start = element.RenderTransform as TranslateTransform;
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (!is_moving) return;

            Point p = Mouse.GetPosition(Core.StateContainer.View);
            Core.Selector.MoveAllSelected(new Point(p.X - move_delta.X, p.Y - move_delta.Y));
            move_delta = p;


            e.Handled = true;
            //MoveDrag?.Invoke(new Point(p.X - move_delta.X, p.Y - move_delta.Y));
        }

        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || is_moving) return;

            Activate();
            element.CaptureMouse();
            e.Handled = true;
        }

        private void Element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released) return;

            Reset();
            element.ReleaseMouseCapture();
        }

        private void Activate()
        {
            is_moving = true;
            move_delta = Mouse.GetPosition(Core.StateContainer.View);

            OnDrag?.Invoke(element);
        }

        private void Reset()
        {
            if (!is_moving) return;

            OnDragEnd?.Invoke(element);

            is_moving = false;
            move_start = element.RenderTransform as TranslateTransform;
        }
    }
}
