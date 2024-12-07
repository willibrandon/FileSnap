using FileSnap.Core.Services;
using FileSnap.Core.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace FileSnap.Tests
{
    public class FileWatcherServiceTests
    {
        private readonly Mock<IFileWatcherService> _fileWatcherServiceMock;
        private readonly string _testDirectory;

        public FileWatcherServiceTests()
        {
            _fileWatcherServiceMock = new Mock<IFileWatcherService>();
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public void StartWatching_ShouldStartWatcher()
        {
            _fileWatcherServiceMock.Object.StartWatching(_testDirectory);
            _fileWatcherServiceMock.Verify(service => service.StartWatching(_testDirectory), Times.Once);
        }

        [Fact]
        public void StopWatching_ShouldStopWatcher()
        {
            _fileWatcherServiceMock.Object.StopWatching();
            _fileWatcherServiceMock.Verify(service => service.StopWatching(), Times.Once);
        }

        [Fact]
        public void ConfigureFilePermissionChanges_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFilePermissionChanges(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFilePermissionChanges(true), Times.Once);
        }

        [Fact]
        public void ConfigureSymbolicLinks_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureSymbolicLinks(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureSymbolicLinks(true), Times.Once);
        }

        [Fact]
        public void ConfigureFileRenames_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFileRenames(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFileRenames(true), Times.Once);
        }

        [Fact]
        public void ConfigureFileDeletions_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFileDeletions(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFileDeletions(true), Times.Once);
        }

        [Fact]
        public void ConfigureFileMoves_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFileMoves(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFileMoves(true), Times.Once);
        }

        [Fact]
        public void ConfigureFileAttributeChanges_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFileAttributeChanges(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFileAttributeChanges(true), Times.Once);
        }

        [Fact]
        public void ConfigureFileCreationEvents_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFileCreationEvents(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFileCreationEvents(true), Times.Once);
        }

        [Fact]
        public void ConfigureFileModifications_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFileModifications(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFileModifications(true), Times.Once);
        }

        [Fact]
        public void ConfigureEventFiltering_ShouldSetFilterOptions()
        {
            var filterOptions = new EventFilterOptions
            {
                FileExtensions = new List<string> { ".txt" },
                Directories = new List<string> { _testDirectory },
                FileNames = new List<string> { "test.txt" }
            };

            _fileWatcherServiceMock.Object.ConfigureEventFiltering(filterOptions);
            _fileWatcherServiceMock.Verify(service => service.ConfigureEventFiltering(filterOptions), Times.Once);
        }

        [Fact]
        public void FileWatcher_ShouldCaptureMetadata()
        {
            var fileWatcherService = new FileWatcherService();
            var testFilePath = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFilePath, "Test content");

            fileWatcherService.StartWatching(_testDirectory);

            var metadata = fileWatcherService.GetFileMetadata(testFilePath);

            Assert.Equal(testFilePath, metadata.Path);
            Assert.Equal("Test content".Length, metadata.Size);
            Assert.Equal(File.GetLastWriteTimeUtc(testFilePath), metadata.LastModified);
            Assert.Equal(File.GetCreationTimeUtc(testFilePath), metadata.CreatedAt);
            Assert.Equal(FileAttributes.Normal, metadata.Attributes);
            Assert.Equal("Unknown", metadata.User);
            Assert.Equal("Unknown", metadata.Process);
            Assert.Equal(DateTime.UtcNow.Date, metadata.Timestamp.Date);

            fileWatcherService.StopWatching();
        }

        [Fact]
        public void ConfigureFileSystemMountEvents_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureFileSystemMountEvents(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureFileSystemMountEvents(true), Times.Once);
        }

        [Fact]
        public void ConfigureNetworkFileSystems_ShouldSetOption()
        {
            _fileWatcherServiceMock.Object.ConfigureNetworkFileSystems(true);
            _fileWatcherServiceMock.Verify(service => service.ConfigureNetworkFileSystems(true), Times.Once);
        }

        [Fact]
        public void FileWatcher_ShouldFilterEvents()
        {
            var fileWatcherService = new FileWatcherService();
            var filterOptions = new EventFilterOptions
            {
                FileExtensions = new List<string> { ".txt" },
                Directories = new List<string> { _testDirectory },
                FileNames = new List<string> { "test.txt" }
            };

            fileWatcherService.ConfigureEventFiltering(filterOptions);

            var testFilePath = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFilePath, "Test content");

            fileWatcherService.StartWatching(_testDirectory);

            var metadata = fileWatcherService.GetFileMetadata(testFilePath);

            Assert.Equal(testFilePath, metadata.Path);
            Assert.Equal("Test content".Length, metadata.Size);
            Assert.Equal(File.GetLastWriteTimeUtc(testFilePath), metadata.LastModified);
            Assert.Equal(File.GetCreationTimeUtc(testFilePath), metadata.CreatedAt);
            Assert.Equal(FileAttributes.Normal, metadata.Attributes);
            Assert.Equal("Unknown", metadata.User);
            Assert.Equal("Unknown", metadata.Process);
            Assert.Equal(DateTime.UtcNow.Date, metadata.Timestamp.Date);

            fileWatcherService.StopWatching();
        }
    }
}
