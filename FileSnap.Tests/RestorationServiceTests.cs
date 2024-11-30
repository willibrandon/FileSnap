using FileSnap.Core.Models;
using FileSnap.Core.Services;

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
        await _restorationService.RestoreSnapshot(snapshot, _testDir);

        // Assert
        var restoredDir = Path.Combine(_testDir, "test");
        Assert.True(Directory.Exists(restoredDir));
        Assert.True(File.Exists(Path.Combine(restoredDir, "test.txt")));
    }

    private static SystemSnapshot CreateSnapshot()
        => new()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            RootDirectory = new DirectorySnapshot
            {
                Path = "test",
                Files =
                [
                    new FileSnapshot
                    {
                        Path = "test.txt",
                        Hash = "hash",
                        Size = 11,
                        LastModified = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        Attributes = FileAttributes.Normal,
                        Content = System.Text.Encoding.UTF8.GetBytes("test content")
                    }
                ]
            }
        };
}
