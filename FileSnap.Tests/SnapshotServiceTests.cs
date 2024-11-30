using FileSnap.Core.Services;
using System.Text;

namespace FileSnap.Tests;

public class SnapshotServiceTests : IDisposable
{
    private readonly SnapshotService _snapshotService;
    private readonly string _testDir;

    public SnapshotServiceTests()
    {
        _snapshotService = new SnapshotService(new HashingService());
        _testDir = Path.Combine(Path.GetTempPath(), "FileSnapTests_SnapshotServiceTests");
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
    public async Task CaptureSnapshot_ShouldCreateValidSnapshot()
    {
        // Arrange
        var testFilePath = Path.Combine(_testDir, "test.txt");
        await File.WriteAllTextAsync(testFilePath, "test content");

        // Act
        var snapshot = await _snapshotService.CaptureSnapshot(_testDir);

        // Assert
        Assert.NotNull(snapshot);
        Assert.NotEqual(Guid.Empty, snapshot.Id);
        Assert.NotNull(snapshot.RootDirectory);
        Assert.Single(snapshot.RootDirectory.Files);
        Assert.Equal("test.txt", Path.GetFileName(snapshot.RootDirectory.Files[0].Path));
        Assert.Equal("test content", Encoding.UTF8.GetString(snapshot.RootDirectory.Files[0].Content!));
    }

    [Fact]
    public async Task SaveAndLoadSnapshot_ShouldPersistAndRetrieveSnapshot()
    {
        // Arrange
        var testFilePath = Path.Combine(_testDir, "test.txt");
        await File.WriteAllTextAsync(testFilePath, "test content");
        var snapshot = await _snapshotService.CaptureSnapshot(_testDir);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");

        // Act
        await _snapshotService.SaveSnapshot(snapshot, snapshotPath);
        var loadedSnapshot = await _snapshotService.LoadSnapshot(snapshotPath);

        // Assert
        Assert.NotNull(loadedSnapshot);
        Assert.Equal(snapshot.Id, loadedSnapshot.Id);
        Assert.Equal(snapshot.CreatedAt, loadedSnapshot.CreatedAt);
        Assert.Equal(snapshot.BasePath, loadedSnapshot.BasePath);
        Assert.Equal(snapshot.RootDirectory!.Path, loadedSnapshot.RootDirectory!.Path);
        Assert.Single(loadedSnapshot.RootDirectory.Files);
        Assert.Equal("test.txt", Path.GetFileName(loadedSnapshot.RootDirectory.Files[0].Path));
        Assert.Equal("test content", Encoding.UTF8.GetString(loadedSnapshot.RootDirectory.Files[0].Content!));
    }
}
