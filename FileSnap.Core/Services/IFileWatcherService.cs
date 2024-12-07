namespace FileSnap.Core.Services;

public interface IFileWatcherService
{
    /// <summary>
    /// Starts the file watcher for the specified path.
    /// </summary>
    /// <param name="path">The path to watch for file system changes.</param>
    void StartWatching(string path);

    /// <summary>
    /// Stops the file watcher.
    /// </summary>
    void StopWatching();

    /// <summary>
    /// Configures the file watcher to treat file permission changes as separate events or part of other events.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file permission changes as separate events, false to treat them as part of other events.</param>
    void ConfigureFilePermissionChanges(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to treat symbolic links as regular files or directories or to follow them and capture the target content.
    /// </summary>
    /// <param name="followSymbolicLinks">True to follow symbolic links and capture the target content, false to treat them as regular files or directories.</param>
    void ConfigureSymbolicLinks(bool followSymbolicLinks);

    /// <summary>
    /// Configures the file watcher to treat file renames as separate events or as a single event.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file renames as separate events, false to treat them as a single event.</param>
    void ConfigureFileRenames(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to treat file deletions as separate events or part of other events.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file deletions as separate events, false to treat them as part of other events.</param>
    void ConfigureFileDeletions(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to treat file moves as separate events (delete and create) or as a single move event.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file moves as separate events, false to treat them as a single move event.</param>
    void ConfigureFileMoves(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to treat file attribute changes as separate events or part of other events.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file attribute changes as separate events, false to treat them as part of other events.</param>
    void ConfigureFileAttributeChanges(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to treat file creation events as separate events or part of other events.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file creation events as separate events, false to treat them as part of other events.</param>
    void ConfigureFileCreationEvents(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to treat file modifications as separate events or part of other events.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file modifications as separate events, false to treat them as part of other events.</param>
    void ConfigureFileModifications(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to filter events based on file extensions, directories, or specific file names.
    /// </summary>
    /// <param name="filterOptions">The filter options to apply.</param>
    void ConfigureEventFiltering(EventFilterOptions filterOptions);

    /// <summary>
    /// Configures the file watcher to treat file system mount and unmount events as separate events or part of other events.
    /// </summary>
    /// <param name="treatAsSeparateEvent">True to treat file system mount and unmount events as separate events, false to treat them as part of other events.</param>
    void ConfigureFileSystemMountEvents(bool treatAsSeparateEvent);

    /// <summary>
    /// Configures the file watcher to support network file systems.
    /// </summary>
    /// <param name="supportNetworkFileSystems">True to support network file systems, false otherwise.</param>
    void ConfigureNetworkFileSystems(bool supportNetworkFileSystems);
}

/// <summary>
/// Represents the options for filtering file system events.
/// </summary>
public class EventFilterOptions
{
    /// <summary>
    /// Gets or sets the file extensions to include in the event filtering.
    /// </summary>
    public List<string> FileExtensions { get; set; } = new();

    /// <summary>
    /// Gets or sets the directories to include in the event filtering.
    /// </summary>
    public List<string> Directories { get; set; } = new();

    /// <summary>
    /// Gets or sets the specific file names to include in the event filtering.
    /// </summary>
    public List<string> FileNames { get; set; } = new();
}
