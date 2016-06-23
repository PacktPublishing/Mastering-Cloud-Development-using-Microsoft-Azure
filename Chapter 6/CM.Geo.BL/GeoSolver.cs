using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CM.Geo.BL
{
    public class GeoSolver
    {
        
        public GeoAddress ResolveAddress(string address)
        {
            var uri = string.Format("[serviceUri]", "[code]",
                Uri.EscapeUriString(address));
            var cl = new WebClient();
            var res = JsonConvert.DeserializeObject<Rootobject>(cl.DownloadString(uri));
            if (res.results.Any())
            {
                return new GeoAddress()
                {
                    Address = res.results.First().formatted,
                    Latitude = res.results.First().geometry.lat,
                    Longitude = res.results.First().geometry.lng
                };
            }            
            return null;
        }

    }


    public class GeoAddress
    {
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
