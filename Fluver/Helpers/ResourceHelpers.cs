using System.Management;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Fluver.Helpers;

/// <summary>
/// Gets CPU statistics.
/// </summary>
public static class CPUHelper
{
    /// <summary>
    /// Gets the name of the CPU.
    /// </summary>
    public static string CPUName { get; } =
        new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor")
            .Get()
            .Cast<ManagementObject>()
            .Select(i => (string)i["Name"])
            .FirstOrDefault(string.Empty);

    /// <summary>
    /// Gets the percentage of the CPU currently being utilized.
    /// </summary>
    public static float CPUUsage => s_utility.NextValue();

    private static readonly PerformanceCounter s_utility =
        new("Processor Information", "% Processor Utility", "_Total");
}

/// <summary>
/// Gets GPU statistics.
/// </summary>
public static class GPUHelper
{
    /// <summary>
    /// Gets the name of the GPU.
    /// </summary>
    public static string GPUName
    {
        get
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
                var results = searcher.Get().Cast<ManagementObject>().ToList();

                // Build candidate list with useful properties
                var candidates = results
                    .Where(mo => !string.IsNullOrWhiteSpace((mo["Name"] as string) ?? string.Empty))
                    .Select(mo => new
                    {
                        Name = (mo["Name"] as string) ?? string.Empty,
                        Pnp = (mo["PNPDeviceID"] as string) ?? string.Empty,
                        Compatibility = (mo["AdapterCompatibility"] as string) ?? string.Empty,
                        Ram = TryGetAdapterRam(mo),
                    })
                    .ToList();

                var physical = candidates
                    .Where(c => c.Pnp.Contains("PCI\\", StringComparison.OrdinalIgnoreCase))
                    .Where(c =>
                        !s_virtualHints.Any(h =>
                            c.Name.Contains(h, StringComparison.OrdinalIgnoreCase)
                            || c.Compatibility.Contains(h, StringComparison.OrdinalIgnoreCase)
                        )
                    )
                    .ToList();

                // Prefer AMD/NVIDIA by compatibility string
                var preferred = physical
                    .Where(c =>
                        c.Compatibility.Contains("AMD", StringComparison.OrdinalIgnoreCase)
                        || c.Name.Contains("Radeon", StringComparison.OrdinalIgnoreCase)
                        || c.Compatibility.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase)
                    )
                    .OrderByDescending(c => c.Ram)
                    .FirstOrDefault();

                if (preferred is not null)
                    return preferred.Name;

                // Otherwise pick the physical GPU with the largest dedicated RAM
                var largestPhysical = physical.OrderByDescending(c => c.Ram).FirstOrDefault();
                if (largestPhysical is not null)
                    return largestPhysical.Name;

                // Fallback to any adapter with largest RAM
                var largestAny = candidates.OrderByDescending(c => c.Ram).FirstOrDefault();
                return largestAny?.Name ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Gets the percentage of the GPU currently being utilized.
    /// </summary>
    public static float GPUUsage
    {
        get
        {
            float sum = 0f;
            var toRemove = new List<PerformanceCounter>();
            lock (gpuCounters)
            lock (s_gpuCounters)
            {
                foreach (var counter in s_gpuCounters)
                {
                    // NextValue is a method; wrap call in lambda for AssignerHelper
                    sum += AssignerHelper.TryAssign(
                        () => counter.NextValue(),
                        () =>
                        {
                            toRemove.Add(counter);
                            return 0f;
                        }
                    );
                }

                foreach (var c in toRemove)
                {
                    try
                    {
                        s_gpuCounters.Remove(c);
                        c.Dispose();
                    }
                    catch { }
                }
            }

            return sum;
        }
    }

    private static readonly string[] s_virtualHints = new[]
    {
        "virtual",
        "microsoft basic",
        "remote",
        "vmware",
        "parallels",
        "remote display",
        "meta",
        "oculus",
    };

    private static readonly List<PerformanceCounter> s_gpuCounters;

    static GPUHelper()
    {
        var list = new List<PerformanceCounter>();
        try
        {
            var category = new PerformanceCounterCategory("GPU Engine");
            var instanceNames = category.GetInstanceNames();
            // Determine which phys index is most likely the primary GPU by counting occurrences
            var physCounts = new Dictionary<int, int>();
            foreach (
                var instance in instanceNames.Where(n =>
                    n.EndsWith("engtype_3D", StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                int physIndex = GetPhysIndexFromInstance(instance);

                if (physIndex >= 0)
                {
                    physCounts.TryGetValue(physIndex, out var cnt);
                    physCounts[physIndex] = cnt + 1;
                }
            }

            int? selectedPhys = null;
            if (physCounts.Count == 1)
            {
                selectedPhys = physCounts.Keys.First();
            }
            else if (physCounts.Count > 1)
            {
                // choose phys index with the most 3D engine instances (heuristic for primary GPU)
                selectedPhys = physCounts.OrderByDescending(kv => kv.Value).First().Key;
            }

            foreach (
                var instance in instanceNames.Where(n =>
                    n.EndsWith("engtype_3D", StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                try
                {
                    // if selectedPhys is set, only take instances that belong to that phys index
                    if (selectedPhys.HasValue)
                    {
                        int physIndex = GetPhysIndexFromInstance(instance);

                        if (physIndex != selectedPhys.Value)
                            continue;
                    }

                    var counters = category.GetCounters(instance);
                    foreach (var c in counters)
                    {
                        if (
                            string.Equals(
                                c.CounterName,
                                "Utilization Percentage",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            list.Add(
                                new PerformanceCounter(
                                    "GPU Engine",
                                    "Utilization Percentage",
                                    instance,
                                    readOnly: true
                                )
                            );
                        }
                    }
                }
                catch
                { /* ignore instances that can't be read */
                }
            }
        }
        catch { }

        s_gpuCounters = list;
    }

    private static int GetPhysIndexFromInstance(string instance)
    {
        var parts = instance.Split('_');
        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (string.Equals(parts[i], "phys", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(parts[i + 1], out var idx))
                    return idx;
            }
        }
        return -1;
    }

    private static ulong TryGetAdapterRam(ManagementObject mo)
    {
        try
        {
            var val = mo["AdapterRAM"];
            if (val == null)
                return 0;

            if (val is ulong ul)
                return ul;
            if (val is uint ui)
                return ui;
            if (val is int i)
                return (ulong)i;
            if (ulong.TryParse(val.ToString(), out var parsed))
                return parsed;
        }
        catch { }

        return 0;
    }
}

/// <summary>
/// Wrapper for the MEMORYSTATUSEX structure, which contains memory statistics.
/// </summary>
public sealed class RAMHelper
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

        public MEMORYSTATUSEX()
        {
            dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>();
        }
    }

    private static bool TryCreateMemoryStatus(out MEMORYSTATUSEX status)
    {
        MEMORYSTATUSEX memStatus = new();
        bool created = GlobalMemoryStatusEx(ref memStatus);
        status = created ? memStatus : default;
        return created;
    }

    #endregion

    private const int BytesInGigabyte = 1024 * 1024 * 1024;

    private readonly MEMORYSTATUSEX _status;

    /// <summary>
    /// Constructs a new instance of the <see cref="RAMHelper"/> class.
    /// </summary>
    public RAMHelper()
    {
        TryCreateMemoryStatus(out _status);
    }

    /// <summary>
    /// Gets the total amount of RAM the system has installed.
    /// </summary>
    public float TotalRAM => (float)_status.ullTotalPhys / BytesInGigabyte;

    /// <summary>
    /// Gets the amount of RAM that is currently being utilized in gigabytes.
    /// </summary>
    public float UsedRAM => (_status.ullTotalPhys - _status.ullAvailPhys) / BytesInGigabyte;

    /// <summary>
    /// Gets the percentage of RAM that is currently being utilized.
    /// </summary>
    public float UsedRAMPercent => UsedRAM / TotalRAM * 100;
}
