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
        private bool isRecording = false;
        private System.Windows.Forms.Timer recordTimer;
        private string traceFilePath = "";
        private DateTime recordStartTime;
        private string tracesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Traces");
        private GMap.NET.WindowsForms.GMapOverlay pointsOverlay = new GMap.NET.WindowsForms.GMapOverlay("points");



        // Prototype de la méthode point record qui sert à enregistrer les infos liées à un point
        private List<PointRecord> pointRecords = new List<PointRecord>();
        private class PointRecord
        {
            public string Timestamp { get; set; }
            public string North { get; set; }
            public string West { get; set; }
            public string Altitude { get; set; }
        }

        //Charge les Traces depuis le CSV
        private void LoadTraces()
        {
            DisplayTraces.Controls.Clear();

            if (!Directory.Exists(tracesFolder))
                return;

            string[] traceFiles = Directory.GetFiles(tracesFolder, "Trace_*.csv");
            int y = 10;

            foreach (string file in traceFiles)
            {
                // extrait la timestamp depuis le nom du fichier
                string fileName = Path.GetFileNameWithoutExtension(file);
                // exemple : "Trace_2025_10_30_103212"
                string timestamp = fileName.Replace("Trace_", "").Replace('_', ' ');

                Button btn = new Button();
                btn.Text = timestamp;
                btn.Tag = file;
                btn.Location = new Point(10, y);
                btn.Width = 120;
                btn.Height = 30;
                btn.Click += TraceButton_Click;

                DisplayTraces.Controls.Add(btn);
                y += 40;
            }
        }

        //Méthode qui prend en compte le click sur uns des boutons de Trace
        private void TraceButton_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string filePath && File.Exists(filePath))
            {
                try
                {
                    var lines = File.ReadAllLines(filePath).Skip(1); // ignore l’en-tête
                    var pts = new List<points>();

                    foreach (var line in lines)
                    {
                        var parts = line.Split(';');
                        if (parts.Length < 3) continue;

                        double lat = double.Parse(parts[0].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                        double lon = double.Parse(parts[1].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                        double alt = double.Parse(parts[2].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);

                        pts.Add(new points(lon, lat, alt));
                    }

                    if (pts.Count < 2)
                    {
                        MessageBox.Show("Pas assez de points pour calculer une distance.", "Trace vide");
                        return;
                    }

                    double totalDistance = 0.0;
                    for (int i = 1; i < pts.Count; i++)
                    {
                        double dx = pts[i].GetX() - pts[i - 1].GetX();
                        double dy = pts[i].GetY() - pts[i - 1].GetY();
                        double dz = pts[i].GetZ() - pts[i - 1].GetZ();
                        totalDistance += Math.Sqrt(dx * dx + dy * dy + dz * dz);
                    }

                    MessageBox.Show($"Distance parcourue : {totalDistance:F2} mètres", btn.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la lecture de la trace : " + ex.Message);
                }
            }
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

        //Enregistrement si clic sur le bouton de record trace
        private void RecordTimer_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lastNorth) || string.IsNullOrEmpty(lastWest))
                return; // aucune donnée à enregistrer

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string line = $"{lastNorth};{lastWest};{lastAltitude}";

            // Crée le fichier s’il n’existe pas encore
            if (!File.Exists(traceFilePath))
            {
                File.AppendAllText(traceFilePath, "North;West;Altitude\n");
            }

            File.AppendAllText(traceFilePath, line + "\n");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Crée le dossier Traces s’il n’existe pas
            if (!Directory.Exists(tracesFolder))
            {
                Directory.CreateDirectory(tracesFolder);
            }

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

            //Refresh des boutons à partir du CSV
            LoadPointsFromCsv();

            //Refresh les traces à partir du CSV
            LoadTraces();

            //Initialisation du timer
            recordTimer = new System.Windows.Forms.Timer();
            recordTimer.Interval = 1000; // 1 seconde
            recordTimer.Tick += RecordTimer_Tick;

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

        private void labelTopo_Click(object sender, EventArgs e)
        {

        }

        private void labelTrace_Click(object sender, EventArgs e)
        {

        }

        private bool isActive = false;

        private void RecordTrace_Click(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                // Démarrer l’enregistrement
                recordStartTime = DateTime.Now;

                // Nom du fichier avec date et heure
                string fileName = "Trace_" + recordStartTime.ToString("yyyyMMdd_HHmmss") + ".csv";

                // Dossier "Traces"
                if (!Directory.Exists(tracesFolder))
                    Directory.CreateDirectory(tracesFolder);

                // Chemin complet
                traceFilePath = Path.Combine(tracesFolder, fileName);

                isRecording = true;
                recordTimer.Start();

                RecordTrace.BackColor = Color.LightCoral;
                RecordTrace.Text = "Recording";

                MessageBox.Show($"Enregistrement démarré.", "Trace");
            }
            else
            {
                // Arrêter l’enregistrement
                recordTimer.Stop();
                isRecording = false;

                RecordTrace.BackColor = Color.White;
                RecordTrace.Text = "Trace";

                MessageBox.Show($"Enregistrement terminé.", "Trace");
                LoadTraces();
            }
            
        }

        private void DisplayTraces_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSIG_Click(object sender, EventArgs e)
        {
            gMapControl1.Overlays.Clear();
            pointsOverlay.Markers.Clear();

            foreach (var record in pointRecords)
            {
                // Replace commas with dots (for parsing)
                string northFixed = record.North.Replace(',', '.');
                string westFixed = record.West.Replace(',', '.');

                // Parse both numbers
                if (double.TryParse(northFixed, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(westFixed, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double lon))
                {
                    // West = negative longitude
                    lon = -lon;

                    // ✅ Correct order: latitude first, longitude second
                    var marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                        new GMap.NET.PointLatLng(lat, lon),
                        GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_dot
                    );

                    marker.ToolTipText = $"{record.Timestamp}\nLat: {lat:F6}\nLon: {lon:F6}\nAlt: {record.Altitude} m";
                    pointsOverlay.Markers.Add(marker);
                }
            }

            // Add overlay
            gMapControl1.Overlays.Add(pointsOverlay);

            // Center on first valid point
            if (pointsOverlay.Markers.Count > 0)
            {
                gMapControl1.Position = pointsOverlay.Markers[0].Position;
                gMapControl1.Zoom = 14;
            }

            gMapControl1.Refresh();

            MessageBox.Show($"{pointsOverlay.Markers.Count} point(s) affiché(s) sur la carte !");
        }
    }
}
