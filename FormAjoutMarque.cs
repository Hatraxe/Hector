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
    public partial class FormAjoutMarque : Form
    {
        private string connectionString;
        private string dbPath;
      
        /// <summary>
        /// Classe qui gere le formualire d'ajout d'une marque
        /// </summary>
        public FormAjoutMarque()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        /// <summary>
        /// Initialise la connexion base de donnes
        /// </summary>
        private void InitializeDatabase()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory; // Chemin vers le répertoire de l'application
            dbPath = System.IO.Path.Combine(appPath, "Data", "Hector.SQLite"); // Construction du chemin de la base de données
            connectionString = $"Data Source={dbPath};Version=3;"; // Construction de la chaîne de connexion

        }
        private void buttonAjouter_Click(object sender, EventArgs e)
        {
            string marque = textBox1.Text;

            // Créer une nouvelle instance de la classe Marque
            Marque newMarque = new Marque { Nom = marque };

            // Ouvrir la connexion à la base de données SQLite et insérer ou mettre à jour la marque
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                newMarque.InsertOrUpdate(conn);
            }

            // Afficher un message indiquant que la marque a été ajoutée avec succès
            MessageBox.Show("La marque a été ajoutée avec succès !");

            // Fermer la fenêtre d'ajout de marque
            this.Close();

        }



        private void buttonAnnuler_Click(object sender, EventArgs e)
        {
            this.Close(); // Ferme la fenêtre d'ajout
        }
    }
}
