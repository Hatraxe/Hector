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
    }
}
