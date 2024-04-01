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

            // Vérifier si la marque existe déjà dans la base de données
            var cmdCheckExistence = new SQLiteCommand("SELECT RefMarque FROM Marques WHERE Nom = @nom", conn);
            cmdCheckExistence.Parameters.AddWithValue("@nom", Nom);
            var existingRef = cmdCheckExistence.ExecuteScalar();

            if (existingRef != null) // La marque existe déjà
            {
                ReferenceMarque = Convert.ToInt32(existingRef); // Utiliser la référence existante
            }
            else // La marque n'existe pas encore, il faut l'insérer
            {
                using (var transaction = conn.BeginTransaction())
                {
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO Marques (Nom) VALUES (@Nom); SELECT last_insert_rowid();", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                        ReferenceMarque = Convert.ToInt32(cmdInsert.ExecuteScalar());
                    }
                    transaction.Commit();
                }
            }
        }
    }
}
