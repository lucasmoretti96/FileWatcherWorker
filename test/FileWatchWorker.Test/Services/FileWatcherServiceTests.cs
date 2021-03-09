using FileWatcherWorker.CrossCutting.Options;
using FileWatchWorker.Services;
using FileWatchWorker.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.IO;
using Xunit;

namespace FileWatchService.Test.Services
{
    public class FileWatcherServiceTests
    {
        private readonly FileWatcherService _service;
        private readonly IOptions<WatcherOptions> _options;
        private readonly IFileServiceHandler _fileServiceHandler;
        private readonly ILogger<FileWatcherService> _logger;

        public FileWatcherServiceTests()
        {
            _logger = Substitute.For<ILogger<FileWatcherService>>();
            _options = Options.Create(new WatcherOptions
            {
                Directory = Directory.GetCurrentDirectory()
            });
            _fileServiceHandler = Substitute.For<IFileServiceHandler>();
            _service = new FileWatcherService(_options, _fileServiceHandler, _logger);
        }

        [Fact]
        public void Should_StartServiceWatcher()
        {
            _service.StartWatcher();
            _logger.ReceivedWithAnyArgs(1).LogInformation(string.Empty, new object());
        }

        [Fact]
        public void Should_StartServiceWatcher_With_FileServiceHandler_IfFile_Is_FILE()
        {
            _service.Created(null, new FileSystemEventArgs(WatcherChangeTypes.Created, Directory.GetCurrentDirectory(), "FILE.txt"));
            _logger.ReceivedWithAnyArgs(2).LogInformation(string.Empty, new object());
        }

        [Fact]
        public void Should_StartServiceWatcher_And_NotStart_FileServiceHandler_IfFile_IsNot_FILE()
        {
            _service.Created(null, new FileSystemEventArgs(WatcherChangeTypes.Created, Directory.GetCurrentDirectory(), "XPTO.txt"));
            _logger.ReceivedWithAnyArgs(1).LogInformation(string.Empty, new object());
        }

        [Fact]
        public void Should_Create_Event_Called_ThrowException()
        {
            _fileServiceHandler.When(x => x.FileDetected(Arg.Any<string>())).Do(x => throw new Exception());

            _service.Created(null, new FileSystemEventArgs(WatcherChangeTypes.Created, Directory.GetCurrentDirectory(), "FILE.txt"));
            _logger.ReceivedWithAnyArgs(2).LogInformation(string.Empty, new object());
            _fileServiceHandler.ReceivedWithAnyArgs(1).FileDetected(string.Empty);
        }

        [Fact]
        public void Should_StopServiceWatcher()
        {
            _service.StartWatcher();
            _service.StopWatcher();
            _logger.ReceivedWithAnyArgs(2).LogInformation(string.Empty, new object());
        }
    }
}
