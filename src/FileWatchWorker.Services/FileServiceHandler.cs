using FileWatchWorker.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FileWatchWorker.Services
{
    public class FileServiceHandler : IFileServiceHandler
    {
        private readonly ILogger<FileServiceHandler> _logger;

        public FileServiceHandler(
            ILogger<FileServiceHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask FileDetected(string fileName)
        {
            try
            {
                _logger.LogInformation("File {FileName} event triggered successfully", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling the file trigger event");
                throw;
            }
        }
    }
}
