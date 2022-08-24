using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal static class Null
    {
        public static void isNull(object obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
        }
    }
}
