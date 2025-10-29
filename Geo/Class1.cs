using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geo
{
    internal class points
    {
        public string id;
        public double longitude;
        public double latitude;
        public double altitude;
        public string time;
        private double x;
        private double y;
        private double z;

        private const double a = 6378137.0; 
        private const double e2 = 6.69437999014e-3; 
        public points(double lg, double lt, double alt, string time)
        {
            this.longitude = lg;
            this.latitude = lt;
            this.altitude = alt;
            this.time = time;
            ConvertToCartesian();
        }

        private void ConvertToCartesian()
        {
            double latRad = Latitude * Math.PI / 180.0;
            double lonRad = Longitude * Math.PI / 180.0;

            double N = a / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(latRad), 2));

            this.x = (N + Altitude) * Math.Cos(latRad) * Math.Cos(lonRad);
            this.y = (N + Altitude) * Math.Cos(latRad) * Math.Sin(lonRad);
            this.z = (N * (1 - e2) + Altitude) * Math.Sin(latRad);
        }

        public double getX()
        {
            return this.x;
        }

        public double getY()
        {
            return this.y;
        }

        public double getZ()
        {
            return this.z;
        }
        public void 
    }
}
