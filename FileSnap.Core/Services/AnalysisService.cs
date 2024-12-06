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
    public double GetAverageFileSize(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        long totalSize = 0;
        int fileCount = 0;

        TraverseDirectory(snapshot.RootDirectory, ref totalSize, ref fileCount);

        return fileCount == 0 ? 0 : (double)totalSize / fileCount;
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

    /// <summary>
    /// Retrieves metadata for a given directory.
    /// </summary>
    public DirectoryMetadata GetDirectoryMetadata(DirectorySnapshot directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        return directory.Metadata;
    }

    /// <summary>
    /// Analyzes directory size distribution in the snapshot.
    /// </summary>
    public Dictionary<long, int> GetDirectorySizeDistribution(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var sizeDistribution = new Dictionary<long, int>();

        TraverseDirectory(snapshot.RootDirectory, sizeDistribution, true);

        return sizeDistribution;
    }

    /// <summary>
    /// Analyzes file attribute distribution in the snapshot.
    /// </summary>
    public Dictionary<FileAttributes, int> GetFileAttributeDistribution(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var attributeDistribution = new Dictionary<FileAttributes, int>();

        TraverseDirectory(snapshot.RootDirectory, attributeDistribution);

        return attributeDistribution;
    }

    /// <inheritdoc />
    public (int addedFiles, int deletedFiles, int modifiedFiles) GetFileChanges(SnapshotDifference difference)
    {
        return (difference.NewFiles.Count, difference.DeletedFiles.Count, difference.ModifiedFiles.Count);
    }

    /// <summary>
    /// Retrieves metadata for a given file.
    /// </summary>
    public FileMetadata GetFileMetadata(FileSnapshot file)
    {
        ArgumentNullException.ThrowIfNull(file);

        return file.Metadata;
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

    /// <summary>
    /// Analyzes file size distribution in the snapshot.
    /// </summary>
    public Dictionary<long, int> GetFileSizeDistribution(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var sizeDistribution = new Dictionary<long, int>();

        TraverseDirectory(snapshot.RootDirectory, sizeDistribution);

        return sizeDistribution;
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
    public (FileSnapshot largestFile, FileSnapshot smallestFile) GetLargestAndSmallestFiles(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        FileSnapshot? largestFile = null;
        FileSnapshot? smallestFile = null;

        TraverseDirectory(snapshot.RootDirectory, ref largestFile, ref smallestFile);

        return (largestFile!, smallestFile!);
    }

    /// <inheritdoc />
    public string GetMostCommonFileType(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        var fileTypeCount = GetFileTypeCount(snapshot);

        return fileTypeCount.OrderByDescending(kvp => kvp.Value).FirstOrDefault().Key;
    }

    /// <inheritdoc />
    public long GetTotalFileSize(SystemSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot.RootDirectory);

        long totalSize = 0;

        TraverseDirectory(snapshot.RootDirectory, ref totalSize);

        return totalSize;
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
            totalSize += file.Metadata.Size;
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
            totalSize += file.Metadata.Size;
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
            if (largestFile == null || file.Metadata.Size > largestFile.Metadata.Size)
            {
                largestFile = file;
            }

            if (smallestFile == null || file.Metadata.Size < smallestFile.Metadata.Size)
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
            var extension = Path.GetExtension(file.Metadata.Path)?.ToLower() ?? string.Empty;

            if (!fileTypeCount.TryGetValue(extension, out int value))
            {
                value = 0;
                fileTypeCount[extension] = value;
            }

            fileTypeCount[extension] = ++value;
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
            var extension = Path.GetExtension(file.Metadata.Path)?.ToLower() ?? string.Empty;

            if (!fileTypeSize.ContainsKey(extension))
            {
                fileTypeSize[extension] = 0;
            }

            fileTypeSize[extension] += file.Metadata.Size;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, fileTypeSize);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, Dictionary<long, int> sizeDistribution)
    {
        foreach (var file in directory.Files)
        {
            if (!sizeDistribution.TryGetValue(file.Metadata.Size, out int value))
            {
                value = 0;
                sizeDistribution[file.Metadata.Size] = value;
            }

            sizeDistribution[file.Metadata.Size] = ++value;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, sizeDistribution);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, Dictionary<long, int> sizeDistribution, bool isDirectory)
    {
       if (isDirectory)
        {
            long directorySize = directory.Files.Sum(file => file.Metadata.Size);
            if (!sizeDistribution.TryGetValue(directorySize, out int value))
            {
                value = 0;
                sizeDistribution[directorySize] = value;
            }

            sizeDistribution[directorySize] = ++value;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, sizeDistribution, isDirectory);
        }
    }

    private void TraverseDirectory(DirectorySnapshot directory, Dictionary<FileAttributes, int> attributeDistribution)
    {
        foreach (var file in directory.Files)
        {
            if (!attributeDistribution.TryGetValue(file.Metadata.Attributes, out int value))
            {
                value = 0;
                attributeDistribution[file.Metadata.Attributes] = value;
            }

            attributeDistribution[file.Metadata.Attributes] = ++value;
        }

        foreach (var subDirectory in directory.Directories)
        {
            TraverseDirectory(subDirectory, attributeDistribution);
        }
    }
}
