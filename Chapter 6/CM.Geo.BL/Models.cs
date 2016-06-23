using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Geo.BL
{


    public class Rootobject
    {
        public string documentation { get; set; }
        public License[] licenses { get; set; }
        public Rate rate { get; set; }
        public Result[] results { get; set; }
        public Status status { get; set; }
        public Stay_Informed stay_informed { get; set; }
        public string thanks { get; set; }
        public Timestamp timestamp { get; set; }
        public int total_results { get; set; }
    }

    public class Rate
    {
        public int limit { get; set; }
        public int remaining { get; set; }
        public int reset { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    public class Stay_Informed
    {
        public string blog { get; set; }
        public string twitter { get; set; }
    }

    public class Timestamp
    {
        public string created_http { get; set; }
        public int created_unix { get; set; }
    }

    public class License
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Result
    {
        public Annotations annotations { get; set; }
        public Bounds bounds { get; set; }
        public Components components { get; set; }
        public int confidence { get; set; }
        public string formatted { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Annotations
    {
        public DMS DMS { get; set; }
        public string MGRS { get; set; }
        public string Maidenhead { get; set; }
        public Mercator Mercator { get; set; }
        public OSM OSM { get; set; }
        public int callingcode { get; set; }
        public string geohash { get; set; }
        public Sun sun { get; set; }
        public Timezone timezone { get; set; }
        public What3words what3words { get; set; }
    }

    public class DMS
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class Mercator
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public class OSM
    {
        public string edit_url { get; set; }
        public string url { get; set; }
    }

    public class Sun
    {
        public Rise rise { get; set; }
        public Set set { get; set; }
    }

    public class Rise
    {
        public int apparent { get; set; }
        public int astronomical { get; set; }
        public int civil { get; set; }
        public int nautical { get; set; }
    }

    public class Set
    {
        public int apparent { get; set; }
        public int astronomical { get; set; }
        public int civil { get; set; }
        public int nautical { get; set; }
    }

    public class Timezone
    {
        public string name { get; set; }
        public int now_in_dst { get; set; }
        public int offset_sec { get; set; }
        public int offset_string { get; set; }
        public string short_name { get; set; }
    }

    public class What3words
    {
        public string words { get; set; }
    }

    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Northeast
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Southwest
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Components
    {
        public string country { get; set; }
        public string country_code { get; set; }
        public string county { get; set; }
        public string postcode { get; set; }
        public string road { get; set; }
        public string state { get; set; }
        public string town { get; set; }
        public string neighbourhood { get; set; }
        public string suburb { get; set; }
        public string local_administrative_area { get; set; }
    }

    public class Geometry
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

  

}
