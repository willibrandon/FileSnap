using FileSnap.Core.Models;
using FileSnap.Core.Services;

namespace FileSnap.Tests;

public class AnalysisServiceTests
{
    private readonly AnalysisService _analysisService;

    public AnalysisServiceTests()
    {
        _analysisService = new AnalysisService();
    }

    [Fact]
    public void GetFileAndDirectoryCount_ShouldReturnCorrectCounts()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var (fileCount, directoryCount) = _analysisService.GetFileAndDirectoryCount(snapshot);

        // Assert
        Assert.Equal(3, fileCount);
        Assert.Equal(2, directoryCount);
    }

    [Fact]
    public void GetTotalFileSize_ShouldReturnCorrectSize()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var totalSize = _analysisService.GetTotalFileSize(snapshot);

        // Assert
        Assert.Equal(600, totalSize);
    }

    [Fact]
    public void GetAverageFileSize_ShouldReturnCorrectAverage()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var averageSize = _analysisService.GetAverageFileSize(snapshot);

        // Assert
        Assert.Equal(200, averageSize);
    }

    [Fact]
    public void GetLargestAndSmallestFiles_ShouldReturnCorrectFiles()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var (largestFile, smallestFile) = _analysisService.GetLargestAndSmallestFiles(snapshot);

        // Assert
        Assert.Equal("file3.txt", largestFile.Path);
        Assert.Equal("file1.txt", smallestFile.Path);
    }

    [Fact]
    public void GetFileTypeCount_ShouldReturnCorrectCounts()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var fileTypeCount = _analysisService.GetFileTypeCount(snapshot);

        // Assert
        Assert.Equal(2, fileTypeCount[".txt"]);
        Assert.Equal(1, fileTypeCount[".jpg"]);
    }

    [Fact]
    public void GetFileTypeSize_ShouldReturnCorrectSizes()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var fileTypeSize = _analysisService.GetFileTypeSize(snapshot);

        // Assert
        Assert.Equal(300, fileTypeSize[".txt"]);
        Assert.Equal(300, fileTypeSize[".jpg"]);
    }

    [Fact]
    public void GetMostCommonFileType_ShouldReturnCorrectType()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var mostCommonFileType = _analysisService.GetMostCommonFileType(snapshot);

        // Assert
        Assert.Equal(".txt", mostCommonFileType);
    }

    [Fact]
    public void GetFileChanges_ShouldReturnCorrectCounts()
    {
        // Arrange
        var difference = CreateTestDifference();

        // Act
        var (addedFiles, deletedFiles, modifiedFiles) = _analysisService.GetFileChanges(difference);

        // Assert
        Assert.Equal(1, addedFiles);
        Assert.Equal(1, deletedFiles);
        Assert.Equal(1, modifiedFiles);
    }

    [Fact]
    public void GetDirectoryChanges_ShouldReturnCorrectCounts()
    {
        // Arrange
        var difference = CreateTestDifference();

        // Act
        var (addedDirectories, deletedDirectories, modifiedDirectories) = _analysisService.GetDirectoryChanges(difference);

        // Assert
        Assert.Equal(1, addedDirectories);
        Assert.Equal(1, deletedDirectories);
        Assert.Equal(1, modifiedDirectories);
    }

    [Fact]
    public void GetFileModificationFrequency_ShouldReturnCorrectFrequencies()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var modificationFrequency = _analysisService.GetFileModificationFrequency(snapshot);

        // Assert
        Assert.Equal(1, modificationFrequency["file1.txt"]);
        Assert.Equal(1, modificationFrequency["file2.txt"]);
        Assert.Equal(1, modificationFrequency["file3.txt"]);
    }

    [Fact]
    public async Task AnalyzeSnapshotAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        await _analysisService.AnalyzeSnapshotAsync(snapshot);

        // Assert
        Assert.True(true); // If no exception is thrown, the test passes
    }

    private static SystemSnapshot CreateTestSnapshot()
    {
        return new SystemSnapshot
        {
            RootDirectory = new DirectorySnapshot
            {
                Files = new List<FileSnapshot>
                {
                    new FileSnapshot { Path = "file1.txt", Size = 100 },
                    new FileSnapshot { Path = "file2.txt", Size = 200 },
                    new FileSnapshot { Path = "file3.jpg", Size = 300 }
                },
                Directories = new List<DirectorySnapshot>
                {
                    new DirectorySnapshot
                    {
                        Files = new List<FileSnapshot>
                        {
                            new FileSnapshot { Path = "subdir/file4.txt", Size = 400 }
                        }
                    }
                }
            }
        };
    }

    private static SnapshotDifference CreateTestDifference()
    {
        return new SnapshotDifference
        {
            NewFiles = new List<FileSnapshot> { new FileSnapshot { Path = "newfile.txt" } },
            DeletedFiles = new List<FileSnapshot> { new FileSnapshot { Path = "deletedfile.txt" } },
            ModifiedFiles = new List<(FileSnapshot, FileSnapshot)>
            {
                (new FileSnapshot { Path = "modifiedfile.txt" }, new FileSnapshot { Path = "modifiedfile.txt" })
            },
            NewDirectories = new List<DirectorySnapshot> { new DirectorySnapshot { Path = "newdir" } },
            DeletedDirectories = new List<DirectorySnapshot> { new DirectorySnapshot { Path = "deleteddir" } },
            ModifiedDirectories = new List<(DirectorySnapshot, DirectorySnapshot)>
            {
                (new DirectorySnapshot { Path = "modifieddir" }, new DirectorySnapshot { Path = "modifieddir" })
            }
        };
    }
}
