using Windows.Win32;
using Windows.Wdk.System.SystemServices;
using Microsoft.Win32;
using Fluver.Extensions;

namespace Fluver.Platform
{
    public static class WindowsEnvironment
    {
        public static string Edition { get; } = GetWindowsEdition();

        public static string DisplayVersion { get; }

        public static Version Version { get; } = Environment.OSVersion.Version;
        public static bool IsWindows11 { get; } = Version.Build >= 22000;

        public static (bool IsEvaluationCopy, DateTime ExpirationDate) EvaluationInfo { get; } = GetWindowEvaluationInfo();

        public static string Owner { get; }
        public static string Organization { get; }

        public static string BuildBranch { get; }

        public static bool IsInsider { get; }
        public static InsiderChannel InsiderChannel { get; }

        static WindowsEnvironment()
        {
            const string CurrentVersion = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                         Applicability = @"SOFTWARE\Microsoft\WindowsSelfHost\Applicability";

            if (Registry.LocalMachine.TryOpenSubKey(CurrentVersion, out var currentVersion))
            {
                using var key = currentVersion;

                currentVersion.TryGetValue("DisplayVersion", out string displayVersion);
                DisplayVersion = displayVersion;

                currentVersion.TryGetValue("UBR", out int ubr);
                Version = new(Version.Major, Version.Minor, Version.Build, revision: ubr);

                currentVersion.TryGetValue("RegisteredOwner", out string registeredOwner);
                Owner = registeredOwner;

                currentVersion.TryGetValue("RegisteredOrganization", out string registeredOrganization);
                Organization = registeredOrganization;

                currentVersion.TryGetValue("BuildBranch", out string buildBranch);
                BuildBranch = buildBranch;
            }

            if (Registry.LocalMachine.TryOpenSubKey(Applicability, out var applicability))
            {
                using var key = applicability;

                IsInsider = applicability.TryGetValue("IsBuildFlightingEnabled", out int isBuildFlightingEnabled) &&
                            isBuildFlightingEnabled != 0;

                applicability.TryGetValue("BranchName", out string branchName);
                InsiderChannel = Enum.TryParse(branchName, out InsiderChannel channel) ? channel : InsiderChannel.Stable;
            }
        }

        private static string GetWindowsEdition()
        {
            Span<char> edition = stackalloc char[PInvoke.WINDOWS_EDITION_PATTERN_LENGTH];
            if (PInvoke.BrandingLoadString("Basebrd", uID: 12, edition, PInvoke.WINDOWS_EDITION_PATTERN_LENGTH))
            {
                string editionStr = edition.Trim('\0').ToString();
                if (!string.IsNullOrWhiteSpace(editionStr))
                {
                    return editionStr;
                }
            }

            return IsWindows11 ? "Windows 11" : "Windows 10";
        }

        private static (bool IsEvaluationCopy, DateTime ExpirationDate) GetWindowEvaluationInfo()
        {
            var expirationDate = KUSER_SHARED_DATA.GetReference().SystemExpirationDate;
            var evaluationCopy = expirationDate != 0;

            return (evaluationCopy, evaluationCopy ? DateTime.FromFileTimeUtc(expirationDate) : default);
        }
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
