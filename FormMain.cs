
using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace Hector
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FormMain : Form
    {
        private string dbPath;
        private string connectionString;

        public FormMain()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            dbPath = Path.Combine(appPath, "Data", "Hector.SQLite");
            connectionString = $"Data Source={dbPath};Version=3;";
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadTreeViewData();
        }

        private void importerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormImport formImport = new FormImport();
            formImport.ShowDialog(this);
        }

        private void exporterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Save as CSV File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                GestionExport.ExportDataToCSV(filePath, connectionString);
                MessageBox.Show("Export réussi avec succès");
            }
        }

        private void LoadTreeViewData()
        {
            TreeNode allArticlesNode = new TreeNode("Tous les articles");
            TreeNode familiesNode = new TreeNode("Familles");
            TreeNode brandsNode = new TreeNode("Marques");

            treeView.Nodes.Add(allArticlesNode);
            treeView.Nodes.Add(familiesNode);
            treeView.Nodes.Add(brandsNode);

            GestionLoad.LoadFamilies(familiesNode, connectionString);
            GestionLoad.LoadBrands(brandsNode, connectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Text == "Tous les articles")
            {
                GestionLoad.LoadAllArticles(listView, connectionString);
            }
            else if (e.Node.Parent != null && e.Node.Parent.Text == "Familles")
            {
                // Si un nœud "Familles" est sélectionné
                string famille = e.Node.Text;
                GestionLoad.LoadSousFamillesByFamille(listView, connectionString, famille);
            }
            else if (e.Node.Parent != null && e.Node.Parent.Text == "Marques")
            {
                // Si un nœud "Marques" est sélectionné
                string marque = e.Node.Text;
                GestionLoad.LoadArticlesByMarque(listView, connectionString, marque);
            }
            else if (e.Node.Parent != null && e.Node.Parent.Parent != null && e.Node.Parent.Parent.Text == "Familles")
            {
                // Si un nœud "Sous-Familles" est sélectionné
                string sousFamille = e.Node.Text;
                GestionLoad.LoadArticlesBySousFamille(listView, connectionString, sousFamille);
            }
            else if (e.Node.Text == "Familles")
            {
                GestionLoad.LoadFamilleListView(listView, connectionString);
            }
            else if (e.Node.Text == "Marques" && e.Node.Parent == null)
            {
                GestionLoad.LoadMarqueListView(listView, connectionString);
            }
        }
        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            switch (e.Column)
            {
                case 0: // Colonne "Description"
                    GestionGroupes.GroupItemsByFirstLetter(listView);
                    break;
                case 3: // Colonne "Familles"
                    GestionGroupes.GroupItemsByFamille(listView);
                    break;
                case 4: // Colonne "Sous-Familles"
                    GestionGroupes.GroupItemsBySousFamille(listView);
                    break;
                case 2: // Colonne "Marque"
                    GestionGroupes.GroupItemsByMarque(listView);
                    break;
                default:
                    // Ne rien faire pour les autres colonnes ou en cas de colonne non gérée
                    break;
            }
        }
        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                if (listView.SelectedItems.Count > 0)
                {
                    FormModifier formModif = new FormModifier(listView);
                    formModif.ShowDialog(this);
                }
            }
        }

        // Méthode pour gérer l'événement DoubleClick
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            OuvrirFenetreModification();

        }

        // Méthode pour ouvrir la fenêtre de modification de l'élément
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="refArticle"></param>
        /// <param name="marque"></param>
        /// <param name="famille"></param>
        /// <param name="sousFamille"></param>
        /// <param name="prixHT"></param>
        /// <param name="quantite"></param>
        private void OuvrirFenetreModification()
        {
            ListViewItem selectedItem = listView.SelectedItems[0];
            int nbrColonnes = selectedItem.SubItems.Count;
            if (listView.SelectedItems.Count > 0 && nbrColonnes == 7)
            {


                // Récupérer les valeurs de chaque colonne de l'élément sélectionné
                string description = selectedItem.SubItems[0].Text;
                string refArticle = selectedItem.SubItems[1].Text;
                string marque = selectedItem.SubItems[2].Text;
                string famille = selectedItem.SubItems[3].Text;
                string sousFamille = selectedItem.SubItems[4].Text;
                string prixHT = selectedItem.SubItems[5].Text;
                string quantite = selectedItem.SubItems[6].Text; // Si vous avez une colonne pour la quantité

                // Ouvrir la fenêtre de modification avec les valeurs récupérées
                FormModifier formModif = new FormModifier(listView);
                // Passer les valeurs à la fenêtre de modification
                formModif.RemplirChamps(description, refArticle, marque, famille, sousFamille, prixHT, quantite);
                formModif.ShowDialog(this);
            }
            GestionLoad.LoadAllArticles(listView, connectionString);


            if (nbrColonnes == 1)
            {
                string description = selectedItem.Text;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                treeView.Nodes.Clear();
                LoadTreeViewData();

            }


            if (e.KeyCode == Keys.Delete)
            {
                if (listView.SelectedItems.Count > 0)
                {
                    GestionSuppression.SupprimerElement(listView,connectionString);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void actualiserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView.Nodes.Clear();
            LoadTreeViewData();
        }
        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {

        }
     
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Désactiver tous les éléments du menu par défaut
            ajouterToolStripMenuItem.Enabled = false;
            supprimerToolStripMenuItem.Enabled = false;
            modifierToolStripMenuItem.Enabled = false;

            // Vérifier s'il y a des éléments sélectionnés dans le ListView
            if (listView.SelectedItems.Count == 1 )
            {
                // Activer l'option de suppression et de modification car au moins un élément est sélectionné
                supprimerToolStripMenuItem.Enabled = true;
                modifierToolStripMenuItem.Enabled = true;
            }
            else
            {
                // Aucun élément sélectionné, donc désactiver les options de suppression et de modification
                supprimerToolStripMenuItem.Enabled = false;
                modifierToolStripMenuItem.Enabled = false;
            }

            // L'option d'ajout est toujours activée
            ajouterToolStripMenuItem.Enabled = true;
        }

        private void ajouterToolStripMenuItem_Click(object sender, EventArgs e)
        {

           
        }

        private void supprimerToolStripMenuItem_Click(object sender, EventArgs e)
        {


            GestionSuppression.SupprimerElement(listView,connectionString);

        }

        private void modifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OuvrirFenetreModification();
        }

        private void articleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAjout formAjout = new FormAjout(listView); // Passer une référence à listView
            formAjout.ShowDialog();
        }

        private void familleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void sousFamilleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAjoutSousFamille formAjoutSF = new FormAjoutSousFamille(listView);
            formAjoutSF.ShowDialog();
        }

        private void marqueToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
