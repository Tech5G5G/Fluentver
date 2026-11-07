using Microsoft.UI.Xaml.Markup;
using Fluver.Platform;

namespace Fluver.UI;

public sealed partial class WindowsVersionCondition : IXamlCondition
{
    public bool Evaluate(string argument)
    {
        return argument switch
        {
            "11" => WindowsEnvironment.IsWindows11,
            "10" => !WindowsEnvironment.IsWindows11,
            "Insider" => WindowsEnvironment.IsInsider,
            "Release" => !WindowsEnvironment.IsInsider,
            _ => false
        };
    }
}
