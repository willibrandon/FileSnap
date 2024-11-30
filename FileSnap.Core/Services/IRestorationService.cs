using FileSnap.Core.Models;

namespace FileSnap.Core.Services;

/// <summary>
/// Defines methods for restoring file system snapshots.
/// </summary>
public interface IRestorationService
{
    /// <summary>
    /// Restores the file system to the state captured in the snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot to restore.</param>
    /// <param name="targetPath">The target path to restore the snapshot to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RestoreSnapshot(SystemSnapshot snapshot, string targetPath);
}
