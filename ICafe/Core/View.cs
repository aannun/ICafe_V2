using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public class View
    {
        public string Name { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        IntPtr c_ptr;

        protected View(string name, uint width, uint height)
        {
            this.Name = name;
            Width = width;
            Height = height;

            c_ptr = Wrapper.CreateWindow(Width, Height);
        }

        public static View CreateView(string Name, uint Width, uint Height)
        {
            return new View(Name, Width, Height);
        }

        public void Draw()
        {
            Wrapper.Draw_View(c_ptr);
        }

        public void AddEffect(Effect effect)
        {
            Wrapper.AddEffectToView(c_ptr, effect.c_ptr);
        }

        public void Release()
        {
            Wrapper.Detroy_View(c_ptr);
            c_ptr = IntPtr.Zero;
        }
    }
}
