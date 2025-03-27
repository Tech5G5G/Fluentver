namespace Fluentver.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PC : InfoPage
    {
        readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };

        public PC()
        {
            this.InitializeComponent();

            Loaded += (s, e) => timer.Start();
            Unloaded += (s, e) => timer.Stop();

            SetPCInfo();
            SetPCUsage(true);
            SetAwakeTime(true);
        }

        private void SetPCInfo()
        {
            var deviceInformation = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            productName.Text = deviceInformation.SystemProductName.ToString();

            var architecture = RuntimeInformation.OSArchitecture;
            osType.Text = architecture switch
            {
                Architecture.X86 => "32-bit",
                Architecture.X64 => "64-bit",
                Architecture.Arm => "ARM",
                Architecture.Arm64 => "ARM64",
                _ => architecture.ToString()
            };
        }

        private async void SetPCUsage(bool hookTimer = false)
        {
            RAMHelper ramHelper = new();

            if (hookTimer)
            {
                cpu.Text = await Task.Run(() => CPUHelper.CPUName);
                gpu.Text = await Task.Run(() => GPUHelper.GPUName);
                ram.Text = $"{Math.Ceiling(ramHelper.TotalRAM)} GB";

                loadingIndicator.Visibility = Visibility.Collapsed;
                specsGrid.Visibility = Visibility.Visible;

                timer.Tick += (s, e) => SetPCUsage();
            }

            cpuUsageLabel.Text = $"{cpuUsage.Value = await Task.Run(() => CPUHelper.CPUUsage):N0}%";
            gpuUsageLabel.Text = $"{gpuUsage.Value = await Task.Run(() => GPUHelper.GPUUsage):N0}%";

            ramUsage.Value = ramHelper.UsedRAMPercent;
            ramUsageLabel.Text = $"{ramHelper.UsedRAM:N0} GB";
        }

        private void SetAwakeTime(bool hookTimer = false)
        {
            if (hookTimer)
            {
                timer.Tick += (s, e) => SetAwakeTime();
                timeAwake.LosingFocus += (s, e) =>
                {
                    if (e.NewFocusedElement is not Popup) //Reset text selection if focus isn't lost to a popup
                        timeAwake.Select(timeAwake.ContentStart, timeAwake.ContentStart);
                };
            }

            if (string.IsNullOrEmpty(timeAwake.SelectedText))
                timeAwake.Text = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\:hh\:mm\:ss");
        }
    }
}
