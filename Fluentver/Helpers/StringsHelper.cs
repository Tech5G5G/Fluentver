using Microsoft.UI.Xaml.Markup;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Fluver.Helpers
{
    public static class StringsHelper
    {
        readonly static ResourceLoader loader = new();
        readonly static ResourceManager manager = new();

        public static string GetString(string id) => loader.GetString(id.Replace('.', '/'));

        public static string GetString(string id, string language)
        {
            var context = manager.CreateResourceContext();
            context.QualifierValues["Language"] = language;

            var map = manager.MainResourceMap.GetSubtree("Resources");
            return map.GetValue(id.Replace('.', '/'), context).ValueAsString;
        }
    }

    public partial class StringResource : MarkupExtension
    {
        public string Id { get; set; }

        protected override object ProvideValue() => StringsHelper.GetString(Id);
    }
}
