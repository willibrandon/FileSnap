namespace FileSnap.Core.Models;

/// <summary>
/// Represents a snapshot of the entire file system.
/// </summary>
public class SystemSnapshot
{
    /// <summary>
    /// Gets or sets the unique identifier of the snapshot.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the creation time of the snapshot.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the root directory snapshot.
    /// </summary>
    public DirectorySnapshot? RootDirectory { get; set; }

    /// <summary>
    /// Gets or sets the base path of the snapshot.
    /// </summary>
    public string? BasePath { get; set; }

    /// <summary>
    /// Gets or sets the insights from the snapshot analysis.
    /// </summary>
    public Dictionary<string, string>? Insights { get; set; }
  
    /// Gets or sets the additional metadata for the snapshot.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}
