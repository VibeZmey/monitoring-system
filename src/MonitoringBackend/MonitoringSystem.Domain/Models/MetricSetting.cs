namespace MonitoringSystem.Domain.Models;

public enum MetricName
{
    CpuUsagePct,
    CpuTempC,
    
    GpuUsagePct,
    GpuTempC,
        
    RamUsagePct,
    RamUsedMb,
    RamTotalMb,
        
    DiskUsagePct,
    DiskUsedMb,
    DiskTotalMb,
        
    NetBytesSentTotal,
    NetBytesRecvTotal
}

public class MetricSetting
{
    public string MetricName {get;init;}
    public bool IsEnabled {get;set;}
    public int DisplayOrder {get;set;}
    public DateTime CreatedAt {get;init;}
    public DateTime? UpdatedAt { get; set; } = null;
}