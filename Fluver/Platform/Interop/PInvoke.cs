using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Windows.Win32;

partial class PInvoke
{
    [LibraryImport("user32.dll")]
    private static partial nint GetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex);

    [LibraryImport("user32.dll")]
    private static partial nint SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong);

    [LibraryImport("user32.dll")]
    [SuppressGCTransition]
    internal static unsafe partial BOOL GetClientRect(HWND hWnd, RECT* lpRect);

    [LibraryImport("uxtheme.dll", EntryPoint = "#135")]
    internal static partial PREFERRED_APP_MODE SetPreferredAppMode(PREFERRED_APP_MODE mode);

    [LibraryImport("uxtheme.dll", EntryPoint = "#136")]
    internal static partial void FlushMenuThemes();

    [LibraryImport("winbrand.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial BOOL BrandingLoadString(
        [MarshalAs(UnmanagedType.LPWStr)] string lpNameSpace,
        uint uID,
        Span<char> lpBuffer,
        int cchBufferMax);

    internal static nint GetWindowLongPtr(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex)
    {
        return Environment.Is64BitProcess ? GetWindowLongPtrW(hWnd, nIndex) : GetWindowLong(hWnd, nIndex);
    }

    internal static nint SetWindowLongPtr(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong)
    {
        return Environment.Is64BitProcess ? SetWindowLongPtrW(hWnd, nIndex, dwNewLong) : SetWindowLong(hWnd, nIndex, (int)dwNewLong);
    }

    internal static unsafe BOOL GetClientRect(HWND hWnd, out RECT lpRect)
    {
        fixed (RECT* lpRectLocal = &lpRect)
        {
            return GetClientRect(hWnd, lpRectLocal);
        }
    }

    public const int WINDOWS_EDITION_PATTERN_LENGTH = 128;
}
