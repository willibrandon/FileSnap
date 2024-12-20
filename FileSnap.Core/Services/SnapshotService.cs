using FileSnap.Core.Exceptions;
using FileSnap.Core.Models;
using FileSnap.Core.Utilities;
using System.Text.Json;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for capturing, saving, and loading file system snapshots.
/// </summary>
public class SnapshotService : ISnapshotService
{
    private const int BatchSize = 100;
    private const string FileExtension = ".json";

    private readonly IHashingService _hashingService;
    private readonly ICompressionService _compressionService;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly bool _isCompressionEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotService"/> class with the default hashing service.
    /// Compression is disabled by default.
    /// </summary>
    public SnapshotService()
        : this(new HashingService(), false, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotService"/> class with the specified hashing service.
    /// Compression is disabled by default.
    /// </summary>
    /// <param name="hashingService">The service used to compute file hashes.</param>
    public SnapshotService(IHashingService hashingService)
        : this(hashingService, false, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotService"/> class with the specified hashing service and compression flag.
    /// </summary>
    /// <param name="hashingService">The service used to compute file hashes.</param>
    /// <param name="isCompressionEnabled">Flag to enable or disable content compression.</param>
    public SnapshotService(IHashingService hashingService, bool isCompressionEnabled)
        : this(hashingService, isCompressionEnabled, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotService"/> class with the specified hashing service, compression flag, and compression service.
    /// </summary>
    /// <param name="hashingService">The service used to compute file hashes.</param>
    /// <param name="isCompressionEnabled">Flag to enable or disable content compression.</param>
    /// <param name="compressionService">The service used to compress and decompress data.</param>
    public SnapshotService(IHashingService hashingService, bool isCompressionEnabled, ICompressionService? compressionService)
    {
        _hashingService = hashingService ?? throw new ArgumentNullException(nameof(hashingService));
        _compressionService = compressionService ?? new BrotliCompressionService();
        _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        _isCompressionEnabled = isCompressionEnabled;
    }

    /// <summary>
    /// Captures a snapshot of the specified directory and its contents.
    /// </summary>
    /// <param name="path">The full path to the directory to capture.</param>
    /// <returns>A <see cref="SystemSnapshot"/> containing the captured directory structure.</returns>
    /// <exception cref="SnapshotException">Thrown when the specified directory does not exist.</exception>
    public async Task<SystemSnapshot> CaptureSnapshotAsync(string path)
    {
        if (!Directory.Exists(path))
            throw new SnapshotException($"Directory not found: {path}");

        var snapshot = new SystemSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            BasePath = path,
            RootDirectory = await CaptureDirectoryAsync(path)
        };

        return snapshot;
    }

    /// <summary>
    /// Captures an incremental snapshot of the specified directory and its contents based on a previous snapshot.
    /// </summary>
    /// <param name="path">The full path to the directory to capture.</param>
    /// <param name="previousSnapshot">The previous snapshot to compare against.</param>
    /// <returns>A <see cref="SystemSnapshot"/> containing the captured incremental directory structure.</returns>
    /// <exception cref="SnapshotException">Thrown when the specified directory does not exist.</exception>
    public async Task<SystemSnapshot> CaptureIncrementalSnapshotAsync(string path, SystemSnapshot previousSnapshot)
    {
        if (!Directory.Exists(path))
            throw new SnapshotException($"Directory not found: {path}");

        var currentSnapshot = await CaptureSnapshotAsync(path);
        var difference = new ComparisonService().Compare(previousSnapshot, currentSnapshot);

        var incrementalSnapshot = new SystemSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            BasePath = path,
            RootDirectory = new DirectorySnapshot
            {
                Files = [.. difference.NewFiles, .. difference.ModifiedFiles.Select(m => m.After), .. difference.DeletedFiles],
                Directories = [.. difference.NewDirectories, .. difference.ModifiedDirectories.Select(m => m.After), .. difference.DeletedDirectories],
                Metadata = new() { Path = path },
            }
        };

        return incrementalSnapshot;
    }

    /// <summary>
    /// Captures a snapshot of the specified directory and its contents with additional metadata.
    /// </summary>
    /// <param name="path">The full path to the directory to capture.</param>
    /// <param name="metadata">Additional metadata to include in the snapshot.</param>
    /// <returns>A <see cref="SystemSnapshot"/> containing the captured directory structure and metadata.</returns>
    /// <exception cref="SnapshotException">Thrown when the specified directory does not exist.</exception>
    public async Task<SystemSnapshot> CaptureSnapshotWithMetadataAsync(string path, Dictionary<string, string> metadata)
    {
        if (!Directory.Exists(path))
            throw new SnapshotException($"Directory not found: {path}");

        var snapshot = new SystemSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            BasePath = path,
            RootDirectory = await CaptureDirectoryAsync(path),
            Metadata = metadata,
        };

        return snapshot;
    }

    /// <summary>
    /// Captures an incremental snapshot of the specified directory and its contents based on a previous snapshot with additional metadata.
    /// </summary>
    /// <param name="path">The full path to the directory to capture.</param>
    /// <param name="previousSnapshot">The previous snapshot to compare against.</param>
    /// <param name="metadata">Additional metadata to include in the snapshot.</param>
    /// <returns>A <see cref="SystemSnapshot"/> containing the captured incremental directory structure and metadata.</returns>
    /// <exception cref="SnapshotException">Thrown when the specified directory does not exist.</exception>
    public async Task<SystemSnapshot> CaptureIncrementalSnapshotWithMetadataAsync(string path, SystemSnapshot previousSnapshot, Dictionary<string, string> metadata)
    {
        if (!Directory.Exists(path))
            throw new SnapshotException($"Directory not found: {path}");

        var currentSnapshot = await CaptureSnapshotAsync(path);
        var difference = new ComparisonService().Compare(previousSnapshot, currentSnapshot);

        var incrementalSnapshot = new SystemSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            BasePath = path,
            RootDirectory = new DirectorySnapshot
            {
                Files = [.. difference.NewFiles, .. difference.ModifiedFiles.Select(m => m.After), .. difference.DeletedFiles],
                Directories = [.. difference.NewDirectories, .. difference.ModifiedDirectories.Select(m => m.After), .. difference.DeletedDirectories],
                Metadata = new() { Path = path },
            },
            Metadata = metadata,
        };

        return incrementalSnapshot;
    }

    /// <summary>
    /// Loads a previously saved snapshot from a file.
    /// </summary>
    /// <param name="path">The path to the compressed snapshot file.</param>
    /// <returns>The deserialized <see cref="SystemSnapshot"/>.</returns>
    /// <exception cref="SnapshotException">Thrown when the snapshot cannot be deserialized.</exception>
    public async Task<SystemSnapshot> LoadSnapshotAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);
        var snapshot = JsonSerializer.Deserialize<SystemSnapshot>(json, _jsonSerializerOptions)
            ?? throw new SnapshotException("Failed to deserialize the snapshot.");

        if (snapshot.RootDirectory == null)
            throw new SnapshotException("Root directory is null in the deserialized snapshot.");

        if (_isCompressionEnabled)
        {
            DecompressContent(snapshot.RootDirectory);
        }

        return snapshot;
    }

    /// <summary>
    /// Saves a system snapshot to the specified file path.
    /// </summary>
    /// <param name="snapshot">The snapshot to save.</param>
    /// <param="outputPath">The path where the compressed snapshot will be saved.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveSnapshotAsync(SystemSnapshot snapshot, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(outputPath);

        if (snapshot.RootDirectory == null)
            throw new SnapshotException("Root directory is null in the snapshot.");

        // Ensure the output file has the .json extension if none is specified.
        var finalPath = !Path.HasExtension(outputPath)
                ? outputPath + FileExtension
                : outputPath;

        if (_isCompressionEnabled)
        {
            CompressContent(snapshot.RootDirectory);
        }

        var json = JsonSerializer.Serialize(snapshot, _jsonSerializerOptions);
        await File.WriteAllTextAsync(finalPath, json);
    }

    private async Task<DirectorySnapshot> CaptureDirectoryAsync(string path)
    {
        var dirInfo = new DirectoryInfo(path);

        var snapshot = new DirectorySnapshot
        {
            Metadata = new DirectoryMetadata
            {
                Path = path,
                CreatedAt = dirInfo.CreationTimeUtc,
                Attributes = dirInfo.Attributes,
                LastModified = dirInfo.LastWriteTimeUtc,
                Size = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length)
            },
            Files = [],
            Directories = [],
        };

        // Process files in current directory.
        var files = dirInfo.GetFiles();

        foreach (var batch in files.Batch(BatchSize))
        {
            var fileTasks = batch.Select(CaptureFileAsync);
            var fileSnapshots = await Task.WhenAll(fileTasks);
            snapshot.Files.AddRange(fileSnapshots);
        }

        // Process subdirectories.
        var directories = dirInfo.GetDirectories();
        var directoryTasks = directories.Select(dir => CaptureDirectoryAsync(dir.FullName));
        var capturedDirectories = await Task.WhenAll(directoryTasks);
        snapshot.Directories.AddRange(capturedDirectories);

        // Sort files and directories by their paths to ensure consistent order.
        snapshot.Files = [.. snapshot.Files.OrderBy(f => f.Metadata.Path)];
        snapshot.Directories = [.. snapshot.Directories.OrderBy(d => d.Metadata.Path)];

        return snapshot;
    }

    private async Task<FileSnapshot> CaptureFileAsync(FileInfo file)
    {
        byte[] content;
        using (var fileStream = file.OpenRead())
        {
            content = new byte[file.Length];
            await fileStream.ReadExactlyAsync(content.AsMemory(0, (int)file.Length));
        }

        return new FileSnapshot
        {
            Metadata = new FileMetadata
            {
                Path = file.FullName,
                Size = file.Length,
                Hash = await _hashingService.ComputeHashAsync(file.FullName),
                LastModified = file.LastWriteTimeUtc,
                CreatedAt = file.CreationTimeUtc,
                Attributes = file.Attributes
            },
            Content = content
        };
    }

    private void CompressContent(DirectorySnapshot directorySnapshot)
    {
        Parallel.ForEach(directorySnapshot.Files, file =>
        {
            if (file.Content != null)
            {
                file.CompressedContent = _compressionService.Compress(file.Content);
                file.Content = null;
            }
        });

        Parallel.ForEach(directorySnapshot.Directories, CompressContent);
    }

    private void DecompressContent(DirectorySnapshot directorySnapshot)
    {
        Parallel.ForEach(directorySnapshot.Files, file =>
        {
            if (file.CompressedContent != null)
            {
                file.Content = _compressionService.Decompress(file.CompressedContent);
                file.CompressedContent = null;
            }
        });

        Parallel.ForEach(directorySnapshot.Directories, DecompressContent);
    }
}
