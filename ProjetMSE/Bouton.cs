

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;


namespace Primeur
{
    internal class Bouton
    {
        private List<Guna2Button> dynamicButtons = new List<Guna2Button>(); // Boutons dynamiques en librairie Guna2
        private int ancienne_valeur = 0;
        private string name = ""; // Nom du dernier produit ajouté au panier
        private Form1 form1;
        private DataExport exporter;

        public Bouton(Form1 formInstance)
        {
            form1 = formInstance;
            exporter = new DataExport();
        }
        // Méthode pour créer un bouton dynamique
        public void createDynamicButton(Point location, string text, EventHandler clickEvent,Color couleur,Size taille,Font style,Color textColor, Color borderColor, int borderThickness)
        {
            Guna2Button gunaButton = new Guna2Button
            {
                Location = location,
                Size = taille, 
                Text = text,
                BorderRadius = 5,
                BorderColor = borderColor,
                BorderThickness = borderThickness,
                FillColor = couleur, 
                Font = style,
                ForeColor = textColor
            };
            gunaButton.Click += clickEvent;
            form1.Controls.Add(gunaButton);
            dynamicButtons.Add(gunaButton);
        }
        // Méthode pour effacer les boutons dynamiques
        public void clearDynamicButtons()
        {
            foreach (var button in dynamicButtons)
            {
                form1.Controls.Remove(button);
                button.Dispose();
            }
            dynamicButtons.Clear();
        }
        // Méthode pour gérer le clic sur un bouton produit
        private void ButtonClickHandler(string produit)
        {
            // Sauvegarde le nom du produit cliqué pour l'ajout au panier
            name = produit;

            // Supprime les boutons et afficher l'interface pour entrer le poids
            clearDynamicButtons();
            form1.clearDynamicTextBox();
            form1.createTextBox(new Point(170, 155),new Size(100,25));
            form1.createLabel(new Point(170, 100), $"Veuillez renseigner le poids pour {produit} en Kg", new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point));

            // Crée un bouton de validation et d'annulation en Guna2
            createDynamicButton(new Point(300, 150), "Valider", new EventHandler(validation_button_click), Color.FromArgb(246, 102, 13), new Size(100, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            createDynamicButton(new Point(300, 230), "Annuler", new EventHandler(form1.Annuler_click), Color.FromArgb(244, 67, 54), new Size(100, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            form1.createLabel(new Point(170, 200), $"Aplliquez réduction :", new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point));
            form1.createTextBox(new Point(170, 225), new Size(50, 25));

            // Rafraîchi le formulaire pour mettre à jour l'affichage
            form1.Refresh(); 
        }
        // Méthode pour valider l'entrée de poids et calculer les totaux
        private void validation_button_click(object sender, EventArgs e)
        {
            try
            {
                // Récupérer le poids saisi
                double reduc = (form1.dynamicTextBox[1].Text == "") ? 0 : double.Parse(form1.dynamicTextBox[1].Text);// Je fais un if else en oneline pour le kiffe
                reduc = (reduc <= 100 && reduc >= 0) ? (100-reduc)* 0.01 : 1; // J'en refais un car flemme d'écrire jsp cb de ligne (=
                String input= form1.dynamicTextBox[0].Text;
                int index = input.IndexOf('.');
                if (form1.dynamicTextBox[0].Text.Contains("."))
                {
                    input=input.Remove(index, 1).Insert(index, ",");
                }
                double poids = double.Parse(input)*reduc;
                // Calculer le prix total pour ce poids
                double prixUnitaire = form1.dicoProduits[name]; // `name` est le produit sélectionné
                double prixTotal = Math.Round(poids * prixUnitaire, 2);

                // Ajouter ou mettre à jour le produit dans le panier
                if (form1.panier.ContainsKey(name))
                {
                    // Si le produit est déjà dans le panier, additionner les poids et prix
                    double anciennePoid = form1.panier[name].Item1;
                    double ancienPrixTotal = form1.panier[name].Item2;
                    form1.panier[name] = new Tuple<double, double>(anciennePoid + poids, ancienPrixTotal + prixTotal);
                }
                else
                {
                    // Si le produit n'est pas encore dans le panier, l'ajouter
                    form1.panier[name] = new Tuple<double, double>(poids, prixTotal);
                }

                // Mettre à jour l'affichage du total
                form1.calculer_totale();
                // Mettre à jour l'affichage du panier
                form1.UpdatePanierDisplay();

                // Autres opérations de mise à jour de l'interface
                clearDynamicButtons();
                form1.clearDynamicTextBox();
                form1.clearDynamicLabel();
                CreateButtonsFromDictionary(form1.dicoProduits,form1.number_of_the_page); // Recréer les boutons après validation
            }
            catch (FormatException)
            {
                MessageBox.Show("Indiquez un nombre valide (utilisez une virgule pour les décimales).");
            }
        }
        // Méthode pour créer des boutons dynamiques en fonction des clés du dictionnaire
        public void CreateButtonsFromDictionary(Dictionary<string, double> dicoImported,int j)
        {
            clearDynamicButtons(); // Effacer les boutons existants, si nécessaire
            createDynamicButton(new Point(201, 30+80), "Charger Données", new EventHandler(form1.loadFileButton_Click), Color.FromArgb(80, 200, 120), new Size(125, 22), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            createDynamicButton(new Point(336, 30+80), "Nouveau Pannier", new EventHandler(form1.newPanier_Click), Color.FromArgb(80, 200, 120), new Size(120, 22), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            createDynamicButton(new Point(271, 65 + 80), "Faire une recherche", new EventHandler(form1.Menus_Recherche_click), Color.FromArgb(170, 120, 220), new Size(120, 35), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            if (form1.dynamicLabelPanier.Count != 0 ) {
                createDynamicButton(new Point(3, 475 + 80), "Aperçu", new EventHandler(form1.prevTicket_Click), Color.FromArgb(100, 181, 246), new Size(70, 22), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
                createDynamicButton(new Point(74, 475 + 80), "Enregistrer", new EventHandler(form1.saveTicket_Click), Color.FromArgb(30, 136, 229), new Size(90, 22), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            }
            List<String> product_searched_list = new List<String>();
            foreach (var produit in dicoImported)
            {
                product_searched_list.Add(produit.Key);
            }
            if (product_searched_list.Count <= 6)
            {
                for (int i = 0; i < product_searched_list.Count; i++)
                {
                    Point location = new Point(176 + 160 * ((i + 1) % 2), 185 + 80 * (i / 2));
                    // Créer un bouton pour chaque produit
                    Guna2Button gunaButton = new Guna2Button
                    {
                        Text = product_searched_list[i], // Utiliser le nom du produit comme texte du bouton
                        Size = new Size(150, 75),
                        Location = location,
                        BorderRadius = 10,
                        BorderColor = Color.Black,
                        BorderThickness = 2,
                        FillColor = Color.FromArgb(255, 180, 90),
                        Font = new Font("Arial", 15F, FontStyle.Bold, GraphicsUnit.Point),
                        ForeColor = Color.Black
                    };

                    String stockage = product_searched_list[i];
                    // Ajouter un événement au clic pour chaque bouton
                    gunaButton.Click += (sender, e) => ButtonClickHandler(stockage);

                    // Ajouter le bouton au formulaire et à la liste dynamique
                    form1.Controls.Add(gunaButton);
                    dynamicButtons.Add(gunaButton);
                }
            }
            else
            {
                if (form1.number_of_the_page < (product_searched_list.Count / 6) - 1)
                {
                    createDynamicButton(new Point(410, 450), ">>", new EventHandler(form1.page_handler_1_1), Color.FromArgb(80, 200, 120), new Size(50, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
                }
                if (form1.number_of_the_page >= 1)
                {
                    createDynamicButton(new Point(360, 450), "<<", new EventHandler(form1.page_handler_2_2), Color.FromArgb(80, 200, 120), new Size(50, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
                }
                for (int i = 0 + j * 6; i < 6 + j * 6; i++)
                {
                    Point location = new Point(176 + 160 * (((i - j * 6) + 1) % 2), 185 + 80 * ((i - j * 6) / 2));
                    // Créer un bouton pour chaque produit
                    Guna2Button gunaButton = new Guna2Button
                    {
                        Text = product_searched_list[i], // Utiliser le nom du produit comme texte du bouton
                        Size = new Size(150, 75),
                        Location = location,
                        BorderRadius = 10,
                        BorderColor = Color.Black,
                        BorderThickness = 2,
                        FillColor = Color.FromArgb(255, 180, 90),
                        Font = new Font("Arial", 15F, FontStyle.Bold, GraphicsUnit.Point),
                        ForeColor = Color.Black
                    };

                    String stockage = product_searched_list[i];
                    // Ajouter un événement au clic pour chaque bouton
                    gunaButton.Click += (sender, e) => ButtonClickHandler(stockage);

                    // Ajouter le bouton au formulaire et à la liste dynamique
                    form1.Controls.Add(gunaButton);
                    dynamicButtons.Add(gunaButton);
                }
            }

            form1.Refresh(); // Rafraîchir le formulaire pour afficher les boutons
        }

        public void SearchButtonsFromDictionary()
        {
            form1.createTextBox(new Point(170, 150), new Size(150, 25));
            createDynamicButton(new Point(320, 150), "Chercher", new EventHandler(form1.Recherche), Color.FromArgb(76, 175, 80), new Size(100, 20), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            createDynamicButton(new Point(330, 205), "Annuler", new EventHandler(form1.Annuler_click), Color.FromArgb(244, 67, 54), new Size(90, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
        }

        public void Search(Dictionary<string, double> dicoImported,int j) {
            String Recherche = form1.dynamicTextBox[0].Text;
            Char first_char = char.ToUpper(Recherche[0]);
            String Recherche2 =first_char+Recherche.Substring(1,Recherche.Length-1);
            List<String> product_searched_list = new List<String>();
            foreach (var produit in dicoImported) {
                String diconame=produit.Key;
                if (diconame.Contains(Recherche)|| diconame.Contains(Recherche2))
                {
                    product_searched_list.Add(diconame);
                }
            }
            if (product_searched_list.Count != ancienne_valeur) {
                ancienne_valeur = product_searched_list.Count;
                form1.set_search_var(0);
            }
            clearDynamicButtons();
            createDynamicButton(new Point(320, 150), "Chercher", new EventHandler(form1.Recherche), Color.FromArgb(76, 175, 80), new Size(100, 20), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            createDynamicButton(new Point(330, 450), "Annuler", new EventHandler(form1.Annuler_click), Color.FromArgb(244, 67, 54), new Size(100, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
            if (product_searched_list.Count <= 6)
            {
                for (int i = 0; i < product_searched_list.Count; i++)
                {
                    Point location = new Point(176 + 160 * ((i + 1) % 2), 185 + 80 * (i / 2));
                    // Créer un bouton pour chaque produit
                    Guna2Button gunaButton = new Guna2Button
                    {
                        Text = product_searched_list[i], // Utiliser le nom du produit comme texte du bouton
                        Size = new Size(150, 75),
                        Location = location,
                        BorderRadius = 10,
                        BorderColor = Color.Black,
                        BorderThickness = 2,
                        FillColor = Color.FromArgb(255, 180, 90),
                        Font = new Font("Arial", 15F, FontStyle.Bold, GraphicsUnit.Point),
                        ForeColor = Color.Black
                    };
                    String stockage = product_searched_list[i];
                    // Ajouter un événement au clic pour chaque bouton
                    gunaButton.Click += (sender, e) => ButtonClickHandler(stockage);

                    // Ajouter le bouton au formulaire et à la liste dynamique
                    form1.Controls.Add(gunaButton);
                    dynamicButtons.Add(gunaButton);
                }
            }
            else{
                clearDynamicButtons();
                createDynamicButton(new Point(320, 150), "Chercher", new EventHandler(form1.Recherche), Color.FromArgb(76, 175, 80), new Size(100, 20), new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
                createDynamicButton(new Point(250, 450), "Annuler", new EventHandler(form1.Annuler_click), Color.FromArgb(244, 67, 54), new Size(100, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
                if (form1.number_of_the_search_page < (product_searched_list.Count/6)-1)
                {
                    createDynamicButton(new Point(410, 450), ">>", new EventHandler(form1.page_handler_1), Color.FromArgb(80, 200, 120), new Size(50, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
                }
                if (form1.number_of_the_search_page >= 1)
                {
                    createDynamicButton(new Point(360, 450), "<<", new EventHandler(form1.page_handler_2), Color.FromArgb(80, 200, 120), new Size(50, 30), new Font("Arial", 10F, FontStyle.Bold, GraphicsUnit.Point), Color.White, Color.Black, 1);
                }
                for (int i = 0+j*6; i < 6+j*6; i++)
                {
                    Point location = new Point(176 + 160 *(((i-j*6)+1)%2) , 185 + 80 *((i-j*6)/2));
                    // Créer un bouton pour chaque produit
                    Guna2Button gunaButton = new Guna2Button
                    {
                        Text = product_searched_list[i], // Utiliser le nom du produit comme texte du bouton
                        Size = new Size(150, 75),
                        Location = location,
                        BorderRadius = 10,
                        BorderColor = Color.Black,
                        BorderThickness = 2,
                        FillColor = Color.FromArgb(255, 180, 90),
                        Font = new Font("Arial", 15F, FontStyle.Bold, GraphicsUnit.Point),
                        ForeColor = Color.Black
                    };

                    String stockage = product_searched_list[i];
                    // Ajouter un événement au clic pour chaque bouton
                    gunaButton.Click += (sender, e) => ButtonClickHandler(stockage);

                    // Ajouter le bouton au formulaire et à la liste dynamique
                    form1.Controls.Add(gunaButton);
                    dynamicButtons.Add(gunaButton);
                }
            }
        }
    }
}

