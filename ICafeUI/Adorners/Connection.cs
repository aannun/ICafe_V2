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
    class Connection : Adorner
    {
        ConnectionSlot element;

        public Connection(ConnectionSlot element) : base(element)
        {
            this.element = element;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);

            //var list = element.GetConnections();
            //if (list == null) return;

            //for (int i = 0; i < list.Count; i++)
            //{
            //    Point start = TranslatePoint(new Point(0, 0), element.node);
            //    Point s_offset = !element.IsInput ? list[i].RightPoint : list[i].LeftPoint;

            //    Point end = TranslatePoint(new Point(0,0), list[i]);
            //    Point e_offset = element.IsInput ? list[i].RightPoint : list[i].LeftPoint;
            //    drawingContext.DrawLine(new Pen(Brushes.Black, 4), new Point(-start.X + s_offset.X, -start.Y + s_offset.Y), new Point(-end.X + e_offset.X, -end.Y + e_offset.Y));
            //}
        }

        public void Refresh()
        {
            InvalidateVisual();
        }
    }
}
