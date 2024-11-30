using FileSnap.Core.Exceptions;
using FileSnap.Core.Models;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for restoring file system snapshots.
/// </summary>
public class RestorationService : IRestorationService
{
    /// <inheritdoc />
    public async Task RestoreSnapshot(SystemSnapshot snapshot, string targetPath)
    {
        if (snapshot.RootDirectory == null)
            throw new SnapshotException("Root directory snapshot is null.");

        if (!Directory.Exists(targetPath))
            throw new SnapshotException($"Target directory not found: {targetPath}");

        await RestoreDirectory(snapshot.RootDirectory, targetPath);
    }

    private static async Task RestoreDirectory(DirectorySnapshot directorySnapshot, string targetPath)
    {
        if (directorySnapshot.Path == null)
            throw new SnapshotException("Directory snapshot path is null.");

        var targetDir = Path.Combine(targetPath, Path.GetFileName(directorySnapshot.Path));
        Directory.CreateDirectory(targetDir);

        foreach (var file in directorySnapshot.Files)
        {
            await RestoreFile(file, targetDir);
        }

        foreach (var dir in directorySnapshot.Directories)
        {
            await RestoreDirectory(dir, targetDir);
        }
    }

    private static async Task<FileInfo> RestoreFile(FileSnapshot fileSnapshot, string targetPath)
    {
        if (fileSnapshot.Path == null)
            throw new SnapshotException("File snapshot path is null.");

        if (fileSnapshot.Content == null)
            throw new SnapshotException("File snapshot content is null.");

        var targetFile = Path.Combine(targetPath, Path.GetFileName(fileSnapshot.Path));
        await File.WriteAllBytesAsync(targetFile, fileSnapshot.Content);

        return new FileInfo(targetFile)
        {
            Attributes = fileSnapshot.Attributes,
            CreationTimeUtc = fileSnapshot.CreatedAt,
            LastWriteTimeUtc = fileSnapshot.LastModified
        };
    }
}
