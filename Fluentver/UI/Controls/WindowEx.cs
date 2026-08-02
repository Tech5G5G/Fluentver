using Windows.Graphics;
using Windows.Foundation;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinUIEx;

namespace Fluver.UI.Controls;

[Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(Content))]
public partial class WindowEx : Window
{
    public bool DoubleClickToMaximize { get; set; } = true;

    public bool IsDialogWindow
    {
        get => !AppWindow.IsShownInSwitchers && !DoubleClickToMaximize && AppWindow.OwnerWindowId.Value < 0 &&
            AppWindow.Presenter is OverlappedPresenter
            {
                IsResizable: false,
                IsMaximizable: false,
                IsMinimizable: false,
                IsAlwaysOnTop: true
            };
        set
        {
            AppWindow.IsShownInSwitchers = DoubleClickToMaximize = !value;
            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsResizable = presenter.IsMaximizable = presenter.IsMinimizable = !(presenter.IsAlwaysOnTop = value);
            }
        }
    }

    #region Dimensions

    public double Width
    {
        get => _manager.Width;
        set => _manager.Width = value;
    }

    public double MinWidth
    {
        get => _manager.MinWidth;
        set => _manager.MinWidth = value;
    }

    public double MaxWidth
    {
        get => _manager.MaxWidth;
        set => _manager.MaxWidth = value;
    }

    public double Height
    {
        get => _manager.Height;
        set => _manager.Height = value;
    }

    public double MinHeight
    {
        get => _manager.MinHeight;
        set => _manager.MinHeight = value;
    }

    public double MaxHeight
    {
        get => _manager.MaxHeight;
        set => _manager.MaxHeight = value;
    }

    #endregion

    #region Size To Content

    public Dimensions SizeToContent { get; set; } = Dimensions.None;

    public new UIElement Content
    {
        get => base.Content;
        set
        {
            if (base.Content is FrameworkElement prevElement)
            {
                prevElement.LayoutUpdated -= Content_LayoutUpdated;
            }

            base.Content = value;

            if (value is FrameworkElement element)
            {
                element.LayoutUpdated += Content_LayoutUpdated;
            }
        }
    }

    private void Content_LayoutUpdated(object sender, object e)
    {
        if (AppWindow is null || base.Content is not FrameworkElement element)
        {
            return;
        }

        if (SizeToContent.HasFlag(Dimensions.Width))
        {
            _manager.Width = element.ActualWidth;
        }

        if (SizeToContent.HasFlag(Dimensions.Height))
        {
            _manager.Height = element.ActualHeight;
        }
    }

    #endregion

    #region Events

    public event TypedEventHandler<WindowEx, object> ResolutionChanged;
    public event TypedEventHandler<WindowEx, PointInt32> PositionChanged;
    public event TypedEventHandler<WindowEx, nint> DeviceChanged;

    #endregion

    private readonly WindowManager _manager;

    public WindowEx()
    {
        _manager = WindowManager.Get(this);

        _manager.PositionChanged += (s, e) => PositionChanged?.Invoke(this, e);
        _manager.WindowMessageReceived += (s, e) =>
        {
            var message = e.Message;
            var handled = false;

            var result = Procedure(message.Hwnd, message.MessageId, message.WParam, message.LParam, ref handled);

            if (e.Handled = handled)
            {
                e.Result = result;
            }
        };
    }

    protected virtual nint Procedure(nint hWnd, uint uMsg, nuint wParam, nint lParam, ref bool handled)
    {
        switch (uMsg)
        {
            case 0x00A3 when !DoubleClickToMaximize: // WM_NCLBUTTONDBLCLK
                handled = true;
                break;

            case 0x007E: // WM_DISPLAYCHANGE
                ResolutionChanged?.Invoke(this, null);
                break;

            case 0x219 when wParam == 0x7: // WM_DEVICECHANGE, DBT_DEVNODES_CHANGED
                DeviceChanged?.Invoke(this, lParam);
                break;
        }

        return nint.Zero;
    }
}

[Flags]
public enum Dimensions
{
    None = 0,
    Width = 1,
    Height = 2
}