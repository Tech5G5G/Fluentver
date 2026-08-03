using System.Globalization;
using Windows.Globalization;

namespace Fluver.Globalization;

public sealed class LanguageService : ILanguageService
{
    public string AppliedLanguage { get; } = ApplicationLanguages.PrimaryLanguageOverride;

    public string Language
    {
        get => ApplicationLanguages.PrimaryLanguageOverride;
        set => ApplicationLanguages.PrimaryLanguageOverride = value;
    }

    public IReadOnlyList<string> Languages => ApplicationLanguages.Languages;

    public LanguageService()
    {
        if (AppliedLanguage != string.Empty)
        {
            CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture =
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new(AppliedLanguage);
        }
    }
}
