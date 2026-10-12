using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();

internal sealed class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Archivist worker started. No queues are configured yet.");
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
