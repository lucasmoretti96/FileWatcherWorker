using FileWatchService.Worker.Extensions.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Http.BatchFormatters;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FileWatchWorker
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static IConfiguration Configuration { get; } = BuildConfiguration();

        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddDependencies(Configuration)
                        .AddHostedService<WorkerService>();
                })
                .UseSerilog((host, loggerConfiguration) => CreateLoggerConfiguration(loggerConfiguration, host));

        private static IConfiguration BuildConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            var appSettingsJson = $"appsettings.{env}.json";

            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile(appSettingsJson, true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void CreateLoggerConfiguration(LoggerConfiguration loggerConfiguration, HostBuilderContext hostingContext)
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var logPath = hostingContext.Configuration.GetSection("LogPath")?.Value;

            if (string.IsNullOrEmpty(logPath))
                logPath = $"{AppContext.BaseDirectory}/log";

            loggerConfiguration
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .Destructure.ToMaximumCollectionCount(10)
                .Destructure.ToMaximumStringLength(1024)
                .Destructure.ToMaximumDepth(5)
                .Enrich.WithProperty("Assembly", $"{assembly.Name}")
                .Enrich.WithProperty("Version", $"{assembly.Version}")
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers()
                    .WithRootName("Exception"))

#if DEBUG
                .WriteTo.Async(logConsole =>
                    logConsole.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}",
                        restrictedToMinimumLevel: LogEventLevel.Debug))
#endif
                .WriteTo.Async(logFile =>
                logFile.File($"{logPath}/{assembly.Name}/{assembly.Name}.txt", rollingInterval: RollingInterval.Day))

                .WriteTo.Async(logAsync =>
                    logAsync.Http(
                        hostingContext.Configuration.GetConnectionString("Logstash"),
                        batchFormatter: new ArrayBatchFormatter(),
                        textFormatter: new ElasticsearchJsonFormatter(
                            inlineFields: true,
                            renderMessageTemplate: false)));
        }
    }
}
