namespace Fluentver.Controls;

public partial class InfoPage : Page
{
    public IList<GlyphButton> ToolbarButtons { get; } = [];

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is MainWindow mw)
        {
            mw.ToolbarButtons.Clear();
            foreach (var button in ToolbarButtons)
                mw.ToolbarButtons.Add(button);
        }
    }
}
