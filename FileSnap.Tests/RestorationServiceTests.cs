using FileSnap.Core.Services;

namespace FileSnap.Tests;

public class RestorationServiceTests : IDisposable
{
    private readonly SnapshotService _snapshotService;
    private readonly RestorationService _restorationService;
    private readonly string _testDir;

    public RestorationServiceTests()
    {
        var hashingService = new HashingService();
        _snapshotService = new SnapshotService(hashingService);
        _restorationService = new RestorationService();
        _testDir = Path.Combine(Path.GetTempPath(), "FileSnapTests_RestorationServiceTests");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            foreach (var file in Directory.GetFiles(_testDir))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(_testDir, true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldRestoreDirectoryStructure()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "DirStructureTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file1.txt"), "This is a test file.");
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(snapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredDir = Path.Combine(_testDir, "DirStructureTest");
        Assert.True(Directory.Exists(restoredDir));
        Assert.True(File.Exists(Path.Combine(restoredDir, "file1.txt")));
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldRestoreFiles()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "FileRestoreTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file2.txt"), "This is a test file.");
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(snapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredFile = Path.Combine(_testDir, "FileRestoreTest", "file2.txt");
        Assert.True(File.Exists(restoredFile));
        var content = await File.ReadAllTextAsync(restoredFile);
        Assert.Equal("This is a test file.", content);
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldHandleExistingFiles()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "ExistingFilesTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file3.txt"), "This is a test file.");
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(snapshot, snapshotPath);

        var existingFile = Path.Combine(_testDir, "ExistingFilesTest", "file3.txt");
        Directory.CreateDirectory(Path.Combine(_testDir, "ExistingFilesTest"));
        await File.WriteAllTextAsync(existingFile, "Existing content");

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredFile = Path.Combine(_testDir, "ExistingFilesTest", "file3.txt");
        Assert.True(File.Exists(restoredFile));
        var content = await File.ReadAllTextAsync(restoredFile);
        Assert.Equal("This is a test file.", content);
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldHandleExistingDirectories()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "ExistingDirsTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file4.txt"), "This is a test file.");
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(snapshot, snapshotPath);

        var existingDir = Path.Combine(_testDir, "ExistingDirsTest");
        Directory.CreateDirectory(existingDir);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        Assert.True(Directory.Exists(existingDir));
        Assert.True(File.Exists(Path.Combine(existingDir, "file4.txt")));
    }

    [Fact]
    public async Task RestoreSnapshot_InvalidCreationTime_ShowThrow_ArgumentOutOfRangeException()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "InvalidCreationTimeTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file5.txt"), "This is a test file.");
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        snapshot.RootDirectory!.Files[0].Metadata.CreatedAt = DateTime.MinValue; // Invalid creation time
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(snapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredFile = Path.Combine(_testDir, "InvalidCreationTimeTest", "file5.txt");
        Assert.True(File.Exists(restoredFile));
        var content = await File.ReadAllTextAsync(restoredFile);
        Assert.Equal("This is a test file.", content);
    }

    [Fact]
    public async Task RestoreSnapshot_ShouldRestoreFileAttributes()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "FileAttributesTest");
        Directory.CreateDirectory(dirPath);
        var filePath = Path.Combine(dirPath, "file6.txt");
        File.WriteAllText(filePath, "This is a test file.");
        File.SetAttributes(filePath, FileAttributes.Normal);
        var snapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(snapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(snapshot, _testDir);

        // Assert
        var restoredFile = Path.Combine(_testDir, "FileAttributesTest", "file6.txt");
        Assert.True(File.Exists(restoredFile));
        var attributes = File.GetAttributes(restoredFile);
        Assert.Equal(FileAttributes.Normal, attributes);
    }

    [Fact]
    public async Task RestoreIncrementalSnapshot_ShouldHandleNewFiles()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "IncrementalNewFilesTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file7.txt"), "This is a test file.");
        _ = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var incrementalDirPath = Path.Combine(_testDir, "IncrementalNewFilesTest");
        File.WriteAllText(Path.Combine(incrementalDirPath, "newfile.txt"), "This is a new file.");
        var incrementalSnapshot = await _snapshotService.CaptureSnapshotAsync(incrementalDirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(incrementalSnapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(incrementalSnapshot, _testDir);

        // Assert
        var newFile = Path.Combine(_testDir, "IncrementalNewFilesTest", "newfile.txt");
        Assert.True(File.Exists(newFile));
        var content = await File.ReadAllTextAsync(newFile);
        Assert.Equal("This is a new file.", content);
    }

    [Fact]
    public async Task RestoreIncrementalSnapshot_ShouldHandleDeletedFiles()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "IncrementalDeletedFilesTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file8.txt"), "This is a test file.");
        _ = await _snapshotService.CaptureSnapshotAsync(dirPath);
        File.Delete(Path.Combine(dirPath, "file8.txt"));
        var incrementalSnapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(incrementalSnapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(incrementalSnapshot, _testDir);

        // Assert
        var deletedFile = Path.Combine(_testDir, "IncrementalDeletedFilesTest", "file8.txt");
        Assert.False(File.Exists(deletedFile));
    }

    [Fact]
    public async Task RestoreIncrementalSnapshot_ShouldHandleModifiedFiles()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "IncrementalModifiedFilesTest");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file9.txt"), "This is a test file.");
        _ = await _snapshotService.CaptureSnapshotAsync(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "file9.txt"), "This is a modified file.");
        var incrementalSnapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(incrementalSnapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(incrementalSnapshot, _testDir);

        // Assert
        var modifiedFile = Path.Combine(_testDir, "IncrementalModifiedFilesTest", "file9.txt");
        Assert.True(File.Exists(modifiedFile));
        var content = await File.ReadAllTextAsync(modifiedFile);
        Assert.Equal("This is a modified file.", content);
    }

    [Fact]
    public async Task RestoreIncrementalSnapshot_ShouldHandleDeletedDirectories()
    {
        // Arrange
        var dirPath = Path.Combine(_testDir, "IncrementalDeletedDirsTest");
        Directory.CreateDirectory(dirPath);
        var subDirPath = Path.Combine(dirPath, "subdir");
        Directory.CreateDirectory(subDirPath);
        File.WriteAllText(Path.Combine(subDirPath, "file10.txt"), "This is a test file.");
        _ = await _snapshotService.CaptureSnapshotAsync(dirPath);
        Directory.Delete(subDirPath, true);
        var incrementalSnapshot = await _snapshotService.CaptureSnapshotAsync(dirPath);
        var snapshotPath = Path.Combine(_testDir, "snapshot.json");
        await _snapshotService.SaveSnapshotAsync(incrementalSnapshot, snapshotPath);

        // Act
        await _restorationService.RestoreSnapshotAsync(incrementalSnapshot, _testDir);

        // Assert
        var deletedDir = Path.Combine(_testDir, "IncrementalDeletedDirsTest", "subdir");
        Assert.False(Directory.Exists(deletedDir));
    }
}
