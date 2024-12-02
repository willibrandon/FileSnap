using FileSnap.Core.Exceptions;
using FileSnap.Core.Services;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace FileSnap.Tests;

public class SnapshotServiceTests : IDisposable
{
    private readonly SnapshotService _snapshotService;
    private readonly SnapshotService _snapshotServiceWithCompression;
    private readonly string _testBasePath;

    public SnapshotServiceTests()
    {
        var hashingService = new HashingService();
        _snapshotService = new SnapshotService(hashingService);
        _snapshotServiceWithCompression = new SnapshotService(hashingService, true);
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
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(dirPath, snapshot.BasePath);
        Assert.Empty(snapshot.RootDirectory!.Files);
        Assert.Empty(snapshot.RootDirectory.Directories);
    }

    [Fact]
    public async Task CaptureSnapshot_WithFiles_ReturnsValidSnapshot()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "DirWithFiles");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file1.txt"), "This is a test file.");
        File.WriteAllText(Path.Combine(dirPath, "file2.txt"), "This is another test file.");

        // Act
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);

        // Debugging information
        foreach (var file in snapshot.RootDirectory!.Files)
        {
            Console.WriteLine($"File: {file.Path}");
        }

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(dirPath, snapshot.BasePath);
        Assert.Equal(2, snapshot.RootDirectory!.Files.Count);
        Assert.Equal("file1.txt", Path.GetFileName(snapshot.RootDirectory.Files[0].Path));
        Assert.Equal("file2.txt", Path.GetFileName(snapshot.RootDirectory.Files[1].Path));
    }

    [Fact]
    public async Task CaptureSnapshot_WithSubDirectories_ReturnsValidSnapshot()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "DirWithSubDirs");
        Directory.CreateDirectory(dirPath);
        var subDirPath = Path.Combine(dirPath, "SubDir");
        Directory.CreateDirectory(subDirPath);
        File.WriteAllText(Path.Combine(subDirPath, "file1.txt"), "This is a test file.");

        // Act
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(dirPath, snapshot.BasePath);
        Assert.Single(snapshot.RootDirectory!.Directories);
        Assert.Equal("SubDir", Path.GetFileName(snapshot.RootDirectory.Directories[0].Path));
        Assert.Single(snapshot.RootDirectory.Directories[0].Files);
        Assert.Equal("file1.txt", Path.GetFileName(snapshot.RootDirectory.Directories[0].Files[0].Path));
    }

    [Fact]
    public async Task CaptureSnapshot_LargeDirectoryStructure_ShouldWorkCorrectly()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "LargeDir");
        Directory.CreateDirectory(dirPath);
        for (int i = 0; i < 100; i++)
        {
            File.WriteAllText(Path.Combine(dirPath, $"file{i}.txt"), "This is a test file.");
        }

        // Act
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);

        // Assert
        Assert.NotNull(snapshot);
        Assert.Equal(dirPath, snapshot.BasePath);
        Assert.Equal(100, snapshot.RootDirectory!.Files.Count);
    }

    [Fact]
    public void DefaultConstructor_UsesDefaultHashingServiceAndCompressionDisabled()
    {
        // Arrange & Act
        var snapshotService = new SnapshotService();

        // Assert
        Assert.NotNull(snapshotService);
        Assert.IsType<SnapshotService>(snapshotService);

        // Use reflection to check the private fields
        var hashingServiceField = typeof(SnapshotService).GetField(
            "_hashingService", BindingFlags.NonPublic | BindingFlags.Instance);
        var hashingServiceInstance = hashingServiceField?.GetValue(snapshotService);

        var isCompressionEnabledField = typeof(SnapshotService).GetField(
            "_isCompressionEnabled", BindingFlags.NonPublic | BindingFlags.Instance);
        var isCompressionEnabled = (bool?)isCompressionEnabledField?.GetValue(snapshotService);

        Assert.NotNull(hashingServiceInstance);
        Assert.IsType<HashingService>(hashingServiceInstance);
        Assert.False(isCompressionEnabled);
    }

    [Fact]
    public async Task SaveSnapshot_AppendsJsonExtensionIfNotSpecified()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "EmptyDir");
        Directory.CreateDirectory(dirPath);
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var outputPath = Path.Combine(_testBasePath, "snapshot");

        // Act
        await _snapshotService.SaveSnapshotAsync(snapshot, outputPath);

        // Assert
        var expectedPath = outputPath + ".json";
        Assert.True(File.Exists(expectedPath));
    }

    [Fact]
    public async Task SaveSnapshot_UsesSpecifiedExtension()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "EmptyDir");
        Directory.CreateDirectory(dirPath);
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var outputPath = Path.Combine(_testBasePath, "snapshot.custom");

        // Act
        await _snapshotService.SaveSnapshotAsync(snapshot, outputPath);

        // Assert
        Assert.True(File.Exists(outputPath));
    }

    [Fact]
    public async Task SaveAndLoadSnapshot_WithoutCompression_ShouldWorkCorrectly()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "DirWithFiles");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file1.txt"), "This is a test file.");
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var outputPath = Path.Combine(_testBasePath, "snapshot.json");

        // Act
        await _snapshotService.SaveSnapshotAsync(snapshot, outputPath);
        var loadedSnapshot = await _snapshotService.LoadSnapshotAsync(outputPath);

        // Assert
        Assert.NotNull(loadedSnapshot);
        Assert.Equal(snapshot.Id, loadedSnapshot.Id);
        Assert.Equal(snapshot.BasePath, loadedSnapshot.BasePath);
        Assert.Single(loadedSnapshot.RootDirectory!.Files);
        Assert.Equal("file1.txt", Path.GetFileName(loadedSnapshot.RootDirectory.Files[0].Path));
        Assert.Equal("This is a test file.", Encoding.UTF8.GetString(loadedSnapshot.RootDirectory.Files[0].Content!));
    }

    [Fact]
    public async Task SaveAndLoadSnapshot_WithCompressionEnabled_ShouldWorkCorrectly()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "DirWithFiles");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file1.txt"), "This is a test file.");
        var snapshot = await _snapshotServiceWithCompression.CaptureSnapshotAsync(dirPath);
        var outputPath = Path.Combine(_testBasePath, "snapshot.json");

        // Act
        await _snapshotServiceWithCompression.SaveSnapshotAsync(snapshot, outputPath);
        var loadedSnapshot = await _snapshotServiceWithCompression.LoadSnapshotAsync(outputPath);

        // Assert
        Assert.NotNull(loadedSnapshot);
        Assert.Equal(snapshot.Id, loadedSnapshot.Id);
        Assert.Equal(snapshot.BasePath, loadedSnapshot.BasePath);
        Assert.Single(loadedSnapshot.RootDirectory!.Files);
        Assert.Equal("file1.txt", Path.GetFileName(loadedSnapshot.RootDirectory.Files[0].Path));
        Assert.Equal("This is a test file.", Encoding.UTF8.GetString(loadedSnapshot.RootDirectory.Files[0].Content!));
    }

    [Fact]
    public async Task SaveAndLoadSnapshot_WithCustomCompressionService_ShouldWorkCorrectly()
    {
        // Arrange
        var customCompressionService = new CustomCompressionService();
        var snapshotServiceWithCustomCompression = new SnapshotService(new HashingService(), true, customCompressionService);
        var dirPath = Path.Combine(_testBasePath, "DirWithFiles");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file1.txt"), "This is a test file.");
        var snapshot = await snapshotServiceWithCustomCompression.CaptureSnapshotAsync(dirPath);
        var outputPath = Path.Combine(_testBasePath, "snapshot.json");

        // Act
        await snapshotServiceWithCustomCompression.SaveSnapshotAsync(snapshot, outputPath);
        var loadedSnapshot = await snapshotServiceWithCustomCompression.LoadSnapshotAsync(outputPath);

        // Assert
        Assert.NotNull(loadedSnapshot);
        Assert.Equal(snapshot.Id, loadedSnapshot.Id);
        Assert.Equal(snapshot.BasePath, loadedSnapshot.BasePath);
        Assert.Single(loadedSnapshot.RootDirectory!.Files);
        Assert.Equal("file1.txt", Path.GetFileName(loadedSnapshot.RootDirectory.Files[0].Path));
        Assert.Equal("This is a test file.", Encoding.UTF8.GetString(loadedSnapshot.RootDirectory.Files[0].Content!));
    }

    [Fact]
    public async Task SaveAndLoadSnapshot_RoundTrip_PreservesData()
    {
        // Arrange
        var dirPath = Path.Combine(_testBasePath, "SaveLoad");
        var outputPath = Path.Combine(_testBasePath, "test.json");
        CreateTestDirectoryStructure(dirPath, 2, 2);
        var originalSnapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);

        // Act
        await _snapshotService.SaveSnapshotAsync(originalSnapshot, outputPath);
        var loadedSnapshot = await _snapshotService.LoadSnapshotAsync(outputPath);

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
            _snapshotService.CaptureSnapshotAsync(nonExistentPath));
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

public class CustomCompressionService : ICompressionService
{
    public byte[] Compress(byte[] data)
    {
        using var msi = new MemoryStream(data);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(mso, CompressionMode.Compress))
        {
            msi.CopyTo(gs);
        }
        return mso.ToArray();
    }

    public byte[] Decompress(byte[] data)
    {
        using var msi = new MemoryStream(data);
        using var mso = new MemoryStream();
        using (var gs = new GZipStream(msi, CompressionMode.Decompress))
        {
            gs.CopyTo(mso);
        }
        return mso.ToArray();
    }
}
