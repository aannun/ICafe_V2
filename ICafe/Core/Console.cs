using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICafe.Core
{
    public static class Console
    {
        public static void Write(string message)
        {
            Trace.Write(message);
        }

        public static void WriteLine(string message)
        {
            Trace.WriteLine(message);
        }
    }
}
