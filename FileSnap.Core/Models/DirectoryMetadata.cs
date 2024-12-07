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

    /// <summary>
    /// Gets or sets the user who made the change.
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    /// Gets or sets the process that triggered the event.
    /// </summary>
    public string? Process { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the event.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryMetadata"/> class.
    /// </summary>
    public DirectoryMetadata()
    {
        Path = string.Empty;
        CreatedAt = DateTime.MinValue;
        Attributes = FileAttributes.Directory;
        LastModified = DateTime.MinValue;
        Size = 0;
        AdditionalMetadata = new Dictionary<string, string>();
        User = string.Empty;
        Process = string.Empty;
        Timestamp = DateTime.MinValue;
    }
}
