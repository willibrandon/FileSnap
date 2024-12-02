# FileSnap

**FileSnap** is a .NET library designed to capture, compare, and restore file system snapshots. It allows you to take snapshots of a file system at a given point in time, record file and directory states (including metadata such as file sizes, timestamps, and attributes), compare different snapshots to identify changes (new, modified, or deleted files), and restore file system states to a previous snapshot. This library is useful for scenarios such as backup, security, auditing, file integrity monitoring, and implementing versioned file systems or system restore capabilities.

## Features

- **Capture Snapshots**: Record the state of files and directories, including metadata and content.
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
await snapshotService.SaveSnapshotAsync(snapshot, "path/to/save/snapshot");
```

### Capturing a Snapshot with Compression Enabled

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService(), isCompressionEnabled: true);
var snapshot = await snapshotService.CaptureSnapshotAsync("path/to/directory");
await snapshotService.SaveSnapshotAsync(snapshot, "path/to/save/snapshot");
```

### Loading and Comparing Snapshots

```csharp
var snapshotService = new SnapshotService(new HashingService());
var beforeSnapshot = await snapshotService.LoadSnapshotAsync("path/to/before/snapshot.json");
var afterSnapshot = await snapshotService.LoadSnapshotAsync("path/to/after/snapshot.json");

var comparisonService = new ComparisonService();
var differences = comparisonService.Compare(beforeSnapshot, afterSnapshot);
```

### Restoring a Snapshot

```csharp
var snapshotService = new SnapshotService(new HashingService());
var snapshot = await snapshotService.LoadSnapshotAsync("path/to/snapshot.json");
await snapshotService.RestoreSnapshotAsync(snapshot, "path/to/restore");
```

### Custom Compression Service

You can inject a custom compression service if you want to use a different compression algorithm:

```csharp
public class CustomCompressionService : ICompressionService
{
    public byte[] Compress(byte[] data)
    {
        // Custom compression logic
    }

    public byte[] Decompress(byte[] data)
    {
        // Custom decompression logic
    }
}

var snapshotService = new SnapshotService(new HashingService(), new CustomCompressionService(), true);
```