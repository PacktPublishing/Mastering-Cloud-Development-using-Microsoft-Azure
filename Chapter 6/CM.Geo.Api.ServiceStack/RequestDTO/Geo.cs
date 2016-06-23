using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CM.Geo.Api.ServiceStack.RequestDTO
{
    [Route("/api/geo/{Address}")]
    public class Geo
    {
        public string Address { get; set; }
    }
}