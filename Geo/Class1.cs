using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    internal class Trace
    {
        private string filePath = @"Geo\bin\Debug\";
        public string Id { get; private set; }
        public List<points> PointList { get; private set; } = new List<points>();

        public Trace(string filename)
        {
            this.Id = filename;
            this.filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
        }

        public void AddPoint(points currentPos)
        {
            PointList.Add(currentPos);
        }

        public void Save()
        {
            if (PointList.Count == 0)
            {
                Console.WriteLine("Aucun point à enregistrer.");
                return;
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // En-tête CSV
                    sw.WriteLine("Longitude,Latitude,Altitude,Time");

                    // Données
                    foreach (var p in PointList)
                    {
                        sw.WriteLine(
                        $"{p.Longitude.ToString("F7", System.Globalization.CultureInfo.InvariantCulture)}," +
                        $"{p.Latitude.ToString("F7", System.Globalization.CultureInfo.InvariantCulture)}," +
                        $"{p.Altitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}," +
                        $"{p.Time}");
                    }
                }

                Console.WriteLine($"Trace enregistrée avec succès dans : {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'enregistrement : {ex.Message}");
            }
        }

    }
}
