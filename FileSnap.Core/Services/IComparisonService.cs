using FileSnap.Core.Models;

namespace FileSnap.Core.Services;

/// <summary>
/// Defines methods for comparing file system snapshots.
/// </summary>
public interface IComparisonService
{
    /// <summary>
    /// Compares two snapshots and returns the differences between them.
    /// </summary>
    /// <param name="before">The snapshot before the changes.</param>
    /// <param name="after">The snapshot after the changes.</param>
    /// <returns>The differences between the snapshots.</returns>
    SnapshotDifference Compare(SystemSnapshot before, SystemSnapshot after);
}
