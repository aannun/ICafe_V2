using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public static class Wrapper
    {
        private const string DllFilePath = @"C:\Users\Ale\source\repos\ICafe_V2\Engine\Bin\Engine.dll";
        public static IntPtr Context = InitEngine();

        static Wrapper()
        {
            //Context = InitEngine();
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static IntPtr Init();

        public static IntPtr InitEngine()
        {
            return Init();
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static IntPtr CreateView(IntPtr context, uint width, uint height);

        public static IntPtr CreateWindow(uint width, uint height)
        {
            return CreateView(Context, width, height);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static IntPtr CreateEffectBuffer(IntPtr context, uint width, uint height, IntPtr shader);

        public static IntPtr CreateBuffer(uint width, uint height, IntPtr shader)
        {
            return CreateEffectBuffer(Context, width, height, shader);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void AddViewEffect(IntPtr view, IntPtr effect);

        public static void AddEffectToView(IntPtr view, IntPtr effect)
        {
            AddViewEffect(view, effect);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void DrawView(IntPtr view);

        public static void Draw_View(IntPtr view)
        {
            DrawView(view);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void DrawEffectBuffer(IntPtr buffer);

        public static void Draw_EffectBuffer(IntPtr buffer)
        {
            DrawEffectBuffer(buffer);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static IntPtr CreatePixelShader(IntPtr context, StringBuilder byte_code);

        public static IntPtr CreateShader(StringBuilder byte_code)
        {
            return CreatePixelShader(Context, byte_code);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void DestroyView(IntPtr view);

        public static void Detroy_View(IntPtr view)
        {
            DestroyView(view);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void DestroyBuffer(IntPtr buffer, IntPtr shader);

        public static void Detroy_Buffer(IntPtr buffer, IntPtr shader)
        {
            DestroyBuffer(buffer, shader);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void DieMessage(StringBuilder message);

        public static void ExitMessage(StringBuilder message)
        {
            DieMessage(message);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void ClearContext(IntPtr context);

        public static void ClearContextState()
        {
            ClearContext(Context);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void UpdateShaderValue(IntPtr shader, uint buffer_register, StringBuilder name, byte[] data);

        public static void UpdateShader(IntPtr shader, uint buffer_register, StringBuilder name, byte[] data)
        {
            UpdateShaderValue(shader, buffer_register, name, data);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static IntPtr CreateTexture(IntPtr context, uint width, uint height, uint pitch, byte[] data, uint format);

        public static IntPtr CreateShaderTexture(uint width, uint height, uint pitch, byte[] data, uint format)
        {
            return CreateTexture(Context, width, height, pitch, data, format);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void SetTexture(IntPtr shader, uint buffer_register, IntPtr texture);

        public static void SetShaderTexture(IntPtr shader, uint buffer_register, IntPtr texture)
        {
            SetTexture(shader, buffer_register, texture);
        }

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private extern static void DestroyTexture(IntPtr texture);

        public static void DisposeTexture(IntPtr texture)
        {
            DestroyTexture(texture);
        }
    }
}
