using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Marque
    {
        public int Id { get; set; }
        public string Nom { get; set; }

      /*  private void CreateMarquesTable(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Marques (" +
                                               "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                               "Nom TEXT UNIQUE" +
                                               ")", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
*/
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
                    // Nouvelle marque : effectue une insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO Marques (Nom) VALUES (@Nom)", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                        cmdInsert.ExecuteNonQuery();
                    }
                    // Récupère l'ID de la marque nouvellement insérée
                    using (var cmdGetLastId = new SQLiteCommand("SELECT last_insert_rowid()", conn))
                    {
                        Id = Convert.ToInt32(cmdGetLastId.ExecuteScalar());
                    }
                }
                else
                {
                    // Marque existante : effectue une mise à jour
                    using (var cmdUpdate = new SQLiteCommand("UPDATE Marques SET Nom = @Nom WHERE Id = @Id", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Nom", Nom);
                        cmdUpdate.Parameters.AddWithValue("@Id", Id);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        private Marque GetMarqueById(SQLiteConnection conn, int id)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT * FROM Marques WHERE Id = @Id", conn))
            {
                cmdCheck.Parameters.AddWithValue("@Id", id);
                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Marque
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
