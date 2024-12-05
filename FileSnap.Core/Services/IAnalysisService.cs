using FileSnap.Core.Models;

namespace FileSnap.Core.Services;

/// <summary>
/// Defines methods for analyzing file system snapshots.
/// </summary>
public interface IAnalysisService
{
    /// <summary>
    /// Analyzes a snapshot asynchronously.
    /// </summary>
    /// <param name="snapshot">The system snapshot to analyze.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Dictionary<string, string>> AnalyzeSnapshotAsync(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the count of files and directories in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>A tuple containing the file count and directory count.</returns>
    (int fileCount, int directoryCount) GetFileAndDirectoryCount(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the total size of files in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>The total size of files in bytes.</returns>
    long GetTotalFileSize(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the average size of files in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>The average file size in bytes.</returns>
    double GetAverageFileSize(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the largest and smallest files in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>A tuple containing the largest and smallest file snapshots.</returns>
    (FileSnapshot largestFile, FileSnapshot smallestFile) GetLargestAndSmallestFiles(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the count of files by their types in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>A dictionary with file extensions as keys and their counts as values.</returns>
    Dictionary<string, int> GetFileTypeCount(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the total size of files by their types in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>A dictionary with file extensions as keys and their total sizes as values.</returns>
    Dictionary<string, long> GetFileTypeSize(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the most common file type in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>The most common file extension.</returns>
    string GetMostCommonFileType(SystemSnapshot snapshot);

    /// <summary>
    /// Gets the count of added, deleted, and modified files between two snapshots.
    /// </summary>
    /// <param name="difference">The snapshot difference.</param>
    /// <returns>A tuple containing the count of added, deleted, and modified files.</returns>
    (int addedFiles, int deletedFiles, int modifiedFiles) GetFileChanges(SnapshotDifference difference);

    /// <summary>
    /// Gets the count of added, deleted, and modified directories between two snapshots.
    /// </summary>
    /// <param name="difference">The snapshot difference.</param>
    /// <returns>A tuple containing the count of added, deleted, and modified directories.</returns>
    (int addedDirectories, int deletedDirectories, int modifiedDirectories) GetDirectoryChanges(SnapshotDifference difference);

    /// <summary>
    /// Gets the modification frequency of files in the snapshot.
    /// </summary>
    /// <param name="snapshot">The system snapshot.</param>
    /// <returns>A dictionary with file paths as keys and their modification frequencies as values.</returns>
    Dictionary<string, int> GetFileModificationFrequency(SystemSnapshot snapshot);
}
