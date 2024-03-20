using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppliaction
{

    public partial class Form1 : Form
    {
        private string cheminFichier = "";
        private Boolean estModifie = false;
        private DateTime derniereModificationLocale;
        public Form1()
        {
            InitializeComponent();
            // Attachez la fonction Form_Load à l'événement Load du formulaire
            this.Load += Form1_Load;
            // Attachez la fonction Form_FormClosing à l'événement FormClosing du formulaire
            this.FormClosing += Form1_FormClosing;
        }

        /// <summary>
        /// Fonction qui permet l'enregistrement de la text box dans un fichier .txt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Charger les paramètres enregistrés
            if (Properties.Settings.Default.Taille != Size.Empty)
            {
                this.Size = Properties.Settings.Default.Taille;
            }

            if (Properties.Settings.Default.Position != Point.Empty)
            {
                this.Location = Properties.Settings.Default.Position;
            }

            this.WindowState = Properties.Settings.Default.EtatAgrandi;

            // Assurer que la fenêtre est entièrement visible à l'écran
            Screen screen = Screen.FromPoint(this.Location);
            Rectangle workingArea = screen.WorkingArea;
           /* if (!workingArea.Contains(this.Bounds))
            {
                this.Location = new Point(Math.Max(workingArea.Left, Math.Min(this.Location.X, workingArea.Right - this.Width))),
                                 Math.Max(workingArea.Top, Math.Min(this.Location.Y, workingArea.Bottom - this.Height)));
            }*/
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Enregistrer les paramètres lorsque la fenêtre est fermée
            Properties.Settings.Default.Taille = this.Size;
            Properties.Settings.Default.Position = this.Location;
            Properties.Settings.Default.EtatAgrandi = this.WindowState;
            Properties.Settings.Default.Save();
        }

        private void ChargerContenuFichier()
        {
            // Charger le contenu du fichier dans la TextBox
            textBox1.Text = File.ReadAllText(cheminFichier);

            // Mettre à jour la date de dernière modification locale
            derniereModificationLocale = File.GetLastWriteTime(cheminFichier);
        }
        private void enregistrerSousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Créer une instance de SaveFileDialog
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Configurer le filtre de fichiers pour n'afficher que les fichiers texte (*.txt)
                saveFileDialog.Filter = "Fichiers texte (*.txt)|*.txt|Tous les fichiers (*.*)|*.*";
                saveFileDialog.FilterIndex = 1; // L'index du filtre par défaut

                // Afficher la boîte de dialogue de sélection de fichier (mode enregistrement)
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // L'utilisateur a sélectionné un fichier

                    // Récupérer le chemin complet du fichier
                    string cheminFichier = saveFileDialog.FileName;

                    // Vérifier si le fichier existe déjà
                    if (File.Exists(cheminFichier))
                    {
                        // Demander confirmation pour écraser le fichier existant
                        DialogResult result = MessageBox.Show("Le fichier existe déjà. Voulez-vous le remplacer ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.No)
                        {
                            return; // Annuler l'enregistrement
                        }
                    }

                    // Enregistrer le contenu du TextBox dans le fichier sélectionné
                    try
                    {
                        File.WriteAllText(cheminFichier, textBox1.Text);
                        MessageBox.Show("Enregistrement réussi.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        string titre = "Éditeur de fichier texte ";

                        titre += "[" + Path.GetFileName(cheminFichier) + "]";

                        this.Text = titre;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur s'est produite lors de l'enregistrement du fichier : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// Fonction qui permet d'ouvrir un fichier .txt dans l'editeur de texte depuis l'explorateur de fichier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ouvrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Afficher la boîte de dialogue pour la sélection de fichier
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Fichiers texte (*.txt)|*.txt|Tous les fichiers (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Charger le contenu du fichier dans le TextBox
                    string cheminFichier = openFileDialog.FileName;
                    //trandformer le chemin en le nom du fichier
                    string titre = "Éditeur de fichier texte ";

                    titre += "[" + Path.GetFileName(cheminFichier) + "]";

                    this.Text = titre;
                    try
                    {
                        string contenuFichier = File.ReadAllText(cheminFichier);
                        textBox1.Text = contenuFichier;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur s'est produite lors de la lecture du fichier : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
        }
        private void MettreAJourTitreApplication()
        {
            // Mettre à jour le titre de l'application avec le nom du fichier et l'étoile si nécessaire
            string titre = this.Text;
            char dernierChar = titre[titre.Length - 1];
            if (estModifie && dernierChar != '*')
            {
                titre += " *";
            }
            this.Text = titre;
        }
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            estModifie = true;
            MettreAJourTitreApplication();
                
        }
    }
}
