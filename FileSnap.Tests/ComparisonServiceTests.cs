using FileSnap.Core.Models;
using FileSnap.Core.Services;

namespace FileSnap.Tests;

public class ComparisonServiceTests : IDisposable
{
    private readonly ComparisonService _comparisonService;
    private readonly string _testDir;

    public ComparisonServiceTests()
    {
        _comparisonService = new ComparisonService();
        _testDir = Path.Combine(Path.GetTempPath(), "FileSnapTests_ComparisonServiceTests");
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
    public void Compare_ShouldDetectNewFiles()
    {
        // Arrange
        var before = CreateSnapshot();
        var after = CreateSnapshot();
        after.RootDirectory!.Files.Add(new FileSnapshot { Path = "new.txt", Hash = "hash" });

        // Act
        var difference = _comparisonService.Compare(before, after);

        // Assert
        Assert.Single(difference.NewFiles);
        Assert.Empty(difference.DeletedFiles);
        Assert.Empty(difference.ModifiedFiles);
    }

    [Fact]
    public void Compare_ShouldDetectDeletedFiles()
    {
        // Arrange
        var before = CreateSnapshot();
        before.RootDirectory!.Files.Add(new FileSnapshot { Path = "old.txt", Hash = "hash" });
        var after = CreateSnapshot();

        // Act
        var difference = _comparisonService.Compare(before, after);

        // Assert
        Assert.Empty(difference.NewFiles);
        Assert.Single(difference.DeletedFiles);
        Assert.Empty(difference.ModifiedFiles);
    }

    [Fact]
    public void Compare_ShouldDetectModifiedFiles()
    {
        // Arrange
        var before = CreateSnapshot();
        before.RootDirectory!.Files.Add(new FileSnapshot { Path = "file.txt", Hash = "oldhash" });
        var after = CreateSnapshot();
        after.RootDirectory!.Files.Add(new FileSnapshot { Path = "file.txt", Hash = "newhash" });

        // Act
        var difference = _comparisonService.Compare(before, after);

        // Assert
        Assert.Empty(difference.NewFiles);
        Assert.Empty(difference.DeletedFiles);
        Assert.Single(difference.ModifiedFiles);
    }

    [Fact]
    public void Compare_ShouldDetectNewDirectories()
    {
        // Arrange
        var before = CreateSnapshot();
        var after = CreateSnapshot();
        after.RootDirectory!.Directories.Add(new DirectorySnapshot { Path = "newdir" });

        // Act
        var difference = _comparisonService.Compare(before, after);

        // Assert
        Assert.Single(difference.NewDirectories);
        Assert.Empty(difference.DeletedDirectories);
        Assert.Empty(difference.ModifiedDirectories);
    }

    [Fact]
    public void Compare_ShouldDetectDeletedDirectories()
    {
        // Arrange
        var before = CreateSnapshot();
        before.RootDirectory!.Directories.Add(new DirectorySnapshot { Path = "olddir" });
        var after = CreateSnapshot();

        // Act
        var difference = _comparisonService.Compare(before, after);

        // Assert
        Assert.Empty(difference.NewDirectories);
        Assert.Single(difference.DeletedDirectories);
        Assert.Empty(difference.ModifiedDirectories);
    }

    [Fact]
    public void Compare_ShouldDetectModifiedDirectories()
    {
        // Arrange
        var before = CreateSnapshot();
        before.RootDirectory!.Directories.Add(new DirectorySnapshot { Path = "dir", CreatedAt = DateTime.UtcNow.AddDays(-1) });
        var after = CreateSnapshot();
        after.RootDirectory!.Directories.Add(new DirectorySnapshot { Path = "dir", CreatedAt = DateTime.UtcNow });

        // Act
        var difference = _comparisonService.Compare(before, after);

        // Assert
        Assert.Empty(difference.NewDirectories);
        Assert.Empty(difference.DeletedDirectories);
        Assert.Single(difference.ModifiedDirectories);
    }

    [Fact]
    public void Compare_ShouldDetectIncrementalChanges()
    {
        // Arrange
        var before = CreateSnapshot();
        before.RootDirectory!.Files.Add(new FileSnapshot { Path = "file1.txt", Hash = "hash1" });
        before.RootDirectory!.Files.Add(new FileSnapshot { Path = "file2.txt", Hash = "hash2" });
        before.RootDirectory!.Directories.Add(new DirectorySnapshot { Path = "dir1" });

        var after = CreateSnapshot();
        after.RootDirectory!.Files.Add(new FileSnapshot { Path = "file2.txt", Hash = "newhash2" }); // Modified
        after.RootDirectory!.Files.Add(new FileSnapshot { Path = "file3.txt", Hash = "hash3" }); // New
        after.RootDirectory!.Directories.Add(new DirectorySnapshot { Path = "dir2" }); // New

        // Act
        var difference = _comparisonService.Compare(before, after);

        // Assert
        Assert.Single(difference.NewFiles);
        Assert.Single(difference.ModifiedFiles);
        Assert.Single(difference.NewDirectories);
        Assert.Single(difference.DeletedFiles);
    }

    private static SystemSnapshot CreateSnapshot()
        => new()
        {
            Id = Guid.NewGuid(),
            BasePath = "basepath",
            RootDirectory = new DirectorySnapshot
            {
                Path = "basepath",
                Files = [],
                Directories = []
            }
        };
}
