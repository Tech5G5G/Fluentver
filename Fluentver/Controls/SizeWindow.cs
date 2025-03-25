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

    readonly WindowManager manager;

    public SizeWindow()
    {
        manager = WindowManager.Get(this);
        manager.WindowMessageReceived += (s, e) =>
        {
            if (!DoubleClickToMaximize && e.Message.MessageId == 0x00A3) //WM_NCLBUTTONDBLCLK
                e.Handled = true;
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