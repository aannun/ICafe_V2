
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    class Branch : Core.Node
    {
        [Out]
        public float Timer;

        public override void Init()
        {
            base.Init();

            BindCollectionParameter(GetParameter("value"), GetParameter("active"));
        }

        public override string GetTypeName()
        {
            return "TestBranch";
        }

        public void Execute([Collection(2, new[] { "False", "True" })] int value, bool active = true)
        {
        }
    }
}
