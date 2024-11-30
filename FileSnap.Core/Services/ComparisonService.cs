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
        var beforeFiles = before.Files
            .Where(f => f.Path != null)
            .ToDictionary(f => f.Path!);

        var afterFiles = after.Files
            .Where(f => f.Path != null)
            .ToDictionary(f => f.Path!);

        foreach (var file in beforeFiles.Values)
        {
            // Ensure file.Path is not null.
            if (file.Path == null) continue;

            if (!afterFiles.TryGetValue(file.Path, out FileSnapshot? value))
            {
                difference.DeletedFiles.Add(file);
            }
            else if (value.Hash != file.Hash)
            {
                difference.ModifiedFiles.Add((file, value));
            }
        }

        foreach (var file in afterFiles.Values)
        {
            // Ensure file.Path is not null.
            if (file.Path == null) continue;

            if (!beforeFiles.ContainsKey(file.Path))
            {
                difference.NewFiles.Add(file);
            }
        }

        foreach (var beforeDir in before.Directories)
        {
            var afterDir = after.Directories.FirstOrDefault(d => d.Path == beforeDir.Path);

            if (afterDir != null)
            {
                CompareDirectories(beforeDir, afterDir, difference);
            }
        }
    }
}
