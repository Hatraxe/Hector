using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace Hector
{
    public partial class FormImport : Form
    {
        public FormImport()
        {
            InitializeComponent();
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
                throw new InvalidOperationException("Argument null");
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
                    ClearDatabase(conn);
                }

                using (var transaction = conn.BeginTransaction())
                {
                    int totalLines = File.ReadLines(filePath).Count() - 1;

                    using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8))
                    {
                        sr.ReadLine(); // Skip header
                        string currentLine;
                        int currentindex = 0;

                        while ((currentLine = sr.ReadLine()) != null)
                        {
                            var fields = currentLine.Split(';');

                            var description = fields[0];
                            var refArticle = fields[1];
                            var marque = fields[2];
                            var famille = fields[3];
                            var sousFamille = fields[4];
                            var prixHT = float.Parse(fields[5]);

                            Marque marqueObj = new Marque { Nom = marque };
                            marqueObj.InsertOrUpdate(conn);
                            int refMarque = marqueObj.ReferenceMarque;

                            Famille familleObj = new Famille { Nom = famille };
                            familleObj.InsertOrUpdate(conn);
                            int refFamille = familleObj.ReferenceFamille;

                            SousFamille sousFamilleObj = new SousFamille { Nom = sousFamille, RefFamille = refFamille };
                            sousFamilleObj.InsertOrUpdate(conn);
                            int refSousFamille = sousFamilleObj.ReferenceSousFamille;

                            Article articleObj = new Article(refArticle, description, prixHT, refSousFamille, refMarque);
                            articleObj.InsertOrUpdate(conn);

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

        private void ClearDatabase(SQLiteConnection conn)
        {
            var cmdDelete = new SQLiteCommand("DELETE FROM Articles; DELETE FROM Familles; DELETE FROM SousFamilles; DELETE FROM Marques;", conn);
            cmdDelete.ExecuteNonQuery();
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

        private void FormImport_Load(object sender, EventArgs e)
        {

        }

        private void ProgressBar_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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
}

