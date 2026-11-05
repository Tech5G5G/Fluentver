using System.Globalization;
using Windows.Globalization;

namespace Fluver.Globalization;

public sealed class CultureService : ICultureService
{
    private static readonly OrderedDictionary<string, CultureInfo> s_supportedCultures;

    private static readonly CultureInfo s_currentCulture;

    static CultureService()
    {
        s_supportedCultures = new(ApplicationLanguages.ManifestLanguages.Select(l => new KeyValuePair<string, CultureInfo>(l, new(l))));
        s_supportedCultures.Insert(index: 0, string.Empty, CultureInfo.InvariantCulture);

        var language = ApplicationLanguages.PrimaryLanguageOverride;
        s_currentCulture = s_supportedCultures.TryGetValue(language, out var culture) ? culture : new(language);
    }

    public CultureInfo OSCulture { get; }

    public CultureInfo CurrentCulture => s_currentCulture;

    public CultureInfo RequestedCulture
    {
        get
        {
            var language = ApplicationLanguages.PrimaryLanguageOverride;
            return s_supportedCultures.TryGetValue(language, out var culture) ? culture : new(language);
        }
        set => ApplicationLanguages.PrimaryLanguageOverride = value.Name;
    }

    public CultureInfo EvaluatedRequestedCulture
    {
        get
        {
            var languages = ApplicationLanguages.Languages;
            return languages.Count > 0 && s_supportedCultures.TryGetValue(languages[0], out var culture) ? culture : RequestedCulture;
        }
    }

    public IReadOnlyCollection<CultureInfo> SupportedCultures => s_supportedCultures.Values;

    public CultureService()
    {
        var osCulture = CultureInfo.CurrentUICulture;
        OSCulture = IsUICultureSupported(osCulture) ? osCulture : CultureInfo.InvariantCulture;

        if (!s_currentCulture.Equals(CultureInfo.InvariantCulture))
        {
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture =
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = s_currentCulture;
        }
    }

    public bool IsUICultureSupported(CultureInfo culture)
    {
        return s_supportedCultures.ContainsValue(culture) ||
            (!culture.Parent.Equals(CultureInfo.InvariantCulture) && s_supportedCultures.ContainsValue(culture.Parent));
    }
}
