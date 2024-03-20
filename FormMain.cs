using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

           /* OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Excel CSV files (*.CSV)|*.CSV|All files (*.*)|*.*",
                Title = "Select an CSV File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtCheminFichier.Text = openFileDialog.FileName;
            }*/
        }
    }
}
