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
using WindowsFormsApp1;

namespace Hector
{
    public partial class FormMain : Form
    {
        private string dbPath;
        private string connectionString;

        public FormMain()
        {
            InitializeComponent();
            InitializeDatabase();
           
        }

        private void InitializeDatabase()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            dbPath = Path.Combine(appPath, "Data", "Hector.SQLite");
            connectionString = $"Data Source={dbPath};Version=3;";
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadTreeViewData();
        }

        private void importerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormImport formImport = new FormImport();
            formImport.ShowDialog(this);
        }

        private void exporterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Save as CSV File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                ExportDataToCSV(filePath);
            }
        }

        private void ExportDataToCSV(string filePath)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(@"SELECT A.Description, A.RefArticle, M.Nom AS Marque, F.Nom AS Famille, SF.Nom AS 'Sous-Famille', A.PrixHT 
                                             FROM Articles A 
                                             INNER JOIN Marques M ON A.RefMarque = M.RefMarque
                                             INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                                             INNER JOIN Familles F ON SF.RefFamille = F.RefFamille", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
                        {
                            file.WriteLine("Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.");

                            while (reader.Read())
                            {
                                string description = reader["Description"].ToString();
                                string refArticle = reader["RefArticle"].ToString();
                                string marque = reader["Marque"].ToString();
                                string famille = reader["Famille"].ToString();
                                string sousFamille = reader["Sous-Famille"].ToString();
                                string prixHT = reader["PrixHT"].ToString();

                                string line = $"{description};{refArticle};{marque};{famille};{sousFamille};{prixHT}";
                                file.WriteLine(line);
                            }
                        }
                    }
                }

                conn.Close();
            }

            MessageBox.Show("Export réussi avec succès");
        }

        private void LoadTreeViewData()
        {
            TreeNode allArticlesNode = new TreeNode("Tous les articles");
            TreeNode familiesNode = new TreeNode("Familles");
            TreeNode brandsNode = new TreeNode("Marques");

            treeView.Nodes.Add(allArticlesNode);
            treeView.Nodes.Add(familiesNode);
            treeView.Nodes.Add(brandsNode);

            LoadFamilies(familiesNode);
            LoadBrands(brandsNode);
        }

        private void LoadFamilies(TreeNode parentNode)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT RefFamille, Nom FROM Familles";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int refFamille = Convert.ToInt32(reader["RefFamille"]);
                            string familleName = reader["Nom"].ToString();
                            TreeNode familyNode = new TreeNode(familleName);
                            LoadSousFamilles(familyNode, refFamille); // Charger les sous-familles pour cette famille
                            parentNode.Nodes.Add(familyNode);
                        }
                    }
                }
                conn.Close();
            }
        }

        private void LoadSousFamilles(TreeNode parentNode, int refFamille)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Nom FROM SousFamilles WHERE RefFamille = @RefFamille";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RefFamille", refFamille);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sousFamilleName = reader["Nom"].ToString();
                            TreeNode sousFamilyNode = new TreeNode(sousFamilleName);
                            parentNode.Nodes.Add(sousFamilyNode);
                        }
                    }
                }
                conn.Close();
            }
        }

        private void LoadBrands(TreeNode parentNode)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Nom FROM Marques";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string MarqueName = reader["Nom"].ToString();
                            parentNode.Nodes.Add(MarqueName);
                        }
                    }
                }
                conn.Close();
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Text == "Tous les articles")
            {
               
                LoadAllArticles();
            }
            if (e.Node.Parent != null && e.Node.Parent.Text == "Marques")
            {
                string marque = e.Node.Text;
                LoadArticlesByMarque(marque);
            }
        }
        private void LoadArticlesByMarque(string marque)
        {
            listView.Items.Clear();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT A.Description, A.RefArticle, M.Nom AS Marque, F.Nom AS Famille, SF.Nom AS 'Sous-Famille', A.PrixHT 
                         FROM Articles A 
                         INNER JOIN Marques M ON A.RefMarque = M.RefMarque
                         INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                         INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                         WHERE M.Nom = @Marque";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Marque", marque);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string marqueName = reader["Marque"].ToString();
                            string famille = reader["Famille"].ToString();
                            string sousFamille = reader["Sous-Famille"].ToString();
                            string prixHT = reader["PrixHT"].ToString();

                            ListViewItem item = new ListViewItem(new string[] { refArticle, description, famille, sousFamille, marqueName, prixHT });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
            }
        }

        private void LoadAllFamily()
        {

        }

        private void LoadAllArticles()
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);
            listView.Columns.Add("Référence", 100);
            listView.Columns.Add("Marque", 150);
            listView.Columns.Add("Famille", 150);
            listView.Columns.Add("Sous-famille", 150);
            listView.Columns.Add("Prix H.T.", 100);
            listView.Columns.Add("Quantité", 100);
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT A.Description, A.RefArticle, M.Nom AS Marque, F.Nom AS Famille, SF.Nom AS 'Sous-Famille', A.PrixHT, A.Quantite 
                         FROM Articles A 
                         INNER JOIN Marques M ON A.RefMarque = M.RefMarque
                         INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                         INNER JOIN Familles F ON SF.RefFamille = F.RefFamille";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string marque = reader["Marque"].ToString();
                            string famille = reader["Famille"].ToString();
                            string sousFamille = reader["Sous-Famille"].ToString();
                            string prixHT = reader["PrixHT"].ToString();
                            string quantite = reader["Quantite"].ToString(); // Ajout de la quantité

                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, marque, famille, sousFamille, prixHT, quantite });
                            listView.Items.Add(item);
                        }
                    }
                }
                conn.Close();
            }
        }



    }
}
