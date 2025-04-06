using Microsoft.UI.Xaml.Markup;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Fluentver.Helpers
{
    public static class StringsHelper
    {
        readonly static ResourceLoader loader = new();

        public static string GetString(string id) => loader.GetString(id.Replace('.', '/'));
    }

    public partial class StringResource : MarkupExtension
    {
        public string Id { get; set; }

        protected override object ProvideValue() => StringsHelper.GetString(Id);
    }
}
