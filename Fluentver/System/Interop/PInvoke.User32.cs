using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

internal static partial class PInvoke
{
    [LibraryImport("user32.dll")]
    public static partial nint GetSystemMenu(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

    [LibraryImport("user32.dll", EntryPoint = "InsertMenuW", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool InsertMenu(
        nint hMenu,
        uint uPosition,
        uint uFlags,
        nuint uIDNewItem,
        [MarshalAs(UnmanagedType.LPWStr), Optional] string lpNewItem);

    public const uint SC_CLOSE = 0xF060;

    public const uint MF_STRING = 0x00000000,
                      MF_BYCOMMAND = 0x00000000,
                      MF_SEPARATOR = 0x00000800;
}
