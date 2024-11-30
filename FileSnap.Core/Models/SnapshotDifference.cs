namespace FileSnap.Core.Models;

/// <summary>
/// Represents the differences between two snapshots.
/// </summary>
public class SnapshotDifference
{
    /// <summary>
    /// Gets or sets the unique identifier of the difference.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the creation time of the difference.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the before snapshot.
    /// </summary>
    public Guid BeforeSnapshotId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the after snapshot.
    /// </summary>
    public Guid AfterSnapshotId { get; set; }

    /// <summary>
    /// Gets or sets the list of new files in the after snapshot.
    /// </summary>
    public List<FileSnapshot> NewFiles { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of deleted files in the after snapshot.
    /// </summary>
    public List<FileSnapshot> DeletedFiles { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of modified files between the snapshots.
    /// </summary>
    public List<(FileSnapshot Before, FileSnapshot After)> ModifiedFiles { get; set; } = [];
}
