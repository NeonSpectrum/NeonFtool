using NeonFtool.Libraries;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NeonFtool.Forms
{
    public partial class OverlayForm : Form
    {
        private readonly IntPtr _targetHandle;
        private readonly Timer _updateTimer;
        private string _spammerName;

        public OverlayForm(IntPtr targetHandle, string spammerName)
        {
            _targetHandle = targetHandle;
            _spammerName  = spammerName;

            InitializeComponent();

            // Form setup for transparency and click-through
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.BackColor       = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.StartPosition   = FormStartPosition.Manual;
            this.Opacity         = 0.5;

            // Make click-through
            int initialStyle = Function.GetWindowLong(this.Handle, Function.GWL_EXSTYLE);
            Function.SetWindowLong(this.Handle, Function.GWL_EXSTYLE, initialStyle | Function.WS_EX_LAYERED | Function.WS_EX_TRANSPARENT);

            _updateTimer = new Timer { Interval = 50 };
            _updateTimer.Tick += UpdatePosition;
            _updateTimer.Start();
        }

        public void UpdateName(string name)
        {
            _spammerName = name;
            statusLabel.Text = $"[ ACTIVE ] {_spammerName}";
        }

        private void UpdatePosition(object sender, EventArgs e)
        {
            // Hide overlay if the target window is not the foreground window
            if (Function.GetForegroundWindow() != _targetHandle)
            {
                this.Hide();
                return;
            }

            if (!Function.GetWindowRect(_targetHandle, out Function.RECT rect))
            {
                this.Hide();
                return;
            }

            // If window is minimized or hidden, hide overlay
            if (rect.Left < -30000) // Standard way to detect minimized windows in RECT
            {
                this.Hide();
                return;
            }

            if (!this.Visible) this.Show();

            int windowWidth = rect.Right - rect.Left;

            // Center horizontally, and place at the bottom with a 20px margin
            this.Left = rect.Left + (windowWidth / 2) - (this.Width / 2) + 50;
            this.Top  = rect.Bottom - this.Height - 65;
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label statusLabel;

        private void InitializeComponent()
        {
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.BackColor = System.Drawing.Color.Black;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.statusLabel.ForeColor = System.Drawing.Color.Cyan;
            this.statusLabel.Location = new System.Drawing.Point(0, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(5);
            this.statusLabel.Size = new System.Drawing.Size(150, 29);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = $"[ ACTIVE ] {_spammerName}";
            // 
            // OverlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 40);
            this.Controls.Add(this.statusLabel);
            this.Name = "OverlayForm";
            this.Text = "OverlayForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
