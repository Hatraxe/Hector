using Hector; // Importation de l'espace de noms Hector
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    // Déclaration partielle de la classe FormAjoutSousFamille
    public partial class FormAjoutSousFamille : Form
    {
        // Déclaration des champs privés
        private string dbPath; // Chemin vers la base de données
        private string connectionString; // Chaîne de connexion à la base de données
        

        // Constructeur de la classe
        public FormAjoutSousFamille()
        {
            InitializeComponent(); // Initialisation des composants du formulaire
            
            InitializeDatabase(); // Initialisation de la base de données
            GestionLoad.LoadFamilleBox(comboBox1, connectionString); // Chargement des familles dans la ComboBox
        }


        /// <summary>
        ///  Méthode pour initialiser la base de données
        /// </summary>
        private void InitializeDatabase()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory; // Chemin vers le répertoire de l'application
            dbPath = System.IO.Path.Combine(appPath, "Data", "Hector.SQLite"); // Construction du chemin de la base de données
            connectionString = $"Data Source={dbPath};Version=3;"; // Construction de la chaîne de connexion
        }

     
        /// <summary>
        /// Événement déclenché lors du clic sur le bouton "Ajouter".
        /// Ajoute une nouvelle sous-famille à la base de données.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement.</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private void buttonAjouter_Click(object sender, EventArgs e)
        {
            string nomSousFamille = textBox2.Text; // Récupération du nom de la sous-famille depuis le champ de texte
            string nomFamille = comboBox1.SelectedItem?.ToString(); // Récupération du nom de la famille sélectionnée dans la ComboBox

            if (!string.IsNullOrEmpty(nomSousFamille) && !string.IsNullOrEmpty(nomFamille))
            {
                // Récupérer la référence de la famille
                int refFamille = Famille.GetReferenceFromNom(nomFamille, connectionString);

                // Création d'une instance de la classe SousFamille
                SousFamille sousFamille = new SousFamille
                {
                    Nom = nomSousFamille,
                    RefFamille = refFamille
                };

                // Insertion ou mise à jour de la sous-famille dans la base de données
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open(); // Ouverture de la connexion à la base de données
                    sousFamille.InsertOrUpdate(conn); // Appel de la méthode d'insertion ou de mise à jour
                }

                // Afficher un message indiquant que la sous famille a été ajoutée avec succès
                MessageBox.Show("La sous famille a été ajoutée avec succès !");

                // Fermeture de la fenêtre d'ajout
                this.Close();
            }
            else
            {
                // Affichage d'un message d'erreur si des champs sont vides
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        
        /// <summary>
        /// Événement déclenché lors du clic sur le bouton "Annuler".
        /// Ferme la fenêtre d'ajout de sous-famille.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement.</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private void buttonAnnuler_Click(object sender, EventArgs e)
        {
            this.Close(); // Fermeture de la fenêtre d'ajout
        }

        
        /// <summary>
        /// Événement déclenché lors de la sélection d'un élément dans la ComboBox.
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement.</param>
        /// <param name="e">Les arguments de l'événement.</param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
