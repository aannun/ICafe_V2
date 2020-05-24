using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ICafe.Core
{
    class OutputNode : Node
    {
        [@Out]
        public int Width = 800;
        [@Out]
        public int Height = 800;

        View view;

        public override void Start()
        {
            view = View.CreateView(NodeName, 800, 800);
        }

        public void Execute(EffectChain effectChain)
        {
            if (effectChain.Valid)
                for (int i = 0; i < effectChain.Count; i++)
                    view.AddEffect(effectChain.Get(i));

            view.Draw();
        }

        public override void Stop()
        {
            view.Release();
        }
    }
}
