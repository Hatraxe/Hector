using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /// <summary>
    /// CLasse qui gère les exportations 
    /// </summary>
    class GestionExport
    {
        /// <summary>
        ///  Methode implementant la logique derriere l'exportation des donnes au format CSV
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="connectionString"></param>
        public static void ExportDataToCSV(string filePath, string connectionString)
        {
            // Crée une nouvelle connexion à la base de données SQLite en utilisant la chaîne de connexion fournie

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Commande SQL pour récupérer les valeurs souhaitées en utilisant des jointures

                using (var cmd = new SQLiteCommand(@"SELECT A.Description, A.RefArticle, M.Nom AS Marque, F.Nom AS Famille, SF.Nom AS 'Sous-Famille', A.PrixHT 
                                             FROM Articles A 
                                             INNER JOIN Marques M ON A.RefMarque = M.RefMarque
                                             INNER JOIN SousFamilles SF ON A.RefSousFamille = SF.RefSousFamille
                                             INNER JOIN Familles F ON SF.RefFamille = F.RefFamille", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        using (StreamWriter file = new StreamWriter(filePath))
                        {
                            // Écrit la ligne d'en-tête dans le fichier CSV avec les noms de colonnes

                            file.WriteLine("Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.");

                            // Boucle à travers les résultats de la requête SQL

                            while (reader.Read())
                            {
                                // Récupère les valeurs de chaque colonne dans la ligne actuelle
                                string description = reader["Description"].ToString();
                                string refArticle = reader["RefArticle"].ToString();
                                string marque = reader["Marque"].ToString();
                                string famille = reader["Famille"].ToString();
                                string sousFamille = reader["Sous-Famille"].ToString();
                                string prixHT = reader["PrixHT"].ToString();

                                string line = $"{description};{refArticle};{marque};{famille};{sousFamille};{prixHT}";

                                // Écrit la ligne dans le fichier CSV
                                file.WriteLine(line);
                            }
                        }
                    }
                }

                conn.Close();
            }
        }
    }
}
