using System.Globalization;
using Windows.Globalization;
using Fluver.Contracts;

namespace Fluver.Services;

public sealed class CultureService : ICultureService
{
    public CultureInfo OSCulture { get; }

    public CultureInfo CurrentCulture => _currentCulture;

    public CultureInfo RequestedCulture
    {
        get
        {
            var language = ApplicationLanguages.PrimaryLanguageOverride;
            return _supportedCultures.TryGetValue(language, out var culture) ? culture : new(language);
        }
        set => ApplicationLanguages.PrimaryLanguageOverride = value.Name;
    }

    public CultureInfo EvaluatedRequestedCulture
    {
        get
        {
            var languages = ApplicationLanguages.Languages;
            return languages.Count > 0 && _supportedCultures.TryGetValue(languages[0], out var culture) ? culture : RequestedCulture;
        }
    }

    public IReadOnlyCollection<CultureInfo> SupportedCultures => _supportedCultures.Values;

    private readonly OrderedDictionary<string, CultureInfo> _supportedCultures;

    private readonly CultureInfo _currentCulture;

    public CultureService()
    {
        _supportedCultures = new(ApplicationLanguages.ManifestLanguages.Select(l => new KeyValuePair<string, CultureInfo>(l, new(l))));
        _supportedCultures.Insert(index: 0, string.Empty, CultureInfo.InvariantCulture);

        var language = ApplicationLanguages.PrimaryLanguageOverride;
        var currentCulture = _currentCulture = _supportedCultures.TryGetValue(language, out var culture) ? culture : new(language);

        var osCulture = CultureInfo.CurrentUICulture;
        OSCulture = IsUICultureSupported(osCulture) ? osCulture : CultureInfo.InvariantCulture;

        if (!currentCulture.Equals(CultureInfo.InvariantCulture))
        {
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture =
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = currentCulture;
        }
    }

    public bool IsUICultureSupported(CultureInfo culture)
    {
        return _supportedCultures.ContainsValue(culture) ||
            (!culture.Parent.Equals(CultureInfo.InvariantCulture) && _supportedCultures.ContainsValue(culture.Parent));
    }
}
