using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace WP8Runner
{
    class CoordinateConverter
    {
        public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geo)
        {
            return new GeoCoordinate
                (
                geo.Latitude,
                geo.Longitude,
                geo.Altitude ?? Double.NaN,
                geo.Accuracy,
                geo.AltitudeAccuracy ?? Double.NaN,
                geo.Speed ?? Double.NaN,
                geo.Heading ?? Double.NaN
                );
        }
    }
}
