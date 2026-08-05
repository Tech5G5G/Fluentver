using System.Globalization;
using System.Diagnostics.CodeAnalysis;

namespace Fluver.System.Interop;

public static class NativeInterop
{
    public static void AddMenuItem(nint hMenu, nuint id, string text)
    {
        PInvoke.InsertMenu(hMenu, PInvoke.SC_CLOSE, PInvoke.MF_BYCOMMAND | PInvoke.MF_STRING, id, text);
    }

    public static void AddMenuSeparator(nint hMenu)
    {
        PInvoke.InsertMenu(hMenu, PInvoke.SC_CLOSE, PInvoke.MF_BYCOMMAND | PInvoke.MF_SEPARATOR, uIDNewItem: 0);
    }
}
