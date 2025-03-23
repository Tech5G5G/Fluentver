namespace Fluentver.Controls;

public partial class SizeWindow : Window
{
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

    public bool DoubleClickToMaximize { get; set; } = true;

    readonly WindowManager manager;

    public SizeWindow()
    {
        manager = WindowManager.Get(this);
        manager.WindowMessageReceived += (s, e) =>
        {
            if (DoubleClickToMaximize && e.Message.MessageId == 0x00A3) //WM_NCLBUTTONDBLCLK
                e.Handled = true;
        };
    }

    public void SizeToElement(FrameworkElement element, Dimensions trackedDimensions = Dimensions.Width | Dimensions.Height)
    {
        element.SizeChanged += (s, e) =>
        {
            if (trackedDimensions.HasFlag(Dimensions.Width))
                manager.Width = element.ActualWidth;
            if (trackedDimensions.HasFlag(Dimensions.Height))
                manager.Height = element.ActualHeight;
        };
    }
}

[Flags]
public enum Dimensions
{
    Width = 1,
    Height = 2
}