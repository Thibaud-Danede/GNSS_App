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

namespace Geo
{
    public partial class Form1 : Form
    {
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
    }
}
