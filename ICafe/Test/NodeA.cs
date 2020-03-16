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
        public object test;

        public void Execute(ref int value, string string_value, object test)
        {
            Console.WriteLine(string_value + " " + value);
            value++;
        }
    }
}
