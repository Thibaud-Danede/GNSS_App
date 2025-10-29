using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Primeur
{
    public class DataExport
    {
        /// <summary>
        /// Chemin vers le fichier saveTicket
        /// </summary>
        private string filePath;

        //Méthode pour prévisualiser le ticket dans une pop-up
        public string SeeTicket(Dictionary<string, Tuple<double, double>> panier)
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            string heure = DateTime.Now.ToString("HH:mm:ss");

            // Calculer le total et la TVA
            double totalPrix = 0;
            foreach (var item in panier.Values)
            {
                totalPrix += item.Item2; // `item.Item2` est le prix total pour chaque produit
            }
            double tva = Math.Round(totalPrix * 0.2, 2); // Supposons une TVA de 5,5%
            double totalTTC = Math.Round(totalPrix + tva, 2);

            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            {
                // En-tête avec la date et l'heure
                writer.WriteLine("--------------------------------");
                writer.WriteLine("Primeur de la côte \nAvenue de beaurivage \n64200 Biarritz \nle " + date + "\n" + "à " + heure);
                writer.WriteLine("====================");

                // Écrire les éléments du panier
                foreach (KeyValuePair<string, Tuple<double, double>> item in panier)
                {
                    writer.WriteLine("{0} - {1} kg : {2} €", item.Key, item.Value.Item1, item.Value.Item2);
                }

                // Ajouter le total, la TVA et le total TTC
                writer.WriteLine("====================");
                writer.WriteLine("TOTAL TTC : {0} €", totalTTC);
                writer.WriteLine("TVA (5,5%) : {0} €", tva);
                writer.WriteLine("\nMerci de votre visite et...\n... Gardez la pêche!");
                writer.WriteLine("--------------------------------");

                // Réinitialise la position pour permettre la lecture
                writer.Flush();
                memoryStream.Position = 0;

                // Convertir le flux en mémoire en une chaîne pour prévisualisation
                using (var reader = new StreamReader(memoryStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        //Méthode pour enregistrer le ticket
        public void SaveTicket(Dictionary<string, Tuple<double, double>> panier, string filePath)
        {
            try
            {
                string date = DateTime.Now.ToString("dd/MM/yyyy");
                string heure = DateTime.Now.ToString("HH:mm:ss");

                // Calculer le total et la TVA
                double totalPrix = 0;
                foreach (var item in panier.Values)
                {
                    totalPrix += item.Item2; // `item.Item2` est le prix total pour chaque produit
                }
                double tva = Math.Round(totalPrix * 0.2, 2); // Supposons une TVA de 20%
                double totalTTC = Math.Round(totalPrix + tva, 2);

                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    // En-tête avec la date et l'heure
                    sw.WriteLine("--------------------------------");
                    sw.WriteLine("Primeur de la côte \nAvenue de beaurivage \n64200 Biarritz \nle " + date + "\n" + "à " + heure);
                    sw.WriteLine("====================");

                    // Écrire les éléments du panier
                    foreach (KeyValuePair<string, Tuple<double, double>> item in panier)
                    {
                        sw.WriteLine("{0} - {1} kg : {2} €", item.Key, item.Value.Item1, item.Value.Item2);
                    }

                    // Ajouter le total, la TVA et le total TTC
                    sw.WriteLine("====================");
                    sw.WriteLine("TOTAL TTC : {0} €", totalTTC);
                    sw.WriteLine("TVA (5,5%) : {0} €", tva);
                    sw.WriteLine("\nMerci de votre visite et...\n... Gardez la pêche!");
                    sw.WriteLine("--------------------------------");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Erreur de log");
            }

            // Ouvrir le fichier texte après l'écriture
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'ouverture du fichier : " + e.Message);
            }
        }

    }
}










