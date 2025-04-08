namespace Fluentver.Controls
{
    public sealed partial class StorageExpander : Expander
    {
        public DriveInfo DriveInfo { get; private set; }

        public StorageExpander(DriveInfo info)
        {
            this.InitializeComponent();
            SetExpanderDetails(DriveInfo = info);
        }

        private void SetExpanderDetails(DriveInfo info)
        {
            Header = info.GetBestDisplayName();

            ring.Maximum = info.TotalSize;
            ring.Value = info.GetTotalUsedSpace();
            icon.Glyph = driveIconDictionary.TryGetValue(info.DriveType, out string glyph) ? glyph : driveIconDictionary[DriveType.Fixed];

            long freeSpace = info.TotalFreeSpace;
            long totalSpace = info.TotalSize;
            float percent = (float)freeSpace / totalSpace;

            this.freeSpace.Text = info.GetFreeSpaceUnit().FormatValue(freeSpace);
            this.totalSpace.Text = info.GetTotalSpaceUnit().FormatValue(totalSpace);

            if (percent < 0.01)
                ring.Style = criticallyLowSpaceRingStyle;
            else if (percent < 0.05)
                ring.Style = lowSpaceRingStyle;

                string name = info.RootDirectory.FullName;
            mountPoint.Content = name;
            mountPoint.Click += (s, e) => Process.Start(new ProcessStartInfo(name) { UseShellExecute = true });

            type.Text = StringsHelper.GetString(info.DriveType.ToString());
            format.Text = info.DriveFormat;
        }

        private readonly static ReadOnlyDictionary<DriveType, string> driveIconDictionary = new(new Dictionary<DriveType, string>
        {
            {DriveType.Removable, "\uE88E"},
            {DriveType.Network, "\uE968"},
            {DriveType.CDRom, "\uE958"},
            {DriveType.Fixed, "\uEDA2"}
        });
    }
}
