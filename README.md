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

## Usage

### Capturing a Snapshot

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService());
var snapshot = await snapshotService.CaptureSnapshotAsync("path/to/directory");
await snapshotService.SaveSnapshotAsync(snapshot, "path/to/save/snapshot.fsnap");
```

### Loading and Comparing Snapshots

```csharp
var snapshotService = new SnapshotService(new HashingService());
var beforeSnapshot = await snapshotService.LoadSnapshotAsync("path/to/before/snapshot.fsnap");
var afterSnapshot = await snapshotService.LoadSnapshotAsync("path/to/after/snapshot.fsnap");

var comparisonService = new ComparisonService();
var differences = comparisonService.Compare(beforeSnapshot, afterSnapshot);
```

### Restoring a Snapshot

```csharp
var snapshotService = new SnapshotService(new HashingService());
var snapshot = await snapshotService.LoadSnapshotAsync("path/to/snapshot.fsnap");

var restorationService = new RestorationService();
await restorationService.RestoreSnapshotAsync(snapshot, "path/to/restore/directory");
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/FileSnap/blob/main/LICENSE.txt) file for details.
