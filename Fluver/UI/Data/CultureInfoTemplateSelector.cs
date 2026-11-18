using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fluver.UI.Data;

public sealed partial class CultureInfoTemplateSelector : DataTemplateSelector
{
    public DataTemplate DefaultCultureTemplate { get; set; }

    public DataTemplate InvariantCultureTemplate { get; set; }

    public DataTemplate CurrentUICultureTemplate { get; set; }

    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is CultureInfo culture)
        {
            if (culture.Equals(CultureInfo.InvariantCulture))
            {
                return InvariantCultureTemplate;
            }
            else if (CultureInfo.CurrentUICulture is { Parent: { } parent } current &&
                (culture.Equals(current) ||
                (!parent.Equals(CultureInfo.InvariantCulture) && culture.Equals(parent))))
            {
                return CurrentUICultureTemplate;
            }

            return DefaultCultureTemplate;
        }

        return base.SelectTemplateCore(item, container);
    }
}
