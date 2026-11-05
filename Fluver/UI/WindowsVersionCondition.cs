using Microsoft.UI.Xaml.Markup;
using Fluver.Platform;

namespace Fluver.UI;

public sealed partial class WindowsVersionCondition : IXamlCondition
{
    public bool Evaluate(string argument)
    {
        return argument switch
        {
            "11" => WindowsInfo.IsWindows11,
            "10" => !WindowsInfo.IsWindows11,
            "Insider" => WindowsInfo.IsInsider,
            "Release" => !WindowsInfo.IsInsider,
            _ => false
        };
    }
}
