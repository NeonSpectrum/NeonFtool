using NeonFtool.Libraries;
using NeonFtool.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NeonFtool.Forms
{
    public partial class OverlayForm : Form
    {
        private static int _globalOffsetX = -2; // -2 means not initialized
        private static int _globalOffsetY = -2;

        private readonly IntPtr _targetHandle;
        private readonly Timer _updateTimer;
        private string _spammerName;
        private bool _isLocked;
        private bool _dragging;
        private Point _dragStart;

        public OverlayForm(IntPtr targetHandle, string spammerName)
        {
            _targetHandle = targetHandle;
            _spammerName  = spammerName;
            _isLocked     = Settings.Get().LockOverlay;

            InitializeComponent();

            // Form setup for transparency
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost         = true;
            this.ShowInTaskbar   = false;
            this.BackColor       = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.StartPosition   = FormStartPosition.Manual;
            this.Opacity         = 0.5;

            // Initial offsets
            if (_globalOffsetX == -2)
            {
                Settings settings = Settings.Get();
                if (settings.OverlayOffsetX != -1)
                {
                    _globalOffsetX = settings.OverlayOffsetX;
                    _globalOffsetY = settings.OverlayOffsetY;
                }
            }

            if (_globalOffsetX == -2 && Function.GetWindowRect(_targetHandle, out Function.RECT rect))
            {
                int windowWidth = rect.Right - rect.Left;
                // Default to center of window
                _globalOffsetX = (windowWidth / 2);
                _globalOffsetY = (rect.Bottom - rect.Top) - 100;
            }

            // Set initial position before showing
            if (Function.GetWindowRect(_targetHandle, out Function.RECT currentRect))
            {
                this.Left = currentRect.Left + _globalOffsetX - (this.Width / 2);
                this.Top  = currentRect.Top  + _globalOffsetY - (this.Height / 2);
            }

            UpdateLockState();

            _updateTimer = new Timer { Interval = 50 };
            _updateTimer.Tick += UpdatePosition;
            _updateTimer.Start();

            // Setup dragging events
            this.statusLabel.MouseDown += OnMouseDown;
            this.statusLabel.MouseMove += OnMouseMove;
            this.statusLabel.MouseUp   += OnMouseUp;
        }

        public void SetLock(bool locked)
        {
            _isLocked = locked;
            UpdateLockState();
        }

        private void UpdateLockState()
        {
            int initialStyle = Function.GetWindowLong(this.Handle, Function.GWL_EXSTYLE);
            // Always keep WS_EX_LAYERED and WS_EX_NOACTIVATE
            int targetStyle = initialStyle | Function.WS_EX_LAYERED | Function.WS_EX_NOACTIVATE;

            if (_isLocked)
            {
                Function.SetWindowLong(this.Handle, Function.GWL_EXSTYLE, targetStyle | Function.WS_EX_TRANSPARENT);
                this.Cursor = Cursors.Default;
            }
            else
            {
                Function.SetWindowLong(this.Handle, Function.GWL_EXSTYLE, targetStyle & ~Function.WS_EX_TRANSPARENT);
                this.Cursor = Cursors.SizeAll;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_isLocked) return;
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStart = e.Location;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                this.Left += e.X - _dragStart.X;
                this.Top  += e.Y - _dragStart.Y;

                // Update offsets relative to target window (use center of form)
                if (Function.GetWindowRect(_targetHandle, out Function.RECT rect))
                {
                    _globalOffsetX = (this.Left + this.Width / 2) - rect.Left;
                    _globalOffsetY = (this.Top + this.Height / 2) - rect.Top;
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                _dragging = false;
                
                // Save to settings
                Settings settings = Settings.Get();
                settings.OverlayOffsetX = _globalOffsetX;
                settings.OverlayOffsetY = _globalOffsetY;
                settings.Save();
            }
        }

        public void UpdateName(string name)
        {
            _spammerName = name;
            statusLabel.Text = $"[ ACTIVE ] {_spammerName}";
            
            // Force layout and reposition immediately
            this.PerformLayout();
            UpdatePosition(null, null);
        }

        private void UpdatePosition(object sender, EventArgs e)
        {
            if (_dragging) return;

            // Check foreground window
            IntPtr fg = Function.GetForegroundWindow();
            
            // Hide overlay if the target window is not the foreground window 
            // AND the overlay itself is not the foreground window
            if (fg != _targetHandle && fg != this.Handle)
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

            // Update position based on offsets (centered)
            this.Left = rect.Left + _globalOffsetX - (this.Width / 2);
            this.Top  = rect.Top  + _globalOffsetY - (this.Height / 2);
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
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1, 1);
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
