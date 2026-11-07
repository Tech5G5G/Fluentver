using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using static Windows.Win32.PInvoke;

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

    private sealed class FluverWindowMenu(HMENU hMenu) : WindowMenu(hMenu)
    {
        public void OnSystemCommand(WindowMenuItemInvokedEventArgs e)
        {
            OnItemInvoked(e);
        }
    }

    protected sealed class DesktopChildBridge(FluverWindow window)
    {
        public nint Handle => _hWnd;
        private readonly HWND _hWnd = FindWindowEx(window._hWnd, lpszClass: "Microsoft.UI.Content.DesktopChildSiteBridge");

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
                _hWnd,
                hWndInsertAfter: HWND.HWND_BOTTOM,
                X: 0, Y: 0,
                width, height,
                SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW);

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

public abstract class WindowMenu
{
    public nint Handle => _hMenu;
    private readonly HMENU _hMenu;

    public event EventHandler<WindowMenuItemInvokedEventArgs> ItemInvoked;

    internal WindowMenu(HMENU hMenu)
    {
        _hMenu = hMenu;
    }

    public WindowMenu AddMenuItem(ushort id, string text)
    {
        InsertMenu(_hMenu, SC_CLOSE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_STRING, id, text);
        return this;
    }

    public WindowMenu AddMenuSeparator()
    {
        InsertMenu(_hMenu, SC_CLOSE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_SEPARATOR, uIDNewItem: 0);
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
