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

            using (var transaction = conn.BeginTransaction())
            {
                if (ReferenceSousFamille == 0)
                {
                    // Nouvelle sous-famille : effectue une insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO SousFamilles (Nom, RefFamille) VALUES (@Nom, @RefFamille)", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@Nom", Nom);
                        cmdInsert.Parameters.AddWithValue("@RefFamille", RefFamille);
                        cmdInsert.ExecuteNonQuery();
                    }
                    // Récupère la référence de la sous-famille nouvellement insérée
                    using (var cmdGetLastId = new SQLiteCommand("SELECT last_insert_rowid()", conn))
                    {
                        ReferenceSousFamille = Convert.ToInt32(cmdGetLastId.ExecuteScalar());
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

        private SousFamille GetSousFamilleByReference(SQLiteConnection conn, int reference)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT * FROM SousFamilles WHERE ReferenceSousFamille = @ReferenceSousFamille", conn))
            {
                cmdCheck.Parameters.AddWithValue("@ReferenceSousFamille", reference);
                using (var reader = cmdCheck.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SousFamille
                        {
                            ReferenceSousFamille = Convert.ToInt32(reader["ReferenceSousFamille"]),
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
