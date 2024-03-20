using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hector
{


   
    public partial class FormImport : Form
    {
       

       
        public FormImport()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                var args = new ImportArguments { Mode = "Add", FilePath = txtCheminFichier.Text };
                backgroundWorker.RunWorkerAsync(args);
            }
        }

        private void BtnEcraser_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                var args = new ImportArguments { Mode = "Overwrite", FilePath = txtCheminFichier.Text };
                backgroundWorker.RunWorkerAsync(args);
            }
        }

        private void FormImport_Load(object sender, EventArgs e)
        {

        }

        private void ProgressBar_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void BtnSelectioner_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Excel CSV files (*.CSV)|*.CSV|All files (*.*)|*.*",
                Title = "Select an CSV File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtCheminFichier.Text = openFileDialog.FileName;
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as ImportArguments;
            if (args == null)
            {
                throw new InvalidOperationException("Argument nul");
            }
            string filePath = args.FilePath;
            string mode = args.Mode;

            string appPath = AppDomain.CurrentDomain.BaseDirectory;


            string dbPath = Path.Combine(appPath, "Data", "Hector.SQLite");

            string connectionString = $"Data Source={dbPath};Version=3;";

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                if (mode == "Overwrite")
                {
                    var cmdDelete = new SQLiteCommand("DELETE FROM Articles; DELETE FROM Familles; DELETE FROM SousFamilles; DELETE FROM Marques;", conn);
                    cmdDelete.ExecuteNonQuery();
                }

                using (var transaction = conn.BeginTransaction())
                {
                    int totalLines = File.ReadLines(filePath).Count() - 1; // Exclure l'en-tête
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        sr.ReadLine(); // Sauter l'en-tête
                        string currentLine;
                        int currentindex = 0;
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            var fields = currentLine.Split(';');
                            // Adaptez les indices selon l'organisation de votre fichier CSV
                            var marque = fields[2];
                            var famille = fields[3];
                            var sousFamille = fields[4];
                            var description = fields[0];
                            var refArticle = fields[1];
                            var prixHT = float.Parse(fields[5]);

                            int refMarque = GetOrInsertMarque(conn, marque);
                            int refFamille = GetOrInsertFamille(conn, famille);
                            int refSousFamille = GetOrInsertSousFamille(conn, refFamille, sousFamille);

                            InsertOrUpdateArticle(conn, refArticle, description, refSousFamille, refMarque, prixHT);
                            currentindex++;
                            Console.Out.Write(currentindex);
                        }
                    }
                    transaction.Commit();
                }
                conn.Close();
            }
        }
        private int GetOrInsertMarque(SQLiteConnection conn, string nomMarque)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT RefMarque FROM Marques WHERE Nom = @Nom", conn))
            {
                cmdCheck.Parameters.AddWithValue("@Nom", nomMarque);
                var result = cmdCheck.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
            }

            using (var cmdInsert = new SQLiteCommand("INSERT INTO Marques (Nom) VALUES (@Nom); SELECT last_insert_rowid();", conn))
            {
                cmdInsert.Parameters.AddWithValue("@Nom", nomMarque);
                return Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
        }
      

        private int GetOrInsertFamille(SQLiteConnection conn, string nomFamille)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT RefFamille FROM Familles WHERE Nom = @Nom", conn))
            {
                cmdCheck.Parameters.AddWithValue("@Nom", nomFamille);
                var result = cmdCheck.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
            }

            using (var cmdInsert = new SQLiteCommand("INSERT INTO Familles (Nom) VALUES (@Nom); SELECT last_insert_rowid();", conn))
            {
                cmdInsert.Parameters.AddWithValue("@Nom", nomFamille);
                return Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
        }
        
        private int GetOrInsertSousFamille(SQLiteConnection conn, int refFamille, string nomSousFamille)
        {
            using (var cmdCheck = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE Nom = @Nom AND RefFamille = @RefFamille", conn))
            {
                cmdCheck.Parameters.AddWithValue("@Nom", nomSousFamille);
                cmdCheck.Parameters.AddWithValue("@RefFamille", refFamille);
                var result = cmdCheck.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
            }

            using (var cmdInsert = new SQLiteCommand("INSERT INTO SousFamilles (RefFamille, Nom) VALUES (@RefFamille, @Nom); SELECT last_insert_rowid();", conn))
            {
                cmdInsert.Parameters.AddWithValue("@RefFamille", refFamille);
                cmdInsert.Parameters.AddWithValue("@Nom", nomSousFamille);
                return Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
        }
        private void InsertOrUpdateArticle(SQLiteConnection conn, string refArticle, string description, int refSousFamille, int refMarque, float prixHT)
        {
            // Vérifiez si l'article existe déjà pour déterminer s'il faut insérer ou mettre à jour
            using (var cmdCheck = new SQLiteCommand("SELECT RefArticle FROM Articles WHERE RefArticle = @RefArticle", conn))
            {
                cmdCheck.Parameters.AddWithValue("@RefArticle", refArticle);
                var exists = cmdCheck.ExecuteScalar() != null;

                if (!exists)
                {
                    // Insertion
                    using (var cmdInsert = new SQLiteCommand("INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@RefArticle, @Description, @RefSousFamille, @RefMarque, @PrixHT, 0)", conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@RefArticle", refArticle);
                        cmdInsert.Parameters.AddWithValue("@Description", description);
                        cmdInsert.Parameters.AddWithValue("@RefSousFamille", refSousFamille);
                        cmdInsert.Parameters.AddWithValue("@RefMarque", refMarque);
                        cmdInsert.Parameters.AddWithValue("@PrixHT", prixHT);
                        cmdInsert.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Mise à jour
                    using (var cmdUpdate = new SQLiteCommand("UPDATE Articles SET Description = @Description, RefSousFamille = @RefSousFamille, RefMarque = @RefMarque, PrixHT = @PrixHT WHERE RefArticle = @RefArticle", conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@RefArticle", refArticle);
                        cmdUpdate.Parameters.AddWithValue("@Description", description);
                        cmdUpdate.Parameters.AddWithValue("@RefSousFamille", refSousFamille);
                        cmdUpdate.Parameters.AddWithValue("@RefMarque", refMarque);
                        cmdUpdate.Parameters.AddWithValue("@PrixHT", prixHT);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
            }
        }

        private int EnsureFamilleExists(SQLiteConnection conn, string nomFamille)
        {
            var cmdCheck = new SQLiteCommand("SELECT RefFamille FROM Familles WHERE Nom = @Nom", conn);
            cmdCheck.Parameters.AddWithValue("@Nom", nomFamille);
            var result = cmdCheck.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                var cmdInsert = new SQLiteCommand("INSERT INTO Familles (Nom) VALUES (@Nom); SELECT last_insert_rowid();", conn);
                cmdInsert.Parameters.AddWithValue("@Nom", nomFamille);
                return Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
        }
        private int EnsureMarqueExists(SQLiteConnection conn, string nomMarque)
        {
            var cmdCheck = new SQLiteCommand("SELECT RefMarque FROM Marques WHERE Nom = @Nom", conn);
            cmdCheck.Parameters.AddWithValue("@Nom", nomMarque);
            var result = cmdCheck.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                var cmdInsert = new SQLiteCommand("INSERT INTO Marques (Nom) VALUES (@Nom); SELECT last_insert_rowid();", conn);
                cmdInsert.Parameters.AddWithValue("@Nom", nomMarque);
                return Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
        }
        private int EnsureSousFamilleExists(SQLiteConnection conn, int refFamille, string nomSousFamille)
        {
            // Vérifier si la sous-famille existe déjà
            var cmdCheck = new SQLiteCommand("SELECT RefSousFamille FROM SousFamilles WHERE Nom = @Nom AND RefFamille = @RefFamille", conn);
            cmdCheck.Parameters.AddWithValue("@Nom", nomSousFamille);
            cmdCheck.Parameters.AddWithValue("@RefFamille", refFamille);
            var result = cmdCheck.ExecuteScalar();

            if (result != null)
            {
                // Si la sous-famille existe, retourner son ID
                return Convert.ToInt32(result);
            }
            else
            {
                // Si la sous-famille n'existe pas, l'insérer et retourner le nouvel ID
                var cmdInsert = new SQLiteCommand("INSERT INTO SousFamilles (RefFamille, Nom) VALUES (@RefFamille, @Nom); SELECT last_insert_rowid();", conn);
                cmdInsert.Parameters.AddWithValue("@RefFamille", refFamille);
                cmdInsert.Parameters.AddWithValue("@Nom", nomSousFamille);
                return Convert.ToInt32(cmdInsert.ExecuteScalar());
            }
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        public class ImportArguments
        {
            public string FilePath { get; set; }
            public string Mode { get; set; } // "Add" ou "Overwrite"
        }
    }
}