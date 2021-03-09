using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FileWatchService.Worker.Extensions.IoC
{
    [ExcludeFromCodeCoverage]
    public static class FileWatcherDependenciesContainer
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices();
            //services.AddRepositories(); If you want to record something on a database that will be the method were you gonna inject the database.
            services.AddInternalOptions(configuration);

            return services;
        }
    }
}
