﻿
using System;

namespace Hector
{
    partial class FormImport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtCheminFichier = new System.Windows.Forms.TextBox();
            this.btnAjouter = new System.Windows.Forms.Button();
            this.btnEcraser = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectioner = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtCheminFichier
            // 
            this.txtCheminFichier.Location = new System.Drawing.Point(50, 478);
            this.txtCheminFichier.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCheminFichier.Name = "txtCheminFichier";
            this.txtCheminFichier.Size = new System.Drawing.Size(535, 26);
            this.txtCheminFichier.TabIndex = 0;
            // 
            // btnAjouter
            // 
            this.btnAjouter.Location = new System.Drawing.Point(741, 415);
            this.btnAjouter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAjouter.Name = "btnAjouter";
            this.btnAjouter.Size = new System.Drawing.Size(120, 44);
            this.btnAjouter.TabIndex = 1;
            this.btnAjouter.Text = "Ajouter";
            this.btnAjouter.UseVisualStyleBackColor = true;
            this.btnAjouter.Click += new System.EventHandler(this.BtnAjouter_Click);
            // 
            // btnEcraser
            // 
            this.btnEcraser.Location = new System.Drawing.Point(741, 470);
            this.btnEcraser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEcraser.Name = "btnEcraser";
            this.btnEcraser.Size = new System.Drawing.Size(122, 42);
            this.btnEcraser.TabIndex = 2;
            this.btnEcraser.Text = "Ecraser";
            this.btnEcraser.UseVisualStyleBackColor = true;
            this.btnEcraser.Click += new System.EventHandler(this.BtnEcraser_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(14, 530);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(873, 29);
            this.progressBar.TabIndex = 3;
            this.progressBar.Click += new System.EventHandler(this.ProgressBar_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 441);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Importer un fichier csv";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnSelectioner
            // 
            this.btnSelectioner.Location = new System.Drawing.Point(606, 476);
            this.btnSelectioner.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSelectioner.Name = "btnSelectioner";
            this.btnSelectioner.Size = new System.Drawing.Size(115, 36);
            this.btnSelectioner.TabIndex = 5;
            this.btnSelectioner.Text = "Selectioner";
            this.btnSelectioner.UseVisualStyleBackColor = true;
            this.btnSelectioner.Click += new System.EventHandler(this.BtnSelectioner_Click);
            // 
            // FormImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 562);
            this.Controls.Add(this.btnSelectioner);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnEcraser);
            this.Controls.Add(this.btnAjouter);
            this.Controls.Add(this.txtCheminFichier);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormImport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Importer";
            this.Load += new System.EventHandler(this.FormImport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

       


        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtCheminFichier;
        private System.Windows.Forms.Button btnAjouter;
        private System.Windows.Forms.Button btnEcraser;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectioner;
    }
}