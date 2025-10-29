using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Geo
{
    internal class Point
    {
        public string Id { get; set; }
        public double Longitude { get; private set; } // en degrés
        public double Latitude { get; private set; }  // en degrés
        public double Altitude { get; private set; }  // en mètres
        public string Time { get; private set; }

        private double x;
        private double y;
        private double z;

        // Constantes WGS84
        private const double a = 6378137.0;             // demi-grand axe
        private const double e2 = 6.69437999014e-3;     // excentricité²

        public Point(double longitude, double latitude, double altitude, string time, string id = "")
        {
            this.Id = id;
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.Altitude = altitude;
            this.Time = time;
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
        public double GetX() => x;
        public double GetY() => y;
        public double GetZ() => z;

    }

    internal class Trace
    {
        private string filePath = @"Geo\bin\Debug\"; 
        public string Id { get; private set; }
        public List<Point> PointList { get; private set; } = new List<Point>();

        public Trace(string filename)
        {
            this.Id = filename;
            this.filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
        }

        public void AddPoint(Point currentPos)
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
