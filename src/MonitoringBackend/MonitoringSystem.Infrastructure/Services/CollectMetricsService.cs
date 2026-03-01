using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace MonitoringSystem.Infrastructure.Services;

public class CollectMetricsService : IDisposable
{
    internal readonly Computer _computer;
    internal readonly PerformanceCounter _cpuCounter;
    internal NetworkInterface? _networkInterface;

    public CollectMetricsService()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };
        _computer.Open();

        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _cpuCounter.NextValue();

        InitializeNetwork();
    }

    internal double? GetCpuUsagePct()
    {
        return Math.Round(_cpuCounter.NextValue(), 2);
    } 
    
    internal int? GetRamTotalMb()
    {
        return (int)(GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024);
    } 
    
    internal int? GetRamUsedMb()
    {
        return (int)(GC.GetGCMemoryInfo().MemoryLoadBytes / 1024 / 1024);
    } 
    
    internal double? GetRamUsagePct()
    {
        return Math.Round((double)GC.GetGCMemoryInfo().MemoryLoadBytes / GC.GetGCMemoryInfo().TotalAvailableMemoryBytes * 100, 2);
    } 
    
    internal double? GetCpuTemperature()
    {
        var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
        if (cpu == null) return null;
        
        cpu.Update();

        var tempSensors = cpu.Sensors
            .Where(s => 
                s.SensorType == SensorType.Temperature && 
                s.Value > 0
            )
            .ToList();

        if (!tempSensors.Any()) return null;

        var maxTemp = tempSensors.Max(s => s.Value.Value);
        return Math.Round(maxTemp, 1);
    }

    internal double? GetGpuUsage()
    {
        var gpu = _computer.Hardware.FirstOrDefault(h => 
            h.HardwareType == HardwareType.GpuNvidia || 
            h.HardwareType == HardwareType.GpuAmd || 
            h.HardwareType == HardwareType.GpuIntel);
        return gpu?.Sensors
            .FirstOrDefault(s => s.SensorType == SensorType.Load && s.Name.Contains("GPU"))?
            .Value;
    }

    internal double? GetGpuTemperature()
    {
        var gpu = _computer.Hardware.FirstOrDefault(h => 
            h.HardwareType == HardwareType.GpuNvidia || 
            h.HardwareType == HardwareType.GpuAmd || 
            h.HardwareType == HardwareType.GpuIntel);
        return gpu?.Sensors
            .FirstOrDefault(s => s.SensorType == SensorType.Temperature)?
            .Value;
    }

    internal int GetDiskTotalMb()
    {
        var drive = new DriveInfo("C");
        return (int)(drive.TotalSize / 1024 / 1024);
    }

    internal int GetDiskUsedMb()
    {
        var drive = new DriveInfo("C");
        return (int)((drive.TotalSize - drive.TotalFreeSpace) / 1024 / 1024);
    }

    internal double? GetDiskUsagePercent()
    {
        var drive = new DriveInfo("C");
        if (drive.TotalSize == 0) return null;
        return Math.Round((double)(drive.TotalSize - drive.TotalFreeSpace) / drive.TotalSize * 100, 2);
    }

    internal void InitializeNetwork()
    {
        _networkInterface = NetworkInterface.GetAllNetworkInterfaces()
            .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up && 
                                 n.NetworkInterfaceType != NetworkInterfaceType.Loopback);
        
        if (_networkInterface != null)
        {
            var stats = _networkInterface.GetIPv4Statistics();
            // Инициализация предыдущих значений при необходимости
        }
    }

    internal long GetNetworkSent()
    {
        if (_networkInterface == null) return 0;
        return _networkInterface.GetIPv4Statistics().BytesSent;
    }

    internal long GetNetworkRecv()
    {
        if (_networkInterface == null) return 0;
        return _networkInterface.GetIPv4Statistics().BytesReceived;
    }

    public void Dispose()
    {
        _cpuCounter.Dispose();
        _computer.Close();
    }
}
