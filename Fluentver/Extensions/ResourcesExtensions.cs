using System.Globalization;
using Microsoft.Windows.ApplicationModel.Resources;
using Fluver.Strings;

namespace Fluver.Extensions;

public static class ResourcesExtensions
{
    private static readonly ResourceManager s_manager = new();

    private static readonly ResourceMap s_map = s_manager.MainResourceMap.TryGetSubtree("Resources");

    extension(IResources resources)
    {
        public string GetString(string key, CultureInfo culture)
        {
            return culture.Equals(CultureInfo.InvariantCulture) ? resources.GetString(key) : resources.GetString(key, culture.Name);
        }

        public string GetString(string key, string language)
        {
            if (s_map is null)
            {
                return resources.GetString(key);
            }

            var context = s_manager.CreateResourceContext();
            context.QualifierValues["Language"] = language;

            return s_map.GetValue(key, context).ValueAsString;
        }
    }
}
