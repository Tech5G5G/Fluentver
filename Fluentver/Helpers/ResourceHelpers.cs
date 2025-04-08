using System.Management;
using System.Runtime.InteropServices;

namespace Fluentver.Helpers;

/// <summary>Gets CPU statistics.</summary>
public static class CPUHelper
{
    /// <summary>Gets the name of the CPU.</summary>
    /// <value>The name of the CPU.</value>
    public static string CPUName => new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor").Get().Cast<ManagementObject>().Select(i => (string)i["Name"]).FirstOrDefault(string.Empty);

    /// <summary>Gets the CPU usage.</summary>
    /// <value>The percent of the CPU used.</value>
    public static float CPUUsage => utility.NextValue();

    private static readonly PerformanceCounter utility = new("Processor Information", "% Processor Utility", "_Total");
}

/// <summary>Gets GPU statistics.</summary>
public static class GPUHelper
{
    /// <summary>Gets the name of the GPU.</summary>
    /// <value>The name of the GPU.</value>
    public static string GPUName => new ManagementObjectSearcher("select * from Win32_VideoController").Get().Cast<ManagementObject>().Select(i => (string)i["Name"]).FirstOrDefault(string.Empty);

    /// <summary>Gets the GPU usage.</summary>
    /// <returns>The percent of the GPU used.</returns>
    public static float GPUUsage
    {
        get
        {
            float sum = 0;
            List<PerformanceCounter> trunicate = [];
            lock (gpuCounters)
            {
                foreach (var counter in gpuCounters)
                {
                    sum += AssignerHelper.TryAssign(counter.NextValue, () =>
                    {
                        trunicate.Add(counter);
                        return 0;
                    });
                }
                foreach (var counter in trunicate)
                    gpuCounters.Remove(counter);
            }
            return sum;
        }
    }

    private static readonly List<PerformanceCounter> gpuCounters = GetGPUCounters();
    private static List<PerformanceCounter> GetGPUCounters()
    {
        var category = new PerformanceCounterCategory("GPU Engine");
        var counterNames = category.GetInstanceNames();

        return [.. counterNames.Where(counterName => counterName.EndsWith("engtype_3D"))
            .SelectMany(category.GetCounters)
            .Where(counter =>
            {
                var values = counter.InstanceName.Split('_');
                return counter.CounterName == "Utilization Percentage" && values[Array.IndexOf(values, "phys") + 1] == "0";
            })];
    }
}

/// <summary>Wrapper for the MemoryStatusEx structure, that contains memory statistics.</summary>
public class RAMHelper
{
    #region PInvoke

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    private struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX() => dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
    }

    private static bool TryCreateMemoryStatus(out MEMORYSTATUSEX status)
    {
        MEMORYSTATUSEX memStatus = new();
        bool created = GlobalMemoryStatusEx(ref memStatus);
        status = created ? memStatus : default;
        return created;
    }

    #endregion

    const int BytesInGigabyte = 1024 * 1024 * 1024;

    readonly MEMORYSTATUSEX status;

    /// <summary>Constructs a new instance of the <see cref="RAMHelper"/> class.</summary>
    public RAMHelper() => TryCreateMemoryStatus(out status);

    /// <summary>Gets the total amount of RAM the system has.</summary>
    /// <value>The amount of RAM installed in the system in gigabytes.</value>
    public float TotalRAM => (float)status.ullTotalPhys / BytesInGigabyte;

    /// <summary>Gets the amount of RAM that is being used.</summary>
    /// <value>The amount of RAM being used by the system in gigabytes.</value>
    public float UsedRAM => (status.ullTotalPhys - status.ullAvailPhys) / BytesInGigabyte;

    /// <summary>Gets the percentage of RAM that is being used.</summary>
    /// <value>The percentage of RAM being used by the system.</value>
    public float UsedRAMPercent => UsedRAM / TotalRAM * 100;
}
