using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace Fluver.UI.Data;

public sealed partial class CultureInfoFormatter : IValueConverter
{
    public bool UseNativeName { get; set; } = true;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is CultureInfo culture)
        {
            var name = UseNativeName ? culture.NativeName : culture.DisplayName;
            return string.IsNullOrEmpty(name) ? name : char.ToUpper(name[0], UseNativeName ? culture : CultureInfo.CurrentCulture) + name[1..];
        }

        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return null;
    }
}
