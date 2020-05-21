﻿using System;
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
    public class EffectImage : EffectNode
    {
        public override void Start()
        {
            Path_To_Effect = @"C:\Users\Ale\source\repos\ICafe_V2\Engine\Shader_Test.hlsl";

            base.Start();

            effect.UpdateShaderValue(1, "test1", 1.0);
            effect.UpdateShaderValue(0, "test_ext", 1.0);
        }
    }
}
