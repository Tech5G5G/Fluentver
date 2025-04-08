namespace Fluentver.Controls;

[Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(Content))]
public partial class SizeWindow : Window
{
    public new UIElement Content
    {
        get => base.Content;
        set
        {
            if (base.Content is FrameworkElement prevElement)
                prevElement.SizeChanged -= UpdateWindowSize;

            base.Content = value;

            if (value is FrameworkElement element)
                element.SizeChanged += UpdateWindowSize;
        }
    }

    private void UpdateWindowSize(object sender, SizeChangedEventArgs e)
    {
        if (SizeToContent.HasFlag(Dimensions.Width))
            manager.Width = e.NewSize.Width;
        if (SizeToContent.HasFlag(Dimensions.Height))
            manager.Height = e.NewSize.Height;
    }

    public bool IsDialogWindow
    {
        get => !AppWindow.IsShownInSwitchers;
        set
        {
            WindowHelper.ActivateWindow(this.GetWindowHandle());
            AppWindow.IsShownInSwitchers = DoubleClickToMaximize = !value;
            if (AppWindow.Presenter is OverlappedPresenter presenter)
                presenter.IsResizable = presenter.IsMaximizable = presenter.IsMinimizable = !(presenter.IsAlwaysOnTop = value);
        }
    }

    public double Width
    {
        get => manager.Width;
        set => manager.Width = value;
    }

    public double Height
    {
        get => manager.Height;
        set => manager.Height = value;
    }

    public Dimensions SizeToContent { get; set; } = Dimensions.None;

    public bool DoubleClickToMaximize { get; set; } = true;

    public event TypedEventHandler<SizeWindow, object> ResolutionChanged;

    public event TypedEventHandler<SizeWindow, Windows.Graphics.PointInt32> PositionChanged;

    public event TypedEventHandler<SizeWindow, nint> DeviceChanged;

    readonly WindowManager manager;

    public SizeWindow()
    {
        manager = WindowManager.Get(this);
        manager.PositionChanged += (s, e) => PositionChanged?.Invoke(this, e);
        manager.WindowMessageReceived += (s, e) =>
        {
            switch (e.Message.MessageId)
            {
                case 0x00A3 when !DoubleClickToMaximize: //WM_NCLBUTTONDBLCLK
                    e.Handled = true;
                    break;
                case 0x007E: //WM_DISPLAYCHANGE
                    ResolutionChanged?.Invoke(this, null);
                    break;
                case 0x219 when e.Message.WParam == 0x7: //WM_DEVICECHANGE, DBT_DEVNODES_CHANGED
                    DeviceChanged?.Invoke(this, e.Message.LParam);
                    break;
            }
        };
    }
}

[Flags]
public enum Dimensions
{
    None = 1,
    Width = 2,
    Height = 4
}