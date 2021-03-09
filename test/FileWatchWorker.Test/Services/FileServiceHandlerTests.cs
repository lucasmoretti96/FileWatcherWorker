using FileWatchWorker.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace FileWatchService.Test.Services
{
    public class FileServiceHandlerTests
    {
        private readonly FileServiceHandler _handler;
        private readonly ILogger<FileServiceHandler> _logger;

        private const string FILE_NAME = "FILE.txt";

        public FileServiceHandlerTests()
        {
            _logger = Substitute.For<ILogger<FileServiceHandler>>();

            _handler = new FileServiceHandler(_logger);
        }

        [Fact]
        public async Task Should_Received_LogMessage()
        {
            await _handler.FileDetected(FILE_NAME);
            _logger.ReceivedWithAnyArgs(1).LogInformation(string.Empty, new object());
        }
    }
}
