namespace Fluver.Extensions;

public static class DriveInfoExtensions
{
    /// <summary>
    /// Gets the total amount of used space available on a drive, in bytes.
    /// </summary>
    /// <param name="drive">The <see cref="DriveInfo"/> to get the total used space of.</param>
    /// <returns>The total used space available on a drive, in bytes.</returns>
    public static long GetTotalUsedSpace(this DriveInfo drive)
    {
        return drive.TotalSize - drive.TotalFreeSpace;
    }

    /// <summary>
    /// Gets the best name to be display in UI.
    /// </summary>
    /// <param name="drive">The <see cref="DriveInfo"/> to get the best name of.</param>
    /// <returns>If it passes <see cref="string.IsNullOrWhiteSpace(string?)"/>, <see cref="DriveInfo.VolumeLabel"/>. Otherwise, <see cref="DriveInfo.Name"/>.</returns>
    public static string GetBestDisplayName(this DriveInfo drive)
    {
        return string.IsNullOrWhiteSpace(drive.VolumeLabel) ? drive.Name.Replace(":\\", ":") : drive.VolumeLabel;
    }

    /// <summary>
    /// Gets the <see cref="StorageUnit"/> of <see cref="DriveInfo.TotalFreeSpace"/>.
    /// </summary>
    /// <param name="drive">The <see cref="DriveInfo"/> to get the <see cref="StorageUnit"/> of.</param>
    /// <returns>The <see cref="StorageUnit"/> of <see cref="DriveInfo.TotalFreeSpace"/>.</returns>
    public static StorageUnit GetFreeSpaceUnit(this DriveInfo drive)
    {
        return GetUnit(drive.TotalFreeSpace);
    }

    /// <summary>
    /// Gets the <see cref="StorageUnit"/> of <see cref="DriveInfo.TotalSize"/>.
    /// </summary>
    /// <param name="drive">The <see cref="DriveInfo"/> to get the <see cref="StorageUnit"/> of.</param>
    /// <returns>The <see cref="StorageUnit"/> of <see cref="DriveInfo.TotalSize"/>.</returns>
    public static StorageUnit GetTotalSpaceUnit(this DriveInfo drive)
    {
        return GetUnit(drive.TotalSize);
    }

    /// <summary>
    /// Formats <paramref name="value"/>, a value in bytes, to a value of <paramref name="unit"/>.
    /// </summary>
    /// <param name="unit">The unit to format to.</param>
    /// <param name="value">The original value in bytes.</param>
    /// <returns>A <see cref="string"/> representation of <paramref name="value"/> formatted as <paramref name="unit"/>.</returns>
    public static string FormatValue(this StorageUnit unit, long value)
    {
        var info = UnitDictionary[unit];
        return $"{Math.Round((double)value / info.AmountInBytes)} {info.Extension}";
    }

    /// <summary>
    /// Contains <see cref="UnitInfo"/>s for <see cref="StorageUnit"/>s.
    /// </summary>
    public static IReadOnlyDictionary<StorageUnit, UnitInfo> UnitDictionary { get; } = new Dictionary<StorageUnit, UnitInfo>
    {
        { StorageUnit.Bytes, new(Text.Bytes, 1) },
        { StorageUnit.Kilobytes, new(Text.Kilobytes, 1024) },
        { StorageUnit.Megabytes, new(Text.Megabytes, 1024 * 1024) },
        { StorageUnit.Gigabytes, new(Text.Gigabytes, 1024 * 1024 * 1024) },
        { StorageUnit.Terabytes, new(Text.Terabytes, (long)1024 * 1024 * 1024 * 1024) },
        { StorageUnit.Petabytes, new(Text.Petabytes, (long)1024 * 1024 * 1024 * 1024 * 1024) },
        { StorageUnit.Exabytes, new(Text.Exabytes, (long)1024 * 1024 * 1024 * 1024 * 1024 * 1024) },
    };

    private static StorageUnit GetUnit(long value)
    {
        int index = (int)UnitDictionary.FirstOrDefault(i => i.Value.AmountInBytes > value, new(StorageUnit.Exabytes, null)).Key;
        return (StorageUnit)Math.Clamp(index - 1, 0, 6);
    }
}

/// <summary>
/// Contains additional information about a <see cref="StorageUnit"/>.
/// </summary>
/// <param name="Extension">A <see cref="string"/> representation of the units extension.</param>
/// <param name="AmountInBytes">The amount in bytes the unit takes up, expressed as a <see cref="long"/>.</param>
public sealed record UnitInfo(string Extension, long AmountInBytes);

/// <summary>
/// Various storage units up to exabytes. Use <see cref="DriveInfoExtensions.UnitDictionary"/> to get <see cref="UnitInfo"/>.
/// </summary>
public enum StorageUnit
{
    Bytes,
    Kilobytes,
    Megabytes,
    Gigabytes,
    Terabytes,
    Petabytes,
    Exabytes
}
