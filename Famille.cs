using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    /// <summary>
    /// classe permettatn la gestion des familles , notammetn dans la base de données afin de les ajouter et modifier
    /// </summary>
    class Famille
    {
        public int ReferenceFamille { get; set; }
        public string Nom { get; set; }


        /// <summary>
        /// Methode permettatn d'ajouter ou modifier les familles dans la base de donnes
        /// </summary>
        /// <param name="conn"></param>
        public void InsertOrUpdate(SQLiteConnection conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn));
            }

            using (var transaction = conn.BeginTransaction())
            {


                // Vérifier si la famille existe déjà dans la base de données
                var cmdCheckExistence = new SQLiteCommand("SELECT RefFamille FROM Familles WHERE Nom = @nom", conn);
                cmdCheckExistence.Parameters.AddWithValue("@nom", Nom);
                var existingRef = cmdCheckExistence.ExecuteScalar();

                if (existingRef != null) // La famille existe déjà
                {
                    ReferenceFamille = Convert.ToInt32(existingRef); // Utiliser la référence existante
                }
                else // La famille n'existe pas encore, il faut l'insérer
                {
                    var cmdInsert = new SQLiteCommand("INSERT INTO Familles (Nom) VALUES (@nom); SELECT last_insert_rowid();", conn);
                    cmdInsert.Parameters.AddWithValue("@nom", Nom);
                    ReferenceFamille = Convert.ToInt32(cmdInsert.ExecuteScalar()); // Récupérer la référence de la nouvelle famille
                }


                transaction.Commit();
            }
        }
        public static int GetReferenceFromNom(string nom, string connectionString)
        {
            int reference = 0;

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Exécuter la requête SQL pour récupérer la référence de la famille
                var cmdGetReference = new SQLiteCommand("SELECT RefFamille FROM Familles WHERE Nom = @nom", conn);
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
