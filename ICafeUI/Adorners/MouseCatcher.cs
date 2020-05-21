using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICafeUI.Adorners
{
    class MouseCatcher : Adorner
    {
        Rect rect;
        Node node;
        float width;

        public MouseCatcher(Node element, float width) : base(element)
        {
            this.node = element;
            this.width = width;

            Loaded += MouseCatcher_Loaded;
            MouseEnter += MouseCatcher_MouseEnter;
        }

        public void SetActive(bool value)
        {
            IsHitTestVisible = value;
        }

        private void MouseCatcher_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed && Core.StateContainer.CurrentHandledNode != null)
            //    node.CatchMouse();
        }

        private void MouseCatcher_Loaded(object sender, RoutedEventArgs e)
        {
            rect = new Rect(-width, 0, width, ActualHeight);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(Brushes.Transparent, null, rect);
        }
    }
}
