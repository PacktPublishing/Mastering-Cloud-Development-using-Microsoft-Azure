using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Geo.BL
{
    public static class UserManagement
    {
        public static GeoUser FromIdentity(string identity)
        {
            return new GeoUser() { Name = identity };
        }
    }
}
