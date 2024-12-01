using FileSnap.Core.Models;

namespace FileSnap.Core.Services;

/// <summary>
/// Defines methods for capturing, saving, and loading file system snapshots.
/// </summary>
public interface ISnapshotService
{
    /// <summary>
    /// Captures a snapshot of the file system at the specified path.
    /// </summary>
    /// <param name="path">The path to capture the snapshot from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the captured snapshot.</returns>
    Task<SystemSnapshot> CaptureSnapshotAsync(string path);

    /// <summary>
    /// Loads a snapshot from the specified path.
    /// </summary>
    /// <param name="path">The path to load the snapshot from.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded snapshot.</returns>
    Task<SystemSnapshot> LoadSnapshotAsync(string path);

    /// <summary>
    /// Saves the snapshot to the specified output path.
    /// </summary>
    /// <param name="snapshot">The snapshot to save.</param>
    /// <param name="outputPath">The output path to save the snapshot to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SaveSnapshotAsync(SystemSnapshot snapshot, string outputPath);
}

