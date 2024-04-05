using System;
using System.Data.SQLite;

namespace WindowsFormsApp1
{
    class SousFamille
    {
        public int ReferenceSousFamille { get; set; }
        public string Nom { get; set; }
        public int RefFamille { get; set; }

        /// <summary>
        /// Methode permettant d'ajouter une sous famille dans la base de donnees
        /// </summary>
        /// <param name="conn"></param>
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

                        // Si ReferenceSousFamille est toujours égal à 0, cela signifie qu'aucune insertion n'a été effectuée
                        // Nous devons alors obtenir la nouvelle référence de sous-famille
                        if (ReferenceSousFamille == 0)
                        {
                            using (var cmdLastRowId = new SQLiteCommand("SELECT last_insert_rowid();", conn))
                            {
                                ReferenceSousFamille = Convert.ToInt32(cmdLastRowId.ExecuteScalar()) + 1;
                            }
                        }
                        else
                        {
                            // Sous-famille existante : effectue une mise à jour
                            using (var cmdUpdate = new SQLiteCommand("UPDATE SousFamilles SET Nom = @Nom, RefFamille = @RefFamille WHERE RefSousFamille = @RefSousFamille", conn))
                            {
                                cmdUpdate.Parameters.AddWithValue("@Nom", Nom);
                                cmdUpdate.Parameters.AddWithValue("@RefFamille", RefFamille);
                                cmdUpdate.Parameters.AddWithValue("@RefSousFamille", ReferenceSousFamille);
                                cmdUpdate.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
        }

       

        /// <summary>
        /// Methode permettatn de retrouver la referene de la sous famille en focntion de son nom
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
