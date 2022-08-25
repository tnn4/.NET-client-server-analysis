using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal static class DebugTool
    {
        [Conditional("DEBUG")]
        public static void debugMethodExample()
        {
            Debug.WriteLine("DEBUG activated");
        }
    }
}
