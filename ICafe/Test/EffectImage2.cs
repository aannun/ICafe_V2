using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ICafe.Core
{
    public class EffectImage2 : EffectNode
    {
        [FileField]
        [Out]
        public string TexturePath;

        Texture t;

        public override void Start()
        {
            Path_To_Effect = @"C:\Users\Ale\source\repos\ICafe_V2\Engine\Shader_Test2.hlsl";

            base.Start();

            //effect.UpdateShaderValue(0, "test", 0.5f);

            t = new Texture(TexturePath);
            effect.SetShaderTexture(0, t);
        }

        public override void Stop()
        {
            base.Stop();

            t.Dispose();
        }
    }
}
