using Microsoft.UI.Xaml.Markup;

namespace Fluver.Strings;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed partial class StringResource : MarkupExtension
{
    public string Key { get; set; }

    protected override object ProvideValue()
    {
        return Text.GetString(Key);
    }
}
