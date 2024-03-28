using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    class Article
    {
        public string RefArticle { get; set; }
        public string Description { get; set; }
        public float PrixHT { get; set; }
        public int RefSousFamille { get; set; }
        public int RefMarque { get; set; }

        public Article(string refArticle, string description, float prixHT, int refSousFamille, int refMarque)
        {
            RefArticle = refArticle;
            Description = description;
            PrixHT = prixHT;
            RefSousFamille = refSousFamille;
            RefMarque = refMarque;
        }

        public void InsertOrUpdate(SQLiteConnection conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn));
            }

            using (var transaction = conn.BeginTransaction())
            {
                // Vérifie si l'article avec la même RefArticle existe déjà
                using (var cmdCheck = new SQLiteCommand("SELECT RefArticle FROM Articles WHERE RefArticle = @RefArticle", conn))
                {
                    cmdCheck.Parameters.AddWithValue("@RefArticle", RefArticle);
                    var existingRefArticle = cmdCheck.ExecuteScalar();

                    if (existingRefArticle != null)
                    {
                        // Article existant : effectue une mise à jour
                        using (var cmdUpdate = new SQLiteCommand("UPDATE Articles SET Description = @Description, RefSousFamille = @RefSousFamille, RefMarque = @RefMarque, PrixHT = @PrixHT WHERE RefArticle = @RefArticle", conn))
                        {
                            cmdUpdate.Parameters.AddWithValue("@Description", Description);
                            cmdUpdate.Parameters.AddWithValue("@RefSousFamille", RefSousFamille);
                            cmdUpdate.Parameters.AddWithValue("@RefMarque", RefMarque);
                            cmdUpdate.Parameters.AddWithValue("@PrixHT", PrixHT);
                            cmdUpdate.Parameters.AddWithValue("@RefArticle", RefArticle);
                            cmdUpdate.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Nouvel article : effectue une insertion
                        using (var cmdInsert = new SQLiteCommand("INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@RefArticle, @Description, @RefSousFamille, @RefMarque, @PrixHT, 0)", conn))
                        {
                            cmdInsert.Parameters.AddWithValue("@RefArticle", RefArticle);
                            cmdInsert.Parameters.AddWithValue("@Description", Description);
                            cmdInsert.Parameters.AddWithValue("@RefSousFamille", RefSousFamille);
                            cmdInsert.Parameters.AddWithValue("@RefMarque", RefMarque);
                            cmdInsert.Parameters.AddWithValue("@PrixHT", PrixHT);
                            cmdInsert.Parameters.AddWithValue("@Quantite", 0); // Initialise la quantité à 0
                            cmdInsert.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }
    }
}
