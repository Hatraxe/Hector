using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace Hector
{
    /// <summary>
    /// Classe permettant la gestion des chargment des donnes dans le lsitview , treeView et combo box
    /// </summary>
    public static class GestionLoad
    {
        /// <summary>
        /// Methode permettatn de charger les familles dans le treeNode
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="connectionString"></param>
        public static void LoadFamilies(TreeNode parentNode, string connectionString)
        {
            using (var conn = new SQLiteConnection(connectionString)) //Connection à la base de donnees
            {
                conn.Open();

                // Requete pour recuperer les valeurs souhaitees
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

        /// <summary>
        /// Methode permettant de charger les Famille dans le comboBox
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="connectionString"></param>
        public static void LoadFamilleBox(ComboBox comboBox, string connectionString)
        {
            using (var conn = new SQLiteConnection(connectionString))//Connection à la base de donnees
            {
                conn.Open();
                string query = "SELECT Nom FROM Familles";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        //boucle sur le resultat de la requete
                        while (reader.Read())
                        {
                            string familleNom = reader["Nom"].ToString();
                            comboBox.Items.Add(familleNom);//Ajout dans le comboBox
                        }
                    }
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Methode permettant de charger les sousfamilles dans le comboBox
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="connectionString"></param>
        /// <param name="familleSelectionnee"></param>
        public static void LoadSousFamilleBox(ComboBox comboBox, string connectionString, string familleSelectionnee)
        {
            comboBox.Items.Clear(); // Effacer les éléments précédents

            using (var conn = new SQLiteConnection(connectionString)) //Connection à la base de donnees
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
                            comboBox.Items.Add(sousFamilleNom); //Ajout dans le comboBox
                        }
                    }
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Methode permettatn de charger les amrque dans le comboBox
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="connectionString"></param>
        public static void LoadMarqueBox(ComboBox comboBox, string connectionString)
        {
            using (var conn = new SQLiteConnection(connectionString)) //Connection à la base de donnees
            {
                conn.Open();
                string query = "SELECT Nom FROM Marques";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        //boucle sur le resultat de la requete
                        while (reader.Read())
                        {
                            string familleNom = reader["Nom"].ToString();
                            comboBox.Items.Add(familleNom); //ajout dans le combobox
                        }
                    }
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Charge les sousFamilles saans le treeVIew
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="refFamille"></param>
        /// <param name="connectionString"></param>
        public static void LoadSousFamilles(TreeNode parentNode, int refFamille, string connectionString)
        {
            using (var conn = new SQLiteConnection(connectionString)) //Connection à la base de donnees
            {
                conn.Open();
                string query = "SELECT Nom FROM SousFamilles WHERE RefFamille = @RefFamille";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RefFamille", refFamille);
                    using (var reader = cmd.ExecuteReader())
                    {
                        //boucle sur le resultat de la 
                        while (reader.Read())
                        {
                            string sousFamilleNom = reader["Nom"].ToString();
                            TreeNode sousFamilyNode = new TreeNode(sousFamilleNom);
                            parentNode.Nodes.Add(sousFamilyNode); //Ajout dans le node
                        }
                    }
                }
                conn.Close();
            }
        }
        
        /// <summary>
        /// Charge les marques dans le treeView
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="connectionString"></param>
        /// 
        public static void LoadBrands(TreeNode parentNode, string connectionString)
        {
            using (var conn = new SQLiteConnection(connectionString)) //Connection à la base de donnees
            {
                conn.Open();
                string query = "SELECT Nom FROM Marques";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        //boucle sur le resultat de la requete
                        while (reader.Read())
                        {
                            string MarqueNom = reader["Nom"].ToString();
                            parentNode.Nodes.Add(MarqueNom);//Ajout dans le node
                        }
                    }
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Charge tout les articles dans la listView
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="connectionString"></param>
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

            using (var conn = new SQLiteConnection(connectionString)) //Connection à la base de donnees
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
                        //boucle sur le resultat de la requete
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
        /// <summary>
        /// Charge la lsite des marques dans le listView
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="connectionString"></param>
        public static void LoadMarqueListView(ListView listView, string connectionString)
        {
            listView.Items.Clear();
            listView.Columns.Clear(); // Effacer les colonnes existantes

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);

            using (var conn = new SQLiteConnection(connectionString)) //Connection à la base de donnees
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

        /// <summary>
        /// CHarge la lsite de sosu familles dans le lsitVIew
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="connectionString"></param>
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

                            // Créer un nouvel objet ListViewItem avec les valeurs récupérées et l'ajouter à la ListView
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="connectionString"></param>
        /// <param name="marque"></param>
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

                // Requête SQL pour sélectionner les valeurs souhaitees

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
                            // Récupérer les valeurs de chaque colonne dans la ligne actuelle
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string marqueNom = reader["Marque"].ToString();
                            string familleNom = reader["Famille"].ToString();
                            string sousFamille = reader["Sous-Famille"].ToString();
                            string prixHT = reader["PrixHT"].ToString();
                            string quantite = reader["Quantite"].ToString();

                            // Créer un nouvel objet ListViewItem avec les valeurs récupérées et l'ajouter à la ListView
                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, marqueNom, familleNom, sousFamille, prixHT, quantite });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="connectionString"></param>
        /// <param name="famille"></param>
        /// 
        public static void LoadSousFamillesByFamille(ListView listView, string connectionString, string famille)
        {
            // Effacer tous les éléments et colonnes existants dans la ListView
            listView.Items.Clear();
            listView.Columns.Clear();

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Sous-Famille", 200);

            // Créer une connexion à la base de données SQLite en utilisant la chaîne de connexion fournie
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open(); // Ouvrir la connexion à la base de données

                // Requête SQL pour sélectionner les sous-familles appartenant à la famille spécifiée
                string query = @"SELECT SF.Nom AS 'Sous-Famille' 
                         FROM SousFamilles SF
                         INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                         WHERE F.Nom = @Famille";

                // Créer une commande SQL avec la requête et la connexion spécifiées
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    // Ajouter un paramètre à la commande pour la famille spécifiée
                    cmd.Parameters.AddWithValue("@Famille", famille);

                    // Exécuter la commande SQL et récupérer les résultats dans un objet SqlDataReader
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Parcourir les résultats de la requête
                        while (reader.Read())
                        {
                            // Récupérer les valeurs de chaque colonne dans la ligne actuelle
                            string description = reader["Sous-Famille"].ToString();

                            // Créer un nouvel objet ListViewItem avec la valeur récupérée et l'ajouter à la ListView
                            ListViewItem item = new ListViewItem(new string[] { description });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close(); // Fermer la connexion à la base de données
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="connectionString"></param>
        /// <param name="sousFamille"></param>
        /// 
        public static void LoadArticlesBySousFamille(ListView listView, string connectionString, string sousFamille)
        {
            // Effacer tous les éléments et colonnes existants dans la ListView
            listView.Items.Clear();
            listView.Columns.Clear();

            // Ajouter les entêtes de colonnes appropriées
            listView.Columns.Add("Description", 200);
            listView.Columns.Add("Ref", 100);
            listView.Columns.Add("Marque", 150);
            listView.Columns.Add("Famille", 150);
            listView.Columns.Add("Sous-Famille", 150);
            listView.Columns.Add("Prix H.T.", 100);
            listView.Columns.Add("Quantité", 100);

            // Créer une connexion à la base de données SQLite en utilisant la chaîne de connexion fournie
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open(); // Ouvrir la connexion à la base de données

                // Requête SQL pour sélectionner les articles appartenant à la sous-famille spécifiée
                string query = @"SELECT A.Description, A.RefArticle, M.Nom AS Marque, F.Nom AS Famille, SF.Nom AS 'Sous-Famille', A.PrixHT, A.Quantite
                         FROM Articles A 
                         INNER JOIN Marques M ON A.RefMarque = M.RefMarque
                         INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                         INNER JOIN Familles F ON SF.RefFamille = F.RefFamille
                         WHERE SF.Nom = @SousFamille";

                // Créer une commande SQL avec la requête et la connexion spécifiées
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    // Ajouter un paramètre à la commande pour la sous-famille spécifiée
                    cmd.Parameters.AddWithValue("@SousFamille", sousFamille);

                    // Exécuter la commande SQL et récupérer les résultats dans un objet SqlDataReader
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Parcourir les résultats de la requête
                        while (reader.Read())
                        {
                            // Récupérer les valeurs de chaque colonne dans la ligne actuelle
                            string description = reader["Description"].ToString();
                            string refArticle = reader["RefArticle"].ToString();
                            string marque = reader["Marque"].ToString();
                            string famille = reader["Famille"].ToString();
                            string sousFamilleNom = reader["Sous-Famille"].ToString();
                            string prixHT = reader["PrixHT"].ToString();
                            string quantite = reader["Quantite"].ToString();

                            // Créer un nouvel objet ListViewItem avec les valeurs récupérées et l'ajouter à la ListView
                            ListViewItem item = new ListViewItem(new string[] { description, refArticle, marque, famille, sousFamilleNom, prixHT, quantite });
                            listView.Items.Add(item);
                        }
                    }
                }

                conn.Close(); // Fermer la connexion à la base de données
            }
        }

    }
}
