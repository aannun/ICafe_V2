using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ICafe.Core
{
    class EffectBackground : Node
    {
        [Out]
        public RenderTargetBitmap I_Image;
        DrawingVisual img;

        public override void Start()
        {
            string path1 = "C:\\Users\\Ale\\Desktop\\test.png";
            var uri = new Uri(path1);

            img = new DrawingVisual();
            DrawingContext drawingContext = img.RenderOpen();
            drawingContext.DrawImage(new BitmapImage(uri), new Rect(0, 0, 800, 800));
            drawingContext.Close();

            I_Image = new RenderTargetBitmap(800, 800, 96, 96, PixelFormats.Pbgra32);
            I_Image.Render(img);
        }

        public void Execute()
        {
        }
    }
}
