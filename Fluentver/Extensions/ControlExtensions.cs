namespace Fluentver.Extensions;

public static class ControlExtensions
{
    public static int GetSelectedIndex(this SelectorBar bar) => bar.Items.IndexOf(bar.SelectedItem);

    public static void SetSelectedIndex(this SelectorBar bar, int index)
    {
        if (index >= 0 && index < bar.Items.Count)
            bar.SelectedItem = bar.Items[index];
        else
            bar.SelectedItem = null;
    }

    public static GlyphButton AddClick(this GlyphButton button, RoutedEventHandler handler)
    {
        button.Click += handler;
        return button;
    }
}
