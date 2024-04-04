using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class GestionSuppression
    {
        public static void SupprimerElement(ListView listView,string connectionString)
        {
            if (listView.SelectedItems.Count > 0)
            {
                // Récupérer l'élément sélectionné dans le ListView
                ListViewItem selectedItem = listView.SelectedItems[0];

                // Récupérer l'identifiant de l'élément sélectionné (par exemple, la référence de l'article)
                string refArticle = selectedItem.SubItems[1].Text; // Supposons que la référence de l'article soit dans la deuxième colonne

                // Supprimer l'élément du ListView
                listView.Items.Remove(selectedItem);

                // Supprimer l'élément de la base de données
                SupprimerElementBaseDeDonnees(refArticle,connectionString);
            }
        }

        private static  void SupprimerElementBaseDeDonnees(string refArticle,string connectionString)
        {
            // Établir une connexion à la base de données
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Définir la commande SQL de suppression
                string query = "DELETE FROM Articles WHERE RefArticle = @refArticle";

                // Créer et paramétrer la commande
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@refArticle", refArticle);

                    // Exécuter la commande de suppression
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Vérifier si la suppression a réussi
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("L'élément a été supprimé avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("La suppression de l'élément a échoué.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
