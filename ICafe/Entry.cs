using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe
{
    class Entry
    {
        static void Main()
        {
            var A = new Core.NodeA();

            var B = new Core.NodeB();

            Core.Connector.ConnectionData data = new Core.Connector.ConnectionData();
            Core.Connector.CreateConnection(ref data, B, "value", A, "value");
            Core.Connector.CreateConnection(ref data, B, "string_value", A, "string_value");
            
            A.CallExecution();

            Console.ReadLine();
        }
    }
}
