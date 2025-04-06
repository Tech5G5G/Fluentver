using System.DirectoryServices.AccountManagement;
using Windows.System.Profile;
using Microsoft.Win32;

namespace Fluentver.Helpers
{
    public static class VersionHelper
    {
        #region Fields

        public const string CurrentVersion = SystemHelper.HKLM + @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        public const string Applicability = @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability";

        static readonly RegistryKey HKLM = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);

        #endregion

        #region Windows Version

        /// <summary>Gets the display name of the current version of Windows.</summary>
        public static string VersionDisplayName => (string)Registry.GetValue(CurrentVersion, "DisplayVersion", string.Empty);

        /// <summary>Gets the build of the current version of Windows.</summary>
        public static int Build => Environment.OSVersion.Version.Build;

        /// <summary>Gets the revision number of the current version of Windows.</summary>
        public static int Revision => (int)(ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion) & 0xFFFF);

        /// <summary>Gets the edition of Windows installed.</summary>
        public static string Edition => ((string)Registry.GetValue(CurrentVersion, "ProductName", string.Empty))[11..];

        /// <summary>Gets the branch of Windows installed.</summary>
        public static string BuildBranch => (string)Registry.GetValue(CurrentVersion, "BuildBranch", string.Empty);

        /// <summary>Gets the registed owner of the edition of Windows installed.</summary>
        public static string RegisteredOwner => (string)Registry.GetValue(CurrentVersion, "RegisteredOwner", string.Empty);

        /// <summary>Gets the registed organization of the edition of Windows installed.</summary>
        public static string RegisteredOrganization => (string)Registry.GetValue(CurrentVersion, "RegisteredOrganization", string.Empty);

        /// <summary>Gets a <see cref="bool"/> indicating whether Windows 11 is installed.</summary>
        public static bool IsWindows11 => Build >= 22000;

        #endregion

        #region Windows Insider

        /// <summary>Get a <see cref="bool"/> indicating whether the system is enrolled in WIP (Windows Insider Program).</summary>
        public static bool IsWindowsInsider => HKLM.OpenSubKey(Applicability) is RegistryKey key &&
            key.GetValue("IsBuildFlightingEnabled") is int enabled &&
            enabled == 1;

        /// <summary>Gets the current <see cref="InsiderChannel"/> installed on the system.</summary>
        /// <remarks>Returns <see cref="InsiderChannel.Stable"/> if <see cref="IsWindowsInsider"/> is <see langword="false"/>.</remarks>
        public static InsiderChannel Channel => Enum.TryParse((string)Registry.GetValue(SystemHelper.HKLM + Applicability, "BranchName", string.Empty), out InsiderChannel channel) ? channel : InsiderChannel.Stable;

        /// <summary>Gets the <see cref="UserPrincipal"/> that enrolled the system in WIP.</summary>
        /// <returns>The <see cref="UserPrincipal"/> that enrolled the system, asynchronously.</returns>
        public static Task<UserPrincipal> GetWindowsInsiderAccountAsync() =>
            UserHelper.GetUserFromSIDAsync(new((string)Registry.GetValue(SystemHelper.HKLM + Applicability, "FlightingOwnerSID", string.Empty)));

        #endregion
    }

    public enum InsiderChannel
    {
        Stable,
        CanaryChannel,
        Dev,
        Beta,
        ReleasePreview
    }
}
