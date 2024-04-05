﻿using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace Hector
{
    partial class FormModifier : Form
    {
        private string dbPath;
        private string connectionString;
        private ListView listView;

        /// <summary>
        /// Classe implementant le formulaire de modification des articles
        /// </summary>
        /// <param name="listView"></param>
        public FormModifier(ListView listView)
        {
            InitializeComponent();//Initialise les composants
            this.listView = listView; // Stocker la référence à listView

            InitializeDatabase();
            LoadData(); //Chargement des donnes dans les comboBox
        }
        /// <summary>
        ///  Methode implementant la logique derriere l'initialisation de la connection a la base de donnes
        /// </summary>
        private void InitializeDatabase()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            dbPath = System.IO.Path.Combine(appPath, "Data", "Hector.SQLite");
            connectionString = $"Data Source={dbPath};Version=3;";
        }

        /// <summary>
        ///  Methode implementant la logique derriere le bouton annuler du formulaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAnnuler_Click(object sender, EventArgs e)
        {
            this.Close(); // Ferme la fenêtre de modification
        }

        /// <summary>
        ///  Methode implementant la logique derriere le chargement des comboBox avec les donnees possible pour faciliter la saisie et eviter les erreurs
        /// </summary>
        private void LoadData()
        {

            // Charger les données des familles dans famBox
            GestionLoad.LoadFamilleBox(famBox, connectionString);

            // Charger les données des marques dans marqueBox
            GestionLoad.LoadMarqueBox(marqueBox, connectionString);

            // Charger les données des sous-familles dans sousFamBox
            if (famBox.SelectedIndex != -1)
            {
                string familleSelectionnee = famBox.SelectedItem.ToString();
                GestionLoad.LoadSousFamilleBox(sousFamBox, connectionString, familleSelectionnee);
            }

        }


        /// <summary>
        ///  Methode implementant la logique derriere la reinitialisation des combobox et des textbox
        /// </summary>
        private void ClearFields()
        {
            // Effacer le contenu des champs
            textBoxDesc.Text = "";
            textBoxRefArt.Text = "";
            textBoxPrix.Text = "";
            textBoxQuantite.Text = "";
            famBox.SelectedIndex = -1;
            marqueBox.SelectedIndex = -1;
            sousFamBox.SelectedIndex = -1;
        }

        /// <summary>
        ///  Methode implementant la logique derriere le remplissage des champs du formulaire par les donnes de la ligne selectionnee
        /// </summary>
        /// <param name="description"></param>
        /// <param name="refArticle"></param>
        /// <param name="marque"></param>
        /// <param name="famille"></param>
        /// <param name="sousFamille"></param>
        /// <param name="prixHT"></param>
        /// <param name="quantite"></param>
        public void RemplirChamps(string description, string refArticle, string marque, string famille, string sousFamille, string prixHT, string quantite)
        {
            // Remplir les champs du formulaire avec les valeurs passées
            textBoxDesc.Text = description;
            textBoxRefArt.Text = refArticle;
            marqueBox.Text = marque;
            famBox.Text = famille;
            sousFamBox.Text = sousFamille;
            textBoxPrix.Text = prixHT;
            textBoxQuantite.Text = quantite;
        }

        /// <summary>
        ///  Methode implementant la logique derriere l'appuie sur le bouton modifier du formulaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonModifier_Click(object sender, EventArgs e)
        {
            // Vérifier si tous les champs obligatoires sont remplis
            if (string.IsNullOrWhiteSpace(textBoxRefArt.Text) || string.IsNullOrWhiteSpace(textBoxDesc.Text) ||
                string.IsNullOrWhiteSpace(textBoxPrix.Text) || famBox.SelectedIndex == -1 ||
                marqueBox.SelectedIndex == -1 || sousFamBox.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur de saisie",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Arrêter l'exécution de la méthode si un champ obligatoire est vide
            }

            // Vérifier le format du prix
            if (!float.TryParse(textBoxPrix.Text, out float prixHT))
            {
                MessageBox.Show("Le prix doit être un nombre valide.", "Erreur de format",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Arrêter l'exécution de la méthode si le format du prix est incorrect
            }

            // Vérifier si la référence de l'article est unique
            string refArticle = textBoxRefArt.Text;
           

            // Récupérer les références sélectionnées dans les ComboBox
            string description = textBoxDesc.Text;
            int refSousFamille = SousFamille.GetReferenceFromNom(sousFamBox.SelectedItem.ToString(), connectionString);
            int refMarque = Marque.GetReferenceFromNom(marqueBox.SelectedItem.ToString(), connectionString);

            // Créer une instance de la classe Article
            Article nouvelArticle = new Article(refArticle, description, prixHT, refSousFamille, refMarque);

            // Insérer ou mettre à jour l'article dans la base de données
            try
            {
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    nouvelArticle.InsertOrUpdate(conn);
                }

                // Afficher un message de succès
                MessageBox.Show("L'article a été modifié avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Réinitialiser les contrôles du formulaire
                ClearFields();
            }
            catch (Exception ex)
            {
                // Afficher un message d'erreur en cas d'échec
                MessageBox.Show($"Une erreur s'est produite lors de la modification de l'article : {ex.Message}", "Erreur",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///  Methode implementant la logique derriere l'appuie sur le bouton Annuler du formulaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAnnuler_Click_1(object sender, EventArgs e)
        {
            this.Close(); // Ferme la fenêtre modifier
        }

        private void sousFamBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///  Methode implementant la logique derriere la selection de famille dans le formulaire afin de charger les sosu familles possible a selectionner dan sle combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void famBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Recharger les sous-familles lorsque la famille sélectionnée change
            if (famBox.SelectedIndex != -1)
            {
                string familleSelectionnee = famBox.SelectedItem.ToString();
                GestionLoad.LoadSousFamilleBox(sousFamBox, connectionString, familleSelectionnee);
            }
        }

    }
}
