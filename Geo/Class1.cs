using System;

namespace Geo
{
    internal class points
    {
        // (facultatif) identifiant si tu en as besoin ailleurs
        public string id;

        // Propriétés géodésiques WGS84
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        public string Time { get; set; }

        // Coordonnées cartésiennes
        private double x, y, z;

        // WGS84
        private const double a = 6378137.0;          // demi-grand axe
        private const double e2 = 6.69437999014e-3;   // excentricité^2

        // NOTE: 'time' param est facultatif pour matcher tes appels new points(lon, lat, alt)
        public points(double lg, double lt, double alt, string time = "")
        {
            Longitude = lg;
            Latitude = lt;
            Altitude = alt;
            Time = time;
            ConvertToCartesian();
        }

        private void ConvertToCartesian()
        {
            double latRad = Latitude * Math.PI / 180.0;
            double lonRad = Longitude * Math.PI / 180.0;

            double N = a / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(latRad), 2));

            x = (N + Altitude) * Math.Cos(latRad) * Math.Cos(lonRad);
            y = (N + Altitude) * Math.Cos(latRad) * Math.Sin(lonRad);
            z = (N * (1 - e2) + Altitude) * Math.Sin(latRad);
        }

        // Méthodes en PascalCase pour correspondre à tes usages GetX/GetY/GetZ
        public double GetX() => x;
        public double GetY() => y;
        public double GetZ() => z;
    }
}
