using FileWatcherWorker.CrossCutting.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FileWatchService.Worker.Extensions.IoC
{
    [ExcludeFromCodeCoverage]
    public static class OptionsExtensions
    {
        public static IServiceCollection AddInternalOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<WatcherOptions>(option => configuration.GetSection("Watcher").Bind(option));

            return serviceCollection;
        }
    }
}
