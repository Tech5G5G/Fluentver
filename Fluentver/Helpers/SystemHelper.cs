using System.Text.RegularExpressions;
using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Microsoft.Win32;

namespace Fluentver.Helpers
{
    public static class SystemHelper
    {
        public const string HKLM = @"HKEY_LOCAL_MACHINE\";
        public const string CurrentVersion = HKLM + @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        private static readonly Regex regex = new(@"[/:*?<>| " + "\"]");
        private static readonly EasClientDeviceInformation easInfo = new();

        /// <summary>The name of the system.</summary>
        public static string SystemName
        {
            get
            {
                string name = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", "Hostname", string.Empty);
                return string.IsNullOrWhiteSpace(name) ? Environment.MachineName : name;
            }
        }

        /// <summary>Sets the name of the system using PowerShell, asynchronously.</summary>
        /// <param name="name">The new name for the system. This should be checked using <see cref="CheckNetBIOSName(string, out NetBIOSNameCheckResult)"/> beforehand.</param>
        /// <returns>A <see cref="bool"/> indicating whether the system was renamed.</returns>
        public static async Task<bool> RenameSystem(string name)
        {
            Process proc = new()
            {
                StartInfo = new()
                {
                    FileName = "powershell.exe",
                    Arguments = $"Rename-Computer \"{name}\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    ErrorDialog = false
                }
            };

            try
            {
                proc.Start();
            }
            catch
            {
                return false;
            }

            await proc.WaitForExitAsync();
            return true;
        }

        /// <summary>Determines whether <paramref name="name"/> is a properly formatted NetBIOS name.</summary>
        /// <param name="name">The name to check.</param>
        /// <param name="result">A <see cref="NetBIOSNameCheckResult"/>, containing more information about the result.</param>
        /// <returns>A <see cref="bool"/> indicating whether <paramref name="name"/> is properly formatted.</returns>
        public static bool CheckNetBIOSName(string name, out NetBIOSNameCheckResult result)
        {
            result = name.Length switch
            {
                < 1 => NetBIOSNameCheckResult.BelowMinLength,
                > 15 => NetBIOSNameCheckResult.ExceedsMaxLength,
                _ => regex.IsMatch(name) || name.Contains('\\') ? NetBIOSNameCheckResult.InvalidCharacter : NetBIOSNameCheckResult.Valid
            };
            return result == NetBIOSNameCheckResult.Valid;
        }

        /// <summary>Gets the product name of the system.</summary>
        public static string SystemProductName => easInfo.SystemProductName;

        /// <summary>Gets the display name of the current version of Windows.</summary>
        public static string WindowsVersionDisplayName => (string)Registry.GetValue(CurrentVersion, "DisplayVersion", string.Empty);

        /// <summary>Gets the build of the current version of Windows.</summary>
        public static int WindowsBuild => Environment.OSVersion.Version.Build;

        /// <summary>Gets the revision number of the current version of Windows.</summary>
        public static int WindowsRevision => (int)(ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion) & 0xFFFF);

        /// <summary>Gets the edition of Windows installed.</summary>
        public static string WindowsEdition => ((string)Registry.GetValue(CurrentVersion, "ProductName", string.Empty))[11..];

        /// <summary>Gets the registed owner of the edition of Windows installed.</summary>
        public static string RegisteredOwner => (string)Registry.GetValue(CurrentVersion, "RegisteredOwner", string.Empty);

        /// <summary>Gets the registed organization of the edition of Windows installed.</summary>
        public static string RegisteredOrganization => (string)Registry.GetValue(CurrentVersion, "RegisteredOrganization", string.Empty);

        /// <summary>Gets a <see cref="bool"/> indicating whether Windows 11 is installed.</summary>
        public static bool IsWindows11 => WindowsBuild >= 22000;

        /// <summary>Gets a <see cref="Uri"/> to the current user's wallpaper.</summary>
        public static Uri CurrentUserWallpaper => new($@"C:\Users\{Environment.UserName}\AppData\Roaming\Microsoft\Windows\Themes\TranscodedWallpaper");
    }

    public enum NetBIOSNameCheckResult
    {
        Valid,
        BelowMinLength,
        ExceedsMaxLength,
        InvalidCharacter
    }
}