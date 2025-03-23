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

    readonly WindowManager manager;

    public SizeWindow() => manager = WindowManager.Get(this);
}
