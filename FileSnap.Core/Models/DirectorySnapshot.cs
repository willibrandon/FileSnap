namespace FileSnap.Core.Models;

/// <summary>
/// Represents a snapshot of a directory.
/// </summary>
public class DirectorySnapshot
{
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
    /// Gets or sets the creation time of the directory.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the directory attributes.
    /// </summary>
    public FileAttributes Attributes { get; set; }
}

