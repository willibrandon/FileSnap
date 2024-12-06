namespace FileSnap.Core.Models;

/// <summary>
/// Represents a snapshot of a file.
/// </summary>
public class FileSnapshot
{
    /// <summary>
    /// Gets or sets a value indicating whether the file has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the file metadata.
    /// </summary>
    public FileMetadata Metadata { get; set; } = new FileMetadata();

    /// <summary>
    /// Gets or sets the content of the file.
    /// </summary>
    public byte[]? Content { get; set; }

    /// <summary>
    /// Gets or sets the compressed content of the file.
    /// </summary>
    public byte[]? CompressedContent { get; set; }
}
