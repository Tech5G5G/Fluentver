using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

internal static partial class PInvoke
{
    [LibraryImport("user32.dll")]
    private static partial nint GetWindowLongW(nint hWnd, int nIndex);

    [LibraryImport("user32.dll")]
    private static partial nint GetWindowLongPtrW(nint hWnd, int nIndex);

    public static nint GetWindowLong(nint hWnd, int nIndex)
    {
        return Environment.Is64BitProcess ? GetWindowLongPtrW(hWnd, nIndex) : GetWindowLongW(hWnd, nIndex);
    }

    [LibraryImport("user32.dll")]
    private static partial nint SetWindowLongW(nint hWnd, int nIndex, nint dwNewLong);

    [LibraryImport("user32.dll")]
    private static partial nint SetWindowLongPtrW(nint hWnd, int nIndex, nint dwNewLong);

    public static nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong)
    {
        return Environment.Is64BitProcess ? SetWindowLongPtrW(hWnd, nIndex, dwNewLong) : SetWindowLongW(hWnd, nIndex, dwNewLong);
    }

    public const int GWL_HWNDPARENT = -8;
}
