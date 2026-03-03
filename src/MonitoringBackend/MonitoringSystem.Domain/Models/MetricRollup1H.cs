namespace MonitoringSystem.Domain.Models;

public class MetricRollup1H
{
    public long Id {get;set;}
    public DateTimeOffset BucketTs {get;set;}
    
    public double? CpuUsageMin {get;set;}
    public double? CpuUsageMax {get;set;}
    public double? CpuUsageAvg {get;set;}
    public double? CpuTempMin {get;set;}
    public double? CpuTempMax {get;set;}
    public double? CpuTempAvg {get;set;}
    
    public double? GpuUsageMin {get;set;}
    public double? GpuUsageMax {get;set;}
    public double? GpuUsageAvg {get;set;}
    public double? GpuTempMin {get;set;}
    public double? GpuTempMax {get;set;}
    public double? GpuTempAvg {get;set;}
    
    public double? RamUsageMin {get;set;}
    public double? RamUsageMax {get;set;}
    public double? RamUsageAvg {get;set;}
    public int? RamUsedMbMin {get;set;}
    public int? RamUsedMbMax {get;set;}
    public int? RamUsedMbAvg {get;set;}
    
    public double? DiskUsageMin {get;set;}
    public double? DiskUsageMax {get;set;}
    public double? DiskUsageAvg {get;set;}
    public int? DiskUsedMbMin {get;set;}
    public int? DiskUsedMbMax {get;set;}
    public int? DiskUsedMbAvg {get;set;}
    
    public long? NetSentBytesPerSecAvg {get;set;}
    public long? NetRecvBytesPerSecAvg {get;set;}
}