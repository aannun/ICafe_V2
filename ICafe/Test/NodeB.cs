using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public class NodeB : Core.Node
    {
        [Out]
        public double value;
        [Out]
        public int a;
        [Out]
        [FileField]
        public string string_value = "val";
        [Out]
        public bool test_bool;
        [Out]
        public Test test;

        [Out]
        public NodeA nodeA;

        [Out]
        public Test_Enum test_Enum;

        public enum Test_Enum { a, b, c, d };

        public struct Test
        {
            [Out]
            public double value_struct;
            [Out]
            public bool value2_struct;

            [Out]
            public tt internal_test;
        }

        public struct tt
        {
            [Out]
            public int a;
        }

        public NodeB()
        {
            value = 3;
            string_value = "test";
        }

        public void Execute(double a, double b, double c, double d, double e, int test)
        {
            Console.Write("test");
        }
    }
}
