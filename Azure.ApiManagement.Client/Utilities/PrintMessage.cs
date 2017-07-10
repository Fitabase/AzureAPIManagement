using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    /// <summary>
    /// Helper class that is used to display debug code
    /// </summary>
    internal static class PrintMessage
    {

        private static int Counter = 0;
        private static readonly string DEBUG = "__[DEBUG]__";

        public static void Debug(string className, object obj)
        {
            System.Diagnostics.Debug.WriteLine(
                String.Format("[\n\tInfo: {0}\n\tCounter: {1}\n\tClass: {2}\n\tResult: {3}\n]", DEBUG, Counter++, className, obj));
        }
    }
}
