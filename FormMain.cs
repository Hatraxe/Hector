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
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

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

                // Logique pour exporter les données de la base de données vers le fichier CSV
                ExportDataToCSV(filePath);

               
            }
        }

        private void ExportDataToCSV(string filePath)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(appPath, "Data", "Hector.SQLite");
            string connectionString = $"Data Source={dbPath};Version=3;";

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand("SELECT Description, RefArticle, RefMarque, RefSousFamille, PrixHT FROM Articles", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
                        {
                            file.WriteLine("Description;Ref;Marque;Famille;Sous-Famille;Prix H.T.");

                            while (reader.Read())
                            {
                                string description = reader["Description"].ToString();
                                string refArticle = reader["RefArticle"].ToString();
                                string refMarque = reader["RefMarque"].ToString();
                                string refSousFamille = reader["RefSousFamille"].ToString();
                                string prixHT = reader["PrixHT"].ToString();

                                // Remplacez les valeurs de RefMarque et RefSousFamille par les valeurs de Famille et SousFamille si nécessaire
                                string line = $"{description};{refArticle};{refMarque};{refSousFamille};{prixHT}";
                                file.WriteLine(line);
                            }
                        }
                    }
                }

                conn.Close();
            }

            MessageBox.Show("Export réussi avec succès");
        }


    }
}

