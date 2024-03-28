using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    class Marque
    {
        public int ReferenceMarque { get; set; }
        public string Nom { get; set; }

     

        public void InsertOrUpdate(SQLiteConnection conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn));
            }

            using (var transaction = conn.BeginTransaction())
            {
                if (ReferenceMarque == 0)
                {
                    // Nouvelle marque : effectue une insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO Marques (Nom) VALUES (@Nom); SELECT last_insert_rowid();", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                        ReferenceMarque = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    }
                }
                else
                {
                    // Marque existante : effectue une mise à jour
                    using (var cmdUpdate = new SQLiteCommand("UPDATE Marques SET Nom = @Nom WHERE Id = @Id", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Nom", Nom);
                        cmdUpdate.Parameters.AddWithValue("@Id", ReferenceMarque);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }
    }
}
