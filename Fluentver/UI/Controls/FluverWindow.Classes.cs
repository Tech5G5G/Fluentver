using Microsoft.UI.Xaml;
using WinUIEx.Messaging;
using static Fluver.System.Interop.PInvoke;

namespace Fluver.UI.Controls;

partial class FluverWindow
{
    private sealed partial class FluverWindowHeader(FluverWindow window) : WindowHeader, IDisposable
    {
        public UIElement InvalidateElement => _element;
        private HeaderInvalidateElement _element;

        public override bool OverlapsContent
        {
            get;
            set
            {
                if (_window.IsDestroyed)
                {
                    return;
                }

                _window._window.ExtendsContentIntoTitleBar = field = value;
                UpdateSizes();

                if (value)
                {
                    TrySubclass();
                }
            }
        }

        public override UIElement DragElement
        {
            get;
            set
            {
                if (_window.IsDestroyed)
                {
                    return;
                }

                _window._window.SetTitleBar(field = value);

                if (value is not null)
                {
                    _element ??= new() { Width = int.MaxValue, Height = int.MaxValue };
                }
            }
        }

        private readonly FluverWindow _window = window;
        private WindowMessageMonitor _monitor;

        private void OnWindowMessage(object sender, WindowMessageEventArgs e)
        {
            if (e.Message.MessageId == WM_SIZE)
            {
                UpdateSizes();
            }
        }

        private void TrySubclass()
        {
            if (_monitor is not null)
            {
                return;
            }

            var hWnd = FindWindowEx(_window._hWnd, lpszClass: "ReunionWindowingCaptionControls");
            if (hWnd != nint.Zero)
            {
                _monitor = new(hWnd);
                _monitor.WindowMessageReceived += OnWindowMessage;
            }
        }

        private void UpdateSizes()
        {
            var titleBar = _window._window.AppWindow?.TitleBar;

            SetValue(HeightProperty, titleBar.Height / _window._dpiScaleY);
            SetValue(LeftInsetProperty, titleBar.LeftInset / _window._dpiScaleX);
            SetValue(RightInsetProperty, titleBar.RightInset / _window._dpiScaleX);
        }

        public void Dispose()
        {
            _monitor?.Dispose();
        }

        private sealed partial class HeaderInvalidateElement : FrameworkElement;
    }

    private sealed class FluverWindowMenu(nint hMenu) : WindowMenu(hMenu)
    {
        public void OnSystemCommand(WindowMenuItemInvokedEventArgs e)
        {
            OnItemInvoked(e);
        }
    }

    protected sealed class DesktopChildBridge(FluverWindow window)
    {
        public nint Handle { get; } = FindWindowEx(window._hWnd, lpszClass: "Microsoft.UI.Content.DesktopChildSiteBridge");

        public bool SizeToParent
        {
            get => _window._sizeBridge;
            set => _window._sizeBridge = value;
        }

        private readonly FluverWindow _window = window;

        public DesktopChildBridge Resize(double width, double height)
        {
            return Resize(
                RoundDoubleToInt(width * _window._dpiScaleX),
                RoundDoubleToInt(height * _window._dpiScaleY));
        }

        public DesktopChildBridge Resize(int width, int height)
        {
            SetWindowPos(
                Handle,
                hWndInsertAfter: HWND_BOTTOM,
                X: 0, Y: 0,
                width, height,
                SWP_SHOWWINDOW);

            return this;
        }
    }
}

public abstract class WindowHeader : DependencyObject
{
    public abstract bool OverlapsContent { get; set; }

    public abstract UIElement DragElement { get; set; }

    public double Height => (double)GetValue(HeightProperty);

    public static DependencyProperty HeightProperty { get; } =
        DependencyProperty.Register(nameof(Height), typeof(double), typeof(WindowHeader), new(defaultValue: 0.0));

    public double LeftInset => (double)GetValue(LeftInsetProperty);

    public static DependencyProperty LeftInsetProperty { get; } =
        DependencyProperty.Register(nameof(LeftInset), typeof(double), typeof(WindowHeader), new(defaultValue: 0.0));

    public double RightInset => (double)GetValue(RightInsetProperty);

    public static DependencyProperty RightInsetProperty { get; } =
        DependencyProperty.Register(nameof(RightInset), typeof(double), typeof(WindowHeader), new(defaultValue: 0.0));
}

public abstract class WindowMenu(nint hMenu)
{
    public nint Handle { get; } = hMenu;

    public event EventHandler<WindowMenuItemInvokedEventArgs> ItemInvoked;

    public WindowMenu AddMenuItem(ushort id, string text)
    {
        InsertMenu(Handle, SC_CLOSE, MF_BYCOMMAND | MF_STRING, id, text);
        return this;
    }

    public WindowMenu AddMenuSeparator()
    {
        InsertMenu(Handle, SC_CLOSE, MF_BYCOMMAND | MF_SEPARATOR, uIDNewItem: 0);
        return this;
    }

    protected void OnItemInvoked(WindowMenuItemInvokedEventArgs e)
    {
        ItemInvoked?.Invoke(sender: this, e);
    }
}

public sealed class WindowMenuItemInvokedEventArgs(ushort id) : EventArgs
{
    private const ushort MinSystemRange = 0xF000,
                         MaxSystemRange = 0xFFFF;

    public ushort ItemId { get; } = id;

    public bool IsSystemItem => ItemId >= MinSystemRange && ItemId <= MaxSystemRange;

    public bool Handled { get; set; }
}
