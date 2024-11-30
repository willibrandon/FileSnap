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

    private static SystemSnapshot CreateSnapshot()
        => new()
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            RootDirectory = new DirectorySnapshot
            {
                Path = "test",
                Files = []
            }
        };
}
