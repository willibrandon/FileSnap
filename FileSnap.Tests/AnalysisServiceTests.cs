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
        Assert.Equal(4, fileCount);
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
        Assert.Equal(1200, totalSize);
    }

    [Fact]
    public void GetAverageFileSize_ShouldReturnCorrectAverage()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var averageSize = _analysisService.GetAverageFileSize(snapshot);

        // Assert
        Assert.Equal(300, averageSize);
    }

    [Fact]
    public void GetLargestAndSmallestFiles_ShouldReturnCorrectFiles()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var (largestFile, smallestFile) = _analysisService.GetLargestAndSmallestFiles(snapshot);

        // Assert
        Assert.Equal("file3.jpg", largestFile.Metadata.Path);
        Assert.Equal("file1.txt", smallestFile.Metadata.Path);
    }

    [Fact]
    public void GetFileTypeCount_ShouldReturnCorrectCounts()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var fileTypeCount = _analysisService.GetFileTypeCount(snapshot);

        // Assert
        Assert.Equal(3, fileTypeCount[".txt"]);
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
        Assert.Equal(700, fileTypeSize[".txt"]);
        Assert.Equal(500, fileTypeSize[".jpg"]);
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
        Assert.Equal(3, modificationFrequency[".txt"]);
        Assert.Equal(1, modificationFrequency[".jpg"]);
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

    [Fact]
    public void GetFileMetadata_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var file = new FileSnapshot
        {
            Metadata = new FileMetadata
            {
                Path = "file1.txt",
                Size = 100,
                Hash = "hash1",
                LastModified = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Attributes = FileAttributes.Normal
            }
        };

        // Act
        var metadata = _analysisService.GetFileMetadata(file);

        // Assert
        Assert.Equal("file1.txt", metadata.Path);
        Assert.Equal(100, metadata.Size);
        Assert.Equal("hash1", metadata.Hash);
        Assert.Equal(file.Metadata.LastModified, metadata.LastModified);
        Assert.Equal(file.Metadata.CreatedAt, metadata.CreatedAt);
        Assert.Equal(FileAttributes.Normal, metadata.Attributes);
    }

    [Fact]
    public void GetDirectoryMetadata_ShouldReturnCorrectMetadata()
    {
        // Arrange
        var directory = new DirectorySnapshot
        {
            Metadata = new DirectoryMetadata
            {
                Path = "dir1",
                CreatedAt = DateTime.UtcNow,
                Attributes = FileAttributes.Directory
            }
        };

        // Act
        var metadata = _analysisService.GetDirectoryMetadata(directory);

        // Assert
        Assert.Equal("dir1", metadata.Path);
        Assert.Equal(directory.Metadata.CreatedAt, metadata.CreatedAt);
        Assert.Equal(FileAttributes.Directory, metadata.Attributes);
    }

    [Fact]
    public void GetFileSizeDistribution_ShouldReturnCorrectDistribution()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var sizeDistribution = _analysisService.GetFileSizeDistribution(snapshot);

        // Assert
        Assert.Equal(1, sizeDistribution[100]);
        Assert.Equal(1, sizeDistribution[200]);
        Assert.Equal(1, sizeDistribution[500]);
        Assert.Equal(1, sizeDistribution[400]);
    }

    [Fact]
    public void GetDirectorySizeDistribution_ShouldReturnCorrectDistribution()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var sizeDistribution = _analysisService.GetDirectorySizeDistribution(snapshot);

        // Assert
        Assert.Equal(1, sizeDistribution[700]);
        Assert.Equal(1, sizeDistribution[400]);
    }

    [Fact]
    public void GetFileAttributeDistribution_ShouldReturnCorrectDistribution()
    {
        // Arrange
        var snapshot = CreateTestSnapshot();

        // Act
        var attributeDistribution = _analysisService.GetFileAttributeDistribution(snapshot);

        // Assert
        Assert.Equal(4, attributeDistribution[FileAttributes.Normal]);
    }

    private static SystemSnapshot CreateTestSnapshot()
    {
        return new SystemSnapshot
        {
            RootDirectory = new DirectorySnapshot
            {
                Files =
                [
                    new FileSnapshot { Metadata = new FileMetadata { Path = "file1.txt", Size = 100, Attributes = FileAttributes.Normal } },
                    new FileSnapshot { Metadata = new FileMetadata { Path = "file2.txt", Size = 200, Attributes = FileAttributes.Normal } },
                    new FileSnapshot { Metadata = new FileMetadata { Path = "file3.jpg", Size = 500, Attributes = FileAttributes.Normal } }
                ],
                Directories =
                [
                    new DirectorySnapshot
                    {
                        Files =
                        [
                            new FileSnapshot { Metadata = new FileMetadata { Path = "subdir/file4.txt", Size = 400, Attributes = FileAttributes.Normal } }
                        ]
                    }
                ]
            }
        };
    }

    private static SnapshotDifference CreateTestDifference()
    {
        return new SnapshotDifference
        {
            NewFiles = [new FileSnapshot { Metadata = new FileMetadata { Path = "newfile.txt" } }],
            DeletedFiles = [new FileSnapshot { Metadata = new FileMetadata { Path = "deletedfile.txt" } }],
            ModifiedFiles = [(new FileSnapshot { Metadata = new FileMetadata { Path = "modifiedfile.txt" } }, new FileSnapshot { Metadata = new FileMetadata { Path = "modifiedfile.txt" } })],
            NewDirectories = [new DirectorySnapshot { Metadata = new DirectoryMetadata { Path = "newdir" } }],
            DeletedDirectories = [new DirectorySnapshot { Metadata = new DirectoryMetadata { Path = "deleteddir" } }],
            ModifiedDirectories = [ (new DirectorySnapshot { Metadata = new DirectoryMetadata { Path = "modifieddir" } }, new DirectorySnapshot { Metadata = new DirectoryMetadata { Path = "modifieddir" } })]
        };
    }
}
