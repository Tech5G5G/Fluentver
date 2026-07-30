using Fluver.Helpers;
using Microsoft.UI.Xaml.Markup;

namespace Fluver.UI;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed partial class StringResource : MarkupExtension
{
    public string Id { get; set; }

    protected override object ProvideValue()
    {
        return StringsHelper.GetString(Id);
    }
}
