namespace Fluver.System.Interop;

internal static partial class PInvoke
{
    public static ushort LOWORD(uint value) => unchecked((ushort)value);

    public static ushort HIWORD(uint value) => unchecked((ushort)(value >> 16));

    public const int S_OK = 0x00000000;

    public const nint HWND_BOTTOM = 1;
}
