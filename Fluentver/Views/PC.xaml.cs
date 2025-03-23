using System.Timers;
using System.Management;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Fluentver.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PC : InfoPage
    {
        public PC()
        {
            this.InitializeComponent();
            App.pcPage = this;

            SetPCInfo();
            SetPCSpecs();
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        private async void SetPCSpecs()
        {
            var specsLabels = new StackPanel() { Spacing = 4 };
            specsLabels.Children.Add(new TextBlock() { Text = "CPU" });
            specsLabels.Children.Add(new TextBlock() { Text = "GPU" });
            specsLabels.Children.Add(new TextBlock() { Text = "RAM" });

            var specsList = new StackPanel() { Spacing = 4 };

            string cpuName = await Task.Run(() =>
            {
                List<string> names = [];
                ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject mo in mos.Get())
                {
                    names.Add((string)mo["Name"]);
                }
                return names[0];
            });

            string gpuName = await Task.Run(() =>
            {
                List<string> names = [];
                ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_VideoController");
                foreach (ManagementObject mo in mos.Get())
                {
                    names.Add((string)mo["Name"]);
                }
                return names[0];
            });

            GetPhysicallyInstalledSystemMemory(out long memoryKB);
            try
            {
                var cpuText = new TextBlock() { Text = cpuName, Foreground = Application.Current.Resources["TextFillColorSecondaryBrush"] as SolidColorBrush, IsTextSelectionEnabled = true };
                cpuText.ActualThemeChanged += (FrameworkElement sender, object args) => (sender as TextBlock).Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"];
                specsList.Children.Add(cpuText);

                var gpuText = new TextBlock() { Text = gpuName, Foreground = Application.Current.Resources["TextFillColorSecondaryBrush"] as SolidColorBrush, IsTextSelectionEnabled = true };
                gpuText.ActualThemeChanged += (FrameworkElement sender, object args) => (sender as TextBlock).Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"];
                specsList.Children.Add(gpuText);

                var ramText = new TextBlock() { Text = ((int)(memoryKB / 1048576)).ToString() + " GB", Foreground = Application.Current.Resources["TextFillColorSecondaryBrush"] as SolidColorBrush, IsTextSelectionEnabled = true };
                ramText.ActualThemeChanged += (FrameworkElement sender, object args) => (sender as TextBlock).Foreground = (SolidColorBrush)App.Current.Resources["TextFillColorSecondaryBrush"];
                specsList.Children.Add(ramText);

                cpuListRing.Visibility = Visibility.Collapsed;
                cpuList.Children.Add(specsLabels);
                cpuList.Children.Add(specsList);

                var mw = App.MainWindow;
            }
            catch (Exception) { }
        }

        private void SetPCInfo()
        {
            SetAwakeTime();

            var deviceInformation = new EasClientDeviceInformation();
            productName.Text = deviceInformation.SystemProductName.ToString();

            try
            {
                string displayName = Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", "Hostname", "").ToString();

                if (!string.IsNullOrWhiteSpace(displayName))
                    pcName.Text = displayName;
                else
                    pcName.Text = Environment.MachineName;
            }
            catch (Exception)
            {
                pcName.Text = Environment.MachineName;
            }

            osType.Text = Environment.Is64BitOperatingSystem ? "64-bit operating system" : "32-bit operating system";

            pcBackground.ImageSource = new BitmapImage() { UriSource = new Uri("C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Microsoft\\Windows\\Themes\\TranscodedWallpaper") };
        }
        
        private object timerLock = new object();
        private Timer timer = new Timer();

        public void StopTimer()
        {
            lock (timerLock) timer.Stop();
        }

        private bool canTimeAwakeBeUpdated = true;
        private void TimeAwake_SelectionChanged(object sender, RoutedEventArgs e) => canTimeAwakeBeUpdated = string.IsNullOrEmpty(timeAwake.SelectedText);
        private void SetAwakeTime()
        {
            var timespan = TimeSpan.FromMilliseconds(Environment.TickCount64);

            string seconds = timespan.Seconds <= 9 ? "0" + timespan.Seconds : timespan.Seconds.ToString();
            string minutes = timespan.Minutes <= 9 ? "0" + timespan.Minutes : timespan.Minutes.ToString();
            string hours = timespan.Hours <= 9 ? "0" + timespan.Hours : timespan.Hours.ToString();
            string days = timespan.Days <= 9 ? "0" + timespan.Days : timespan.Days.ToString();

            timeAwake.Text = days + ":" + hours + ":" + minutes + ":" + seconds;

            timer.Interval = 1000;
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                lock (timerLock)
                {
                    if(!timer.Enabled)
                        return;

                    timer.Stop();

                    if (canTimeAwakeBeUpdated)
                    {
                        var timespan = TimeSpan.FromMilliseconds(Environment.TickCount64);

                        string seconds = timespan.Seconds <= 9 ? "0" + timespan.Seconds : timespan.Seconds.ToString();
                        string minutes = timespan.Minutes <= 9 ? "0" + timespan.Minutes : timespan.Minutes.ToString();
                        string hours = timespan.Hours <= 9 ? "0" + timespan.Hours : timespan.Hours.ToString();
                        string days = timespan.Days <= 9 ? "0" + timespan.Days : timespan.Days.ToString();

                        this.DispatcherQueue.TryEnqueue(() => timeAwake.Text = days + ":" + hours + ":" + minutes + ":" + seconds);
                    }
                    timer.Interval = 1000;
                    timer.Start();
                }
            };
            timer.Start();
        }
    }
}
