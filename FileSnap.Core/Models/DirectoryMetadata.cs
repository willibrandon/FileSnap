namespace FileSnap.Core.Models;

/// <summary>
/// Represents metadata for a directory.
/// </summary>
public class DirectoryMetadata
{
    /// <summary>
    /// Gets or sets the directory path.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the creation time of the directory.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the directory attributes.
    /// </summary>
    public FileAttributes Attributes { get; set; }

    /// <summary>
    /// Gets or sets the last modified time of the directory.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the size of the directory.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the directory.
    /// </summary>
    public Dictionary<string, string>? AdditionalMetadata { get; set; }
}
