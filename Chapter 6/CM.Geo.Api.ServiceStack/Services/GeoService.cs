using CM.Geo.BL;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CM.Geo.Api.ServiceStack.Services
{
    public class GeoService : Service
    {
        public GeoAddress Any(RequestDTO.Geo request)
        {
            return new GeoSolver()
                    .ResolveAddress(request.Address);
        }
    }
}