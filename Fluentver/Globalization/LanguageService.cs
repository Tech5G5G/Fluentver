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
}
