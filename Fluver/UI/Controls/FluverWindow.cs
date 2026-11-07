using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using WinRT;
using WinRT.Interop;
using WinUIEx;
using WinUIEx.Messaging;
using Fluver.Contracts;
using static Windows.Win32.PInvoke;
using SystemBackdrop = Microsoft.UI.Xaml.Media.SystemBackdrop;

namespace Fluver.UI.Controls;

public partial class FluverWindow : ContentControl, ICompositionSupportsSystemBackdrop, IWindow
{
    public static IReadOnlyList<FluverWindow> Windows => s_windows;
    private static readonly List<FluverWindow> s_windows = [];

    private static readonly ITaskbarList s_list;

    static FluverWindow()
    {
        SetPreferredAppMode(PREFERRED_APP_MODE.PAM_ALLOWDARK);
        FlushMenuThemes();

        var list = TaskbarList.CreateInstance<ITaskbarList>();
        if (list.HrInit().Succeeded)
        {
            s_list = list;
        }
    }

    #region Properties

    public nint Handle => _hWnd;
    private HWND _hWnd;

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

    IWindow IWindow.Owner
    {
        get => Owner;
        set => Owner = value as FluverWindow;
    }

    Windows.UI.Composition.CompositionBrush ICompositionSupportsSystemBackdrop.SystemBackdrop
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

    private bool _updatesAppearance;

    private bool _showing;
    private WINDOWPOS _persistedPos;

    public FluverWindow()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        IsTabStop = false;

        _window.Content = this;

        var hWnd = _hWnd = (HWND)WindowNative.GetWindowHandle(_window);
        _header = new(window: this);
        _menu = new(hMenu: GetSystemMenu(hWnd, bRevert: false));
        _bridge = new(window: this);

        _dpiScaleX = _dpiScaleY = GetDpiForWindow(hWnd) / (double)USER_DEFAULT_SCREEN_DPI;

        _manager = WindowManager.Get(_window);
        _manager.WindowMessageReceived += OnWindowMessageReceived;

        UpdateFrameSize(hWnd);

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
            SetForegroundWindow(_hWnd);
            return;
        }

        _disposed = false;
        Resize(ClientWidth, ClientHeight);

        if (_updatesAppearance = s_windows.Count == 0)
        {
            UpdateAnimations();
            UpdateAccentColor();
        }

        s_windows.Add(item: this);

        _showing = true;
        _window.Activate();
        _showing = false;

        Opened?.Invoke(sender: this, EventArgs.Empty);
    }

    public void Close()
    {
        PostMessage(_hWnd, WM_CLOSE, wParam: default, lParam: default);
    }

    public void Resize(double width, double height)
    {
        var size = _frameSize;

        SetWindowPos(
            _hWnd,
            X: 0, Y: 0,
            cx: RoundDoubleToInt(width * _dpiScaleX) + size.Width,
            cy: RoundDoubleToInt(height * _dpiScaleY) + size.Height,
            uFlags: SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                    SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                    SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
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
                    _manager.Dispose();

                    s_windows.Remove(item: this);

                    if (_updatesAppearance)
                    {
                        s_windows.FirstOrDefault()?._updatesAppearance = true;
                        _updatesAppearance = false;
                    }

                    Closed?.Invoke(sender: this, EventArgs.Empty);

                    _hWnd = HWND.Null;
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

                    GetClientRect((HWND)hWnd, out var rect);

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

            case WM_CLOSE:
                {
                    // Hide here to stop content from disappearing
                    // during close animation - we're already hiding anyway
                    ShowWindow((HWND)hWnd, SHOW_WINDOW_CMD.SW_HIDE);

                    if (IsModal && Owner is { _hWnd: { } hWndParent })
                {
                    EnableWindow(hWndParent, bEnable: true);
                    if (IsActive)
                    {
                        SetActiveWindow(hWndParent);
                    }
                }
                }
                break;

            case WM_SHOWWINDOW
            when IsModal && Owner is { _hWnd: { } hWndParent }:
                {
                    EnableWindow(hWndParent, wParam == 0);
                }
                break;

            case WM_SETTINGCHANGE when _updatesAppearance:
                {
                    if (wParam == (uint)SYSTEM_PARAMETERS_INFO_ACTION.SPI_SETCLIENTAREAANIMATION)
                    {
                        UpdateAnimations();
                    }
                    else if (Marshal.PtrToStringUni(lParam) == "ImmersiveColorSet")
                {
                        UpdateAccentColor();
                    }
                }
                break;

            case WM_GETMINMAXINFO:
                {
                    var mm = (MINMAXINFO*)lParam;
                    var size = _frameSize;
                    double scaleX = _dpiScaleX, scaleY = _dpiScaleY;

                    // Update minimum size
                    double minWidth = MinClientWidth, minHeight = MinClientHeight;
                    mm->ptMinTrackSize.X = (int)Math.Max((minWidth * scaleX) + size.Width, mm->ptMinTrackSize.X);
                    mm->ptMinTrackSize.Y = (int)Math.Max((minHeight * scaleY) + size.Height, mm->ptMinTrackSize.Y);

                    // Update maximum size
                    double maxWidth = MaxClientWidth, maxHeight = MaxClientHeight;
                    mm->ptMaxTrackSize.X = (int)Math.Min((Math.Max(minWidth, maxWidth) * scaleX) + size.Width, mm->ptMaxTrackSize.X);
                    mm->ptMaxTrackSize.Y = (int)Math.Min((Math.Max(minHeight, maxHeight) * scaleY) + size.Height, mm->ptMaxTrackSize.Y);
                }
                handled = true;
                break;

            case WM_WINDOWPOSCHANGING when _showing:
                {
                    _persistedPos = *(WINDOWPOS*)lParam;
                }
                break;

            case WM_NCCALCSIZE:
                {
                    var rect = (RECT*)lParam;
                    var windowRect = *rect;

                    DefSubclassProc((HWND)hWnd, uMsg, wParam, lParam);

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
                    double scaleX = _dpiScaleX = LOWORD(dpis) / (double)USER_DEFAULT_SCREEN_DPI,
                           scaleY = _dpiScaleY = HIWORD(dpis) / (double)USER_DEFAULT_SCREEN_DPI;

                    // Update window size + position

                    if (_showing)
                    {
                        // Reapply persisted WINDOWPOS on the proper display to constrain window size correctly

                        _showing = false;

                        var pos = _persistedPos;
                        UpdateFrameSize((HWND)hWnd);
                        SetWindowPos(pos.hwnd, pos.hwndInsertAfter, pos.x, pos.y, pos.cx, pos.cy, pos.flags);

                        break;
                    }

                    var rect = (RECT*)lParam;
                    double width = ClientWidth, height = ClientHeight;

                    // Avoid sizing the bridge - we'll update it later
                    var sizeBridge = _sizeBridge;
                    _sizeBridge = false;
                    SetWindowPos(
                        (HWND)hWnd,
                        X: rect->left,
                        Y: rect->top,
                        cx: rect->Width,
                        cy: rect->Height,
                        uFlags: SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE |
                                SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED);
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
        if (sender is FluverWindow { IsDestroyed: false } window)
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
            SetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, hWndParent);
            if (enableParents)
            {
                EnableWindow(hWndParent, bEnable: false);
            }
        }
        else
        {
            SetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, dwNewLong: nint.Zero);
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
            ToggleWindowStyle(hWnd, isResizable, WINDOW_STYLE.WS_SIZEBOX);
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
            ToggleWindowStyle(hWnd, isMaximizable, WINDOW_STYLE.WS_MAXIMIZEBOX);
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
            ToggleWindowStyle(hWnd, isMinimizable, WINDOW_STYLE.WS_MINIMIZEBOX);
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

    private static void UpdateFrameSize(HWND hWnd)
    {
        SetWindowPos(
            hWnd,
            X: 0, Y: 0,
            cx: 0, cy: 0,
            uFlags: SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                    SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                    SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                    SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED |
                    SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER);
    }

    private static void ToggleWindowStyle(HWND hWnd, bool enable, WINDOW_STYLE flag)
    {
        var style = GetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        if (enable)
        {
            style |= unchecked((int)flag);
        }
        else
        {
            style &= ~unchecked((int)flag);
        }

        SetWindowLongPtr(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
        UpdateFrameSize(hWnd);
    }

    private static int RoundDoubleToInt(double value)
    {
        return (0 < value) ? (int)(value + 0.5) : (int)(value - 0.5);
    }
}
