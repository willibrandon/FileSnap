namespace FileSnap.Core.Models;

/// <summary>
/// Represents metadata for a file.
/// </summary>
public class FileMetadata
{
    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the size of the file.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the hash of the file content.
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// Gets or sets the last modified time of the file.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the creation time of the file.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the file attributes.
    /// </summary>
    public FileAttributes Attributes { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the file.
    /// </summary>
    public Dictionary<string, string>? AdditionalMetadata { get; set; }
}
