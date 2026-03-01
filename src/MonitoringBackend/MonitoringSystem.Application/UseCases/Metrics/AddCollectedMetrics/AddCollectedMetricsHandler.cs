using MediatR;
using Microsoft.Extensions.Logging;
using MonitoringSystem.Application.Interfaces;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Application.UseCases.Metrics.AddCollectedMetrics;

public class AddCollectedMetricsHandler(IAppDbContext context, ILogger<AddCollectedMetricsHandler> logger)
    : IRequestHandler<AddCollectedMetricsCommand, Unit>
{
    public async Task<Unit> Handle(AddCollectedMetricsCommand request, CancellationToken cancellationToken)
    {
        Metric metric = new Metric()
        {
            Ts = DateTimeOffset.UtcNow,

            CpuUsagePct = request.CpuUsagePct,
            CpuTempC = request.CpuTempC,

            GpuUsagePct = request.GpuUsagePct,
            GpuTempC = request.GpuTempC,

            RamUsagePct = request.RamUsagePct,
            RamUsedMb = request.RamUsedMb,
            RamTotalMb = request.RamTotalMb,

            DiskUsagePct = request.DiskUsagePct,
            DiskUsedMb = request.DiskUsedMb,
            DiskTotalMb = request.DiskTotalMb,

            NetBytesSentTotal = request.NetBytesSentTotal,
            NetBytesRecvTotal = request.NetBytesRecvTotal
        };
        logger.LogInformation($"Collected metrics added {metric.Ts}");
        await context.Metrics.AddAsync(metric,  cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

}