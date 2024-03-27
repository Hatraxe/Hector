using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class SousFamille
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public int RefFamille { get; set; }

        private void CreateSousFamillesTable(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS SousFamilles (" +
                                               "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                               "Nom TEXT," +
                                               "RefFamille INTEGER," +
                                               "FOREIGN KEY(RefFamille) REFERENCES Familles(RefFamille)" +
                                               ")", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

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
                    // Nouvelle sous-famille : effectue une insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO SousFamilles (Nom, RefFamille) VALUES (@Nom, @RefFamille)", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                        cmdInsert.Parameters.AddWithValue("@RefFamille", RefFamille);
                        cmdInsert.ExecuteNonQuery();
                    }
                    // Récupère l'ID de la sous-famille nouvellement insérée
                    using (var cmdGetLastId = new SQLiteCommand("SELECT last_insert_rowid()", conn))
                    {
                        Id = Convert.ToInt32(cmdGetLastId.ExecuteScalar());
                    }
                }
                else
                {
                    // Sous-famille existante : effectue une mise à jour
                    using (var cmdUpdate = new SQLiteCommand("UPDATE SousFamilles SET Nom = @Nom, RefFamille = @RefFamille WHERE Id = @Id", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Nom", Nom);
                        cmdUpdate.Parameters.AddWithValue("@RefFamille", RefFamille);
                        cmdUpdate.Parameters.AddWithValue("@Id", Id);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        private SousFamille GetSousFamilleById(SQLiteConnection conn, int id)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT * FROM SousFamilles WHERE Id = @Id", conn))
            {
                cmdCheck.Parameters.AddWithValue("@Id", id);
                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SousFamille
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nom = reader["Nom"].ToString(),
                            RefFamille = Convert.ToInt32(reader["RefFamille"])
                        };
                    }
                    return null;
                }
            }
        }
    }
}
