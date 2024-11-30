using FileSnap.Core.Exceptions;
using FileSnap.Core.Services;

namespace FileSnap.Tests;

public class SnapshotServiceTests : IDisposable
{
    private readonly SnapshotService _snapshotService;
    private readonly string _testBasePath;

    public SnapshotServiceTests()
    {
        var hashingService = new HashingService();
        _snapshotService = new SnapshotService(hashingService);
        _testBasePath = Path.Combine(Path.GetTempPath(), "FileSnapTests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testBasePath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testBasePath))
        {
            Directory.Delete(_testBasePath, true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task CaptureSnapshot_EmptyDirectory_ReturnsValidSnapshot()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "EmptyDir");
        Directory.CreateDirectory(dirPath);

        // Act
        var snapshot = await _snapshotService.CaptureSnapshot(dirPath);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(dirPath, snapshot.BasePath);
        Assert.Empty(snapshot.RootDirectory!.Files);
        Assert.Empty(snapshot.RootDirectory.Directories);
    }

    [Fact]
    public async Task CaptureSnapshot_LargeDirectoryStructure_ShouldWorkCorrectly()
    {
        // Arrange
        var basePath = Path.Combine(_testBasePath, "LargeDirectoryStructure");
        CreateLargeDirectoryStructure(basePath, 3, 3);

        try
        {
            // Act
            var snapshot = await _snapshotService.CaptureSnapshot(basePath);
            var savePath = Path.Combine(_testBasePath, "test.fsnap");
            await _snapshotService.SaveSnapshot(snapshot, savePath);

            // Assert
            Assert.NotNull(snapshot);
            Assert.Equal(basePath, snapshot.BasePath);
            Assert.NotNull(snapshot.RootDirectory);

            // Root should have 3 directories but no files.
            Assert.Equal(3, snapshot.RootDirectory.Directories.Count);
            Assert.Empty(snapshot.RootDirectory.Files);

            // Verify first level directories.
            foreach (var dir in snapshot.RootDirectory.Directories)
            {
                Assert.Equal(3, dir.Files.Count);
                Assert.NotNull(dir.Files[0].Content);
                Assert.NotNull(dir.Files[0].Hash);
                Assert.True(dir.Files[0].Size > 0);

                // Verify subdirectories.
                Assert.Equal(3, dir.Directories.Count);
                foreach (var subdir in dir.Directories)
                {
                    Assert.Equal(3, subdir.Files.Count);
                    Assert.All(subdir.Files, file =>
                    {
                        Assert.NotNull(file.Content);
                        Assert.NotNull(file.Hash);
                        Assert.True(file.Size > 0);
                    });
                }
            }

            // Verify we can load the saved snapshot.
            var loadedSnapshot = await _snapshotService.LoadSnapshot(savePath);
            Assert.Equal(snapshot.Id, loadedSnapshot.Id);
            Assert.Equal(snapshot.BasePath, loadedSnapshot.BasePath);
            Assert.Equal(3, loadedSnapshot.RootDirectory!.Directories.Count);
        }
        finally
        {
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
        }
    }

    [Fact]
    public async Task CaptureSnapshot_WithFiles_ReturnsValidSnapshot()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "WithFiles");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "test.txt"), "test content");

        // Act
        var snapshot = await _snapshotService.CaptureSnapshot(dirPath);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Single(snapshot.RootDirectory!.Files);
        Assert.NotNull(snapshot.RootDirectory.Files[0].Content);
        Assert.NotNull(snapshot.RootDirectory.Files[0].Hash);
    }

    [Fact]
    public async Task CaptureSnapshot_WithSubDirectories_ReturnsValidSnapshot()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "WithSubDirs");
        CreateTestDirectoryStructure(dirPath, 3, 3);

        // Act
        var snapshot = await _snapshotService.CaptureSnapshot(dirPath);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(3, snapshot.RootDirectory!.Directories.Count);
        Assert.All(snapshot.RootDirectory.Directories, dir =>
            Assert.Equal(3, dir.Files.Count));
    }

    [Fact]
    public async Task SaveAndLoadSnapshot_RoundTrip_PreservesData()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "SaveLoad");
        var outputPath = Path.Combine(_testBasePath, "test.fsnap");
        CreateTestDirectoryStructure(dirPath, 2, 2);
        var originalSnapshot = await _snapshotService.CaptureSnapshot(dirPath);

        // Act
        await _snapshotService.SaveSnapshot(originalSnapshot, outputPath);
        var loadedSnapshot = await _snapshotService.LoadSnapshot(outputPath);

        // Assert
        Assert.Equal(originalSnapshot.Id, loadedSnapshot.Id);
        Assert.Equal(originalSnapshot.BasePath, loadedSnapshot.BasePath);
        Assert.Equal(
            originalSnapshot.RootDirectory!.Files.Count,
            loadedSnapshot.RootDirectory!.Files.Count);
    }

    [Fact]
    public async Task CaptureSnapshot_DirectoryNotFound_ThrowsException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(_testBasePath, "NonExistent");

        // Act & Assert
        await Assert.ThrowsAsync<SnapshotException>(() =>
            _snapshotService.CaptureSnapshot(nonExistentPath));
    }

    private static void CreateLargeDirectoryStructure(string basePath, int depth, int breadth)
    {
        Directory.CreateDirectory(basePath);

        for (int i = 0; i < breadth; i++)
        {
            var dirPath = Path.Combine(basePath, $"Dir{i}");
            Directory.CreateDirectory(dirPath);

            for (int j = 0; j < breadth; j++)
            {
                var filePath = Path.Combine(dirPath, $"File{j}.txt");
                File.WriteAllText(filePath, $"Test content for file {j} in directory {i}");
            }

            if (depth > 1)
            {
                CreateLargeDirectoryStructure(dirPath, depth - 1, breadth);
            }
        }
    }

    private static void CreateTestDirectoryStructure(string basePath, int depth, int breadth)
    {
        Directory.CreateDirectory(basePath);

        for (int i = 0; i < breadth; i++)
        {
            var dirPath = Path.Combine(basePath, $"Dir{i}");
            Directory.CreateDirectory(dirPath);

            for (int j = 0; j < breadth; j++)
            {
                var filePath = Path.Combine(dirPath, $"File{j}.txt");
                File.WriteAllText(filePath, $"Test content {j}");
            }

            if (depth > 1)
            {
                CreateTestDirectoryStructure(dirPath, depth - 1, breadth);
            }
        }
    }
}