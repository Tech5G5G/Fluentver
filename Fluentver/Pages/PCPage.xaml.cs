using System.Runtime.InteropServices;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Fluver.Helpers;
using Fluver.Extensions;
using Fluver.UI.Controls;
using WinUIEx;

namespace Fluver.Pages
{
    public sealed partial class PCPage : InfoPage
    {
        private static readonly string s_gbStr = StringsHelper.GetString("Gigabytes");

        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

        public PCPage()
        {
            InitializeComponent();

            Loaded += (s, e) => _timer.Start();
            Unloaded += (s, e) => _timer.Stop();

            SetPCInfo();
            SetPCUsage(true);
            SetAwakeTime(true);
            ApplyDisplayResolution(true);
        }

        private void SetPCInfo()
        {
            pcName.Text = SystemHelper.SystemName;
            backgroundRect.Fill = SystemHelper.UserWallpaperBrush;

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
                ram.Text = $"{Math.Ceiling(ramHelper.TotalRAM)} {s_gbStr}";

                cpuUsageLabel.LosingFocus += TextDisplay_LosingFocus;
                gpuUsageLabel.LosingFocus += TextDisplay_LosingFocus;
                ramUsageLabel.LosingFocus += TextDisplay_LosingFocus;

                loadingIndicator.Visibility = Visibility.Collapsed;
                specsGrid.Visibility = Visibility.Visible;

                _timer.Tick += (s, e) => SetPCUsage();
            }

            cpuUsageLabel.SetTextFriendly($"{cpuUsage.Value = await Task.Run(() => CPUHelper.CPUUsage):N0}%");
            gpuUsageLabel.SetTextFriendly($"{gpuUsage.Value = await Task.Run(() => GPUHelper.GPUUsage):N0}%");

            ramUsage.Value = ramHelper.UsedRAMPercent;
            ramUsageLabel.SetTextFriendly($"{ramHelper.UsedRAM:N0} {s_gbStr}");
        }

        private void SetAwakeTime(bool hookTimer = false)
        {
            if (hookTimer)
            {
                _timer.Tick += (s, e) => SetAwakeTime();
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

        private void TextDisplay_LosingFocus(UIElement sender, LosingFocusEventArgs e)
        {
            if (sender is TextBlock text && e.NewFocusedElement is not Popup) // Reset text selection if focus isn't lost to a popup
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
