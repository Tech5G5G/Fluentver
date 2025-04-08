using Microsoft.UI.Composition.SystemBackdrops;

namespace Fluentver.Extensions
{
    public static class OtherExtensions
    {
        public static Microsoft.UI.Xaml.Media.SystemBackdrop ToSystemBackdrop(this BackdropType backdrop) => backdrop switch
        {
            BackdropType.Tabbed => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => new MicaBackdrop { Kind = MicaKind.Base },
        };
    }
}
