using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    class Marque
    {
        public int ReferenceMarque { get; set; }
        public string Nom { get; set; }


        /// <summary>
        /// Permet l'ajout d'une amrque dans la base de donnees
        /// </summary>
        /// <param name="conn"></param>
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
        

          /// <summary>
          /// Pemrmet de recupere la refernece en focntion du nom
          /// </summary>
          /// <param name="nom"></param>
          /// <param name="connectionString"></param>
          /// <returns></returns>
        public static int GetReferenceFromNom(string nom, string connectionString)
        {
            int reference = 0;

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

               
                var cmdGetReference = new SQLiteCommand("SELECT RefMarque FROM Marques WHERE Nom = @nom", conn);
                cmdGetReference.Parameters.AddWithValue("@nom", nom);
                var result = cmdGetReference.ExecuteScalar();
                // Vérifier si une référence a été trouvée
                if (result != null && result != DBNull.Value)
                {
                    reference = Convert.ToInt32(result);
                }
            }

            return reference;
        }

    }
}
