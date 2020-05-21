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
    /// Per utilizzare questo controllo personalizzato in un file XAML, eseguire i passaggi 1a o 1b e 2.
    ///
    /// Passaggio 1a) Utilizzo di questo controllo personalizzato in un file XAML esistente nel progetto corrente.
    /// Aggiungere questo attributo XmlNamespace all'elemento radice del file di markup dove 
    /// deve essere utilizzato:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ICafeUI"
    ///
    ///
    /// Passaggio 1b) Utilizzo di questo controllo personalizzato in un file XAML esistente in un progetto diverso.
    /// Aggiungere questo attributo XmlNamespace all'elemento radice del file di markup dove 
    /// deve essere utilizzato:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ICafeUI;assembly=ICafeUI"
    ///
    /// Sarà inoltre necessario aggiungere nel progetto in cui si trova il file XAML
    /// un riferimento a questo progetto, quindi ricompilare per evitare errori di compilazione:
    ///
    ///     In Esplora soluzioni, fare clic con il pulsante destro del mouse sul progetto di destinazione, quindi scegliere
    ///     "Aggiungi riferimento"->"Progetti"->[Individuare e selezionare questo progetto]
    ///
    ///
    /// Passaggio 2)
    /// Utilizzare il controllo nel file XAML.
    ///
    ///     <MyNamespace:ZoomBorder/>
    ///
    /// </summary>
    public class ZoomBorder : Border
    {
        static ZoomBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomBorder), new FrameworkPropertyMetadata(typeof(ZoomBorder)));
        }
        private UIElement child = null;
        private Point origin;
        private Point start;
        bool mouse_down;

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != this.Child)
                    this.Initialize(value);
                base.Child = value;
            }
        }

        public void Initialize(UIElement element)
        {
            this.child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);
                this.MouseWheel += child_MouseWheel;
                this.MouseRightButtonDown += child_MouseLeftButtonDown;
                this.MouseRightButtonUp += child_MouseLeftButtonUp;
                this.MouseMove += child_MouseMove;
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                var st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                var tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        #region Child Events

        private void child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                var st = GetScaleTransform(child);
                var tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * st.ScaleX + tt.X;
                absoluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
            }
        }

        private void child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                mouse_down = true;
                var tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                child.CaptureMouse();
            }
        }

        private void child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                mouse_down = false;
                child.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        void child_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null && mouse_down)
            {
                if (child.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                }
            }
        }

        #endregion
    }
}
