using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitoringSystem.Application.UseCases.Metrics.AddCollectedMetrics;
using MonitoringSystem.Application.UseCases.MetricSettings.GetEnabledMetrics;

namespace MonitoringSystem.Infrastructure.Services;

public class CollectMetricsHostService
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        Dictionary<string, bool> enabledMetricNames = await mediator
            .Send(new GetEnabledMetricsQuery(), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            AddCollectedMetricsCommand command = new AddCollectedMetricsCommand()
            {
                CpuUsagePct = enabledMetricNames["CpuUsagePct"] ? collectService.GetCpuUsagePct() : null,
                CpuTempC = enabledMetricNames["CpuTempC"] ? collectService.GetCpuTemperature() : null,
                GpuUsagePct = enabledMetricNames["GpuUsagePct"] ? collectService.GetGpuUsage() : null,
                GpuTempC = enabledMetricNames["GpuTempC"] ? collectService.GetGpuTemperature() : null,
                RamUsagePct = enabledMetricNames["RamUsagePct"] ? collectService.GetRamUsagePct() : null,
                RamUsedMb = enabledMetricNames["RamUsedMb"] ? collectService.GetRamUsedMb() : null,
                RamTotalMb = enabledMetricNames["RamTotalMb"] ? collectService.GetRamTotalMb() : null,
                DiskUsagePct = enabledMetricNames["DiskUsagePct"] ? collectService.GetDiskUsagePercent() : null,
                DiskUsedMb = enabledMetricNames["DiskUsedMb"] ? collectService.GetDiskUsedMb() : null,
                DiskTotalMb = enabledMetricNames["DiskTotalMb"] ? collectService.GetDiskTotalMb() : null,
                NetBytesSentTotal = enabledMetricNames["NetBytesSentTotal"] ? collectService.GetNetworkSent() : null,
                NetBytesRecvTotal = enabledMetricNames["NetBytesRecvTotal"] ? collectService.GetNetworkRecv() : null,
            };
            //_logger.LogInformation($"Collected metrics for {command.RamUsagePct}%");
            await mediator.Send(command, stoppingToken);
            await Task.Delay(10000, stoppingToken);
        }
    }
    
    
}
