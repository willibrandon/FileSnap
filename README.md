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
var snapshot = await snapshotService.CaptureSnapshot("path/to/directory");
await snapshotService.SaveSnapshot(snapshot, "path/to/save/snapshot.json");
```

### Loading and Comparing Snapshots

```csharp
var snapshotService = new SnapshotService(new HashingService());
var beforeSnapshot = await snapshotService.LoadSnapshot("path/to/before/snapshot.json");
var afterSnapshot = await snapshotService.LoadSnapshot("path/to/after/snapshot.json");

var comparisonService = new ComparisonService();
var differences = comparisonService.Compare(beforeSnapshot, afterSnapshot);
```

### Restoring a Snapshot

```csharp
var snapshotService = new SnapshotService(new HashingService());
var snapshot = await snapshotService.LoadSnapshot("path/to/snapshot.json");

var restorationService = new RestorationService();
await restorationService.RestoreSnapshot(snapshot, "path/to/restore/directory");
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/FileSnap/blob/main/LICENSE.txt) file for details.
