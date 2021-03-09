using FileWatchWorker.Services;
using FileWatchWorker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FileWatchService.Worker.Extensions.IoC
{
    [ExcludeFromCodeCoverage]
    public static class ServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IFileWatcherService, FileWatcherService>();
            services.AddTransient<IFileServiceHandler, FileServiceHandler>();

            return services;
        }
    }
}
