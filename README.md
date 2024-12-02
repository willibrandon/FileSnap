# FileSnap

**FileSnap** is a .NET library designed to capture, compare, and restore file system snapshots. It allows you to take snapshots of a file system at a given point in time, record file and directory states (including metadata such as file sizes, timestamps, and attributes), compare different snapshots to identify changes (new, modified, or deleted files), and restore file system states to a previous snapshot. This library is useful for scenarios such as backup, security, auditing, file integrity monitoring, and implementing versioned file systems or system restore capabilities.

## Features

- **Capture Snapshots**: Record the state of files and directories, including metadata and content.
- **Capture Incremental Snapshots**: Record only the changes (new, modified, or deleted files) since the last snapshot.
- **Compare Snapshots**: Identify changes between snapshots, including new, modified, and deleted files.
- **Restore Snapshots**: Restore file system states to a previous snapshot, including file content and metadata.
- **Optimized Storage**: Uses compression to minimize storage space for snapshots.
- **Cross-Platform**: Compatible with Windows, macOS, and Linux.

## Installation

To install FileSnap, add the following package to your .NET project:

```bash
dotnet add package FileSnap
```

## Notes

- The snapshot files are saved with a `.json` extension by default. If you don't specify an extension, `.json` will be appended automatically.
- The `Content` field of files is compressed to minimize storage space when compression is enabled.

## Default Compression Service

By default, FileSnap uses Brotli compression for optimal storage efficiency. If you don't specify a compression service, the default Brotli compression service will be used. Compression is disabled by default and can be enabled by passing `true` for the `isCompressionEnabled` parameter.

## Usage

### Capturing a Snapshot

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService());
var snapshot = await snapshotService.CaptureSnapshotAsync("path/to/directory");
await snapshotService.SaveSnapshotAsync(snapshot, "snapshot.json");
```

### Capturing a Snapshot with Compression Enabled

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService(), true);
var snapshot = await snapshotService.CaptureSnapshotAsync("path/to/directory");
await snapshotService.SaveSnapshotAsync(snapshot, "snapshot.json");
```

### Capturing an Incremental Snapshot

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService());
var previousSnapshot = await snapshotService.CaptureSnapshotAsync("path/to/directory");
// Make some changes to the directory...
var incrementalSnapshot = await snapshotService.CaptureIncrementalSnapshotAsync("path/to/directory", previousSnapshot);
await snapshotService.SaveSnapshotAsync(incrementalSnapshot, "incrementalSnapshot.json");
```

### Loading and Comparing Snapshots

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService());
var comparisonService = new ComparisonService();

var snapshot1 = await snapshotService.LoadSnapshotAsync("snapshot1.json");
var snapshot2 = await snapshotService.LoadSnapshotAsync("snapshot2.json");

var difference = comparisonService.Compare(snapshot1, snapshot2);

Console.WriteLine("New Files:");
foreach (var file in difference.NewFiles)
{
    Console.WriteLine(file.Path);
}

Console.WriteLine("Modified Files:");
foreach (var (before, after) in difference.ModifiedFiles)
{
    Console.WriteLine($"{before.Path} -> {after.Path}");
}

Console.WriteLine("Deleted Files:");
foreach (var file in difference.DeletedFiles)
{
    Console.WriteLine(file.Path);
}
```

### Restoring a Snapshot

```csharp
using FileSnap.Core.Services;

var restorationService = new RestorationService();
await restorationService.RestoreSnapshotAsync(snapshot, "path/to/restore");
```

### Restoring an Incremental Snapshot

```csharp
using FileSnap.Core.Services;

var restorationService = new RestorationService();
await restorationService.RestoreSnapshotAsync(incrementalSnapshot, "path/to/restore");
```

### Custom Compression Service

You can implement your own compression service by implementing the `ICompressionService` interface. Here is an example using GZip compression:

```csharp
using System.IO.Compression;

public class GZipCompressionService : ICompressionService
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
```

### Using a Custom Compression Service

```csharp
using FileSnap.Core.Services;

var customCompressionService = new GZipCompressionService();
var snapshotService = new SnapshotService(new HashingService(), true, customCompressionService);
var snapshot = await snapshotService.CaptureSnapshotAsync("path/to/directory");
await snapshotService.SaveSnapshotAsync(snapshot, "snapshot.json");
```

### Example

Here's a complete example that demonstrates a simple backup and restore system:

1. Write initial files to disk to simulate a backup.
2. Capture initial snapshot.
3. Write a couple more files to disk to simulate changes.
4. Capture incremental snapshot.
5. Delete all files from the disk to simulate data loss.
6. Restore initial snapshot to recover the original state.
7. Restore incremental snapshot to apply the changes.

```csharp
using FileSnap.Core.Services;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var snapshotService = new SnapshotService(new HashingService());
        var restorationService = new RestorationService();
        var comparisonService = new ComparisonService();

        var tempPath = Path.GetTempPath();
        var dirPath = Path.Combine(tempPath, "FileSnapExample");
        Directory.CreateDirectory(dirPath);

        // 1. Write initial files to disk to simulate a backup
        File.WriteAllText(Path.Combine(dirPath, "file1.txt"), "This is a test file.");
        File.WriteAllText(Path.Combine(dirPath, "file2.txt"), "This is another test file.");

        // 2. Capture initial snapshot
        var initialSnapshot = await snapshotService.CaptureSnapshotAsync(dirPath);
        await snapshotService.SaveSnapshotAsync(initialSnapshot, Path.Combine(tempPath, "initialSnapshot.json"));

        // 3. Write a couple more files to disk to simulate changes
        File.WriteAllText(Path.Combine(dirPath, "file3.txt"), "This is a new file.");
        File.WriteAllText(Path.Combine(dirPath, "file2.txt"), "This is a modified file.");

        // 4. Capture incremental snapshot
        var incrementalSnapshot = await snapshotService.CaptureIncrementalSnapshotAsync(dirPath, initialSnapshot);
        await snapshotService.SaveSnapshotAsync(incrementalSnapshot, Path.Combine(tempPath, "incrementalSnapshot.json"));

        // 5. Delete all files from the disk to simulate data loss
        Directory.Delete(dirPath, true);
        Directory.CreateDirectory(dirPath);

        // 6. Restore initial snapshot to recover the original state
        await restorationService.RestoreSnapshotAsync(initialSnapshot, dirPath);

        // 7. Restore incremental snapshot to apply the changes
        await restorationService.RestoreSnapshotAsync(incrementalSnapshot, dirPath);

        Console.WriteLine("Restoration complete.");
    }
}
```

This example demonstrates how to use FileSnap to implement a simple backup and restore system, including capturing, comparing, and restoring file system snapshots, with incremental snapshots.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/FileSnap/blob/main/LICENSE.txt) file for details.