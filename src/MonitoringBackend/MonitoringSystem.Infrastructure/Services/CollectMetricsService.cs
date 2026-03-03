using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;

namespace MonitoringSystem.Infrastructure.Services;

public sealed class CollectMetricsService : IDisposable
{
    private readonly Computer _computer;
    private readonly UpdateVisitor _updateVisitor;
    private readonly PerformanceCounter _cpuCounter;

    private NetworkInterface? _networkInterface;
    private long _prevSent;
    private long _prevRecv;
    private DateTime _prevNetSampleAtUtc;

    public CollectMetricsService()
    {
        _updateVisitor = new UpdateVisitor();

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

    public void Refresh()
    {
        _computer.Accept(_updateVisitor);
    }

    public double GetCpuUsagePct()
    {
        return Math.Round(_cpuCounter.NextValue(), 2);
    }

    public int GetRamTotalMb()
    {
        return (int)(GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024);
    }

    public int GetRamUsedMb()
    {
        return (int)(GC.GetGCMemoryInfo().MemoryLoadBytes / 1024 / 1024);
    }

    public double GetRamUsagePct()
    {
        var info = GC.GetGCMemoryInfo();
        if (info.TotalAvailableMemoryBytes <= 0) return 0;
        return Math.Round((double)info.MemoryLoadBytes / info.TotalAvailableMemoryBytes * 100, 2);
    }

    public double? GetCpuTemperature()
    {
        return TemperatureReader.GetCpuTemperatureC(_computer, _updateVisitor);
    }

    public double? GetGpuUsagePct()
    {
        Refresh();

        var gpu = FindGpuHardware();
        if (gpu == null) return null;

        var loads = EnumerateSensorsRecursive(gpu)
            .Where(s => s.SensorType == SensorType.Load && s.Value.HasValue)
            .ToList();

        if (loads.Count == 0) return null;

        string[] preferred =
        [
            "GPU Core",
            "GPU",
            "Core",
            "3D",
            "D3D",
            "Graphics",
            "GFX",
            "Compute",
            "Total"
        ];

        foreach (var key in preferred)
        {
            var match = loads.FirstOrDefault(s => s.Name.Contains(key, StringComparison.OrdinalIgnoreCase));
            if (match?.Value is float v && v >= 0 && v <= 100)
                return Math.Round((double)v, 1);
        }

        var max = loads.Max(s => s.Value);
        if (!max.HasValue) return null;

        var value = (double)max.Value;
        if (value < 0 || value > 100) return null;

        return Math.Round(value, 1);
    }

    public double? GetGpuTemperature()
    {
        Refresh();

        var gpu = FindGpuHardware();
        if (gpu == null) return null;

        var temps = EnumerateSensorsRecursive(gpu)
            .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
            .ToList();

        if (temps.Count == 0) return null;

        string[] preferred = ["Hot Spot", "Hotspot", "GPU Core", "Core", "Memory", "VRM"];

        ISensor? picked = null;
        foreach (var key in preferred)
        {
            picked = temps.FirstOrDefault(t => t.Name.Contains(key, StringComparison.OrdinalIgnoreCase));
            if (picked != null) break;
        }

        picked ??= temps.OrderByDescending(t => t.Value).FirstOrDefault();

        return picked?.Value is null ? null : Math.Round(picked.Value.Value, 1);
    }

    public int GetDiskTotalMb(string driveLetter = "C")
    {
        var drive = new DriveInfo(driveLetter);
        return (int)(drive.TotalSize / 1024 / 1024);
    }

    public int GetDiskUsedMb(string driveLetter = "C")
    {
        var drive = new DriveInfo(driveLetter);
        return (int)((drive.TotalSize - drive.TotalFreeSpace) / 1024 / 1024);
    }

    public double? GetDiskUsagePercent(string driveLetter = "C")
    {
        var drive = new DriveInfo(driveLetter);
        if (drive.TotalSize <= 0) return null;

        return Math.Round((double)(drive.TotalSize - drive.TotalFreeSpace) / drive.TotalSize * 100, 2);
    }

    public void InitializeNetwork()
    {
        _networkInterface = NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up)
            .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .OrderByDescending(n => n.Speed)
            .FirstOrDefault();

        if (_networkInterface != null)
        {
            var stats = _networkInterface.GetIPv4Statistics();
            _prevSent = stats.BytesSent;
            _prevRecv = stats.BytesReceived;
            _prevNetSampleAtUtc = DateTime.UtcNow;
        }
    }

    public long GetNetworkSentTotal()
    {
        if (_networkInterface == null) return 0;
        return _networkInterface.GetIPv4Statistics().BytesSent;
    }

    public long GetNetworkRecvTotal()
    {
        if (_networkInterface == null) return 0;
        return _networkInterface.GetIPv4Statistics().BytesReceived;
    }

    public double GetNetworkSentBps()
    {
        return SampleNetworkSpeed().sentBps;
    }

    public double GetNetworkRecvBps()
    {
        return SampleNetworkSpeed().recvBps;
    }

    private (double sentBps, double recvBps) SampleNetworkSpeed()
    {
        if (_networkInterface == null) return (0, 0);

        var now = DateTime.UtcNow;
        var elapsed = (now - _prevNetSampleAtUtc).TotalSeconds;
        if (elapsed <= 0.0001) return (0, 0);
        
        var stats = _networkInterface.GetIPv4Statistics();
        var sentNow = stats.BytesSent;
        var recvNow = stats.BytesReceived;

        var sentDelta = sentNow - _prevSent;
        var recvDelta = recvNow - _prevRecv;

        _prevSent = sentNow;
        _prevRecv = recvNow;
        _prevNetSampleAtUtc = now;

        return (sentDelta / elapsed, recvDelta / elapsed);
    }

    private IHardware? FindGpuHardware()
    {
        foreach (var hw in _computer.Hardware)
        {
            var found = EnumerateHardwareRecursive(hw)
                .FirstOrDefault(h =>
                    h.HardwareType == HardwareType.GpuNvidia ||
                    h.HardwareType == HardwareType.GpuAmd ||
                    h.HardwareType == HardwareType.GpuIntel);

            if (found != null) return found;
        }

        return null;
    }

    private static IEnumerable<IHardware> EnumerateHardwareRecursive(IHardware root)
    {
        yield return root;

        foreach (var sub in root.SubHardware)
        {
            foreach (var nested in EnumerateHardwareRecursive(sub))
                yield return nested;
        }
    }

    private static IEnumerable<ISensor> EnumerateSensorsRecursive(IHardware root)
    {
        foreach (var s in root.Sensors)
            yield return s;

        foreach (var sub in root.SubHardware)
        {
            foreach (var ss in EnumerateSensorsRecursive(sub))
                yield return ss;
        }
    }

    public void DebugAllHardware()
    {
        Console.WriteLine("=== LIBRE HARDWARE MONITOR DEBUG ===\n");

        var exeDir = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine($"[INFO] App dir: {exeDir}");
        Console.WriteLine($"[INFO] WinRing0x64.dll exists: {File.Exists(Path.Combine(exeDir, "WinRing0x64.dll"))}");
        Console.WriteLine($"[INFO] WinRing0x64.sys exists: {File.Exists(Path.Combine(exeDir, "WinRing0x64.sys"))}\n");

        Refresh();
        Thread.Sleep(250);
        Refresh();

        foreach (var hardware in _computer.Hardware)
        {
            Console.WriteLine($"\n[HW] {hardware.Name} | Type: {hardware.HardwareType} | Identifier: {hardware.Identifier}");

            foreach (var sensor in hardware.Sensors)
            {
                if (sensor.SensorType == SensorType.Temperature)
                    Console.WriteLine($"  [TEMP] '{sensor.Name}' | Value: {sensor.Value?.ToString() ?? "null"}");
            }

            foreach (var subHardware in hardware.SubHardware)
            {
                Console.WriteLine($"  [SUB] {subHardware.Name} | Type: {subHardware.HardwareType}");
                foreach (var sensor in subHardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                        Console.WriteLine($"    [TEMP] '{sensor.Name}' | Value: {sensor.Value?.ToString() ?? "null"}");
                }
            }
        }

        Console.WriteLine("\n=== END DEBUG ===");
    }

    public void Dispose()
    {
        _cpuCounter.Dispose();
        _computer.Close();
    }
}

public sealed class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer) => computer.Traverse(this);

    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (var subHardware in hardware.SubHardware)
            subHardware.Accept(this);
    }

    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}



public static class TemperatureReader
{
    public static double? GetCpuTemperatureC(Computer computer, IVisitor updateVisitor)
    {
        var lhm = TryGetCpuTempFromLibreHardwareMonitor(computer, updateVisitor);
        if (IsValidCpuTemp(lhm)) return Math.Round(lhm!.Value, 1);

        var wmi = TryGetCpuTempFromAcpiThermalZoneWmi();
        if (IsValidCpuTemp(wmi)) return Math.Round(wmi!.Value, 1);

        return null;
    }

    private static double? TryGetCpuTempFromLibreHardwareMonitor(Computer computer, IVisitor updateVisitor)
    {
        computer.Accept(updateVisitor);
        Thread.Sleep(30);
        computer.Accept(updateVisitor);

        var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
        if (cpu == null) return null;

        var temps = EnumerateSensorsRecursive(cpu)
            .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
            .ToList();

        if (temps.Count == 0) return null;

        string[] preferredNames =
        [
            "Core (Tctl/Tdie)",
            "CPU Package",
            "Package",
            "Tdie",
            "Tctl"
        ];

        foreach (var key in preferredNames)
        {
            var match = temps.FirstOrDefault(s => s.Name.Contains(key, StringComparison.OrdinalIgnoreCase));
            var v = match?.Value;
            if (v.HasValue) return (double)v.Value;
        }

        var max = temps.Max(s => s.Value);
        return max.HasValue ? (double)max.Value : null;
    }

    private static double? TryGetCpuTempFromAcpiThermalZoneWmi()
    {
        try
        {
            var scope = new ManagementScope(@"\\.\root\WMI");
            scope.Connect();

            var query = new ObjectQuery("SELECT CurrentTemperature FROM MSAcpi_ThermalZoneTemperature");

            using var searcher = new ManagementObjectSearcher(scope, query);
            searcher.Options.ReturnImmediately = false;
            searcher.Options.Rewindable = false;

            using var results = searcher.Get();

            double? best = null;

            foreach (ManagementObject obj in results)
            {
                try
                {
                    if (obj["CurrentTemperature"] is uint raw && raw != 0)
                    {
                        var c = (raw / 10.0) - 273.15;
                        if (!best.HasValue || c > best.Value) best = c;
                    }
                }
                finally
                {
                    obj.Dispose();
                }
            }

            return best;
        }
        catch
        {
            return null;
        }
    }

    private static IEnumerable<ISensor> EnumerateSensorsRecursive(IHardware root)
    {
        foreach (var s in root.Sensors)
            yield return s;

        foreach (var sub in root.SubHardware)
        {
            foreach (var ss in EnumerateSensorsRecursive(sub))
                yield return ss;
        }
    }

    private static bool IsValidCpuTemp(double? celsius)
    {
        if (!celsius.HasValue) return false;

        var t = celsius.Value;

        if (double.IsNaN(t) || double.IsInfinity(t)) return false;
        if (t == 0.0) return false;

        return t > -20 && t < 125;
    }
}