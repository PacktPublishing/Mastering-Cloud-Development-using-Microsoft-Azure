using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Geo.BL
{
    public static class GeoLogger
    {
        public static void Log(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
