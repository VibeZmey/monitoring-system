namespace MonitoringSystem.Domain.Models;

public class MetricRollup1M
{
    public long Id {get;init;}
    public DateTimeOffset BucketTs {get;init;}
    
    public double? CpuUsageMin {get;init;}
    public double? CpuUsageMax {get;init;}
    public double? CpuUsageAvg {get;init;}
    public double? CpuTempMin {get;init;}
    public double? CpuTempMax {get;init;}
    public double? CpuTempAvg {get;init;}
    
    public double? GpuUsageMin {get;init;}
    public double? GpuUsageMax {get;init;}
    public double? GpuUsageAvg {get;init;}
    public double? GpuTempMin {get;init;}
    public double? GpuTempMax {get;init;}
    public double? GpuTempAvg {get;init;}
    
    public double? RamUsageMin {get;init;}
    public double? RamUsageMax {get;init;}
    public double? RamUsageAvg {get;init;}
    public int? RamUsedMbMin {get;init;}
    public int? RamUsedMbMax {get;init;}
    public int? RamUsedMbAvg {get;init;}
    
    public double? DiskUsageMin {get;init;}
    public double? DiskUsageMax {get;init;}
    public double? DiskUsageAvg {get;init;}
    public int? DiskUsedMbMin {get;init;}
    public int? DiskUsedMbMax {get;init;}
    public int? DiskUsedMbAvg {get;init;}
    
    public long? NetSentBytesPerSecAvg {get;init;}
    public long? NetRecvBytesPerSecAvg {get;init;}
}