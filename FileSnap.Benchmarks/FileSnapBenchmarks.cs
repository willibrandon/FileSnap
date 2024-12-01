﻿using BenchmarkDotNet.Attributes;
using FileSnap.Core.Models;
using FileSnap.Core.Services;

namespace FileSnap.Benchmarks;

[MemoryDiagnoser]
public class FileSnapBenchmarks
{
    private SnapshotService? _snapshotService;
    private ComparisonService? _comparisonService;
    private RestorationService? _restorationService;
    private HashingService? _hashingService;

    private SystemSnapshot? _smallSnapshot;
    private SystemSnapshot? _largeSnapshot;

    // Define test directories
    private const string SmallDirPath = @"C:\Test\SmallDir";
    private const string LargeDirPath = @"C:\Test\LargeDir";

    [GlobalSetup]
    public async Task Setup()
    {
        // Ensure test directories exist
        CreateTestDirectory(SmallDirPath, 10);  // 10 small files for "small" directory
        CreateTestDirectory(LargeDirPath, 1000); // 1000 files for "large" directory

        // Initialize services
        _hashingService = new HashingService();
        _snapshotService = new SnapshotService(_hashingService);
        _comparisonService = new ComparisonService();
        _restorationService = new RestorationService();

        // Capture snapshots
        _smallSnapshot = await _snapshotService.CaptureSnapshot(SmallDirPath);
        _largeSnapshot = await _snapshotService.CaptureSnapshot(LargeDirPath);
    }

    #region SnapshotService Benchmarks

    /// <summary>
    /// Benchmark for capturing a snapshot of the small directory.
    /// </summary>
    [Benchmark]
    public async Task CaptureSmallSnapshot()
    {
        await _snapshotService!.CaptureSnapshot(SmallDirPath);
    }

    /// <summary>
    /// Benchmark for capturing a snapshot of the large directory.
    /// </summary>
    [Benchmark]
    public async Task CaptureLargeSnapshot()
    {
        await _snapshotService!.CaptureSnapshot(LargeDirPath);
    }

    /// <summary>
    /// Benchmark for saving the snapshot of the small directory.
    /// </summary>
    [Benchmark]
    public async Task SaveSmallSnapshot()
    {
        if (_smallSnapshot != null)
        {
            await _snapshotService!.SaveSnapshot(_smallSnapshot, Path.Combine(SmallDirPath, "smallSnapshot.fsnap"));
        }
    }

    /// <summary>
    /// Benchmark for saving the snapshot of the large directory.
    /// </summary>
    [Benchmark]
    public async Task SaveLargeSnapshot()
    {
        if (_largeSnapshot != null)
        {
            await _snapshotService!.SaveSnapshot(_largeSnapshot, Path.Combine(LargeDirPath, "largeSnapshot.fsnap"));
        }
    }

    /// <summary>
    /// Benchmark for loading the snapshot of the small directory.
    /// </summary>
    [Benchmark]
    public async Task LoadSmallSnapshot()
    {
        if (_smallSnapshot != null)
        {
            await _snapshotService!.SaveSnapshot(_smallSnapshot, Path.Combine(SmallDirPath, "smallSnapshot.fsnap"));
            await _snapshotService.LoadSnapshot(Path.Combine(SmallDirPath, "smallSnapshot.fsnap"));
        }
    }

    /// <summary>
    /// Benchmark for loading the snapshot of the large directory.
    /// </summary>
    [Benchmark]
    public async Task LoadLargeSnapshot()
    {
        if (_largeSnapshot != null)
        {
            await _snapshotService!.SaveSnapshot(_largeSnapshot, Path.Combine(LargeDirPath, "largeSnapshot.fsnap"));
            await _snapshotService.LoadSnapshot(Path.Combine(LargeDirPath, "largeSnapshot.fsnap"));
        }
    }

    #endregion

    #region ComparisonService Benchmarks

    /// <summary>
    /// Benchmark for comparing small directory snapshots.
    /// </summary>
    [Benchmark]
    public void CompareSmallSnapshots()
    {
        if (_smallSnapshot != null)
        {
            _comparisonService!.Compare(_smallSnapshot, _smallSnapshot);
        }
    }

    /// <summary>
    /// Benchmark for comparing large directory snapshots.
    /// </summary>
    [Benchmark]
    public void CompareLargeSnapshots()
    {
        if (_largeSnapshot != null)
        {
            _comparisonService!.Compare(_largeSnapshot, _largeSnapshot);
        }
    }

    /// <summary>
    /// Benchmark for comparing small and large directory snapshots.
    /// </summary>
    [Benchmark]
    public void CompareSmallAndLargeSnapshots()
    {
        if (_smallSnapshot != null && _largeSnapshot != null)
        {
            _comparisonService!.Compare(_smallSnapshot, _largeSnapshot);
        }
    }

    #endregion

    #region RestorationService Benchmarks

    /// <summary>
    /// Benchmark for restoring the small directory snapshot.
    /// </summary>
    [Benchmark]
    public async Task RestoreSmallSnapshot()
    {
        if (_smallSnapshot != null)
        {
            await _restorationService!.RestoreSnapshot(_smallSnapshot, @"C:\Restored\SmallDir");
        }
    }

    /// <summary>
    /// Benchmark for restoring the large directory snapshot.
    /// </summary>
    [Benchmark]
    public async Task RestoreLargeSnapshot()
    {
        if (_largeSnapshot != null)
        {
            await _restorationService!.RestoreSnapshot(_largeSnapshot, @"C:\Restored\LargeDir");
        }
    }

    #endregion

    #region HashingService Benchmarks

    /// <summary>
    /// Benchmark for hashing a small file.
    /// </summary>
    [Benchmark]
    public async Task HashSmallFile()
    {
        var smallFilePath = Path.Combine(SmallDirPath, "file0.txt");
        await _hashingService!.ComputeHashAsync(smallFilePath);
    }

    /// <summary>
    /// Benchmark for hashing a large file.
    /// </summary>
    [Benchmark]
    public async Task HashLargeFile()
    {
        var largeFilePath = Path.Combine(LargeDirPath, "file999.txt");
        await _hashingService!.ComputeHashAsync(largeFilePath);
    }

    #endregion

    [GlobalCleanup]
    public static void Cleanup()
    {
        DeleteDirectory(SmallDirPath);
        DeleteDirectory(LargeDirPath);
        DeleteDirectory(@"C:\Restored\SmallDir");
        DeleteDirectory(@"C:\Restored\LargeDir");
    }

    /// <summary>
    /// Creates a test directory with dummy files.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <param name="fileCount">The number of files to create.</param>
    private static void CreateTestDirectory(string path, int fileCount)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        for (int i = 0; i < fileCount; i++)
        {
            var filePath = Path.Combine(path, $"file{i}.txt");
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, $"This is a test file {i}");
            }
        }
    }

    /// <summary>
    /// Deletes a directory and all its contents.
    /// </summary>
    /// <param name="path">The directory path.</param>
    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(path);
        }
    }
}
