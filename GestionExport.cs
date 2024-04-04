using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class GestionExport
    {
        public static void ExportDataToCSV(string filePath, string connectionString)
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
                        using (StreamWriter file = new StreamWriter(filePath))
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
        }
    }
}
