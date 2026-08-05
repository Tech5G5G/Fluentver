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

namespace Fluver.Views
{
    public sealed partial class PCPage : InfoPage
    {
        private static readonly string s_gbStr = Text.Gigabytes;

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
            PcNameText.Text = SystemHelper.SystemName;
            BackgroundRectangle.Fill = SystemHelper.UserWallpaperBrush;

            string name = SystemHelper.SystemProductName;
            ProductNameText.Text = name == "System Product Name" ? Text.Unknown : name;

            var architecture = RuntimeInformation.OSArchitecture;
            ArchitectureText.Text = AssignerHelper.TryAssign(() => Text.GetString(architecture.ToString()), architecture.ToString);
        }

        private async void SetPCUsage(bool hookTimer = false)
        {
            RAMHelper ramHelper = new();

            if (hookTimer)
            {
                CpuText.Text = await Task.Run(() => CPUHelper.CPUName);
                GpuText.Text = await Task.Run(() => GPUHelper.GPUName);
                RamText.Text = $"{Math.Ceiling(ramHelper.TotalRAM)} {s_gbStr}";

                CpuUsageText.LosingFocus += TextDisplay_LosingFocus;
                GpuUsageText.LosingFocus += TextDisplay_LosingFocus;
                RamUsageText.LosingFocus += TextDisplay_LosingFocus;

                LoadingRing.Visibility = Visibility.Collapsed;
                SpecsGrid.Visibility = Visibility.Visible;

                _timer.Tick += (s, e) => SetPCUsage();
            }

            CpuUsageText.SetTextFriendly($"{CpuUsageBar.Value = await Task.Run(() => CPUHelper.CPUUsage):N0}%");
            GpuUsageText.SetTextFriendly($"{GpuUsageBar.Value = await Task.Run(() => GPUHelper.GPUUsage):N0}%");

            RamUsageBar.Value = ramHelper.UsedRAMPercent;
            RamUsageText.SetTextFriendly($"{ramHelper.UsedRAM:N0} {s_gbStr}");
        }

        private void SetAwakeTime(bool hookTimer = false)
        {
            if (hookTimer)
            {
                _timer.Tick += (s, e) => SetAwakeTime();
                TimeAwakeText.LosingFocus += TextDisplay_LosingFocus;
            }

            TimeAwakeText.SetTextFriendly(TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"dd\:hh\:mm\:ss"));
        }

        private void ApplyDisplayResolution(bool hookEvents = false)
        {
            if (hookEvents)
            {
                App.MainWindow.ResolutionChanged += (s, e) => ApplyDisplayResolution();
                App.MainWindow.PositionChanged += (s, e) => ApplyDisplayResolution();
            }

            var size = DisplayArea.GetFromWindowId(App.MainWindow.AppWindow.Id, DisplayAreaFallback.Primary).OuterBounds;
            BackgroundRectangle.Width = size.Width;
            BackgroundRectangle.Height = size.Height;
        }

        private void TextDisplay_LosingFocus(UIElement sender, LosingFocusEventArgs e)
        {
            if (sender is TextBlock text && e.NewFocusedElement is not Popup) // Reset text selection if focus isn't lost to a popup
            {
                text.Select(text.ContentStart, text.ContentStart);
            }
        }

        private void RenamePCButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.RenamerWindow is null)
            {
                (App.RenamerWindow = new()).Activate();
            }
            else
            {
                WindowHelper.ActivateWindow(App.RenamerWindow.GetWindowHandle());
            }
        }
    }
}
