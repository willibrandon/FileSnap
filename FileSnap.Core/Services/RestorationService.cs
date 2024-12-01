using FileSnap.Core.Exceptions;
using FileSnap.Core.Models;
using System.Buffers;
using System.IO.MemoryMappedFiles;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for restoring file system snapshots with optimized parallelism, memory management, and disk I/O.
/// </summary>
public class RestorationService : IRestorationService
{
    private readonly DirectoryCache _directoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="RestorationService"/> class.
    /// </summary>
    public RestorationService()
    {
        _directoryCache = new DirectoryCache();
    }

    /// <inheritdoc />
    /// <summary>
    /// Restores a snapshot to the specified target path, optimizing for parallelism and memory usage.
    /// </summary>
    public async Task RestoreSnapshot(SystemSnapshot snapshot, string targetPath)
    {
        if (snapshot.RootDirectory == null)
            throw new SnapshotException("Root directory snapshot is null.");

        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);

        // Begin parallel restoration for the root directory.
        await RestoreDirectory(snapshot.RootDirectory, targetPath);
    }

    /// <summary>
    /// Restores a directory snapshot to the target path, utilizing parallelism for directories and files.
    /// </summary>
    private async Task RestoreDirectory(DirectorySnapshot directorySnapshot, string targetPath)
    {
        if (directorySnapshot.Path == null)
            throw new SnapshotException("Directory snapshot path is null.");

        var targetDir = Path.Combine(targetPath, Path.GetFileName(directorySnapshot.Path));

        // Ensure that the directory is created and cached efficiently.
        await _directoryCache.EnsureDirectoryExistsAsync(targetDir);

        // Parallelize restoration of files and directories.
        var fileRestorationTasks = directorySnapshot.Files.Select(file => Task.Run(() => RestoreFile(file, targetDir))).ToArray();
        var directoryRestorationTasks = directorySnapshot.Directories.Select(dir => RestoreDirectory(dir, targetDir)).ToArray();

        await Task.WhenAll(fileRestorationTasks.Concat(directoryRestorationTasks));
    }

    /// <summary>
    /// Restores a file from a snapshot to the specified target path, optimizing disk I/O.
    /// </summary>
    private static void RestoreFile(FileSnapshot fileSnapshot, string targetPath)
    {
        if (fileSnapshot == null)
            throw new SnapshotException("File snapshot is null.");

        if (fileSnapshot.Path == null)
            throw new SnapshotException("File snapshot path is null.");

        if (fileSnapshot.Content == null)
            throw new SnapshotException("File snapshot content is null.");

        var targetFile = Path.Combine(targetPath, Path.GetFileName(fileSnapshot.Path));

        // Use memory-mapped file for optimal I/O performance.
        using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(targetFile, FileMode.Create, null, fileSnapshot.Content.Length))
        {
            using var accessor = memoryMappedFile.CreateViewAccessor(0, fileSnapshot.Content.Length, MemoryMappedFileAccess.Write);
            accessor.WriteArray(0, fileSnapshot.Content, 0, fileSnapshot.Content.Length);
        }

        // Set file attributes and timestamps.
        File.SetAttributes(targetFile, fileSnapshot.Attributes);
        File.SetCreationTimeUtc(targetFile, fileSnapshot.CreatedAt);
        File.SetLastWriteTimeUtc(targetFile, fileSnapshot.LastModified);
    }
}
