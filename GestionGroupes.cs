using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class GestionGroupes
    {

        public static void GroupItemsByFirstLetter(ListView listView)
        {
            listView.Groups.Clear();

            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            foreach (ListViewItem item in listView.Items)
            {
                string firstLetter = item.Text.Substring(0, 1).ToUpper();
                if (!groups.ContainsKey(firstLetter))
                {
                    ListViewGroup group = new ListViewGroup(firstLetter);
                    listView.Groups.Add(group);
                    groups.Add(firstLetter, group);
                }
                item.Group = groups[firstLetter];
            }
        }

        public static void GroupItemsByFamille(ListView listView)
        {
            listView.Groups.Clear();

            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            foreach (ListViewItem item in listView.Items)
            {
                string famille = item.SubItems[3].Text; // Index de la colonne Famille
                if (!groups.ContainsKey(famille))
                {
                    ListViewGroup group = new ListViewGroup(famille);
                    listView.Groups.Add(group);
                    groups.Add(famille, group);
                }
                item.Group = groups[famille];
            }
        }

        // Autres méthodes de regroupement...

        // Exemple de méthode pour regrouper par une autre colonne
        public static void GroupItemsByMarque(ListView listView)
        {
            listView.Groups.Clear();

            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            foreach (ListViewItem item in listView.Items)
            {
                string marque = item.SubItems[2].Text; // Index de la colonne Marque
                if (!groups.ContainsKey(marque))
                {
                    ListViewGroup group = new ListViewGroup(marque);
                    listView.Groups.Add(group);
                    groups.Add(marque, group);
                }
                item.Group = groups[marque];
            }
        }

        // Méthode pour regrouper par une colonne contenant une sous-famille, etc.
    }
}