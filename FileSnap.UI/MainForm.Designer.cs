namespace FileSnap.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnCaptureSnapshot = new System.Windows.Forms.Button();
            this.btnSaveSnapshot = new System.Windows.Forms.Button();
            this.btnLoadSnapshot = new System.Windows.Forms.Button();
            this.btnCompareSnapshots = new System.Windows.Forms.Button();
            this.btnRestoreSnapshot = new System.Windows.Forms.Button();
            this.txtSnapshotPath = new System.Windows.Forms.TextBox();
            this.lblSnapshotPath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCaptureSnapshot
            // 
            this.btnCaptureSnapshot.Location = new System.Drawing.Point(12, 12);
            this.btnCaptureSnapshot.Name = "btnCaptureSnapshot";
            this.btnCaptureSnapshot.Size = new System.Drawing.Size(150, 23);
            this.btnCaptureSnapshot.TabIndex = 0;
            this.btnCaptureSnapshot.Text = "Capture Snapshot";
            this.btnCaptureSnapshot.UseVisualStyleBackColor = true;
            this.btnCaptureSnapshot.Click += new System.EventHandler(this.btnCaptureSnapshot_Click);
            // 
            // btnSaveSnapshot
            // 
            this.btnSaveSnapshot.Location = new System.Drawing.Point(12, 41);
            this.btnSaveSnapshot.Name = "btnSaveSnapshot";
            this.btnSaveSnapshot.Size = new System.Drawing.Size(150, 23);
            this.btnSaveSnapshot.TabIndex = 1;
            this.btnSaveSnapshot.Text = "Save Snapshot";
            this.btnSaveSnapshot.UseVisualStyleBackColor = true;
            this.btnSaveSnapshot.Click += new System.EventHandler(this.btnSaveSnapshot_Click);
            // 
            // btnLoadSnapshot
            // 
            this.btnLoadSnapshot.Location = new System.Drawing.Point(12, 70);
            this.btnLoadSnapshot.Name = "btnLoadSnapshot";
            this.btnLoadSnapshot.Size = new System.Drawing.Size(150, 23);
            this.btnLoadSnapshot.TabIndex = 2;
            this.btnLoadSnapshot.Text = "Load Snapshot";
            this.btnLoadSnapshot.UseVisualStyleBackColor = true;
            this.btnLoadSnapshot.Click += new System.EventHandler(this.btnLoadSnapshot_Click);
            // 
            // btnCompareSnapshots
            // 
            this.btnCompareSnapshots.Location = new System.Drawing.Point(12, 99);
            this.btnCompareSnapshots.Name = "btnCompareSnapshots";
            this.btnCompareSnapshots.Size = new System.Drawing.Size(150, 23);
            this.btnCompareSnapshots.TabIndex = 3;
            this.btnCompareSnapshots.Text = "Compare Snapshots";
            this.btnCompareSnapshots.UseVisualStyleBackColor = true;
            this.btnCompareSnapshots.Click += new System.EventHandler(this.btnCompareSnapshots_Click);
            // 
            // btnRestoreSnapshot
            // 
            this.btnRestoreSnapshot.Location = new System.Drawing.Point(12, 128);
            this.btnRestoreSnapshot.Name = "btnRestoreSnapshot";
            this.btnRestoreSnapshot.Size = new System.Drawing.Size(150, 23);
            this.btnRestoreSnapshot.TabIndex = 4;
            this.btnRestoreSnapshot.Text = "Restore Snapshot";
            this.btnRestoreSnapshot.UseVisualStyleBackColor = true;
            this.btnRestoreSnapshot.Click += new System.EventHandler(this.btnRestoreSnapshot_Click);
            // 
            // txtSnapshotPath
            // 
            this.txtSnapshotPath.Location = new System.Drawing.Point(168, 43);
            this.txtSnapshotPath.Name = "txtSnapshotPath";
            this.txtSnapshotPath.Size = new System.Drawing.Size(200, 20);
            this.txtSnapshotPath.TabIndex = 5;
            // 
            // lblSnapshotPath
            // 
            this.lblSnapshotPath.AutoSize = true;
            this.lblSnapshotPath.Location = new System.Drawing.Point(168, 27);
            this.lblSnapshotPath.Name = "lblSnapshotPath";
            this.lblSnapshotPath.Size = new System.Drawing.Size(80, 13);
            this.lblSnapshotPath.TabIndex = 6;
            this.lblSnapshotPath.Text = "Snapshot Path:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 163);
            this.Controls.Add(this.lblSnapshotPath);
            this.Controls.Add(this.txtSnapshotPath);
            this.Controls.Add(this.btnRestoreSnapshot);
            this.Controls.Add(this.btnCompareSnapshots);
            this.Controls.Add(this.btnLoadSnapshot);
            this.Controls.Add(this.btnSaveSnapshot);
            this.Controls.Add(this.btnCaptureSnapshot);
            this.Name = "MainForm";
            this.Text = "FileSnap";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnCaptureSnapshot;
        private System.Windows.Forms.Button btnSaveSnapshot;
        private System.Windows.Forms.Button btnLoadSnapshot;
        private System.Windows.Forms.Button btnCompareSnapshots;
        private System.Windows.Forms.Button btnRestoreSnapshot;
        private System.Windows.Forms.TextBox txtSnapshotPath;
        private System.Windows.Forms.Label lblSnapshotPath;
    }
}
