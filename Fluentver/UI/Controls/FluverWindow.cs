using System.Runtime.CompilerServices;
using Windows.Graphics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using WinRT;
using WinUIEx;
using WinUIEx.Messaging;
using Fluver.System.Interop;
using Fluver.System.Interop.Com;
using static Fluver.System.Interop.PInvoke;
using SystemBackdrop = Microsoft.UI.Xaml.Media.SystemBackdrop;

namespace Fluver.UI.Controls;

public partial class FluverWindow : ContentControl, ICompositionSupportsSystemBackdrop, Windows.IWindow
{
    private static readonly ITaskbarList s_list;

    static FluverWindow()
    {
        var list = (ITaskbarList)new TaskbarList();
        if (list.HrInit() == S_OK)
        {
            s_list = list;
        }
    }

    #region Properties

    public nint Handle => _hWnd;
    private readonly nint _hWnd;

    public Matrix TransformToDevice => new(_dpiScaleX, 0.0, 0.0, _dpiScaleY, 0.0, 0.0);
    public Matrix TransformFromDevice => new(1.0 / _dpiScaleX, 0.0, 0.0, 1.0 / _dpiScaleY, 0.0, 0.0);
    private double _dpiScaleX = 1.0, _dpiScaleY = 1.0;

    public WindowHeader Header => _header;
    private readonly FluverWindowHeader _header;

    public WindowMenu Menu => _menu;
    private readonly FluverWindowMenu _menu;

    protected DesktopChildBridge DesktopBridge => _bridge;
    private readonly DesktopChildBridge _bridge;
    private bool _sizeBridge = true;

    Windows.IWindow Windows.IWindow.Owner
    {
        get => Owner;
        set => Owner = value as FluverWindow;
    }

    global::Windows.UI.Composition.CompositionBrush ICompositionSupportsSystemBackdrop.SystemBackdrop
    {
        get => _window.As<ICompositionSupportsSystemBackdrop>().SystemBackdrop;
        set => _window.As<ICompositionSupportsSystemBackdrop>().SystemBackdrop = value;
    }

    #endregion

    public event EventHandler Opened;
    public event EventHandler Closed;

    private readonly Window _window = new();
    private readonly WindowManager _manager;

    private bool IsOpen => _disposed == false;
    private bool IsDestroyed => _disposed == true;
    // _disposed has three states:
    // null  -> window hasn't been opened
    // false -> window is currently open
    // true  -> window is destroyed
    private bool? _disposed;

    private SizeInt32 _frameSize;

    private bool _showing;
    private WINDOWPOS _persistedPos;

    public FluverWindow()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        IsTabStop = false;

        _window.Content = this;

        var hWnd = _hWnd = _window.GetWindowHandle();
        _header = new(window: this);
        _menu = new(hMenu: GetSystemMenu(hWnd, bRevert: false));
        _bridge = new(window: this);

        _dpiScaleX = _dpiScaleY = HwndExtensions.GetDpiForWindow(hWnd) / 96.0;

        _manager = WindowManager.Get(_window);
        _manager.WindowMessageReceived += OnWindowMessageReceived;

        // Update _frameSize
        SetWindowPos(hWnd, X: 0, Y: 0, cx: 0, cy: 0, uFlags: SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);

        GetClientRect(hWnd, out var rect);
        SyncClientRect(rect);
    }

    #region Methods

    public void Show()
    {
        if (IsDestroyed)
        {
            return;
        }

        if (IsOpen)
        {
            _window.SetForegroundWindow();
            return;
        }

        _disposed = false;
        Resize(ClientWidth, ClientHeight);

        _showing = true;
        _window.Activate();
        _showing = false;

        Opened?.Invoke(sender: this, EventArgs.Empty);
    }

    public void Close()
    {
        PostMessage(_hWnd, WM_CLOSE, wParam: nuint.Zero, lParam: nint.Zero);
    }

    public void Resize(double width, double height)
    {
        var hWnd = _hWnd;
        var size = _frameSize;

        SetWindowPos(
            hWnd,
            X: 0, Y: 0,
            cx: RoundDoubleToInt(width * _dpiScaleX) + size.Width,
            cy: RoundDoubleToInt(height * _dpiScaleY) + size.Height,
            uFlags: SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE);
    }

    #endregion

    #region Window Procedure

    private void OnWindowMessageReceived(object sender, WindowMessageEventArgs e)
    {
        var message = e.Message;

        var handled = false;
        var result = Procedure(message.Hwnd, message.MessageId, message.WParam, message.LParam, ref handled);

        if (e.Handled = handled)
        {
            e.Result = result;
        }
    }

    protected virtual unsafe nint Procedure(nint hWnd, uint uMsg, nuint wParam, nint lParam, ref bool handled)
    {
        switch (uMsg)
        {
            case WM_DESTROY:
                {
                    _disposed = true;
                    _header.Dispose();
                    _manager.Dispose();

                    Closed?.Invoke(sender: this, EventArgs.Empty);
                }
                break;

            case WM_MOVE:
                handled = true;
                break;

            case WM_SIZE:
                handled = true;
                {
                    if (wParam == SIZE_MINIMIZED || wParam >= SIZE_MAXSHOW)
                    {
                        break;
                    }

                    GetClientRect(hWnd, out var rect);

                    if (IsOpen)
                    {
                        SyncClientRect(rect);
                    }

                    if (_sizeBridge)
                    {
                        _bridge.Resize(rect.Width, rect.Height);
                    }
                }
                break;

            case WM_ACTIVATE:
                {
                    SyncValue(IsActiveProperty, value: LOWORD((uint)wParam) != WA_INACTIVE);
                }
                break;

            case WM_CLOSE
            when IsModal && Owner is { _hWnd: { } hWndParent }:
                {
                    EnableWindow(hWndParent, bEnable: true);
                    if (IsActive)
                    {
                        SetActiveWindow(hWndParent);
                    }
                }
                break;

            case WM_SHOWWINDOW
            when IsModal && Owner is { _hWnd: { } hWndParent }:
                {
                    EnableWindow(hWndParent, wParam == 0);
                }
                break;

            case WM_WINDOWPOSCHANGING when _showing:
                {
                    _persistedPos = *(WINDOWPOS*)lParam;
                }
                break;

            case WM_GETMINMAXINFO:
                {
                    var mm = (MINMAXINFO*)lParam;
                    var size = _frameSize;
                    double scaleX = _dpiScaleX, scaleY = _dpiScaleY;

                    // Update minimum size
                    double minWidth = MinClientWidth, minHeight = MinClientHeight;
                    mm->ptMinTrackSize.x = (int)Math.Max((minWidth * scaleX) + size.Width, mm->ptMinTrackSize.x);
                    mm->ptMinTrackSize.y = (int)Math.Max((minHeight * scaleY) + size.Height, mm->ptMinTrackSize.y);

                    // Update maximum size
                    double maxWidth = MaxClientWidth, maxHeight = MaxClientHeight;
                    mm->ptMaxTrackSize.x = (int)Math.Min((Math.Max(minWidth, maxWidth) * scaleX) + size.Width, mm->ptMaxTrackSize.x);
                    mm->ptMaxTrackSize.y = (int)Math.Min((Math.Max(minHeight, maxHeight) * scaleY) + size.Height, mm->ptMaxTrackSize.y);
                }
                handled = true;
                break;

            case WM_NCCALCSIZE:
                {
                    var rect = (RECT*)lParam;
                    var windowRect = *rect;

                    DefSubclassProc(hWnd, uMsg, wParam, lParam);

                    _frameSize = new(windowRect.Width - rect->Width, windowRect.Height - rect->Height);
                }
                handled = true;
                break;

            case WM_NCLBUTTONDBLCLK when !IsMaximizable:
                handled = true;
                break;

            case WM_SYSCOMMAND:
                {
                    WindowMenuItemInvokedEventArgs e = new((ushort)(wParam & 0xFFF0));
                    _menu.OnSystemCommand(e);
                    handled = e.Handled;
                }
                break;

            case WM_DPICHANGED:
                handled = true;
                {
                    // Update scale factors
                    var dpis = (uint)wParam;
                    double scaleX = _dpiScaleX = LOWORD(dpis) / 96.0,
                           scaleY = _dpiScaleY = HIWORD(dpis) / 96.0;

                    // Update window size + position

                    if (_showing)
                    {
                        // Reapply persisted WINDOWPOS on the proper display to constrain window size correctly

                        _showing = false;

                        var pos = _persistedPos;
                        SetWindowPos(hWnd, X: 0, Y: 0, cx: 0, cy: 0, uFlags: SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
                        SetWindowPos(pos.hwnd, pos.hwndInsertAfter, pos.x, pos.y, pos.cx, pos.cy, pos.flags);

                        break;
                    }

                    var rect = (RECT*)lParam;
                    double width = ClientWidth, height = ClientHeight;

                    // Avoid sizing the bridge - we'll update it later
                    var sizeBridge = _sizeBridge;
                    _sizeBridge = false;
                    SetWindowPos(
                        hWnd,
                        X: rect->left,
                        Y: rect->top,
                        cx: rect->Width,
                        cy: rect->Height,
                        uFlags: SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
                    _sizeBridge = sizeBridge;

                    if (_header is { DragElement: { } element, InvalidateElement: { } invalidateElement })
                    {
                        // Forcefully refresh drag regions
                        _window.SetTitleBar(invalidateElement);
                        _window.SetTitleBar(element);
                    }

                    // WM_GETMINMAXINFO doesn't return the correct min/max sizes during our initial SetWindowPos
                    // Resize once again on the proper display to constrain the window size correctly
                    // 
                    // This resize also restores the previous ClientWidth + ClientHeight and updates the bridge
                    Resize(
                        Math.Min(width, rect->Width / scaleX),
                        Math.Min(height, rect->Height / scaleY));
                }
                break;
        }

        return nint.Zero;
    }

    #endregion

    #region Dependency Properties

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static DependencyProperty IconProperty { get; } =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(FluverWindow), new(defaultValue: string.Empty, OnIconChanged));

    private static void OnIconChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        (sender as FluverWindow)?._window.AppWindow?.SetIcon(e.NewValue as string ?? string.Empty);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static DependencyProperty TitleProperty { get; } =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(FluverWindow), new(defaultValue: string.Empty, OnTitleChanged));

    private static void OnTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { IsSyncing: false } window)
        {
            SetWindowText(window._hWnd, e.NewValue as string);
        }
    }

    public bool IsActive => (bool)GetValue(IsActiveProperty);

    public static DependencyProperty IsActiveProperty { get; } =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(FluverWindow), new(defaultValue: false, OnIsActiveChanged));

    private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { IsSyncing: false } window)
        {
            // Only allow set if syncing
            window.SyncValue(IsActiveProperty, e.OldValue);
        }
    }

    public SystemBackdrop SystemBackdrop
    {
        get => (SystemBackdrop)GetValue(SystemBackdropProperty);
        set => SetValue(SystemBackdropProperty, value);
    }

    public static DependencyProperty SystemBackdropProperty { get; } =
        DependencyProperty.Register(nameof(SystemBackdrop), typeof(SystemBackdrop), typeof(FluverWindow), new(defaultValue: null, OnSystemBackdropChanged));

    private static void OnSystemBackdropChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { IsDestroyed: not true } window)
        {
            window._window.SystemBackdrop = e.NewValue as SystemBackdrop;
        }
    }

    public FluverWindow Owner
    {
        get => (FluverWindow)GetValue(OwnerProperty);
        set => SetValue(OwnerProperty, value);
    }

    public static DependencyProperty OwnerProperty { get; } =
        DependencyProperty.Register(nameof(Owner), typeof(FluverWindow), typeof(FluverWindow), new(defaultValue: null, OnOwnerChanged));

    private static void OnOwnerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FluverWindow { _hWnd: { } hWnd } window)
        {
            return;
        }

        var enableParents = window.IsModal && window.IsOpen;

        if (enableParents && e.OldValue is FluverWindow { _hWnd: { } hWndParentOld })
        {
            EnableWindow(hWndParentOld, bEnable: true);
        }

        if (e.NewValue is FluverWindow { _hWnd: { } hWndParent })
        {
            SetWindowLong(hWnd, GWL_HWNDPARENT, hWndParent);
            if (enableParents)
            {
                EnableWindow(hWndParent, bEnable: false);
            }
        }
        else
        {
            SetWindowLong(hWnd, GWL_HWNDPARENT, nint.Zero);
        }
    }

    public bool IsShownInSwitchers
    {
        get => (bool)GetValue(IsShownInSwitchersProperty);
        set => SetValue(IsShownInSwitchersProperty, value);
    }

    public static DependencyProperty IsShownInSwitchersProperty { get; } =
        DependencyProperty.Register(nameof(IsShownInSwitchers), typeof(bool), typeof(FluverWindow), new(defaultValue: true, OnIsShownInSwitchersChanged));

    private static void OnIsShownInSwitchersChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { _hWnd: { } hWnd } && e.NewValue is bool isShown)
        {
            _ = isShown ? s_list?.AddTab(hWnd) : s_list?.DeleteTab(hWnd);
        }
    }

    public bool IsResizable
    {
        get => (bool)GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
    }

    public static DependencyProperty IsResizableProperty { get; } =
        DependencyProperty.Register(nameof(IsResizable), typeof(bool), typeof(FluverWindow), new(defaultValue: true, OnIsResizableChanged));

    private static void OnIsResizableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { _hWnd: { } hWnd } && e.NewValue is bool isResizable)
        {
            HwndExtensions.ToggleWindowStyle(hWnd, isResizable, WindowStyle.SizeBox);
        }
    }

    public bool IsMaximizable
    {
        get => (bool)GetValue(IsMaximizableProperty);
        set => SetValue(IsMaximizableProperty, value);
    }

    public static DependencyProperty IsMaximizableProperty { get; } =
        DependencyProperty.Register(nameof(IsMaximizable), typeof(bool), typeof(FluverWindow), new(defaultValue: true, OnIsMaximizableChanged));

    private static void OnIsMaximizableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { _hWnd: { } hWnd } && e.NewValue is bool isMaximizable)
        {
            HwndExtensions.ToggleWindowStyle(hWnd, isMaximizable, WindowStyle.MaximizeBox);
        }
    }

    public bool IsMinimizable
    {
        get => (bool)GetValue(IsMinimizableProperty);
        set => SetValue(IsMinimizableProperty, value);
    }

    public static DependencyProperty IsMinimizableProperty { get; } =
        DependencyProperty.Register(nameof(IsMinimizable), typeof(bool), typeof(FluverWindow), new(defaultValue: true, OnIsMinimizableChanged));

    private static void OnIsMinimizableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { _hWnd: { } hWnd } && e.NewValue is bool isMinimizable)
        {
            HwndExtensions.ToggleWindowStyle(hWnd, isMinimizable, WindowStyle.MinimizeBox);
        }
    }

    public bool IsModal
    {
        get => (bool)GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public static DependencyProperty IsModalProperty { get; } =
        DependencyProperty.Register(nameof(IsModal), typeof(bool), typeof(FluverWindow), new(defaultValue: false, OnIsModalChanged));

    private static void OnIsModalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { IsOpen: true, Owner._hWnd: { } hWndParent } && e.NewValue is bool isModal)
        {
            EnableWindow(hWndParent, !isModal);
        }
    }

    public double ClientWidth
    {
        get => (double)GetValue(ClientWidthProperty);
        set => SetValue(ClientWidthProperty, value);
    }

    public static DependencyProperty ClientWidthProperty { get; } =
        DependencyProperty.Register(nameof(ClientWidth), typeof(double), typeof(FluverWindow), new(defaultValue: 0.0, OnClientWidthChanged));

    private static void OnClientWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { IsSyncing: false, IsOpen: true } window && e.NewValue is double width)
        {
            window.Resize(width, window.ClientHeight);
        }
    }

    public double MinClientWidth
    {
        get => (double)GetValue(MinClientWidthProperty);
        set => SetValue(MinClientWidthProperty, value);
    }

    public static DependencyProperty MinClientWidthProperty { get; } =
        DependencyProperty.Register(nameof(MinClientWidth), typeof(double), typeof(FluverWindow), new(defaultValue: 0.0, OnClientConstraintChanged));

    public double MaxClientWidth
    {
        get => (double)GetValue(MaxClientWidthProperty);
        set => SetValue(MaxClientWidthProperty, value);
    }

    public static DependencyProperty MaxClientWidthProperty { get; } =
        DependencyProperty.Register(
            nameof(MaxClientWidth),
            typeof(double),
            typeof(FluverWindow),
            new(defaultValue: double.PositiveInfinity, OnClientConstraintChanged));

    public double ClientHeight
    {
        get => (double)GetValue(ClientHeightProperty);
        set => SetValue(ClientHeightProperty, value);
    }

    public static DependencyProperty ClientHeightProperty { get; } =
        DependencyProperty.Register(nameof(ClientHeight), typeof(double), typeof(FluverWindow), new(defaultValue: 0.0, OnClientHeightChanged));

    private static void OnClientHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { IsSyncing: false, IsOpen: true } window && e.NewValue is double height)
        {
            window.Resize(window.ClientWidth, height);
        }
    }

    public double MinClientHeight
    {
        get => (double)GetValue(MinClientHeightProperty);
        set => SetValue(MinClientHeightProperty, value);
    }

    public static DependencyProperty MinClientHeightProperty { get; } =
        DependencyProperty.Register(nameof(MinClientHeight), typeof(double), typeof(FluverWindow), new(defaultValue: 0.0, OnClientConstraintChanged));

    public double MaxClientHeight
    {
        get => (double)GetValue(MaxClientHeightProperty);
        set => SetValue(MaxClientHeightProperty, value);
    }

    public static DependencyProperty MaxClientHeightProperty { get; } =
        DependencyProperty.Register(
            nameof(MaxClientHeight),
            typeof(double),
            typeof(FluverWindow),
            new(defaultValue: double.PositiveInfinity, OnClientConstraintChanged));

    private static void OnClientConstraintChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is FluverWindow { IsOpen: true } window)
        {
            window.Resize(window.ClientWidth, window.ClientHeight);
        }
    }

    public string PersistenceId
    {
        get => (string)GetValue(PersistenceIdProperty);
        set => SetValue(PersistenceIdProperty, value);
    }

    public static DependencyProperty PersistenceIdProperty { get; } =
        DependencyProperty.Register(nameof(PersistenceId), typeof(string), typeof(FluverWindow), new(defaultValue: null, OnPersistenceIdChanged));

    private static void OnPersistenceIdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        (sender as FluverWindow)?._manager.PersistenceId = e.NewValue as string;
    }

    #endregion

    #region Synchronization

    private bool IsSyncing => _syncDepth > 0;

    private int _syncDepth;

    private void SyncValue(DependencyProperty property, object value)
    {
        ++_syncDepth;
        try
        {
            SetValue(property, value);
        }
        finally
        {
            --_syncDepth;
        }
    }

    private void SyncClientRect(RECT clientRect)
    {
        SyncValue(ClientWidthProperty, clientRect.Width / _dpiScaleX);
        SyncValue(ClientHeightProperty, clientRect.Height / _dpiScaleY);
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int RoundDoubleToInt(double value)
    {
        return (0 < value) ? (int)(value + 0.5) : (int)(value - 0.5);
    }
}
