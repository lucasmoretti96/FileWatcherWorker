using FileWatchWorker;
using FileWatchWorker.Services.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileWatchService.Test
{
    public class WorkerServiceTests
    {
        private readonly IFileWatcherService _fileWatcherService;
        private readonly ILogger<WorkerService> _logger;
        private readonly WorkerService _worker;

        public WorkerServiceTests()
        {
            _fileWatcherService = Substitute.For<IFileWatcherService>();
            _logger = Substitute.For<ILogger<WorkerService>>();

            _worker = new WorkerService(_fileWatcherService, _logger);
        }

        [Fact]
        public async Task Should_ExecuteAsync_Success()
        {
            await _worker.StartAsync(CancellationToken.None);
            _fileWatcherService.ReceivedWithAnyArgs(1).StartWatcher();
        }

        [Fact]
        public async Task Should_ExecuteAsync_Exception()
        {
            _fileWatcherService.When(x => x.StartWatcher()).Do(x => throw new Exception());

            await Assert.ThrowsAsync<Exception>(async () => await _worker.StartAsync(CancellationToken.None));
            _fileWatcherService.ReceivedWithAnyArgs(1).StartWatcher();
            _fileWatcherService.ReceivedWithAnyArgs(1).StopWatcher();
        }
    }
}
