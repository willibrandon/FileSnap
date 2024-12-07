using FileSnap.Core.Models;
using System.Runtime.InteropServices;

namespace FileSnap.Core.Services;

public class FileWatcherService : IFileWatcherService
{
    private readonly List<FileSystemWatcher> _watchers = new();
    private EventFilterOptions _filterOptions = new();
    private bool _treatFilePermissionChangesAsSeparateEvent;
    private bool _followSymbolicLinks;
    private bool _treatFileRenamesAsSeparateEvent;
    private bool _treatFileDeletionsAsSeparateEvent;
    private bool _treatFileMovesAsSeparateEvent;
    private bool _treatFileAttributeChangesAsSeparateEvent;
    private bool _treatFileCreationEventsAsSeparateEvent;
    private bool _treatFileModificationsAsSeparateEvent;
    private bool _treatFileSystemMountEventsAsSeparateEvent;
    private bool _supportNetworkFileSystems;

    public void StartWatching(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            StartWindowsWatcher(path);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            StartMacOSWatcher(path);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            StartLinuxWatcher(path);
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported platform");
        }
    }

    public void StopWatching()
    {
        foreach (var watcher in _watchers)
        {
            watcher.Dispose();
        }
        _watchers.Clear();
    }

    public void ConfigureFilePermissionChanges(bool treatAsSeparateEvent)
    {
        _treatFilePermissionChangesAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureSymbolicLinks(bool followSymbolicLinks)
    {
        _followSymbolicLinks = followSymbolicLinks;
    }

    public void ConfigureFileRenames(bool treatAsSeparateEvent)
    {
        _treatFileRenamesAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureFileDeletions(bool treatAsSeparateEvent)
    {
        _treatFileDeletionsAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureFileMoves(bool treatAsSeparateEvent)
    {
        _treatFileMovesAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureFileAttributeChanges(bool treatAsSeparateEvent)
    {
        _treatFileAttributeChangesAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureFileCreationEvents(bool treatAsSeparateEvent)
    {
        _treatFileCreationEventsAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureFileModifications(bool treatAsSeparateEvent)
    {
        _treatFileModificationsAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureEventFiltering(EventFilterOptions filterOptions)
    {
        _filterOptions = filterOptions;
    }

    public void ConfigureFileSystemMountEvents(bool treatAsSeparateEvent)
    {
        _treatFileSystemMountEventsAsSeparateEvent = treatAsSeparateEvent;
    }

    public void ConfigureNetworkFileSystems(bool supportNetworkFileSystems)
    {
        _supportNetworkFileSystems = supportNetworkFileSystems;
    }

    public FileMetadata GetFileMetadata(string path)
    {
        var fileInfo = new FileInfo(path);
        var metadata = new FileMetadata
        {
            Path = path,
            Size = fileInfo.Length,
            LastModified = fileInfo.LastWriteTimeUtc,
            CreatedAt = fileInfo.CreationTimeUtc,
            Attributes = fileInfo.Attributes,
            User = GetUser(fileInfo),
            Process = GetProcess(fileInfo),
            Timestamp = DateTime.UtcNow
        };

        return metadata;
    }

    private void StartWindowsWatcher(string path)
    {
        var watcher = new FileSystemWatcher(path)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes |
                           NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.LastAccess |
                           NotifyFilters.CreationTime | NotifyFilters.Security
        };

        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;

        watcher.EnableRaisingEvents = true;
        _watchers.Add(watcher);
    }

    private void StartMacOSWatcher(string path)
    {
        // Implement SFEvents-based watcher for MacOS
        throw new NotImplementedException("MacOS watcher is not implemented yet.");
    }

    private void StartLinuxWatcher(string path)
    {
        // Implement inotify-based watcher for Linux
        throw new NotImplementedException("Linux watcher is not implemented yet.");
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (ShouldFilterEvent(e.FullPath))
        {
            return;
        }

        var metadata = GetFileMetadata(e.FullPath);
        // Handle file changed event
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        if (ShouldFilterEvent(e.FullPath))
        {
            return;
        }

        var metadata = GetFileMetadata(e.FullPath);
        // Handle file created event
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        if (ShouldFilterEvent(e.FullPath))
        {
            return;
        }

        var metadata = GetFileMetadata(e.FullPath);
        // Handle file deleted event
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        if (ShouldFilterEvent(e.FullPath))
        {
            return;
        }

        var metadata = GetFileMetadata(e.FullPath);
        // Handle file renamed event
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        // Handle error event
    }

    private bool ShouldFilterEvent(string path)
    {
        var extension = Path.GetExtension(path);
        var directory = Path.GetDirectoryName(path);
        var fileName = Path.GetFileName(path);

        if (_filterOptions.FileExtensions.Count > 0 && !_filterOptions.FileExtensions.Contains(extension))
        {
            return true;
        }

        if (_filterOptions.Directories.Count > 0 && !_filterOptions.Directories.Contains(directory!))
        {
            return true;
        }

        if (_filterOptions.FileNames.Count > 0 && !_filterOptions.FileNames.Contains(fileName))
        {
            return true;
        }

        return false;
    }

    private string GetUser(FileInfo fileInfo)
    {
        // Implement logic to get the user who made the change
        return "Unknown";
    }

    private string GetProcess(FileInfo fileInfo)
    {
        // Implement logic to get the process that triggered the event
        return "Unknown";
    }
}
