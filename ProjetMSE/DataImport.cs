using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Primeur
{
    public class DataImport
    {
        //emplacement du fichier
        public string filePath { get; set; }

        public Dictionary<string, double> importProduitsData()
        {
            // Dictionnaire tampon pour stocker les produits et leurs prix
            Dictionary<string, double> bufferDictionary = new Dictionary<string, double>();

            // Vérification du chemin et de l'extension du fichier
            Console.WriteLine($"Chemin du fichier : {this.filePath}");
            if (string.IsNullOrEmpty(this.filePath) || !File.Exists(this.filePath))
            {
                MessageBox.Show("Le fichier spécifié n'existe pas ou le chemin est invalide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return bufferDictionary; // Retourne un dictionnaire vide
            }

            // Vérifier que le fichier a l'extension .csv
            if (Path.GetExtension(this.filePath).ToLower() != ".csv")
            {
                MessageBox.Show("Seuls les fichiers CSV sont acceptés.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return bufferDictionary;
            }

            try
            {
                using (StreamReader sr = new StreamReader(this.filePath))
                {
                    string line;
                    int lineNumber = 0;

                    // Lire chaque ligne du fichier
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineNumber++; // Compte les lignes pour afficher dans les erreurs

                        // Vérifier que la ligne n'est pas vide
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            MessageBox.Show($"Le fichier est vide ou contient une ligne vide à la ligne {lineNumber}.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }

                        // Découpage de chaque ligne sur le caractère ";"
                        string[] productAndPrice = line.Split(';');

                        // Vérifier que la ligne contient exactement 2 colonnes
                        if (productAndPrice.Length != 2)
                        {
                            MessageBox.Show($"La ligne {lineNumber} ne contient pas exactement deux colonnes.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }

                        // Lire la première colonne (nom du produit)
                        string product = productAndPrice[0].Trim();

                        // Vérifier que la deuxième colonne est un double (prix)
                        if (!double.TryParse(productAndPrice[1].Trim(), out double price))
                        {
                            MessageBox.Show($"La valeur '{productAndPrice[1]}' à la ligne {lineNumber} n'est pas un nombre valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }

                        // Ajouter le produit et le prix au dictionnaire
                        bufferDictionary.Add(product, price);
                    }

                    // Vérifier si le fichier est vide après lecture
                    if (bufferDictionary.Count == 0)
                    {
                        MessageBox.Show("Le fichier CSV est vide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return bufferDictionary;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Le fichier ne peut pas être lu:");
                Console.WriteLine(e.Message);
                MessageBox.Show("Une erreur est survenue lors de la lecture du fichier.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Retourner le dictionnaire rempli
            return bufferDictionary;
        }
    }
}
