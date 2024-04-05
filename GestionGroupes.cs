using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    /// <summary>
    /// CLasse qui s'occupe de la gestion des groupes c'est à dire la facond e trier  les colonnes dans lsit view
    /// </summary>
    class GestionGroupes
    {
        /// <summary>
        /// Tri en focntion de la premiere lettre dans le lsitView
        /// </summary>
        /// <param name="listView"></param>
        public static void GroupItemsByFirstLetter(ListView listView)
        {
            // Efface tous les groupes précédemment définis dans le ListView
            listView.Groups.Clear();

            // Crée un dictionnaire pour stocker les groupes par leur première lettre
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            // Parcours chaque élément dans la ListView
            foreach (ListViewItem item in listView.Items)
            {
                // Récupère la première lettre du texte de l'élément et la met en majuscule
                string firstLetter = item.Text.Substring(0, 1).ToUpper();

                // Vérifie si le groupe pour cette première lettre n'existe pas déjà
                if (!groups.ContainsKey(firstLetter))
                {
                    // Si le groupe n'existe pas, crée un nouveau ListViewGroup avec cette première lettre comme nom
                    ListViewGroup group = new ListViewGroup(firstLetter);

                    // Ajoute le groupe à la ListView
                    listView.Groups.Add(group);

                    // Ajoute également le groupe au dictionnaire pour une référence facile
                    groups.Add(firstLetter, group);
                }

                // Définit le groupe de l'élément actuel sur le groupe correspondant à sa première lettre
                item.Group = groups[firstLetter];
            }
        }
        /// <summary>
        /// Tri les donnes du listView par Familles
        /// </summary>
        /// <param name="listView"></param>
        public static void GroupItemsByFamille(ListView listView)
        {
            // Efface tous les groupes précédemment définis dans le ListView
            listView.Groups.Clear();

            // Crée un dictionnaire pour stocker les groupes par leur nom de famille
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            // Parcours chaque élément dans la ListView
            foreach (ListViewItem item in listView.Items)
            {
                // Récupère le nom de famille à partir de la sous-colonne d'index 3 (colonnes indexées à partir de 0)
                string famille = item.SubItems[3].Text;

                // Vérifie si le groupe pour cette famille n'existe pas déjà
                if (!groups.ContainsKey(famille))
                {
                    // Si le groupe n'existe pas, crée un nouveau ListViewGroup avec ce nom de famille
                    ListViewGroup group = new ListViewGroup(famille);

                    // Ajoute le groupe à la ListView
                    listView.Groups.Add(group);

                    // Ajoute également le groupe au dictionnaire pour une référence facile
                    groups.Add(famille, group);
                }

                // Définit le groupe de l'élément actuel sur le groupe correspondant à sa famille
                item.Group = groups[famille];
            }
        }




        /// <summary>
        /// Tri les donnees du listView par marque
        /// </summary>
        /// <param name="listView"></param>
        public static void GroupItemsByMarque(ListView listView)
        {
            // Efface tous les groupes précédemment définis dans le ListView
            listView.Groups.Clear();

            // Crée un dictionnaire pour stocker les groupes par leur marque
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            // Parcours chaque élément dans la ListView
            foreach (ListViewItem item in listView.Items)
            {
                // Récupère la marque à partir de la sous-colonne d'index 2 (colonnes indexées à partir de 0)
                string marque = item.SubItems[2].Text;

                // Vérifie si le groupe pour cette marque n'existe pas déjà
                if (!groups.ContainsKey(marque))
                {
                    // Si le groupe n'existe pas, crée un nouveau ListViewGroup avec cette marque
                    ListViewGroup group = new ListViewGroup(marque);

                    // Ajoute le groupe à la ListView
                    listView.Groups.Add(group);

                    // Ajoute également le groupe au dictionnaire pour une référence facile
                    groups.Add(marque, group);
                }

                // Définit le groupe de l'élément actuel sur le groupe correspondant à sa marque
                item.Group = groups[marque];
            }
        }


        /// <summary>
        /// Tri les donnes du lsitView par sousFamille
        /// </summary>
        /// <param name="listView"></param>
        public static void GroupItemsBySousFamille(ListView listView)
        {
            // Efface tous les groupes précédemment définis dans le ListView
            listView.Groups.Clear();

            // Crée un dictionnaire pour stocker les groupes par leur sous-famille
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            // Parcours chaque élément dans la ListView
            foreach (ListViewItem item in listView.Items)
            {
                // Récupère la sous-famille à partir de la sous-colonne d'index 4 (colonnes indexées à partir de 0)
                string sousFamille = item.SubItems[4].Text;

                // Vérifie si le groupe pour cette sous-famille n'existe pas déjà
                if (!groups.ContainsKey(sousFamille))
                {
                    // Si le groupe n'existe pas, crée un nouveau ListViewGroup avec cette sous-famille
                    ListViewGroup group = new ListViewGroup(sousFamille);

                    // Ajoute le groupe à la ListView
                    listView.Groups.Add(group);

                    // Ajoute également le groupe au dictionnaire pour une référence facile
                    groups.Add(sousFamille, group);
                }

                // Définit le groupe de l'élément actuel sur le groupe correspondant à sa sous-famille
                item.Group = groups[sousFamille];
            }
        }

    }
}