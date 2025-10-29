using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public double ComputeSurface()
        {
            if (PointList.Count < 3)
            {
                Console.WriteLine(" Impossible de calculer la surface : il faut au moins 3 points.");
                return 0;
            }

            // Étape 1️⃣ : Trouver la plus grande distance (en mètres)
            double maxDist = 0;
            points p1 = null, p2 = null;

            for (int i = 0; i < PointList.Count; i++)
            {
                for (int j = i + 1; j < PointList.Count; j++)
                {
                    double d = Distance3D(PointList[i], PointList[j]);
                    if (d > maxDist)
                    {
                        maxDist = d;
                        p1 = PointList[i];
                        p2 = PointList[j];
                    }
                }
            }

            if (p1 == null || p2 == null)
                return 0;

            Console.WriteLine($" Plus grande distance : {maxDist:F2} m entre deux points.");

            // Étape 2️⃣ : Créer un plan local basé sur cette diagonale
            // Vecteur directeur de la diagonale
            double dx = p2.GetX() - p1.GetX();
            double dy = p2.GetY() - p1.GetY();
            double norm = Math.Sqrt(dx * dx + dy * dy);

            double ux = dx / norm;
            double uy = dy / norm;

            // Étape 3️⃣ : Projeter tous les points sur ce plan local
            List<(double s, double h)> local = new List<(double s, double h)>();

            foreach (var p in PointList)
            {
                // Coordonnée projetée sur la diagonale (axe s)
                double sx = (p.GetX() - p1.GetX()) * ux + (p.GetY() - p1.GetY()) * uy;

                // Coordonnée perpendiculaire (axe h)
                double hx = (p.GetX() - p1.GetX()) * (-uy) + (p.GetY() - p1.GetY()) * ux;

                local.Add((sx, hx));
            }

            // Étape 4️⃣ : Trier les points par s croissant
            local = local.OrderBy(p => p.s).ToList();

            // Étape 5️⃣ : Intégration par morceaux (méthode du trapèze)
            double area = 0;
            for (int i = 0; i < local.Count - 1; i++)
            {
                double ds = local[i + 1].s - local[i].s;
                double h1 = local[i].h;
                double h2 = local[i + 1].h;
                area += (h1 + h2) / 2 * ds;
            }

            // Surface absolue (m²)
            double surface = Math.Abs(area);
            Console.WriteLine($"Surface estimée : {surface:F2} m²");

            return surface;
        }

        // === Fonction utilitaire pour la distance 3D entre deux points ===
        private double Distance3D(points a, points b)
        {
            double dx = a.GetX() - b.GetX();
            double dy = a.GetY() - b.GetY();
            double dz = a.GetZ() - b.GetZ();
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
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
