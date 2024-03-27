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
                var args = new ImportArguments { Mode = "Ajouter", FilePath = txtCheminFichier.Text };
                backgroundWorker.RunWorkerAsync(args);
            }
        }

        private void BtnEcraser_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                var args = new ImportArguments { Mode = "Ecraser", FilePath = txtCheminFichier.Text };
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
            // Vérifie si l'article existe déjà pour déterminer s'il faut insérer ou mettre à jour
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

        
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show($"Une erreur est survenue : {e.Error.Message}");
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("L'opération a été annulée.");
            }
            else
            {
                var result = e.Result as ImportResult;
                MessageBox.Show($"Importation terminée. {result.Count} articles ont été traités.");
            }
            progressBar.Value = 0;
        }


        /*private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
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
                if (mode == "Ecraser")
                {
                    // Suppression des données existantes si mode "Ecraser"
                    ClearDatabase(conn);
                }

                using (var transaction = conn.BeginTransaction())
                {
                    int totalLines = File.ReadLines(filePath).Count() - 1; // Exclue l'en-tête
                    using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
                    {
                        sr.ReadLine(); // Saute l'en-tête
                        string currentLine;
                        int currentindex = 0;

                        // Boucle while qui va parcourrir chaque ligne du fichier csv
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            var fields = currentLine.Split(';'); // le séparateur est ";"

                            
                            
                            var description = fields[0];
                            var refArticle = fields[1];
                            var marque = fields[2];
                            var famille = fields[3];
                            var sousFamille = fields[4];
                            var prixHT = float.Parse(fields[5]);

                            int refMarque = GetOrInsertMarque(conn, marque);
                            int refFamille = GetOrInsertFamille(conn, famille);
                            int refSousFamille = GetOrInsertSousFamille(conn, refFamille, sousFamille);

                            InsertOrUpdateArticle(conn, refArticle, description, refSousFamille, refMarque, prixHT);
                            int percentComplete = (int)((float)currentindex / totalLines * 100);
                            currentindex++;
                            backgroundWorker.ReportProgress(percentComplete);
                        }
                        e.Result = new ImportResult
                        {
                            Count = currentindex,

                        };
                    }
                    transaction.Commit();
                }
                conn.Close();
            }


        }*/
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
                if (mode == "Ecraser")
                {
                    // Suppression des données existantes si mode "Ecraser"
                    ClearDatabase(conn);
                }

                using (var transaction = conn.BeginTransaction())
                {
                    int totalLines = File.ReadLines(filePath).Count() - 1; // Exclue l'en-tête
                    using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
                    {
                        sr.ReadLine(); // Saute l'en-tête
                        string currentLine;
                        int currentindex = 0;

                        // Boucle while qui va parcourir chaque ligne du fichier csv
                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            var fields = currentLine.Split(';'); // le séparateur est ";"

                            var description = fields[0];
                            var refArticle = fields[1];
                            var marque = fields[2];
                            var famille = fields[3];
                            var sousFamille = fields[4];
                            var prixHT = float.Parse(fields[5]);

                            // Récupération ou insertion de la marque
                            int refMarque = GetOrInsertMarque(conn, marque);

                            // Récupération ou insertion de la famille
                            int refFamille = GetOrInsertFamille(conn, famille);

                            // Récupération ou insertion de la sous-famille
                            int refSousFamille = GetOrInsertSousFamille(conn, refFamille, sousFamille);

                            // Insertion ou mise à jour de l'article
                            InsertOrUpdateArticle(conn, refArticle, description, refSousFamille, refMarque, prixHT);

                            int percentComplete = (int)((float)currentindex / totalLines * 100);
                            currentindex++;
                            backgroundWorker.ReportProgress(percentComplete);
                        }
                        e.Result = new ImportResult
                        {
                            Count = currentindex,
                        };
                    }
                    transaction.Commit();
                }
                conn.Close();
            }
        }


        // Méthode pour supprimer toutes les données de la base de données
        private void ClearDatabase(SQLiteConnection conn)
        {
            var cmdDelete = new SQLiteCommand("DELETE FROM Articles; DELETE FROM Familles; DELETE FROM SousFamilles; DELETE FROM Marques;", conn);
            cmdDelete.ExecuteNonQuery();
        }
    }

    public class ImportArguments
        {
            public string FilePath { get; set; }
            public string Mode { get; set; } // "Ajouter" ou "Ecraser"
        }

        public class ImportResult
        {
            public int Count { get; set; }
            // C'est ici qu'on peut ajouter plus de détails pour le résultat de l'importation
        }

    }
