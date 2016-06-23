using CM.Geo.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CM.Geo.Api.Controllers
{
    public class GeoController : ApiController
    {
        public GeoAddress Get(string address)
        {

            return new GeoSolver().ResolveAddress(address);
        }
    }
}
