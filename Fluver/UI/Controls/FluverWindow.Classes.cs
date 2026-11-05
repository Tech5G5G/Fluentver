using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using static Fluver.Platform.Interop.PInvoke;

namespace Fluver.UI.Controls;

partial class FluverWindow
{
    private sealed partial class FluverWindowHeader(FluverWindow window) : WindowHeader
    {
        public UIElement InvalidateElement => _element;
        private HeaderInvalidateElement _element;

        public override bool OverlapsContent
        {
            get;
            set
            {
                if (!_window.IsDestroyed)
                {
                    _window._window.ExtendsContentIntoTitleBar = field = value;
                }
            }
        }

        public override TitleBarHeightOption HeightOption
        {
            get;
            set
            {
                if (!_window.IsDestroyed && OverlapsContent && _window._window.AppWindow is { TitleBar: { } titleBar })
                {
                    titleBar.PreferredHeightOption = field = value;
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

    public abstract TitleBarHeightOption HeightOption { get; set; }

    public abstract UIElement DragElement { get; set; }
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
