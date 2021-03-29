
using System.Drawing;

namespace NeonFtool.Forms
{
    partial class WindowManagerForm
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
            this.leftListBox = new System.Windows.Forms.ListBox();
            this.rightListBox = new System.Windows.Forms.ListBox();
            this.changeToHiddenButton = new System.Windows.Forms.Button();
            this.changeToShownButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cpuPriorityCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.hideWindowCheckBox = new System.Windows.Forms.CheckBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // leftListBox
            // 
            this.leftListBox.FormattingEnabled = true;
            this.leftListBox.Location = new System.Drawing.Point(12, 106);
            this.leftListBox.Name = "leftListBox";
            this.leftListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.leftListBox.Size = new System.Drawing.Size(183, 277);
            this.leftListBox.TabIndex = 0;
            // 
            // rightListBox
            // 
            this.rightListBox.FormattingEnabled = true;
            this.rightListBox.Location = new System.Drawing.Point(261, 106);
            this.rightListBox.Name = "rightListBox";
            this.rightListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.rightListBox.Size = new System.Drawing.Size(183, 277);
            this.rightListBox.TabIndex = 0;
            // 
            // changeToHiddenButton
            // 
            this.changeToHiddenButton.Location = new System.Drawing.Point(212, 196);
            this.changeToHiddenButton.Name = "changeToHiddenButton";
            this.changeToHiddenButton.Size = new System.Drawing.Size(28, 28);
            this.changeToHiddenButton.TabIndex = 1;
            this.changeToHiddenButton.Text = ">";
            this.changeToHiddenButton.UseVisualStyleBackColor = true;
            this.changeToHiddenButton.Click += new System.EventHandler(this.changeToHiddenButton_Click);
            // 
            // changeToShownButton
            // 
            this.changeToShownButton.Location = new System.Drawing.Point(212, 250);
            this.changeToShownButton.Name = "changeToShownButton";
            this.changeToShownButton.Size = new System.Drawing.Size(28, 28);
            this.changeToShownButton.TabIndex = 1;
            this.changeToShownButton.Text = "<";
            this.changeToShownButton.UseVisualStyleBackColor = true;
            this.changeToShownButton.Click += new System.EventHandler(this.changeToShownButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Current";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(322, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Processed";
            // 
            // cpuPriorityCheckBox
            // 
            this.cpuPriorityCheckBox.AutoSize = true;
            this.cpuPriorityCheckBox.Location = new System.Drawing.Point(15, 22);
            this.cpuPriorityCheckBox.Name = "cpuPriorityCheckBox";
            this.cpuPriorityCheckBox.Size = new System.Drawing.Size(133, 17);
            this.cpuPriorityCheckBox.TabIndex = 3;
            this.cpuPriorityCheckBox.Text = "Set CPU Priority to Idle";
            this.cpuPriorityCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.hideWindowCheckBox);
            this.groupBox1.Controls.Add(this.cpuPriorityCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(432, 74);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // hideWindowCheckBox
            // 
            this.hideWindowCheckBox.AutoSize = true;
            this.hideWindowCheckBox.Location = new System.Drawing.Point(15, 45);
            this.hideWindowCheckBox.Name = "hideWindowCheckBox";
            this.hideWindowCheckBox.Size = new System.Drawing.Size(90, 17);
            this.hideWindowCheckBox.TabIndex = 3;
            this.hideWindowCheckBox.Text = "Hide Window";
            this.hideWindowCheckBox.UseVisualStyleBackColor = true;
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(190, 400);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 5;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // WindowManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 435);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.changeToShownButton);
            this.Controls.Add(this.changeToHiddenButton);
            this.Controls.Add(this.rightListBox);
            this.Controls.Add(this.leftListBox);
            this.Icon = global::NeonFtool.Properties.Resources.logo_ico;
            this.MaximizeBox = false;
            this.Name = "WindowManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Window Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WindowManager_FormClosing);
            this.Load += new System.EventHandler(this.WindowManagementForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox leftListBox;
        private System.Windows.Forms.ListBox rightListBox;
        private System.Windows.Forms.Button changeToHiddenButton;
        private System.Windows.Forms.Button changeToShownButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cpuPriorityCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.CheckBox hideWindowCheckBox;
    }
}