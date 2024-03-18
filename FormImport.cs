using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
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
                var args = new { Mode = "Add", FilePath = txtCheminFichier.Text };
                backgroundWorker.RunWorkerAsync(args);
            }
        }

        private void BtnEcraser_Click(object sender, EventArgs e)
        {

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
                    Filter = "CSV files (.csv)|.csv|All files (.)|.",
                    Title = "Select a CSV File"
             };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtCheminFichier.Text = openFileDialog.FileName;
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Importation terminée.");
        }
    }
}
