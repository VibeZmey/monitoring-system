namespace MonitoringSystem.Infrastructure.Services;

public static class MetricConfigurersService
{
     public static bool CpuUsagePct {get;set;}
     public static bool CpuTempC {get;set;}
    
     public static bool GpuUsagePct {get;set;}
     public static bool GpuTempC {get;set;}
    
     public static bool RamUsagePct {get;set;}
     public static bool RamUsedMb {get;set;}
     public static bool RamTotalMb {get;set;}
    
     public static bool DiskUsagePct {get;set;}
     public static bool DiskUsedMb {get;set;}
     public static bool DiskTotalMb {get;set;}
    
     public static bool NetBytesSentTotal {get;set;}
     public static bool NetBytesRecvTotal {get;set;}

     public static void Configure()
     {
          
     }
}