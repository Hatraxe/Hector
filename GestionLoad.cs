using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace Hector
{
    public static class GestionLoad
    {
      
        public static void LoadFamilies(TreeNode parentNode, string connectionString)
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
                            string familleNom = reader["Nom"].ToString();
                            TreeNode familyNode = new TreeNode(familleNom);
                            LoadSousFamilles(familyNode, refFamille, connectionString); // Charger les sous-familles pour cette famille
                            parentNode.Nodes.Add(familyNode);
                        }
                    }
                }
                conn.Close();
            }
        }

        public static void LoadFamilleBox(ComboBox comboBox, string connectionString)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Nom FROM Familles";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string familleNom = reader["Nom"].ToString();
                            comboBox.Items.Add(familleNom);
                        }
                    }
                }
                conn.Close();
            }
        }
        public static void LoadSousFamilleBox(ComboBox comboBox, string connectionString, string familleSelectionnee)
        {
            comboBox.Items.Clear(); // Effacer les éléments précédents

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT SF.Nom 
                         FROM SousFamilles SF
                         INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                         WHERE F.Nom = @FamilleNom";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FamilleNom", familleSelectionnee);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sousFamilleNom = reader["Nom"].ToString();
                            comboBox.Items.Add(sousFamilleNom);
                        }
                    }
                }
                conn.Close();
            }
        }

        public static void LoadMarqueBox(ComboBox comboBox, string connectionString)
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
                            string familleNom = reader["Nom"].ToString();
                            comboBox.Items.Add(familleNom);
                        }
                    }
                }
                conn.Close();
            }
        }
        public static void LoadSousFamilles(TreeNode parentNode, int refFamille, string connectionString)
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
                            string sousFamilleNom = reader["Nom"].ToString();
                            TreeNode sousFamilyNode = new TreeNode(sousFamilleNom);
                            parentNode.Nodes.Add(sousFamilyNode);
                        }
                    }
                }
                conn.Close();
            }
        }

        public static void LoadBrands(TreeNode parentNode, string connectionString)
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
                            string MarqueNom = reader["Nom"].ToString();
                            parentNode.Nodes.Add(MarqueNom);
                        }
                    }
                }
                conn.Close();
            }
        }

        public static void LoadAllArticles(ListView listView, string connectionString)
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
        public static void LoadMarqueListView(ListView listView, string connectionString)
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);

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
                            string marqueNom = reader["Nom"].ToString();
                            ListViewItem item = new ListViewItem(new string[] { marqueNom });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
                GestionGroupes.GroupItemsByFirstLetter(listView);
            }
        }
        //charge la liste des osusfamilles
        public static void LoadSousFamilleListView(ListView listView, string connectionString)
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Nom FROM SousFamilles";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sousFamilleNom = reader["Nom"].ToString();
                            ListViewItem item = new ListViewItem(new string[] { sousFamilleNom });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
            }
        }
        //charge la liste des familles
        public static void LoadFamilleListView(ListView listView, string connectionString)
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Nom FROM Familles";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string familleNom = reader["Nom"].ToString();
                            ListViewItem item = new ListViewItem(new string[] { familleNom });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
            }
        }



        public static void LoadArticlesByMarque(ListView listView, string connectionString, string marque)
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);
            listView.Columns.Add("Ref", 100);
            listView.Columns.Add("Marque", 150);
            listView.Columns.Add("Famille", 150);
            listView.Columns.Add("Sous-Famille", 150);
            listView.Columns.Add("Prix H.T.", 100);
            listView.Columns.Add("Quantité", 100);

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT A.Description, A.RefArticle, M.Nom AS Marque, F.Nom AS Famille, SF.Nom AS 'Sous-Famille', A.PrixHT, A.Quantite
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
                            string marqueNom = reader["Marque"].ToString();
                            string familleNom = reader["Famille"].ToString();
                            string sousFamille = reader["Sous-Famille"].ToString();
                            string prixHT = reader["PrixHT"].ToString();
                            string quantite = reader["Quantite"].ToString();

                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, marqueNom, familleNom, sousFamille, prixHT, quantite });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
            }
        }

        public static void LoadSousFamillesByFamille(ListView listView, string connectionString, string famille)
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200); // Correction de l'entête de colonne

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT SF.Nom AS 'Sous-Famille' 
                         FROM SousFamilles SF
                         INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                         WHERE F.Nom = @Famille";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Famille", famille);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["Sous-Famille"].ToString(); // Correction de la clé de lecture

                            ListViewItem item = new ListViewItem(new string[] { description });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
            }
        }

        public static void LoadArticlesBySousFamille(ListView listView, string connectionString, string sousFamille)
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);
            listView.Columns.Add("Ref", 100);
            listView.Columns.Add("Marque", 150);
            listView.Columns.Add("Famille", 150);
            listView.Columns.Add("Sous-Famille", 150);
            listView.Columns.Add("Prix H.T.", 100);
            listView.Columns.Add("Quantité", 100);

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT A.Description, A.RefArticle, M.Nom AS Marque, F.Nom AS Famille, SF.Nom AS 'Sous-Famille', A.PrixHT, A.Quantite
                             FROM Articles A 
                             INNER JOIN Marques M ON A.RefMarque = M.RefMarque
                             INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                             INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                             WHERE SF.Nom = @SousFamille";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SousFamille", sousFamille);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string marque = reader["Marque"].ToString();
                            string famille = reader["Famille"].ToString();
                            string sousFamilleNom = reader["Sous-Famille"].ToString();
                            string prixHT = reader["PrixHT"].ToString();
                            string quantite = reader["Quantite"].ToString();

                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, marque, famille, sousFamilleNom, prixHT, quantite });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
            }
        }
    }
}
