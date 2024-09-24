using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Security.Cryptography.Core;
using Windows.System;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluentver
{
    public sealed class GlyphButton : Control
    {
        public GlyphButton()
        {
            this.DefaultStyleKey = typeof(GlyphButton);
        }

        public event RoutedEventHandler Click;

        private Button ButtonControl { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ButtonControl is not null)
            {
                ButtonControl.Click -= ButtonControl_Click;
            }

            if (GetTemplateChild(nameof(ButtonControl)) is Button button)
            {
                ButtonControl = button;
                ButtonControl.Click += ButtonControl_Click;
            }
        }

        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GlyphButton), new PropertyMetadata(string.Empty));

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(GlyphButton), new PropertyMetadata(string.Empty));
    }

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            double scale;
            if (appWindow is not null)
            {
                uint dpi = GetDpiForWindow(hWnd);
                switch (dpi)
                {
                    case 90:
                        scale = 1;
                        break;
                    case 120:
                        scale = 1.25;
                        break;
                    case 144:
                        scale = 1.5;
                        break;
                    case 168:
                        scale = 1.75;
                        break;
                    case 192:
                        scale = 2;
                        break;
                    case 216:
                        scale = 2.25;
                        break;
                    case 240:
                        scale = 2.5;
                        break;
                    case 288:
                        scale = 3;
                        break;
                    default:
                        scale = (double)dpi / 100;
                        break;
                }
                var newSize = new SizeInt32();
                newSize.Width = (int)(480 * scale);
                newSize.Height = (int)(590 * scale);
                appWindow.Resize(newSize);
            }

            m_window.Activate();
        }

        public Window m_window;

        public async Task<string> GetCurrentUserInfo(string userProperty)
        {
            IReadOnlyList<User> users = await User.FindAllAsync();

            var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                        p.Type == UserType.LocalUser).FirstOrDefault();

            var data = await current.GetPropertyAsync(userProperty);
            string userPropertyResult = (string)data;

            return userPropertyResult;
        }
    }
}
