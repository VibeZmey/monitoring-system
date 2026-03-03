using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MonitoringSystem.Application.Interfaces;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Application.UseCases.Rollups.CreateRollup1M;

public class CreateRollup1MHandler(IAppDbContext context) : IRequestHandler<CreateRollup1MCommand, Unit>
{
    public async Task<Unit> Handle(CreateRollup1MCommand request, CancellationToken cancellationToken)
    {
        List<Metric> selectedMetrics = await context.Metrics
            .Where(x => x.Ts >= request.MetricTs.AddMinutes(-1) && x.Ts <= request.MetricTs)
            .ToListAsync(cancellationToken);

        int count = selectedMetrics.Count;
        Console.WriteLine($"!!!!!!!!!{selectedMetrics.Count}!!!!!!!!!!!");
        if (count == 0) return Unit.Value;

        MetricRollup1M rollup = new MetricRollup1M();


        foreach (var metric in selectedMetrics)
        {
            if (metric.CpuUsagePct is not null)
            {
                rollup.CpuUsageMin = metric.CpuUsagePct < rollup.CpuUsageMin ? metric.CpuUsagePct : rollup.CpuUsageMin;
                rollup.CpuUsageAvg += metric.CpuUsagePct;
                rollup.CpuUsageMax = metric.CpuUsagePct > rollup.CpuUsageMax ? metric.CpuUsagePct : rollup.CpuUsageMax;
            }

            if (metric.CpuTempC is not null)
            {
                rollup.CpuTempMin = metric.CpuTempC < rollup.CpuTempMin ? metric.CpuTempC : rollup.CpuTempMin;
                rollup.CpuTempAvg += metric.CpuTempC;
                rollup.CpuTempMax = metric.CpuTempC > rollup.CpuTempMax ? metric.CpuTempC : rollup.CpuTempMax;
            }

            if (metric.GpuUsagePct is not null)
            {
                rollup.GpuUsageMin = metric.GpuUsagePct < rollup.GpuUsageMin ? metric.GpuUsagePct : rollup.GpuUsageMin;
                rollup.GpuUsageAvg += metric.GpuUsagePct;
                rollup.GpuUsageMax = metric.GpuUsagePct > rollup.GpuUsageMax ? metric.GpuUsagePct : rollup.GpuUsageMax;
            }

            if (metric.GpuTempC is not null)
            {
                rollup.GpuTempMin = metric.GpuTempC < rollup.GpuTempMin ? metric.GpuTempC : rollup.GpuTempMin;
                rollup.GpuTempAvg += metric.GpuTempC;
                rollup.GpuTempMax = metric.GpuTempC > rollup.GpuTempMax ? metric.GpuTempC : rollup.GpuTempMax;
            }

            if (metric.RamUsagePct is not null)
            {
                rollup.RamUsageMin = metric.RamUsagePct < rollup.RamUsageMin ? metric.RamUsagePct : rollup.RamUsageMin;
                rollup.RamUsageAvg += metric.RamUsagePct;
                rollup.RamUsageMax = metric.RamUsagePct > rollup.RamUsageMax ? metric.RamUsagePct : rollup.RamUsageMax;
            }

            if (metric.RamUsedMb is not null)
            {
                rollup.RamUsedMbMin = metric.RamUsedMb < rollup.RamUsedMbMin ? metric.RamUsedMb : rollup.RamUsedMbMin;
                rollup.RamUsedMbAvg += metric.RamUsedMb;
                rollup.RamUsedMbMax = metric.RamUsedMb > rollup.RamUsedMbMax ? metric.RamUsedMb : rollup.RamUsedMbMax;
            }

            if (metric.DiskUsagePct is not null)
            {
                rollup.DiskUsageMin = metric.DiskUsagePct < rollup.DiskUsageMin
                    ? metric.DiskUsagePct
                    : rollup.DiskUsageMin;
                rollup.DiskUsageAvg += metric.DiskUsagePct;
                rollup.DiskUsageMax = metric.DiskUsagePct > rollup.DiskUsageMax
                    ? metric.DiskUsagePct
                    : rollup.DiskUsageMax;
            }

            if (metric.DiskUsedMb is not null)
            {
                rollup.DiskUsedMbMin =
                    metric.DiskUsedMb < rollup.DiskUsedMbMin ? metric.DiskUsedMb : rollup.DiskUsedMbMin;
                rollup.DiskUsedMbAvg += metric.DiskUsedMb;
                rollup.DiskUsedMbMax =
                    metric.DiskUsedMb > rollup.DiskUsedMbMax ? metric.DiskUsedMb : rollup.DiskUsedMbMax;
            }

            if (metric.NetBytesRecvTotal is not null)
            {
                rollup.NetRecvBytesPerSecAvg += metric.NetBytesRecvTotal;
            }

            if (metric.NetBytesSentTotal is not null)
            {
                rollup.NetSentBytesPerSecAvg += metric.NetBytesSentTotal;
            }
        }

        rollup.CpuTempAvg = rollup.CpuTempAvg / count;
        rollup.CpuUsageAvg = rollup.CpuUsageAvg / count;
        rollup.GpuTempAvg = rollup.GpuTempAvg / count;
        rollup.GpuUsageAvg = rollup.GpuUsageAvg / count;
        rollup.RamUsageAvg = rollup.RamUsageAvg / count;
        rollup.RamUsedMbAvg = rollup.RamUsedMbAvg / count;
        rollup.DiskUsageAvg = rollup.DiskUsageAvg / count;
        rollup.DiskUsedMbAvg = rollup.DiskUsedMbAvg / count;
        rollup.NetRecvBytesPerSecAvg = rollup.NetRecvBytesPerSecAvg / count;
        rollup.NetSentBytesPerSecAvg = rollup.NetSentBytesPerSecAvg / count;
        rollup.BucketTs = DateTimeOffset.UtcNow;
        Console.WriteLine($"!!!!!!!!!{rollup.RamUsedMbAvg}!!!!!!!!!!!");

    await context.MetricRollup1M.AddAsync(rollup, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}