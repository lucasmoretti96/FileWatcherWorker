using FileWatchWorker.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileWatchWorker
{
    public class WorkerService : BackgroundService
    {
        private readonly IFileWatcherService _fileWatcherService;
        private readonly ILogger<WorkerService> _logger;

        public WorkerService(IFileWatcherService fileWatcherService, ILogger<WorkerService> logger)
        {
            _fileWatcherService = fileWatcherService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting File Watcher");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                AppDomain.MonitoringIsEnabled = true;
                _fileWatcherService.StartWatcher();
                await Task.Delay(-1, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting File Watcher");
                _fileWatcherService.StopWatcher();
                throw;
            }
        } 
    }
}
