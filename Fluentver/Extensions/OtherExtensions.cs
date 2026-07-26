using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Composition.SystemBackdrops;
using Fluver.Options;

namespace Fluver.Extensions
{
    public static class OtherExtensions
    {
        public static SystemBackdrop ToSystemBackdrop(this BackdropType backdrop) => backdrop switch
        {
            BackdropType.Tabbed => new MicaBackdrop { Kind = MicaKind.BaseAlt },
            BackdropType.Acrylic => new DesktopAcrylicBackdrop(),
            _ => new MicaBackdrop { Kind = MicaKind.Base },
        };
    }
}
