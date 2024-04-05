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
    public partial class FormAjoutFamille : Form
    {
        private string dbPath;
        private string connectionString;
       
        public FormAjoutFamille()
        {
            InitializeComponent();
            InitializeDatabase();
            
        }

        private void InitializeDatabase()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            dbPath = System.IO.Path.Combine(appPath, "Data", "Hector.SQLite");
            connectionString = $"Data Source={dbPath};Version=3;";
        }
        private void buttonAjouter_Click(object sender, EventArgs e)
        {
            string nomFamille = textBox1.Text; // Récupération du nom de la famille depuis le champ de texte
          
            if ( !string.IsNullOrEmpty(nomFamille))
            {


                // Création d'une instance de la classe SousFamille
                Famille famille = new Famille { Nom = nomFamille };


                // Insertion ou mise à jour de la sous-famille dans la base de données
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open(); // Ouverture de la connexion à la base de données
                    famille.InsertOrUpdate(conn); // Appel de la méthode d'insertion ou de mise à jour
                }

                // Afficher un message indiquant que la famille a été ajoutée avec succès
                MessageBox.Show("La famille a été ajoutée avec succès !");


                // Fermeture de la fenêtre d'ajout
                this.Close();
            }
            else
            {
                // Affichage d'un message d'erreur si des champs sont vides
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonAnnuler_Click(object sender, EventArgs e)
        {
            this.Close(); // Ferme la fenêtre d'ajout
        }
    }
}
