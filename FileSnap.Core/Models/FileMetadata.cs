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
    /// Initializes a new instance of the <see cref="FileMetadata"/> class.
    /// </summary>
    public FileMetadata()
    {
        Path = string.Empty;
        Size = 0;
        Hash = string.Empty;
        LastModified = DateTime.MinValue;
        CreatedAt = DateTime.MinValue;
        Attributes = FileAttributes.Normal;
        AdditionalMetadata = new Dictionary<string, string>();
        User = string.Empty;
        Process = string.Empty;
        Timestamp = DateTime.MinValue;
    }
}
