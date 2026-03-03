using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitoringSystem.Application.UseCases.Metrics.AddCollectedMetrics;
using MonitoringSystem.Application.UseCases.MetricSettings.GetEnabledMetrics;
using MonitoringSystem.Application.UseCases.Rollups.CreateRollup1M;

namespace MonitoringSystem.Infrastructure.Services;

public sealed class CollectMetricsHostService
    : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CollectMetricsHostService> _logger;
    private readonly CollectMetricsService collectService;

    public CollectMetricsHostService(
        IServiceProvider serviceProvider,
        ILogger<CollectMetricsHostService> logger,
        CollectMetricsService collectService)
    {

        _serviceProvider = serviceProvider;
        _logger = logger;
        this.collectService =  collectService;
    }

    private async Task CollectMetricsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        Dictionary<string, bool> enabledMetricNames = await mediator
            .Send(new GetEnabledMetricsQuery(), stoppingToken);
        
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        
        while (!stoppingToken.IsCancellationRequested &&
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            AddCollectedMetricsCommand command = new AddCollectedMetricsCommand()
            {
                CpuUsagePct = enabledMetricNames["CpuUsagePct"] ? collectService.GetCpuUsagePct() : null,
                CpuTempC = enabledMetricNames["CpuTempC"] ? collectService.GetCpuTemperature() : null,
                GpuUsagePct = enabledMetricNames["GpuUsagePct"] ? collectService.GetGpuUsagePct() : null,
                GpuTempC = enabledMetricNames["GpuTempC"] ? collectService.GetGpuTemperature() : null,
                RamUsagePct = enabledMetricNames["RamUsagePct"] ? collectService.GetRamUsagePct() : null,
                RamUsedMb = enabledMetricNames["RamUsedMb"] ? collectService.GetRamUsedMb() : null,
                RamTotalMb = enabledMetricNames["RamTotalMb"] ? collectService.GetRamTotalMb() : null,
                DiskUsagePct = enabledMetricNames["DiskUsagePct"] ? collectService.GetDiskUsagePercent() : null,
                DiskUsedMb = enabledMetricNames["DiskUsedMb"] ? collectService.GetDiskUsedMb() : null,
                DiskTotalMb = enabledMetricNames["DiskTotalMb"] ? collectService.GetDiskTotalMb() : null,
                NetBytesSentTotal = enabledMetricNames["NetBytesSentTotal"] ? (long)collectService.GetNetworkSentBps() : null,
                NetBytesRecvTotal = enabledMetricNames["NetBytesRecvTotal"] ? (long)collectService.GetNetworkRecvBps() : null,
            };
            //_logger.LogInformation($"Collected metrics for {command.RamUsagePct}%");
            await mediator.Send(command, stoppingToken);
        }
    }

    private async Task RollupAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        
        while (!stoppingToken.IsCancellationRequested &&
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            CreateRollup1MCommand c = new CreateRollup1MCommand()
            {
                MetricTs = DateTimeOffset.UtcNow,
            };
            await mediator.Send(c, stoppingToken);
                
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var metricTask = CollectMetricsAsync(stoppingToken);
        var rollupTask = RollupAsync(stoppingToken);

        await Task.WhenAll(metricTask, rollupTask);
    }
    
    
}
