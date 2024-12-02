using FileSnap.Core.Models;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for comparing file system snapshots.
/// </summary>
public class ComparisonService : IComparisonService
{
    /// <inheritdoc />
    public SnapshotDifference Compare(SystemSnapshot before, SystemSnapshot after)
    {
        if (before.RootDirectory == null)
        {
            throw new ArgumentNullException(nameof(before), "RootDirectory cannot be null");
        }

        if (after.RootDirectory == null)
        {
            throw new ArgumentNullException(nameof(after), "RootDirectory cannot be null");
        }

        var difference = new SnapshotDifference
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            BeforeSnapshotId = before.Id,
            AfterSnapshotId = after.Id
        };

        CompareDirectories(before.RootDirectory, after.RootDirectory, difference);
        return difference;
    }

    private static void CompareDirectories(DirectorySnapshot before, DirectorySnapshot after, SnapshotDifference difference)
    {
        // Compare files.
        var beforeFiles = before.Files
            .Where(f => f.Path != null)
            .ToDictionary(f => f.Path!);
        var afterFiles = after.Files
            .Where(f => f.Path != null)
            .ToDictionary(f => f.Path!);

        foreach (var afterFile in afterFiles.Values)
        {
            if (!beforeFiles.TryGetValue(afterFile.Path!, out var beforeFile))
            {
                difference.NewFiles.Add(afterFile);
            }
            else if (beforeFile.Hash != afterFile.Hash)
            {
                difference.ModifiedFiles.Add((beforeFile, afterFile));
            }
        }

        foreach (var beforeFile in beforeFiles.Values)
        {
            if (!afterFiles.ContainsKey(beforeFile.Path!))
            {
                difference.DeletedFiles.Add(beforeFile);
            }
        }

        // Compare directories.
        var beforeDirs = before.Directories
            .Where(d => d.Path != null)
            .ToDictionary(d => d.Path!);
        var afterDirs = after.Directories
            .Where(d => d.Path != null)
            .ToDictionary(d => d.Path!);

        foreach (var afterDir in afterDirs.Values)
        {
            if (afterDir.Path == null)
            {
                continue;
            }

            if (!beforeDirs.TryGetValue(afterDir.Path, out var beforeDir))
            {
                difference.NewDirectories.Add(afterDir);
            }
            else
            {
                CompareDirectories(beforeDir, afterDir, difference);

                if (beforeDir.CreatedAt != afterDir.CreatedAt || beforeDir.Attributes != afterDir.Attributes)
                {
                    difference.ModifiedDirectories.Add((beforeDir, afterDir));
                }
            }
        }

        foreach (var beforeDir in beforeDirs.Values)
        {
            if (beforeDir.Path == null)
            {
                continue;
            }

            if (!afterDirs.ContainsKey(beforeDir.Path))
            {
                difference.DeletedDirectories.Add(beforeDir);
            }
        }
    }
}
