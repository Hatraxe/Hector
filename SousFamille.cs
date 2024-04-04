using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    class SousFamille
    {
        public int ReferenceSousFamille { get; set; }
        public string Nom { get; set; }
        public int RefFamille { get; set; }

        public void InsertOrUpdate(SQLiteConnection conn)
        {
            if (conn == null)
            {
                throw new ArgumentNullException(nameof(conn));
            }

            // Vérifier si la sous-famille existe déjà dans la base de données
            using (var cmdCheck = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE Nom = @Nom", conn))
            {
                cmdCheck.Parameters.AddWithValue("@Nom", Nom);
                var existingReference = cmdCheck.ExecuteScalar();

                if (existingReference != null)
                {
                    // Utiliser la référence de la sous-famille existante
                    ReferenceSousFamille = Convert.ToInt32(existingReference);
                }
                else
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        if (ReferenceSousFamille == 0)
                        {
                            // Nouvelle sous-famille : effectue une insertion
                            using (var cmdInsert = new SQLiteCommand("INSERT INTO SousFamilles (Nom, RefFamille) VALUES (@Nom, @RefFamille); SELECT last_insert_rowid();", conn))
                            {
                                cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                                cmdInsert.Parameters.AddWithValue("@RefFamille", RefFamille);
                                ReferenceSousFamille = Convert.ToInt32(cmdInsert.ExecuteScalar());
                            }
                        }
                        else
                        {
                            // Sous-famille existante : effectue une mise à jour
                            using (var cmdUpdate = new SQLiteCommand("UPDATE SousFamilles SET Nom = @Nom, RefFamille = @RefFamille WHERE ReferenceSousFamille = @ReferenceSousFamille", conn))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Nom", Nom);
                                cmdUpdate.Parameters.AddWithValue("@RefFamille", RefFamille);
                                cmdUpdate.Parameters.AddWithValue("@ReferenceSousFamille", ReferenceSousFamille);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
        }
        public static int GetReferenceFromNom(string nom, string connectionString)
        {
            int reference = 0;

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Exécuter la requête SQL pour récupérer la référence de la sous-famille
                var cmdGetReference = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE Nom = @nom", conn);
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
