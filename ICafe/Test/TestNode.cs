using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    class TestNode : Node
    {
        [Out]
        public float Timer1;

        [Out]
        public float Timer2;

        int Texture_index;

        public override void Init()
        {
            base.Init();
        }

        public override void Start()
        {
            base.Start();

            BindCollectionParameter(GetParameter("Textures"), GetParameter("Texture_index"));
        }

        public override void Stop()
        {
            base.Stop();
        }

        public void Execute([Collection(2)] string Textures, int Texture_index)
        {
            //TIMER

        }
    }
}
