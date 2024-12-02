using FileSnap.Core.Models;
using FileSnap.Core.Services;
using System.Text;

namespace FileSnap.Tests;

public class RestorationServiceTests : IDisposable
{
    private readonly RestorationService _restorationService;
    private readonly string _testDir;

    public RestorationServiceTests()
    {
        _restorationService = new RestorationService();
        _testDir = Path.Combine(Path.GetTempPath(), "FileSnapTests_RestorationServiceTests");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldRestoreDirectoryStructure()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await File.WriteAllBytesAsync(snapshotPath, snapshot.RootDirectory!.Files[0].Content!);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredDir = Path.Combine(_testDir, "test");
        Assert.True(Directory.Exists(restoredDir));
        Assert.True(File.Exists(Path.Combine(restoredDir, "test.txt")));
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldRestoreFiles()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await File.WriteAllBytesAsync(snapshotPath, snapshot.RootDirectory!.Files[0].Content!);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredFile = Path.Combine(_testDir, "test", "test.txt");
        Assert.True(File.Exists(restoredFile));
        var content = await File.ReadAllTextAsync(restoredFile);
        Assert.Equal("This is a test file.", content);
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldHandleExistingFiles()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await File.WriteAllBytesAsync(snapshotPath, snapshot.RootDirectory!.Files[0].Content!);

        var existingFile = Path.Combine(_testDir, "test", "test.txt");
        Directory.CreateDirectory(Path.Combine(_testDir, "test"));
        await File.WriteAllTextAsync(existingFile, "This is a test file.");

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredFile = Path.Combine(_testDir, "test", "test.txt");
        Assert.True(File.Exists(restoredFile));
        var content = await File.ReadAllTextAsync(restoredFile);
        Assert.Equal("This is a test file.", content);
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldHandleExistingDirectories()
    {
        // Arrange
        var snapshot = CreateSnapshot();
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await File.WriteAllBytesAsync(snapshotPath, snapshot.RootDirectory!.Files[0].Content!);

        var existingDir = Path.Combine(_testDir, "test");
        Directory.CreateDirectory(existingDir);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        Assert.True(Directory.Exists(existingDir));
        Assert.True(File.Exists(Path.Combine(existingDir, "test.txt")));
    }

    private SystemSnapshot CreateSnapshot()
    {
        var content = Encoding.UTF8.GetBytes("This is a test file.");
        var filePath = Path.Combine(_testDir, "test", "test.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllBytes(filePath, content);

        return new SystemSnapshot
        {
            Id = Guid.NewGuid(),
            BasePath = "basepath",
            RootDirectory = new DirectorySnapshot
            {
                Path = "basepath",
                Files =
                [
                    new() {
                        Path = "test/test.txt",
                        Content = content,
                        Hash = Convert.ToBase64String(content),
                        CreatedAt = DateTime.Now,
                        LastModified = DateTime.Now,
                        Attributes = FileAttributes.Normal,
                    }
                ],
                Directories =
                [
                    new DirectorySnapshot
                    {
                        Path = "test"
                    }
                ]
            }
        };
    }
}