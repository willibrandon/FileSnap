namespace FileSnap.Core.Models;

/// <summary>
/// Represents a snapshot of a directory.
/// </summary>
public class DirectorySnapshot
{
    /// <summary>
    /// Gets or sets a value indicating whether the directory has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the directory path.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the list of file snapshots in the directory.
    /// </summary>
    public List<FileSnapshot> Files { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of subdirectory snapshots.
    /// </summary>
    public List<DirectorySnapshot> Directories { get; set; } = [];

    /// <summary>
    /// Gets or sets the directory metadata.
    /// </summary>
    public DirectoryMetadata Metadata { get; set; } = new DirectoryMetadata();
}
