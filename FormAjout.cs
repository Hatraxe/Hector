using Hector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormAjout : Form
    {
        private string dbPath;
        private string connectionString;
        private ListView listView;
        public FormAjout(ListView listView)
        {
            InitializeComponent();
            this.listView = listView; // Stocker la référence à listView

            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            dbPath = Path.Combine(appPath, "Data", "Hector.SQLite");
            connectionString = $"Data Source={dbPath};Version=3;";
        }



        // Gestionnaire d'événements pour le clic sur le bouton "Annuler"
        private void buttonAnnuler_Click(object sender, EventArgs e)
        {
            this.Close(); // Ferme la fenêtre d'ajout
        }

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
        private void famBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Recharger les sous-familles lorsque la famille sélectionnée change
            if (famBox.SelectedIndex != -1)
            {
                string familleSelectionnee = famBox.SelectedItem.ToString();
                GestionLoad.LoadSousFamilleBox(sousFamBox, connectionString, familleSelectionnee);
            }
        }

        private void sousFamBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void marqueBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonAjouter_Click(object sender, EventArgs e)
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
            if (ArticleExists(refArticle))
            {
                MessageBox.Show("La référence de l'article existe déjà.", "Erreur de saisie",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Arrêter l'exécution de la méthode si la référence de l'article existe déjà
            }

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
                MessageBox.Show("L'article a été ajouté avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Réinitialiser les contrôles du formulaire
                ClearFields();
            }
            catch (Exception ex)
            {
                // Afficher un message d'erreur en cas d'échec
                MessageBox.Show($"Une erreur s'est produite lors de l'ajout de l'article : {ex.Message}", "Erreur",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ArticleExists(string refArticle)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Articles WHERE RefArticle = @refArticle";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@refArticle", refArticle);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
      
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
    }
}
