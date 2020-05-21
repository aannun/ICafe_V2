using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public class Effect
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public IntPtr c_ptr { get; private set; }

        IntPtr shader_ptr;

        BinaryFormatter bf;

        protected Effect(uint width, uint height, string path)
        {
            Width = width;
            Height = height;

            shader_ptr = CompileEffect(path);
            c_ptr = Wrapper.CreateBuffer(Width, Height, shader_ptr);

            bf = new BinaryFormatter();
        }

        public static Effect CreateEffect(uint Width, uint Height, string path)
        {
            return new Effect(Width, Height, path);
        }

        public void Draw()
        {
            Wrapper.Draw_EffectBuffer(c_ptr);
        }

        IntPtr CompileEffect(string path)
        {
            string error, ps_path;
            if (EffectCompiler.TryCompile(path, ICafe.Core.EffectCompiler.ProfileByKey("ps_4_0_level_9_3"), "main", out error, out ps_path))
            {
                //byte[] data = File.ReadAllBytes(ps_path);
                return Wrapper.CreateShader(new StringBuilder(ps_path));
            }
            else
                Wrapper.ExitMessage(new StringBuilder(error));

            return IntPtr.Zero;
        }

        public void Release()
        {
            Wrapper.Detroy_Buffer(c_ptr, shader_ptr);
            c_ptr = IntPtr.Zero;
            shader_ptr = IntPtr.Zero;
        }

        public void UpdateShaderValue(uint register, string name, object value)
        {
            Wrapper.UpdateShader(shader_ptr, register, new StringBuilder(name), ObjectToByteArray(value));
        }

        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
