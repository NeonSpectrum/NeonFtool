
using System.Drawing;

namespace NeonFtool.Forms
{
    partial class DumpCleanerForm
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
            this.txtCheckBox = new System.Windows.Forms.CheckBox();
            this.dmpCheckBox = new System.Windows.Forms.CheckBox();
            this.rptCheckBox = new System.Windows.Forms.CheckBox();
            this.cleanDumpButton = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.executeStartupCheckBox = new System.Windows.Forms.CheckBox();
            this.insanityFlyffPathTextBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // txtCheckBox
            // 
            this.txtCheckBox.AutoSize = true;
            this.txtCheckBox.Location = new System.Drawing.Point(246, 12);
            this.txtCheckBox.Name = "txtCheckBox";
            this.txtCheckBox.Size = new System.Drawing.Size(40, 17);
            this.txtCheckBox.TabIndex = 3;
            this.txtCheckBox.Text = ".txt";
            this.txtCheckBox.UseVisualStyleBackColor = true;
            // 
            // dmpCheckBox
            // 
            this.dmpCheckBox.AutoSize = true;
            this.dmpCheckBox.Location = new System.Drawing.Point(191, 12);
            this.dmpCheckBox.Name = "dmpCheckBox";
            this.dmpCheckBox.Size = new System.Drawing.Size(49, 17);
            this.dmpCheckBox.TabIndex = 4;
            this.dmpCheckBox.Text = ".dmp";
            this.dmpCheckBox.UseVisualStyleBackColor = true;
            // 
            // rptCheckBox
            // 
            this.rptCheckBox.AutoSize = true;
            this.rptCheckBox.Location = new System.Drawing.Point(144, 12);
            this.rptCheckBox.Name = "rptCheckBox";
            this.rptCheckBox.Size = new System.Drawing.Size(41, 17);
            this.rptCheckBox.TabIndex = 1;
            this.rptCheckBox.Text = ".rpt";
            this.rptCheckBox.UseVisualStyleBackColor = true;
            // 
            // cleanDumpButton
            // 
            this.cleanDumpButton.Location = new System.Drawing.Point(359, 35);
            this.cleanDumpButton.Name = "cleanDumpButton";
            this.cleanDumpButton.Size = new System.Drawing.Size(75, 23);
            this.cleanDumpButton.TabIndex = 5;
            this.cleanDumpButton.Text = "Clean";
            this.cleanDumpButton.UseVisualStyleBackColor = true;
            this.cleanDumpButton.Click += new System.EventHandler(this.cleanDumpButton_Click);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(278, 35);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // executeStartupCheckBox
            // 
            this.executeStartupCheckBox.AutoSize = true;
            this.executeStartupCheckBox.Location = new System.Drawing.Point(12, 12);
            this.executeStartupCheckBox.Name = "executeStartupCheckBox";
            this.executeStartupCheckBox.Size = new System.Drawing.Size(117, 17);
            this.executeStartupCheckBox.TabIndex = 0;
            this.executeStartupCheckBox.Text = "Execute on Startup";
            this.executeStartupCheckBox.UseVisualStyleBackColor = true;
            this.executeStartupCheckBox.CheckedChanged += new System.EventHandler(this.executeStartupCheckBox_CheckedChanged);
            // 
            // insanityFlyffPathTextBox
            // 
            this.insanityFlyffPathTextBox.Location = new System.Drawing.Point(12, 37);
            this.insanityFlyffPathTextBox.Name = "insanityFlyffPathTextBox";
            this.insanityFlyffPathTextBox.ReadOnly = true;
            this.insanityFlyffPathTextBox.Size = new System.Drawing.Size(260, 20);
            this.insanityFlyffPathTextBox.TabIndex = 0;
            this.insanityFlyffPathTextBox.TabStop = false;
            // 
            // DumpCleanerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 70);
            this.Controls.Add(this.txtCheckBox);
            this.Controls.Add(this.dmpCheckBox);
            this.Controls.Add(this.executeStartupCheckBox);
            this.Controls.Add(this.rptCheckBox);
            this.Controls.Add(this.insanityFlyffPathTextBox);
            this.Controls.Add(this.cleanDumpButton);
            this.Controls.Add(this.browseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::NeonFtool.Properties.Resources.logo_ico;
            this.MinimizeBox = false;
            this.Name = "DumpCleanerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dump Cleaner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DumpCleanerForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DumpCleanerForm_FormClosed);
            this.Load += new System.EventHandler(this.DumpCleanerForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button cleanDumpButton;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.CheckBox executeStartupCheckBox;
        private System.Windows.Forms.TextBox insanityFlyffPathTextBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.CheckBox txtCheckBox;
        private System.Windows.Forms.CheckBox dmpCheckBox;
        private System.Windows.Forms.CheckBox rptCheckBox;
    }
}