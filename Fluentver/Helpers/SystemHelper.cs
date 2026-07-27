using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Microsoft.Win32;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Fluver.Helpers
{
    public static class SystemHelper
    {
        public const string HKLM = @"HKEY_LOCAL_MACHINE\";
        public const string HKCU = @"HKEY_CURRENT_USER\";

        private static readonly Regex s_regex = new(@"[/:*?<>| " + "\"]");
        private static readonly EasClientDeviceInformation s_easInfo = new();

        /// <summary>
        /// Gets the name of the system.
        /// </summary>
        public static string SystemName
        {
            get
            {
                string name = (string)Registry.GetValue(HKLM + "SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", "Hostname", string.Empty);
                return string.IsNullOrWhiteSpace(name) ? Environment.MachineName : name;
            }
        }

        /// <summary>
        /// Renames the system using PowerShell, asynchronously.
        /// </summary>
        /// <param name="name">The new name for the system. This should be checked using <see cref="CheckNetBiosName(string, out NetBiosNameCheckResult)"/> beforehand.</param>
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

        /// <summary>
        /// Determines whether <paramref name="name"/> is a properly formatted NetBIOS name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <param name="result">A <see cref="NetBiosNameCheckResult"/>, containing more information about the result.</param>
        /// <returns>A <see cref="bool"/> indicating whether <paramref name="name"/> is properly formatted.</returns>
        public static bool CheckNetBiosName(string name, out NetBiosNameCheckResult result)
        {
            result = name.Length switch
            {
                < 1 => NetBiosNameCheckResult.BelowMinLength,
                > 15 => NetBiosNameCheckResult.ExceedsMaxLength,
                _ => s_regex.IsMatch(name) || name.Contains('\\') ? NetBiosNameCheckResult.InvalidCharacter : NetBiosNameCheckResult.Valid
            };
            return result == NetBiosNameCheckResult.Valid;
        }

        /// <summary>
        /// Gets the product name of the system.
        /// </summary>
        public static string SystemProductName => s_easInfo.SystemProductName;

        /// <summary>
        /// Gets a <see cref="Brush"/> representing the curent user's wallpaper.
        /// </summary>
        public static Brush UserWallpaperBrush
        {
            get
            {
                var type = (BackgroundType)Registry.GetValue(HKCU + "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Wallpapers", "BackgroundType", BackgroundType.Picture);
                switch (type)
                {
                    case BackgroundType.SolidColor:
                        byte[] rgb = [.. ((string)Registry.GetValue(HKCU + "Control Panel\\Colors", "Background", "0 0 0")).Split(' ').Select(i => byte.TryParse(i, out byte result) ? result : (byte)0)];
                        return new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, rgb[0], rgb[1], rgb[2]));

                    case BackgroundType.Picture when Registry.GetValue(HKCU + "Control Panel\\Desktop", "WallPaper", string.Empty) is string wallpaper && !string.IsNullOrWhiteSpace(wallpaper):
                        return new ImageBrush { ImageSource = new BitmapImage { UriSource = new(wallpaper) }, Stretch = Stretch.UniformToFill };
                }

                return new ImageBrush { ImageSource = new BitmapImage { UriSource = new($@"C:\Users\{Environment.UserName}\AppData\Roaming\Microsoft\Windows\Themes\TranscodedWallpaper") }, Stretch = Stretch.UniformToFill };
            }
        }

        private enum BackgroundType
        {
            Picture,
            SolidColor,
            Slideshow,
            Spotlight
        }
    }

    public enum NetBiosNameCheckResult
    {
        Valid,
        BelowMinLength,
        ExceedsMaxLength,
        InvalidCharacter
    }
}