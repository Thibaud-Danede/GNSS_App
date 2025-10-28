using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Primeur
{
    public partial class Form1 : Form
    {
        public Dictionary<string, double> dicoProduits = new Dictionary<string, double>(); // Dictionnaire des produits
        public Dictionary<string, Tuple<double, double>> panier = new Dictionary<string, Tuple<double, double>>(); // Dictionnaire pour afficher les produits du panier
        
        public List<TextBox> dynamicTextBox = new List<TextBox>(); // Champs de texte dynamiques
        public List<Label> dynamicLabelPanier = new List<Label>(); // Labels pour le panier
        private List<Label> dynamicLabel = new List<Label>(); // Labels classiques
        private List<Label> dynamicLabelTot = new List<Label>(); // Labels pour les totaux
        public int number_of_the_search_page = 0;//gestion des pages dans le menus de recherche
        public int number_of_the_page = 0;//gestion des pages dans le menus principale

        // Instances des classes 
        private DataImport importer;
        private DataExport exporter;
        private OpenFileDialog openFileDialog1;
        private Bouton bouton;

        //Constructeur de la classe
        public Form1()
        {
            InitializeComponent();

            // Initialisation des attributs de classes
            importer = new DataImport();
            exporter = new DataExport();
            openFileDialog1 = new OpenFileDialog();
            bouton = new Bouton(this);
            bouton.createDynamicButton(new Point(200, 200), "Charger Produits", new EventHandler(loadFileButton_Click), Color.FromArgb(80, 200, 120), new Size(200, 50), new Font("Arial", 15F, FontStyle.Bold, GraphicsUnit.Point), Color.FromArgb(20, 20, 20), Color.Black, 1);
            bouton.createDynamicButton(new Point(340, 40), "Nouveau Pannier", new EventHandler(newPanier_Click), Color.FromArgb(80, 200, 120), new Size(100, 22), new Font("Arial", 8F, FontStyle.Regular, GraphicsUnit.Point), Color.FromArgb(20, 20, 20), Color.Black, 1);
        }


        // Méthode pour créer un champ de texte dynamique
        public void createTextBox(Point location, Size taille)
        {
            System.Windows.Forms.TextBox newtextBox = new System.Windows.Forms.TextBox
            {
                BackColor = Color.WhiteSmoke,
                Location = location,
                Name = "newtextBox",
                Size = taille,
                TabIndex = 0
            };
            this.Controls.Add(newtextBox);
            newtextBox.BringToFront();
            dynamicTextBox.Add(newtextBox);
        }

        // Méthode pour créer un label dynamique
        public void createLabel(Point location, string yourText, Font style)
        {
            Label newlabel = new Label
            {
                AutoSize = true,
                Location = location,
                Name = "newlabel",
                Size = new Size(110, 20),
                TabIndex = 6,
                ForeColor = Color.Black,
                BackColor = Color.FromArgb(210, 255, 210),
                Text = yourText,
                Font = style,
            };
            this.Controls.Add(newlabel);
            newlabel.BringToFront();
            dynamicLabel.Add(newlabel);
        }


        // Méthode pour charger un fichier CSV et mettre à jour le dictionnaire de produits
        public void loadFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //Filtres pour ne charger que les fichiers .csv
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Sélectionner un fichier CSV";

                //Test de la réussite de l'ouverture du fichier
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Paramétrage du chemin du fichier sur le fichier sélectionné
                    importer.filePath = openFileDialog.FileName;
                    Console.WriteLine($"Chemin du fichier sélectionné : {importer.filePath}");

                    // Chargement des données du fichier dans dicoProduits
                    dicoProduits = importer.importProduitsData();

                    // Vérification du contenu de dicoProduits
                    if (dicoProduits != null && dicoProduits.Count > 0)
                    {
                        Console.WriteLine("Produits importés :");
                        foreach (var item in dicoProduits)
                        {
                            Console.WriteLine($"{item.Key}: {item.Value}€");
                        }

                        // Appel de la méthode pour créer les boutons
                        bouton.CreateButtonsFromDictionary(this.dicoProduits, number_of_the_page);
                        bouton.createDynamicButton(new Point(225, 40), "Charger Produits", new EventHandler(loadFileButton_Click), Color.FromArgb(80, 200, 120), new Size(100, 22), new Font("Arial", 8F, FontStyle.Regular, GraphicsUnit.Point), Color.FromArgb(20, 20, 20), Color.Black, 1);
                    }
                    else
                    {
                        Console.WriteLine("Le fichier est vide ou invalide.");
                    }
                }
                else
                {
                    MessageBox.Show("Aucun fichier n'a été sélectionné.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Méthode pour sauvegarder et ouvrir le ticket pour impression
        public void saveTicket_Click(object sender, EventArgs e)
        {
            //Ouverture de la boite de dialogue pour sauvegarder le fichier
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                //Filtres pour sauvegarder uniquement en txt et suggérer un nom de fichier
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                saveFileDialog.Title = "Enregistrer le ticket";
                saveFileDialog.FileName = "ticket_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";

                //Contrôle sur l'enregistrement du fichier
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    exporter.SaveTicket(panier, filePath);
                    Console.WriteLine("Ticket enregistré avec succès.", "Enregistrement");
                }
                else
                {
                    MessageBox.Show("Le fichier n'a pas été enregistré.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Méthode pour afficher les totaux
        public void calculer_totale()
        {
            double totalPrix = 0;

            // Calcul du total du panier
            foreach (var item in panier.Values)
            {
                totalPrix += item.Item2; // `item.Item2` correspond au prix total de chaque produit
            }

            // Calcul de la TVA et du Total TTC
            double tva = Math.Round(totalPrix * 0.055, 2); // Supposons une TVA de 5.5%
            double totalTTC = Math.Round(totalPrix + tva, 2);

            // Mise à jour des labels pour afficher les totaux
            clearDynamicLabelTot(); // Efface les anciens labels

            createLabelTot(new Point(20, 375 + 80), "Total produit : " + Math.Round(totalPrix, 2) + "€");
            createLabelTot(new Point(20, 400 + 80), "Total TVA : " + tva + "€");
            createLabelTotFinal(new Point(20, 425 + 80), "Total : " + totalTTC + "€");

            this.Refresh(); // Rafraîchir l'interface pour afficher les changements
        }

        //Méthode pour mettre à jour l'affichage du panier
        public void UpdatePanierDisplay()
        {
            // Efface les anciens labels du panier
            clearDynamicLabelPanier();//42 la répose à tout

            int yPosition = 25; // Position verticale initiale pour l'affichage des produits

            foreach (var item in panier)
            {
                string productName = item.Key;
                double poids = item.Value.Item1;
                double prixTotal = item.Value.Item2;
                // Crée un label pour afficher le produit avec son poids et prix total
                Label productLabel = new Label
                {
                    Text = $"{productName} - {poids} kg : {prixTotal} €",
                    AutoSize = false,
                    Size=new Size(150, 15),
                    Location = new Point(10, yPosition + 120), // Position à droite de l'interface
                    BackColor = Color.FromArgb(210, 255, 210)
                };

                this.Controls.Add(productLabel);
                dynamicLabelPanier.Add(productLabel);

                // Ajuster la position pour le prochain produit
                yPosition += 25;
            }

            this.Refresh(); // Rafraîchir l'interface pour afficher les changements
        }


        // Méthode pour effacer les champs de texte dynamiques
        public void clearDynamicTextBox()
        {
            foreach (var textBox in dynamicTextBox)
            {
                this.Controls.Remove(textBox);
                textBox.Dispose();
            }
            dynamicTextBox.Clear();
        }

        // Méthode pour effacer les labels dynamiques
        public void clearDynamicLabel()
        {
            foreach (var label in dynamicLabel)
            {
                this.Controls.Remove(label);
                label.Dispose();
            }
            dynamicLabel.Clear();
        }

        // Méthode pour effacer les labels de total dynamiques
        private void clearDynamicLabelTot()
        {
            foreach (var label in dynamicLabelTot)
            {
                this.Controls.Remove(label);
                label.Dispose();
            }
            dynamicLabelTot.Clear();
        }
        //Méthode pour effacer les labels du panier
        private void clearDynamicLabelPanier()
        {
            foreach (var label in dynamicLabelPanier)
            {
                this.Controls.Remove(label);
                label.Dispose();
            }
            dynamicLabelPanier.Clear();
        }

        // Méthode pour créer les labels de total
        private void createLabelTot(Point location, string text)
        {
            Label newLabel = new Label
            {
                Text = text,
                AutoSize = true,
                Location = location,
                BackColor = Color.FromArgb(210, 255, 210),
                Font = new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point)
            };
            this.Controls.Add(newLabel);
            dynamicLabelTot.Add(newLabel);
        }

        // Méthode pour créer les labels du total final
        private void createLabelTotFinal(Point location, string text)
        {
            Label newLabel = new Label
            {
                Text = text,
                AutoSize = true,
                Location = location,
                BackColor = Color.FromArgb(210, 255, 210),
                Font = new Font("Arial", 15F, FontStyle.Bold, GraphicsUnit.Point)
            };
            this.Controls.Add(newLabel);
            dynamicLabelTot.Add(newLabel);
        }
        //Méthode pour revenir sur le menu pricipal
        public void Annuler_click(object sender, EventArgs e)
        {
            bouton.clearDynamicButtons();
            clearDynamicTextBox();
            clearDynamicLabel();
            bouton.CreateButtonsFromDictionary(dicoProduits, number_of_the_page); // Recréer les boutons après validation
        }
        //Méthodepour afficher le menus de recherche de produit
        public void Menus_Recherche_click(object sender, EventArgs e)
        {
            bouton.clearDynamicButtons();
            clearDynamicTextBox();
            clearDynamicLabel();
            bouton.SearchButtonsFromDictionary();


        }
        //Méthode pour effectuer une recherche
        public void Recherche(object sender, EventArgs e)
        {
            bouton.Search(dicoProduits, 0);
        }
        //Méthode pour afficher la page de recherche suivante dans le menus de recherche
        public void page_handler_1 (object sender, EventArgs e) {
             number_of_the_search_page++;
             bouton.Search(dicoProduits, number_of_the_search_page);

        }
        //Méthode pour afficher la page de recherche précédente dans le menus de recherche
        public void page_handler_2(object sender, EventArgs e)
        {
            number_of_the_search_page--;
            bouton.Search(dicoProduits, number_of_the_search_page);

        }
        //Méthode pour afficher la page suivante dans le menus principal
        public void page_handler_1_1(object sender, EventArgs e)
        {
            number_of_the_page++; 
            bouton.CreateButtonsFromDictionary(dicoProduits, number_of_the_page);

        }
        //Méthode pour afficher la page précédente dans le menus principal
        public void page_handler_2_2(object sender, EventArgs e)
        {
            number_of_the_page--;
            bouton.CreateButtonsFromDictionary(dicoProduits, number_of_the_page);

        }
        //Méthode de setter pour eviter des bugs
        public void set_search_var(int value)
        {
            number_of_the_search_page = value;
        }
        //Méthode pour effacer le panier actuel en gardant la même base de donnée
        public void newPanier_Click(object sender, EventArgs e)
        {
            // Vide le dictionnaire du panier
            panier.Clear();

            // Efface les labels d'affichage du panier et des totaux
            clearDynamicLabelPanier(); // Méthode pour effacer les labels de produits dans le panier
            clearDynamicLabelTot();    // Méthode pour effacer les labels de total

            // Réinitialise les totaux à zéro
            createLabelTot(new Point(20, 375+80), "Total produit : 0€");
            createLabelTot(new Point(20, 400+80), "Total TVA : 0€");
            createLabelTotFinal(new Point(20, 425+80), "Total : 0€");

            this.Refresh(); // Rafraîchi l'interface pour afficher les changements
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void prevTicket_Click(object sender, EventArgs e)
        {
            string ticketContent = exporter.SeeTicket(panier);
            MessageBox.Show(ticketContent, "Prévisualisation du ticket", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
