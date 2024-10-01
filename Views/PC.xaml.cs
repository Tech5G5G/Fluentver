using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluentver.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PC : Page
    {
        public PC()
        {
            this.InitializeComponent();

            SetAwakeTime();
            SetPCInfo();
        }

        private void SetPCInfo()
        {
            pcName.Text = Environment.MachineName;
            is64Bit.Text = Environment.Is64BitOperatingSystem ? "Yes" : "No";
        }

        private void SetAwakeTime()
        {
            var timespan = TimeSpan.FromMilliseconds(Environment.TickCount64);

            string seconds = timespan.Seconds <= 9 ? "0" + timespan.Seconds : timespan.Seconds.ToString();
            string minutes = timespan.Minutes <= 9 ? "0" + timespan.Minutes : timespan.Minutes.ToString();
            string hours = timespan.Hours <= 9 ? "0" + timespan.Hours : timespan.Hours.ToString();
            string days = timespan.Days <= 9 ? "0" + timespan.Days : timespan.Days.ToString();

            timeAwake.Text = days + ":" + hours + ":" + minutes + ":" + seconds;

            var timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                timer.Stop();

                var timespan = TimeSpan.FromMilliseconds(Environment.TickCount64);

                string seconds = timespan.Seconds <= 9 ? "0" + timespan.Seconds : timespan.Seconds.ToString();
                string minutes = timespan.Minutes <= 9 ? "0" + timespan.Minutes : timespan.Minutes.ToString();
                string hours = timespan.Hours <= 9 ? "0" + timespan.Hours : timespan.Hours.ToString();
                string days = timespan.Days <= 9 ? "0" + timespan.Days : timespan.Days.ToString();

                if (timeAwake is not null)
                    this.DispatcherQueue.TryEnqueue(() => timeAwake.Text = days + ":" + hours + ":" + minutes + ":" + seconds);
                timer.Interval = 1000;
                timer.Start();
            };
            timer.Start();
        }

        private void PCInfo_WindowHeight_Increase(Expander sender, ExpanderExpandingEventArgs args)
        {
            MainWindow mw = (MainWindow)((App)(Application.Current)).m_window;
            if (pcInfo.XamlRoot is not null)
            {
                SizeInt32 newSize = new SizeInt32();
                newSize.Width = mw.AppWindow.Size.Width;
                newSize.Height = mw.AppWindow.Size.Height + (int)(90 * pcInfo.XamlRoot.RasterizationScale);
                mw.AppWindow.Resize(newSize);
            }
        }

        private void PCInfo_WindowHeight_Decrease(Expander sender, ExpanderCollapsedEventArgs args)
        {
            MainWindow mw = (MainWindow)((App)(Application.Current)).m_window;
            if (pcInfo.XamlRoot is not null)
            {
                SizeInt32 newSize = new SizeInt32();
                newSize.Width = mw.AppWindow.Size.Width;
                newSize.Height = mw.AppWindow.Size.Height - (int)(90 * pcInfo.XamlRoot.RasterizationScale);
                mw.AppWindow.Resize(newSize);
            }
        }
    }
}
