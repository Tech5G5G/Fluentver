using System.Runtime.InteropServices;

namespace Fluentver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PC : InfoPage
    {
        readonly string GB = StringsHelper.GetString("Gigabytes");

        readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };

        public PC()
        {
            this.InitializeComponent();

            Loaded += (s, e) => timer.Start();
            Unloaded += (s, e) => timer.Stop();

            SetPCInfo();
            SetPCUsage(true);
            SetAwakeTime(true);
            ApplyDisplayResolution(true);
        }

        private void SetPCInfo()
        {
            pcName.Text = SystemHelper.SystemName;
            pcBackground.ImageSource = new BitmapImage { UriSource = SystemHelper.CurrentUserWallpaper };

            string name = SystemHelper.SystemProductName;
            productName.Text = name == "System Product Name" ? StringsHelper.GetString("Unknown") : name;

            var architecture = RuntimeInformation.OSArchitecture;
            osType.Text = AssignerHelper.TryAssign(() => StringsHelper.GetString(architecture.ToString()), architecture.ToString);
        }

        private async void SetPCUsage(bool hookTimer = false)
        {
            RAMHelper ramHelper = new();

            if (hookTimer)
            {
                cpu.Text = await Task.Run(() => CPUHelper.CPUName);
                gpu.Text = await Task.Run(() => GPUHelper.GPUName);
                ram.Text = $"{Math.Ceiling(ramHelper.TotalRAM)} {GB}";

                cpuUsageLabel.LosingFocus += TextDisplay_LosingFocus;
                gpuUsageLabel.LosingFocus += TextDisplay_LosingFocus;
                ramUsageLabel.LosingFocus += TextDisplay_LosingFocus;

                loadingIndicator.Visibility = Visibility.Collapsed;
                specsGrid.Visibility = Visibility.Visible;

                timer.Tick += (s, e) => SetPCUsage();
            }

            cpuUsageLabel.SetTextFriendly($"{cpuUsage.Value = await Task.Run(() => CPUHelper.CPUUsage):N0}%");
            gpuUsageLabel.SetTextFriendly($"{gpuUsage.Value = await Task.Run(() => GPUHelper.GPUUsage):N0}%");

            ramUsage.Value = ramHelper.UsedRAMPercent;
            ramUsageLabel.SetTextFriendly($"{ramHelper.UsedRAM:N0} {GB}");
        }

        private void SetAwakeTime(bool hookTimer = false)
        {
            if (hookTimer)
            {
                timer.Tick += (s, e) => SetAwakeTime();
                timeAwake.LosingFocus += TextDisplay_LosingFocus;
            }

            timeAwake.SetTextFriendly(TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\:hh\:mm\:ss"));
        }

        private void ApplyDisplayResolution(bool hookEvents = false)
        {
            if (hookEvents)
            {
                App.MainWindow.ResolutionChanged += (s, e) => ApplyDisplayResolution();
                App.MainWindow.PositionChanged += (s, e) => ApplyDisplayResolution();
            }

            var size = DisplayArea.GetFromWindowId(App.MainWindow.AppWindow.Id, DisplayAreaFallback.Primary).OuterBounds;
            backgroundRect.Width = size.Width;
            backgroundRect.Height = size.Height;
        }

        private void TextDisplay_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            if (sender is TextBlock text && args.NewFocusedElement is not Popup) //Reset text selection if focus isn't lost to a popup
                text.Select(text.ContentStart, text.ContentStart);
        }

        private void RenamePCButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.RenamerWindow is null)
                (App.RenamerWindow = new()).Activate();
            else
                WindowHelper.ActivateWindow(App.RenamerWindow.GetWindowHandle());
        }
    }
}
