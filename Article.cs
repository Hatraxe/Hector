using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Article

    {
        public int Id { get; set; }
        public string RefArticle { get; set; }
        public string Description { get; set; }
        public float PrixHT { get; set; }
        public int RefSousFamille { get; set; }
        public int RefMarque { get; set; }

       /* private void CreateArticlesTable(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Articles (" +
                                               "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                               "RefArticle TEXT UNIQUE," +
                                               "Description TEXT," +
                                               "RefSousFamille INTEGER," +
                                               "RefMarque INTEGER," +
                                               "PrixHT REAL," +
                                               "Quantite INTEGER DEFAULT 0," +
                                               "FOREIGN KEY(RefSousFamille) REFERENCES SousFamilles(RefSousFamille)," +
                                               "FOREIGN KEY(RefMarque) REFERENCES Marques(RefMarque)" +
                                               ")", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }*/

        public void InsertOrUpdate(SQLiteConnection conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn));
            }

            using (var transaction = conn.BeginTransaction())
            {
                if (Id == 0)
                {
                    // Nouvel article : effectue une insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@RefArticle, @Description, @RefSousFamille, @RefMarque, @PrixHT, 0)", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@RefArticle", RefArticle);
                        cmdInsert.Parameters.AddWithValue("@Description", Description);
                        cmdInsert.Parameters.AddWithValue("@RefSousFamille", RefSousFamille);
                        cmdInsert.Parameters.AddWithValue("@RefMarque", RefMarque);
                        cmdInsert.Parameters.AddWithValue("@PrixHT", PrixHT);
                        cmdInsert.ExecuteNonQuery();
                    }
                    // Récupère l'ID de l'article nouvellement inséré
                    using (var cmdGetLastId = new SQLiteCommand("SELECT last_insert_rowid()", conn))
                    {
                        Id = Convert.ToInt32(cmdGetLastId.ExecuteScalar());
                    }
                }
                else
                {
                    // Article existant : effectue une mise à jour
                    using (var cmdUpdate = new SQLiteCommand("UPDATE Articles SET Description = @Description, RefSousFamille = @RefSousFamille, RefMarque = @RefMarque, PrixHT = @PrixHT WHERE Id = @Id", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Description", Description);
                        cmdUpdate.Parameters.AddWithValue("@RefSousFamille", RefSousFamille);
                        cmdUpdate.Parameters.AddWithValue("@RefMarque", RefMarque);
                        cmdUpdate.Parameters.AddWithValue("@PrixHT", PrixHT);
                        cmdUpdate.Parameters.AddWithValue("@Id", Id);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }


        private void InsertOrUpdateArticle(SQLiteConnection conn, string refArticle, string description, int refSousFamille, int refMarque, float prixHT)
        {
            Article article = GetArticleByRef(conn, refArticle);

            if (article == null)
            {
                // Insertion
                article = new Article
                {
                    RefArticle = refArticle,
                    Description = description,
                    RefSousFamille = refSousFamille,
                    RefMarque = refMarque,
                    PrixHT = prixHT
                };
                article.InsertOrUpdate(conn);
            }
            else
            {
                // Mise à jour
                article.Description = description;
                article.RefSousFamille = refSousFamille;
                article.RefMarque = refMarque;
                article.PrixHT = prixHT;
                article.InsertOrUpdate(conn);
            }
        }

        private Article GetArticleByRef(SQLiteConnection conn, string refArticle)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT * FROM Articles WHERE RefArticle = @RefArticle", conn))
            {
                cmdCheck.Parameters.AddWithValue("@RefArticle", refArticle);
                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Article
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            RefArticle = reader["RefArticle"].ToString(),
                            Description = reader["Description"].ToString(),
                            RefSousFamille = Convert.ToInt32(reader["RefSousFamille"]),
                            RefMarque = Convert.ToInt32(reader["RefMarque"]),
                            PrixHT = Convert.ToSingle(reader["PrixHT"])
                        };
                    }
                    return null;
                }
            }
        }

    }
}
