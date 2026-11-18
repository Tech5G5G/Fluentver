using System.Globalization;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Fluver.UI.Data;

public static class BindingHelpers
{
    public static bool IsNotWhiteSpace(string str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    public static BitmapImage ToImage(string path)
    {
        return !string.IsNullOrWhiteSpace(path) && Uri.TryCreate(path, UriKind.Absolute, out var uri) ? new(uri) : null;
    }

    public static string FormatCultureInfo(CultureInfo culture, bool nativeName)
    {
        var name = nativeName ? culture.NativeName : culture.DisplayName;
        return string.IsNullOrEmpty(name) ? name : char.ToUpper(name[0], nativeName ? culture : CultureInfo.CurrentCulture) + name[1..];
    }
}
