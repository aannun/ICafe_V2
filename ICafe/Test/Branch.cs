
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    class Branch : Core.Node
    {
        [Reactive]
        public float Timer;

        public override void Init()
        {
            base.Init();

            BindCollectionParameter(GetParameter("value"), GetParameter("active"));
        }

        //public void PreExecute(bool active)
        //{
        //    GetParameter("value").CurrentIndex = active ? 0 : 1;
        //}

        public void Execute([Collection(2, new[] { "False", "True" })] int value, bool active = true)
        {
        }
    }
}
