using FileSnap.Core.Models;

namespace FileSnap.Core.Services;

/// <summary>
/// Provides methods for analyzing file system snapshots.
/// </summary>
public class AnalysisService : IAnalysisService
{
    /// <inheritdoc />
    public async Task<Dictionary<string, string>> AnalyzeSnapshotAsync(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var insights = new Dictionary<string, string>();

        var fileCount = 0;
        var directoryCount = 0;

        void AnalyzeDirectory(DirectorySnapshot directory)
        {
            fileCount += directory.Files.Count;
            directoryCount += directory.Directories.Count;

            foreach (var subDirectory in directory.Directories)
            {
                AnalyzeDirectory(subDirectory);
            }
        }

        if (snapshot.RootDirectory != null)
        {
            AnalyzeDirectory(snapshot.RootDirectory);
        }

        insights["FileCount"] = fileCount.ToString();
        insights["DirectoryCount"] = directoryCount.ToString();

        // Add more analysis as needed

        return await Task.FromResult(insights);
    }

    /// <inheritdoc />
    public (int fileCount, int directoryCount) GetFileAndDirectoryCount(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        int fileCount = 0;
        int directoryCount = 0;

        TraverseDirectory(snapshot.RootDirectory, ref fileCount, ref directoryCount);

        return (fileCount, directoryCount);
    }

    /// <inheritdoc />
    public long GetTotalFileSize(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        long totalSize = 0;

        TraverseDirectory(snapshot.RootDirectory, ref totalSize);

        return totalSize;
    }

    /// <inheritdoc />
    public double GetAverageFileSize(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        long totalSize = 0;
        int fileCount = 0;

        TraverseDirectory(snapshot.RootDirectory, ref totalSize, ref fileCount);

        return fileCount == 0 ? 0 : (double)totalSize / fileCount;
    }

    /// <inheritdoc />
    public (FileSnapshot largestFile, FileSnapshot smallestFile) GetLargestAndSmallestFiles(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        FileSnapshot? largestFile = null;
        FileSnapshot? smallestFile = null;

        TraverseDirectory(snapshot.RootDirectory, ref largestFile, ref smallestFile);

        return (largestFile!, smallestFile!);
    }

    /// <inheritdoc />
    public Dictionary<string, int> GetFileTypeCount(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var fileTypeCount = new Dictionary<string, int>();

        TraverseDirectory(snapshot.RootDirectory, fileTypeCount);

        return fileTypeCount;
    }

    /// <inheritdoc />
    public Dictionary<string, long> GetFileTypeSize(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var fileTypeSize = new Dictionary<string, long>();

        TraverseDirectory(snapshot.RootDirectory, fileTypeSize);

        return fileTypeSize;
    }

    /// <inheritdoc />
    public string GetMostCommonFileType(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var fileTypeCount = GetFileTypeCount(snapshot);

        return fileTypeCount.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
    }

    /// <inheritdoc />
    public (int addedFiles, int deletedFiles, int modifiedFiles) GetFileChanges(SnapshotDifference difference)
    {
        return (difference.NewFiles.Count, difference.DeletedFiles.Count, difference.ModifiedFiles.Count);
    }

    /// <inheritdoc />
    public (int addedDirectories, int deletedDirectories, int modifiedDirectories) GetDirectoryChanges(SnapshotDifference difference)
    {
        return (difference.NewDirectories.Count, difference.DeletedDirectories.Count, difference.ModifiedDirectories.Count);
    }

    /// <inheritdoc />
    public Dictionary<string, int> GetFileModificationFrequency(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var modificationFrequency = new Dictionary<string, int>();

        TraverseDirectory(snapshot.RootDirectory, modificationFrequency);

        return modificationFrequency;
    }

    private void TraverseDirectory(DirectorySnapshot directory, ref int fileCount, ref int directoryCount)
    {
        directoryCount++;

        foreach (var file in directory.Files)
        {
            fileCount++;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, ref fileCount, ref directoryCount);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, ref long totalSize)
    {
        foreach (var file in directory.Files)
        {
            totalSize += file.Size;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, ref totalSize);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, ref long totalSize, ref int fileCount)
    {
        foreach (var file in directory.Files)
        {
            totalSize += file.Size;
            fileCount++;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, ref totalSize, ref fileCount);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, ref FileSnapshot? largestFile, ref FileSnapshot? smallestFile)
    {
        foreach (var file in directory.Files)
        {
            if (largestFile == null || file.Size > largestFile.Size)
            {
                largestFile = file;
            }

            if (smallestFile == null || file.Size < smallestFile.Size)
            {
                smallestFile = file;
            }
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, ref largestFile, ref smallestFile);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, Dictionary<string, int> fileTypeCount)
    {
        foreach (var file in directory.Files)
        {
            var extension = Path.GetExtension(file.Path)?.ToLower() ?? string.Empty;

            if (!fileTypeCount.ContainsKey(extension))
            {
                fileTypeCount[extension] = 0;
            }

            fileTypeCount[extension]++;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, fileTypeCount);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, Dictionary<string, long> fileTypeSize)
    {
        foreach (var file in directory.Files)
        {
            var extension = Path.GetExtension(file.Path)?.ToLower() ?? string.Empty;

            if (!fileTypeSize.ContainsKey(extension))
            {
                fileTypeSize[extension] = 0;
            }

            fileTypeSize[extension] += file.Size;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, fileTypeSize);
        }
    }
}
