using FileSnap.Core.Exceptions;
using FileSnap.Core.Models;
using FileSnap.Core.Utilities;
using System.IO.Compression;
using System.Text.Json;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for capturing, saving, and loading file system snapshots.
/// </summary>
public class SnapshotService : ISnapshotService
{
    private const int BatchSize = 100;
    private const string FileExtension = ".fsnap";

    private readonly IHashingService _hashingService;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotService"/> class.
    /// </summary>
    /// <param name="hashingService">The service used to compute file hashes.</param>
    public SnapshotService(IHashingService hashingService)
    {
        _hashingService = hashingService;
        _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    }

    /// <summary>
    /// Captures a snapshot of the specified directory and its contents.
    /// </summary>
    /// <param name="path">The full path to the directory to capture.</param>
    /// <returns>A <see cref="SystemSnapshot"/> containing the captured directory structure.</returns>
    /// <exception cref="SnapshotException">Thrown when the specified directory does not exist.</exception>
    public async Task<SystemSnapshot> CaptureSnapshot(string path)
    {
        if (!Directory.Exists(path))
            throw new SnapshotException($"Directory not found: {path}");

        var snapshot = new SystemSnapshot
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            BasePath = path,
            RootDirectory = await CaptureDirectory(path)
        };

        return snapshot;
    }

    /// <summary>
    /// Loads a previously saved snapshot from a file.
    /// </summary>
    /// <param name="path">The path to the compressed snapshot file.</param>
    /// <returns>The deserialized <see cref="SystemSnapshot"/>.</returns>
    /// <exception cref="SnapshotException">Thrown when the snapshot cannot be deserialized.</exception>
    public async Task<SystemSnapshot> LoadSnapshot(string path)
    {
        var compressedData = await File.ReadAllBytesAsync(path);
        var json = DecompressString(compressedData);
        var snapshot = JsonSerializer.Deserialize<SystemSnapshot>(json, _jsonSerializerOptions);
        return snapshot ?? throw new SnapshotException("Failed to deserialize the snapshot.");
    }

    /// <summary>
    /// Saves a system snapshot to the specified file path.
    /// </summary>
    /// <param name="snapshot">The snapshot to save.</param>
    /// <param name="outputPath">The path where the compressed snapshot will be saved. If no extension is provided, .fsnap will be appended.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
    /// <exception cref="SnapshotException">Thrown when the snapshot cannot be serialized or compressed.</exception>
    public async Task SaveSnapshot(SystemSnapshot snapshot, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        try
        {
            var finalPath = !Path.HasExtension(outputPath)
                ? outputPath + FileExtension
                : outputPath;

            var json = JsonSerializer.Serialize(snapshot, _jsonSerializerOptions);
            var compressedData = CompressString(json);
            await File.WriteAllBytesAsync(finalPath, compressedData);
        }
        catch (Exception ex) when (ex is not ArgumentNullException)
        {
            throw new SnapshotException("Failed to save snapshot", ex);
        }
    }

    private async Task<DirectorySnapshot> CaptureDirectory(string path)
    {
        var dirInfo = new DirectoryInfo(path);

        var snapshot = new DirectorySnapshot
        {
            Path = path,
            CreatedAt = dirInfo.CreationTimeUtc,
            Attributes = dirInfo.Attributes,
            Files = [],
            Directories = [],
        };

        // Process files in current directory.
        var files = dirInfo.GetFiles();

        foreach (var batch in files.Batch(BatchSize))
        {
            var fileTasks = batch.Select(CaptureFile);
            var fileSnapshots = await Task.WhenAll(fileTasks);
            snapshot.Files.AddRange(fileSnapshots);
        }

        // Process subdirectories.
        var directories = dirInfo.GetDirectories();
        var directoryTasks = directories.Select(dir => CaptureDirectory(dir.FullName));
        var capturedDirectories = await Task.WhenAll(directoryTasks);

        snapshot.Directories.AddRange(capturedDirectories);

        return snapshot;
    }

    private async Task<FileSnapshot> CaptureFile(FileInfo file)
    {
        byte[] content;
        using (var fileStream = file.OpenRead())
        {
            content = new byte[file.Length];
            await fileStream.ReadExactlyAsync(content.AsMemory(0, (int)file.Length));
        }

        return new FileSnapshot
        {
            Path = file.FullName,
            Content = content,
            Hash = await _hashingService.ComputeHashAsync(file.FullName),
            Size = file.Length,
            LastModified = file.LastWriteTimeUtc,
            CreatedAt = file.CreationTimeUtc,
            Attributes = file.Attributes
        };
    }

    private static byte[] CompressString(string str)
    {
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(brotli))
        {
            writer.Write(str);
        }
        return output.ToArray();
    }

    private static string DecompressString(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var reader = new StreamReader(brotli);
        return reader.ReadToEnd();
    }
}
