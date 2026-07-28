using Fluver.Helpers;
using Microsoft.UI.Xaml.Markup;

namespace Fluver.UI;

public sealed partial class WindowsVersionCondition : IXamlCondition
{
    public bool Evaluate(string argument)
    {
        return argument switch
        {
            "11" => VersionHelper.IsWindows11,
            "10" => !VersionHelper.IsWindows11,
            "Insider" => VersionHelper.IsWindowsInsider,
            "Release" => !VersionHelper.IsWindowsInsider,
            _ => false
        };
    }
}
