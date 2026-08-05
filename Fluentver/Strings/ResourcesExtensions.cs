using System.Globalization;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Fluver.Strings;

public static class ResourcesExtensions
{
    private static readonly ResourceManager s_manager = new();

    extension(Text)
    {
        public static string GetString(string key, CultureInfo culture)
        {
            return culture.Equals(CultureInfo.InvariantCulture) ? Text.GetString(key) : GetString(key, culture.Name);
        }

        public static string GetString(string key, string language)
        {
            var map = s_manager.MainResourceMap.GetSubtree("Resources");

            var context = s_manager.CreateResourceContext();
            context.QualifierValues["Language"] = language;

            return map.GetValue(key, context).ValueAsString;
        }
    }
}
