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
        public Action<UIElement> OnDrag, OnDragEnd, MoveDrag;
        public bool SnapOnEndDrag = false;

        UIElement element, view;
        bool is_moving;
        TranslateTransform move_start;
        Point move_delta;

        public Dragger(UIElement element, UIElement parent) : base(element)
        {
            this.element = element;
            view = parent;

            element.MouseUp += Element_MouseUp;
            element.MouseDown += Element_MouseDown;
            parent.MouseMove += Element_MouseMove;
            parent.MouseLeave += Element_MouseLeave;
            parent.MouseUp += Element_MouseUp;

            move_start = element.RenderTransform as TranslateTransform;
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            Reset();
        }

        private void Element_MouseMove(object sender, MouseEventArgs e)
        {
            if (!is_moving) return;

            Point p = Mouse.GetPosition(view);
            element.RenderTransform = new TranslateTransform((move_start != null ? move_start.X : 0) + p.X - move_delta.X, (move_start != null ? move_start.Y : 0) + p.Y - move_delta.Y);
            e.Handled = true;

            MoveDrag?.Invoke(element);
        }

        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || is_moving) return;

            Activate();
            e.Handled = true;
        }

        private void Element_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released) return;

            Reset();
        }

        private void Activate()
        {
            is_moving = true;
            move_delta = Mouse.GetPosition(view);

            OnDrag?.Invoke(element);
        }

        private void Reset()
        {
            if (!is_moving) return;

            OnDragEnd?.Invoke(element);

            is_moving = false;
            if (SnapOnEndDrag)
                element.RenderTransform = move_start;
           move_start = element.RenderTransform as TranslateTransform;
        }
    }
}
