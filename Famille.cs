using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    class Famille
    {
        public int Id { get; set; }
        public string Nom { get; set; }

      /*  private void CreateFamillesTable(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Familles (" +
                                               "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                               "Nom TEXT UNIQUE" +
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
                    // Nouvelle famille : effectue une insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO Familles (Nom) VALUES (@Nom)", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                        cmdInsert.ExecuteNonQuery();
                    }
                    // Récupère l'ID de la famille nouvellement insérée
                    using (var cmdGetLastId = new SQLiteCommand("SELECT last_insert_rowid()", conn))
                    {
                        Id = Convert.ToInt32(cmdGetLastId.ExecuteScalar());
                    }
                }
                else
                {
                    // Famille existante : effectue une mise à jour
                    using (var cmdUpdate = new SQLiteCommand("UPDATE Familles SET Nom = @Nom WHERE Id = @Id", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Nom", Nom);
                        cmdUpdate.Parameters.AddWithValue("@Id", Id);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        private Famille GetFamilleByNom(SQLiteConnection conn, string nomFamille)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT * FROM Familles WHERE Nom = @Nom", conn))
            {
                cmdCheck.Parameters.AddWithValue("@Nom", nomFamille);
                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Famille
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nom = reader["Nom"].ToString()
                        };
                    }
                    return null;
                }
            }
        }
    }
}
