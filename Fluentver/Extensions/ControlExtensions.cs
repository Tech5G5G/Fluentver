using Microsoft.UI.Xaml.Controls;

namespace Fluver.Extensions;

public static class ControlExtensions
{
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
