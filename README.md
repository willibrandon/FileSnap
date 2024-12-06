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

### Capturing a Snapshot with Metadata

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService());
var metadata = new Dictionary<string, string>
{
    { "Author", "John Doe" },
    { "Description", "Test directory with metadata" }
};
var snapshot = await snapshotService.CaptureSnapshotWithMetadataAsync("path/to/directory", metadata);
await snapshotService.SaveSnapshotAsync(snapshot, "snapshot_with_metadata.json");
```

### Capturing an Incremental Snapshot with Metadata

```csharp
using FileSnap.Core.Services;
using FileSnap.Core.Models;

var snapshotService = new SnapshotService(new HashingService());
var previousSnapshot = await snapshotService.CaptureSnapshotAsync("path/to/directory");
var metadata = new Dictionary<string, string>
{
    { "Author", "Jane Doe" },
    { "Description", "Incremental snapshot with metadata" }
};
// Make some changes to the directory...
var incrementalSnapshot = await snapshotService.CaptureIncrementalSnapshotWithMetadataAsync("path/to/directory", previousSnapshot, metadata);
await snapshotService.SaveSnapshotAsync(incrementalSnapshot, "incremental_snapshot_with_metadata.json");
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
    Console.WriteLine(file.Metadata.Path);
}

Console.WriteLine("Modified Files:");
foreach (var (before, after) in difference.ModifiedFiles)
{
    Console.WriteLine($"{before.Metadata.Path} -> {after.Metadata.Path}");
}

Console.WriteLine("Deleted Files:");
foreach (var file in difference.DeletedFiles)
{
    Console.WriteLine(file.Metadata.Path);
}
```

### Analyzing Snapshots

```csharp
using FileSnap.Core.Services;

var snapshotService = new SnapshotService(new HashingService());
var analysisService = new AnalysisService();

var snapshot = await snapshotService.LoadSnapshotAsync("snapshot.json");

var insights = await analysisService.AnalyzeSnapshotAsync(snapshot);

Console.WriteLine("File Count: " + insights["FileCount"]);
Console.WriteLine("Directory Count: " + insights["DirectoryCount"]);
```

### Analyzing Snapshots

```csharp
using FileSnap.Core.Services;
using FileSnap.Core.Models;

var snapshotService = new SnapshotService(new HashingService());
var analysisService = new AnalysisService();

var snapshot = await snapshotService.LoadSnapshotAsync("snapshot.json");

var fileMetadata = analysisService.GetFileMetadata(snapshot.RootDirectory.Files[0]);
var directoryMetadata = analysisService.GetDirectoryMetadata(snapshot.RootDirectory);

Console.WriteLine("File Path: " + fileMetadata.Path);
Console.WriteLine("File Size: " + fileMetadata.Size);
Console.WriteLine("Directory Path: " + directoryMetadata.Path);
Console.WriteLine("Directory Created At: " + directoryMetadata.CreatedAt);
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

## Using the Graphical Interface

A graphical interface has been developed to manage snapshots, view differences, and restore file system states. The graphical interface interacts with the existing backend services for snapshot management, comparison, and restoration.

### Running the Graphical Interface

To run the graphical interface, follow these steps:

1. Open the `FileSnap.UI` project in your preferred IDE.
2. Build the project to restore the necessary dependencies.
3. Run the project. The graphical interface will launch, allowing you to capture, save, load, compare, and restore snapshots through a user-friendly interface.

### Capturing a Snapshot

1. Click the "Capture Snapshot" button.
2. Select the directory you want to capture.
3. A message will appear indicating that the snapshot was captured successfully.

### Saving a Snapshot

1. Enter the path of the snapshot in the "Snapshot Path" textbox.
2. Click the "Save Snapshot" button.
3. Choose the location and filename to save the snapshot.
4. A message will appear indicating that the snapshot was saved successfully.

### Loading a Snapshot

1. Click the "Load Snapshot" button.
2. Select the snapshot file you want to load.
3. A message will appear indicating that the snapshot was loaded successfully.

### Comparing Snapshots

1. Click the "Compare Snapshots" button.
2. Select the first snapshot file.
3. Select the second snapshot file.
4. A message will appear indicating that the snapshots were compared successfully.

### Restoring a Snapshot

1. Click the "Restore Snapshot" button.
2. Select the snapshot file you want to restore.
3. Select the directory where you want to restore the snapshot.
4. A message will appear indicating that the snapshot was restored successfully.

## Next Steps

Here are some potential future enhancements for the FileSnap library:

- **Snapshot Analysis**: Add support for analyzing snapshots to provide insights such as the most frequently modified files, largest files, and directories with the most changes.
- **Cloud Storage Integration**: Integrate with cloud storage providers (e.g., AWS S3, Azure Blob Storage, Google Cloud Storage) to store and retrieve snapshots.
- **Real-time Monitoring**: Implement real-time monitoring of file system changes and automatically capture snapshots based on detected changes.
- **Snapshot Encryption**: Add support for encrypting snapshots to enhance security and protect sensitive data.
- **Improved Performance**: Optimize the performance of snapshot capturing, comparison, and restoration processes, especially for large file systems.
- **User Interface**: Develop a user-friendly graphical interface for managing snapshots, viewing differences, and restoring file system states.
- **API Enhancements**: Extend the API to provide more customization options and support additional use cases.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/FileSnap/blob/main/LICENSE.txt) file for details.
