using Microsoft.UI.Xaml.Markup;
using Fluver.System;

namespace Fluver.UI;

public sealed partial class WindowsVersionCondition : IXamlCondition
{
    public bool Evaluate(string argument)
    {
        return argument switch
        {
            "11" => SystemInformation.IsWindows11,
            "10" => !SystemInformation.IsWindows11,
            "Insider" => SystemInformation.IsWindowsInsider,
            "Release" => !SystemInformation.IsWindowsInsider,
            _ => false
        };
    }
}
