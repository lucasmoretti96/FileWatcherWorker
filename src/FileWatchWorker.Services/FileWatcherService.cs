using FileWatcherWorker.CrossCutting.Options;
using FileWatchWorker.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace FileWatchWorker.Services
{
    public class FileWatcherService : IFileWatcherService
    {
        private FileSystemWatcher _watcher;
        private readonly WatcherOptions _options;
        private readonly IFileServiceHandler _fileServiceHandler;
        private readonly ILogger<FileWatcherService> _logger;

        public FileWatcherService(
            IOptions<WatcherOptions> options,
            IFileServiceHandler fileServiceHandler,
            ILogger<FileWatcherService> logger)
        {
            _options = options.Value;
            _fileServiceHandler = fileServiceHandler;
            _logger = logger;
        }

        public void StartWatcher()
        {
            _watcher = new FileSystemWatcher(_options.Directory);
            _watcher.Created += Created;
            _watcher.Error += Error;
            _watcher.IncludeSubdirectories = false;

            _watcher.EnableRaisingEvents = true;

            _logger.LogInformation("File watcher started in the directory {Directory}", _options.Directory);
        }

        public void Error(object sender, ErrorEventArgs e)
        {
            _logger.LogError(e.GetException(), "Error reading the directory {Directory}:{FileName}", _options.Directory);
        }

        public void StopWatcher()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Created -= Created;
                _watcher = null;
            }

            _logger.LogInformation("File watcher stoped");
        }

        public async void Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                _logger.LogInformation("File {FileName} included in directory {Directory}", e.Name, _options.Directory);
                var fileName = e.Name.Substring(0, 4);

                if (fileName.Equals("FILE"))
                {
                    await _fileServiceHandler.FileDetected(fileName);
                    _logger.LogInformation("Handle executed with success");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from handling the file");
            }
        }
    }
}
