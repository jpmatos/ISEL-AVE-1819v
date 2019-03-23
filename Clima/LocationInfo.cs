using System;
using System.ComponentModel;

namespace Clima
{
    public class LocationInfo
    {
        [Description("Country")]
        public String Country { get; set; }
        
        [Description("Region")]
        public String Region { get; set; }
        
        [Description("Latitude")]
        public double Latitude { get; set; }
        
        [Description("Longitude")]
        public double Longitude { get; set; }

        [Description("Ctor1")]
        public LocationInfo()
        {
        }

        [Description("Ctor2")]
        public LocationInfo(string country, string region, double latitude, double longitude)
        {
            Country = country;
            Region = region;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
