using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public class NodeB : Core.Node
    {
        [Reactive]
        public double value;
        [Reactive]
        public int a;
        [Reactive]
        public string string_value = "val";
        [Reactive]
        public bool test_bool;
        [Reactive]
        public Test test;

        [Reactive]
        public NodeA nodeA;

        [Reactive]
        public Test_Enum test_Enum;

        public enum Test_Enum { a, b, c, d };

        public struct Test
        {
            [Reactive]
            public double value_struct;
            [Reactive]
            public bool value2_struct;

            [Reactive]
            public tt internal_test;
        }

        public struct tt
        {
            [Reactive]
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
