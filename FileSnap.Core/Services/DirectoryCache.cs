using System.Collections.Concurrent;

namespace FileSnap.Core.Services;

/// <summary>
/// A utility class that manages the caching of directories to optimize directory creation.
/// This class ensures that directories are created only once and cached for reuse, supporting parallelism.
/// </summary>
public class DirectoryCache
{
    private readonly ConcurrentDictionary<string, Task> _directoryCache;

    public DirectoryCache()
    {
        _directoryCache = new ConcurrentDictionary<string, Task>();
    }

    /// <summary>
    /// Asynchronously ensures that the specified directory exists. If the directory does not exist,
    /// it is created and added to the cache.
    /// </summary>
    /// <param name="directoryPath">The full path of the directory to check or create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="directoryPath"/> is null or empty.</exception>
    public async Task EnsureDirectoryExistsAsync(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath))
            throw new ArgumentNullException(nameof(directoryPath), "Directory path cannot be null or empty.");

        // If the directory is already being created or exists, return the cached task
        if (_directoryCache.TryGetValue(directoryPath, out var existingTask))
        {
            await existingTask;
            return;
        }

        // Create a new task to create the directory asynchronously
        var task = Task.Run(() =>
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        });

        // Add the task to the cache and wait for it to complete
        _directoryCache[directoryPath] = task;
        await task;
    }
}
