using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Fluver.Extensions;

public static class ControlExtensions
{
    public static int GetSelectedIndex(this SelectorBar bar)
    {
        return bar.Items.IndexOf(bar.SelectedItem);
    }

    public static void SetSelectedIndex(this SelectorBar bar, int index)
    {
        bar.SelectedItem = index >= 0 && index < bar.Items.Count ? bar.Items[index] : null;
    }

    public static T AddClick<T>(this T button, RoutedEventHandler handler) where T : ButtonBase
    {
        button.Click += handler;
        return button;
    }

    /// <summary>
    /// Sets the text of a TextBlock if no text is currently selected.
    /// </summary>
    /// <param name="block">Represents the <see cref="TextBlock"/> to modify.</param>
    /// <param name="text">The <see cref="string"/> to set as the text of <paramref name="block"/>.</param>
    public static void SetTextFriendly(this TextBlock block, string text)
    {
        if (string.IsNullOrEmpty(block.SelectedText))
        {
            block.Text = text;
        }
    }
}
