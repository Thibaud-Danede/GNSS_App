using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;

namespace Geo
{
    public partial class Form1 : Form
    {
        //---------Déclarations de variables du projet-------//
        private string lastNorth = ""; //Sauvegarde du dernier point nord capté
        private string lastWest = ""; //Sauvegarde du dernier point west capté
        private string lastAltitude = ""; //Sauvegarde du dernier point altitude capté
        private string timestampId = "";
        private string csvPoints = "points.csv"; //Nom du fichier qui contient les données de topography

        // Prototype de la méthode point record qui sert à enregistrer les infos liées à un point
        private List<PointRecord> pointRecords = new List<PointRecord>();
        private class PointRecord
        {
            public string Timestamp { get; set; }
            public string North { get; set; }
            public string West { get; set; }
            public string Altitude { get; set; }
        }


        //Méthode de chargement des différents points à partir du fichier CSV
        private void LoadPointsFromCsv()
        {
            pointRecords.Clear();
            DisplayPoints.Controls.Clear();

            if (!File.Exists(csvPoints))
                return;

            var lines = File.ReadAllLines(csvPoints).Skip(1); // saute l'entête
            int y = 10;

            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length < 4) continue;

                var record = new PointRecord
                {
                    Timestamp = parts[0],
                    North = parts[1],
                    West = parts[2],
                    Altitude = parts[3]
                };

                pointRecords.Add(record);

                // création d’un bouton par timestamp
                Button btn = new Button();
                btn.Text = record.Timestamp;
                btn.Tag = record; // pour y stocker les données associées
                btn.Location = new Point(10, y);
                btn.Width = 120;
                btn.Height = 30;
                btn.Click += TimestampButton_Click;

                DisplayPoints.Controls.Add(btn);

                y += 40;
            }
        }


        //Affiche les informations relatives 
        private void TimestampButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is PointRecord record)
            {
                MessageBox.Show($"Latitude Nord : {record.North}\nLongitude West : {record.West}\nAltitude : {record.Altitude} m",record.Timestamp);
            }
        }


        private Thread readFileThread;
        private string filePath = "trace.txt"; // Remplacez par le chemin de votre fichier
        private List<GGA> listeGGA;
        public Form1()
        {
            InitializeComponent();
            this.listeGGA = new List<GGA>();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Démarrer le thread pour lire le fichier lorsque le formulaire est chargé
            readFileThread = new Thread(ReadFile);
            readFileThread.Start();

            // Set the map provider (e.g., Google Maps)
            gMapControl1.MapProvider = GoogleMapProvider.Instance;

            // Set the map mode to fetch tiles from the server
            gMapControl1.Manager.Mode = AccessMode.ServerOnly;

            // Disable proxy
            GMapProvider.WebProxy = null;

            // Center the map on a specific location (e.g., Moscow)
            gMapControl1.Position = new PointLatLng(55.7558, 37.6176);

            // Set zoom levels
            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 10;

            LoadPointsFromCsv();
        }

        private void ReadFile()
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Utiliser Invoke pour mettre à jour l'interface utilisateur depuis un thread différent
                        this.Invoke(new Action(() =>
                        {
                            //Si la trame est une GGA, je créé un objet GGA
                            // que je remplis avec les infos de la trame
                            Console.WriteLine(line);
                            if (line.StartsWith("$GPGGA"))
                            {
                                GGA pointGGA = new GGA(line);
                                //Je range cet objet dans la liste des GGA
                                listeGGA.Add(pointGGA);

                                // Mise à jour des variables globales
                                lastNorth = pointGGA.North.ToString();
                                lastWest = pointGGA.West.ToString();
                                lastAltitude = pointGGA.Altitude.ToString();

                                //Je met à jour les champs lat, long et alt dans l'interface
                                Console.WriteLine("Latitude : " + pointGGA.North.ToString());
                                Console.WriteLine("Longitude : " + pointGGA.West.ToString());
                                Console.WriteLine("Altitude : " + pointGGA.Altitude.ToString());
                            }

                            //et je fais ce que je veux avec !
                        }));

                        // Ajouter un délai pour simuler la lecture ligne par ligne (facultatif)
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la lecture du fichier : " + ex.Message);
            }
        }

        private void Topography_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lastNorth))
            {
                MessageBox.Show("Aucune donnée GGA touvée.");
                return;
            }

            // prépare la ligne CSV
            timestampId = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string line = $"{timestampId};{lastNorth};{lastWest};{lastAltitude}";

            // crée le fichier s’il n’existe pas et ajoute l’en-tête
            if (!File.Exists(csvPoints))
            {
                File.AppendAllText(csvPoints, "timestampId;North;West;Altitude\n");
            }

            // ajoute la ligne
            File.AppendAllText(csvPoints, line + "\n");

            // Rafraîchit la zone d'affichage
            DisplayPoints.Invalidate();
            LoadPointsFromCsv();

            // affiche confirmation
            //MessageBox.Show($"Point enregistré !\n\n{line}", "Topography");
            MessageBox.Show($"Latitude Nord : {lastNorth}\nLongitude West : {lastWest}\nAltitude : {lastAltitude} m", "Topography");
        }

        private void DisplayPoints_Paint(object sender, PaintEventArgs e)
        {
            
        }
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void labelTopo_Click(object sender, EventArgs e)
        {

        }

        private void labelTrace_Click(object sender, EventArgs e)
        {

        }

        private bool isActive = false;

        private void RecordTrace_Click(object sender, EventArgs e)
        {
            if (!isActive)
            {
                // Turn green (active)
                RecordTrace.BackColor = Color.LightCoral;
                RecordTrace.Text = "Recording";
                isActive = true;
            }
            else
            {
                // Turn red (inactive)
                RecordTrace.BackColor = Color.White;
                RecordTrace.Text = "Trace";
                isActive = false;
            }
        }
    }
}
