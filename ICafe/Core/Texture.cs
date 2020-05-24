using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public class Texture
    {
        public int Width { get { return Bitmap.Width; } }
        public int Height { get { return Bitmap.Height; } }
        public int FormatSize { get; private set; }
        public PixelFormat Format { get { return Bitmap.PixelFormat; } }

        public Bitmap Bitmap { get; private set; }
        public IntPtr c_ptr { get; private set; }
        public bool isValid { get; private set; }

        string path;

        public Texture(string path)
        {
            if (!File.Exists(path)) return;

            isValid = true;
            this.path = path;
            Bitmap = new Bitmap(path);
            FormatSize = (int)(Bitmap.GetPixelFormatSize(Bitmap.PixelFormat) / 8f);

            var data = BitmapToByteArray(Bitmap);
            data = AlignTo32Bits(data, FormatSize);

            c_ptr = Wrapper.CreateShaderTexture((uint)Width, (uint)Height, (uint)Width * 4, data, 87);
        }

        public void Dispose()
        {
            if (isValid)
            {
                Bitmap.Dispose();
                Wrapper.DisposeTexture(c_ptr);
            }
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }

        public static byte[] AlignTo32Bits(byte[] data, int stride)
        {
            if (stride < 0 || stride >= 4) return data;

            int length = data.Length / stride;

            byte[] out_data = new byte[length * 4];
            int index = 0;
            int stride_remaining = 4 - stride;

            for (int i = 0; i < out_data.Length; i += 4)
            {
                Array.Copy(data, index, out_data, i, stride);
                index += stride;

                for (int j = 0; j < stride_remaining; j++)
                    out_data[i + stride + j] = 255;
            }

            return out_data;
        }
    }
}
