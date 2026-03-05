using MediatR;

namespace MonitoringSystem.Application.UseCases.Metrics.AddCollectedMetrics;

public class AddCollectedMetricsCommand : IRequest<Unit>
{
    public double? CpuUsagePct {get;init;}
    public double? CpuTempC {get;init;}
    
    public double? GpuUsagePct {get;init;}
    public double? GpuTempC {get;init;}
    
    public double? RamUsagePct {get;init;}
    public int? RamUsedMb {get;init;}
    public int? RamTotalMb {get;init;}
    
    public double? DiskUsagePct {get;init;}
    public int? DiskUsedMb {get;init;}
    public int? DiskTotalMb {get;init;}
    
    public long? NetBytesSentTotal {get;init;}
    public long? NetBytesRecvTotal {get;init;}
}