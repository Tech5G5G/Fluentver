using System.Globalization;

namespace Fluver.Contracts;

public interface ICultureService
{
    CultureInfo OSCulture { get; }

    CultureInfo CurrentCulture { get; }

    CultureInfo RequestedCulture { get; set; }
    CultureInfo EvaluatedRequestedCulture { get; }

    IReadOnlyCollection<CultureInfo> SupportedCultures { get; }

    bool IsUICultureSupported(CultureInfo culture);
}
