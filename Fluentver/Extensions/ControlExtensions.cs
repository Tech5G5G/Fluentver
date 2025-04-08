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


    /// <summary>Sets the text of a TextBlock if no text is currently selected.</summary>
    /// <param name="block">Represents the <see cref="TextBlock"/> to modify.</param>
    /// <param name="text">The <see cref="string"/> to set as the text of <paramref name="block"/>.</param>
    public static void SetTextFriendly(this TextBlock block, string text)
    {
        if (string.IsNullOrEmpty(block.SelectedText))
            block.Text = text;
    }
}
