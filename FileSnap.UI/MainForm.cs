using System;
using System.Windows.Forms;
using FileSnap.Core.Services;
using FileSnap.Core.Models;

namespace FileSnap.UI
{
    public partial class MainForm : Form
    {
        private readonly SnapshotService _snapshotService;
        private readonly ComparisonService _comparisonService;
        private readonly RestorationService _restorationService;

        public MainForm()
        {
            InitializeComponent();
            _snapshotService = new SnapshotService();
            _comparisonService = new ComparisonService();
            _restorationService = new RestorationService();
        }

        private async void btnCaptureSnapshot_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var snapshot = await _snapshotService.CaptureSnapshotAsync(folderBrowserDialog.SelectedPath);
                MessageBox.Show("Snapshot captured successfully.");
            }
        }

        private async void btnSaveSnapshot_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "JSON Files|*.json" };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var snapshot = await _snapshotService.CaptureSnapshotAsync(txtSnapshotPath.Text);
                await _snapshotService.SaveSnapshotAsync(snapshot, saveFileDialog.FileName);
                MessageBox.Show("Snapshot saved successfully.");
            }
        }

        private async void btnLoadSnapshot_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "JSON Files|*.json" };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var snapshot = await _snapshotService.LoadSnapshotAsync(openFileDialog.FileName);
                MessageBox.Show("Snapshot loaded successfully.");
            }
        }

        private async void btnCompareSnapshots_Click(object sender, EventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog { Filter = "JSON Files|*.json" };
            var openFileDialog2 = new OpenFileDialog { Filter = "JSON Files|*.json" };

            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                var snapshot1 = await _snapshotService.LoadSnapshotAsync(openFileDialog1.FileName);
                var snapshot2 = await _snapshotService.LoadSnapshotAsync(openFileDialog2.FileName);

                var differences = _comparisonService.Compare(snapshot1, snapshot2);
                MessageBox.Show("Snapshots compared successfully.");
            }
        }

        private async void btnRestoreSnapshot_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "JSON Files|*.json" };
            var folderBrowserDialog = new FolderBrowserDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK && folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var snapshot = await _snapshotService.LoadSnapshotAsync(openFileDialog.FileName);
                await _restorationService.RestoreSnapshotAsync(snapshot, folderBrowserDialog.SelectedPath);
                MessageBox.Show("Snapshot restored successfully.");
            }
        }
    }
}
