namespace Fluver.Globalization;

public interface ILanguageService
{
    string AppliedLanguage { get; }

    string Language { get; set; }

    IReadOnlyList<string> Languages { get; } 
}
