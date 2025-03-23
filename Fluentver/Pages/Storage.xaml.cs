namespace Fluentver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Storage : InfoPage
    {
        public Storage()
        {
            this.InitializeComponent();
            
            GetAllDisks();
            App.StoragePage = this;
        }

        public void Reload(object sender, RoutedEventArgs e) => Content = new Storage();

        private async void GetAllDisks()
        {
            var mw = App.MainWindow;

            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                var expander = new Expander
                {
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsExpanded = true,
                    Padding = new Thickness(0),
                    Header = string.IsNullOrWhiteSpace(drive.VolumeLabel) ? drive.Name : drive.VolumeLabel
                };

                var content = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(12), Spacing = 12 };

                string totalSpaceDataUnit = drive.TotalSize switch { < 1024 => " B", < 1048576 => " KB", < 1073741824 => " MB", < 1099511627776 => " GB", < 1125899906842624 => " TB", < 1152921504606846976 => " PB", _ => " EB" };
                string freeSpaceDataUnit = drive.TotalFreeSpace switch { < 1024 => " B", < 1048576 => " KB", < 1073741824 => " MB", < 1099511627776 => " GB", < 1125899906842624 => " TB", < 1152921504606846976 => " PB", _ => " EB" };

                int totalSpaceNumber = drive.TotalSize switch { < 1024 => (int)drive.TotalSize, < 1048576 => (int)(drive.TotalSize / 1024), < 1073741824 => (int)(drive.TotalSize / 1048576), < 1099511627776 => (int)(drive.TotalSize / 1073741824), < 1125899906842624 => (int)(drive.TotalSize / 1099511627776), < 1152921504606846976 => (int)(drive.TotalSize / 1125899906842624), _ => (int)(drive.TotalSize / 1152921504606846976) };
                int freeSpaceNumber = drive.TotalFreeSpace switch { < 1024 => (int)drive.TotalFreeSpace, < 1048576 => (int)(drive.TotalFreeSpace / 1024), < 1073741824 => (int)(drive.TotalFreeSpace / 1048576), < 1099511627776 => (int)(drive.TotalFreeSpace / 1073741824), < 1125899906842624 => (int)(drive.TotalFreeSpace / 1099511627776), < 1152921504606846976 => (int)(drive.TotalFreeSpace / 1125899906842624), _ => (int)(drive.TotalFreeSpace / 1152921504606846976) };

                var diskSpace = new Grid();
                diskSpace.Children.Add(new ProgressRing() { IsIndeterminate = false, Maximum = drive.TotalSize, Value = drive.TotalSize - drive.TotalFreeSpace, Background = new SolidColorBrush(Colors.DarkGray), Height = 75, Width = 75 });
                diskSpace.Children.Add(new FontIcon() { Glyph = drive.DriveType switch { DriveType.Removable => "\uE88E", DriveType.Network => "\uE968", DriveType.CDRom => "\uE958", _ => "\uEDA2" }, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                content.Children.Add(diskSpace);

                var labels = new StackPanel() { Orientation = Orientation.Vertical, Spacing = 4 };
                labels.Children.Add(new TextBlock() { Text = "Free space" });
                labels.Children.Add(new TextBlock() { Text = "Total space" });
                labels.Children.Add(new TextBlock() { Text = "Mounted at" });
                labels.Children.Add(new TextBlock() { Text = "Type" });
                labels.Children.Add(new TextBlock() { Text = "Format" });
                content.Children.Add(labels);

                var info = new StackPanel() { Orientation = Orientation.Vertical, Spacing = 4 };

                var freeSpace = new TextBlock() { Text = freeSpaceNumber + freeSpaceDataUnit, Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"], IsTextSelectionEnabled = true };
                freeSpace.ActualThemeChanged += (FrameworkElement sender, object args) => (sender as TextBlock).Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"];
                info.Children.Add(freeSpace);

                var totalSpace = new TextBlock() { Text = totalSpaceNumber + totalSpaceDataUnit, Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"], IsTextSelectionEnabled = true };
                totalSpace.ActualThemeChanged += (FrameworkElement sender, object args) => (sender as TextBlock).Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"];
                info.Children.Add(totalSpace);

                var rootDirectory = new HyperlinkButton() { Content = drive.RootDirectory.ToString(), Padding = new Thickness(0) };
                rootDirectory.Click += (object sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo(drive.RootDirectory.ToString()) { UseShellExecute = true });
                info.Children.Add(rootDirectory);

                var driveType = new TextBlock() { Text = drive.DriveType.ToString(), Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"], IsTextSelectionEnabled = true };
                driveType.ActualThemeChanged += (FrameworkElement sender, object args) => (sender as TextBlock).Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"];
                info.Children.Add(driveType);

                var driveFormat = new TextBlock() { Text = drive.DriveFormat.ToString(), Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"], IsTextSelectionEnabled = true };
                driveFormat.ActualThemeChanged += (FrameworkElement sender, object args) => (sender as TextBlock).Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"];
                info.Children.Add(driveFormat);

                content.Children.Add(info);

                expander.Content = content;

                disksList.Children.Add(expander);

                await Task.Delay(10);
            }
        }
    }
}
