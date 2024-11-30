using FileSnap.Core.Exceptions;
using FileSnap.Core.Models;
using System.IO.Compression;
using System.Text.Json;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for capturing, saving, and loading file system snapshots.
/// </summary>
public class SnapshotService : ISnapshotService
{
    private readonly IHashingService _hashingService;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SnapshotService(IHashingService hashingService)
    {
        _hashingService = hashingService;
        _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    }

    private async Task<DirectorySnapshot> CaptureDirectory(string path)
    {
        var dirInfo = new DirectoryInfo(path);
        var snapshot = new DirectorySnapshot
        {
            Path = path,
            CreatedAt = dirInfo.CreationTimeUtc,
            Attributes = dirInfo.Attributes
        };

        foreach (var file in dirInfo.GetFiles())
        {
            snapshot.Files.Add(await CaptureFile(file));
        }

        foreach (var dir in dirInfo.GetDirectories())
        {
            snapshot.Directories.Add(await CaptureDirectory(dir.FullName));
        }

        return snapshot;
    }

    private async Task<FileSnapshot> CaptureFile(FileInfo file)
    {
        byte[] content;
        using (var stream = file.OpenRead())
        {
            content = new byte[file.Length];
            await stream.ReadExactlyAsync(content);
        }

        return new FileSnapshot
        {
            Path = file.FullName,
            Hash = await _hashingService.ComputeHashAsync(file.FullName),
            Size = file.Length,
            LastModified = file.LastWriteTimeUtc,
            CreatedAt = file.CreationTimeUtc,
            Attributes = file.Attributes,
            Content = content // Store the file content
        };
    }

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

    public async Task<SystemSnapshot> LoadSnapshot(string path)
    {
        var compressedData = await File.ReadAllBytesAsync(path);
        var json = DecompressString(compressedData);
        var snapshot = JsonSerializer.Deserialize<SystemSnapshot>(json, _jsonSerializerOptions);
        return snapshot ?? throw new SnapshotException("Failed to deserialize the snapshot.");
    }

    public async Task SaveSnapshot(SystemSnapshot snapshot, string outputPath)
    {
        var json = JsonSerializer.Serialize(snapshot, _jsonSerializerOptions);
        var compressedData = CompressString(json);
        await File.WriteAllBytesAsync(outputPath, compressedData);
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
