using Microsoft.UI.Xaml.Markup;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Fluver.Helpers
{
    public static class StringsHelper
    {
        private static readonly ResourceLoader s_loader = new();
        private static readonly ResourceManager s_manager = new();

        public static string GetString(string id) => s_loader.GetString(id.Replace('.', '/'));

        public static string GetString(string id, string language)
        {
            var context = s_manager.CreateResourceContext();
            context.QualifierValues["Language"] = language;

            var map = s_manager.MainResourceMap.GetSubtree("Resources");
            return map.GetValue(id.Replace('.', '/'), context).ValueAsString;
        }
    }

    public sealed partial class StringResource : MarkupExtension
    {
        public string Id { get; set; }

        protected override object ProvideValue() => StringsHelper.GetString(Id);
    }
}
