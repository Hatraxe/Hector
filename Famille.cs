using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    class Famille
    {
        public int ReferenceFamille { get; set; }
        public string Nom { get; set; }

      

        public void InsertOrUpdate(SQLiteConnection conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn));
            }

            using (var transaction = conn.BeginTransaction())
            {
                if (ReferenceFamille == 0)
                {
                    // Nouvelle famille : effectue une insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO Familles (Nom) VALUES (@Nom); SELECT last_insert_rowid();", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                        ReferenceFamille = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    }
                }
                else
                {
                    // Famille existante : effectue une mise à jour
                    using (var cmdUpdate = new SQLiteCommand("UPDATE Familles SET Nom = @Nom WHERE ReferenceFamille = @ReferenceFamille", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Nom", Nom);
                        cmdUpdate.Parameters.AddWithValue("@ReferenceFamille", ReferenceFamille);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}
