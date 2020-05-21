using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public class NodeA : Core.Node
    {
        [Reactive]
        public int b = 1;

        [Reactive]
        public bool test = true;

        public void Start(bool test)
        {
            Console.Write("start");
        }

        public void Execute([Collection(2)] int value, string string_value)
        {
            //test = !test;

            Console.Write("aaa");
        }
    }
}
