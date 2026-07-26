using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Fluver.Helpers;
using Fluver.Extensions;

namespace Fluver.UI.Controls
{
    public sealed partial class StorageExpander : Expander
    {
        private static readonly Dictionary<DriveType, string> s_driveIconDictionary = new()
        {
            { DriveType.Removable, "\uE88E" },
            { DriveType.Network, "\uE968" },
            { DriveType.CDRom, "\uE958" },
            { DriveType.Fixed, "\uEDA2" }
        };

        public DriveInfo DriveInfo { get; }

        public StorageExpander(DriveInfo info)
        {
            InitializeComponent();
            SetExpanderDetails(DriveInfo = info);
        }

        private void SetExpanderDetails(DriveInfo info)
        {
            Header = info.GetBestDisplayName();

            Ring.Maximum = info.TotalSize;
            Ring.Value = info.GetTotalUsedSpace();
            Icon.Glyph = s_driveIconDictionary.TryGetValue(info.DriveType, out string glyph) ? glyph : s_driveIconDictionary[DriveType.Fixed];

            long freeSpace = info.TotalFreeSpace;
            long totalSpace = info.TotalSize;
            float percent = (float)freeSpace / totalSpace;

            FreeSpaceText.Text = info.GetFreeSpaceUnit().FormatValue(freeSpace);
            TotalSpaceText.Text = info.GetTotalSpaceUnit().FormatValue(totalSpace);

            if (percent < 0.01)
                Ring.Style = CriticallyLowSpaceRingStyle;
            else if (percent < 0.05)
                Ring.Style = LowSpaceRingStyle;

                string name = info.RootDirectory.FullName;
            MountPointLink.Content = name;
            MountPointLink.Click += (s, e) => Process.Start(new ProcessStartInfo(name) { UseShellExecute = true });

            TypeText.Text = StringsHelper.GetString(info.DriveType.ToString());
            FormatText.Text = info.DriveFormat;
        }
    }
}
