using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

partial class PInvoke
{
    [LibraryImport("user32.dll", EntryPoint = "FindWindowExW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint FindWindowEx(
        [Optional] nint hWndParent,
        [Optional] nint hWndChildAfter,
        [Optional, MarshalAs(UnmanagedType.LPWStr)] string lpszClass,
        [Optional, MarshalAs(UnmanagedType.LPWStr)] string lpszWindow);

    [LibraryImport("user32.dll", EntryPoint = "SendMessageW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint SendMessage(nint hWnd, uint Msg, nuint wParam, nint lParam);

    [LibraryImport("user32.dll", EntryPoint = "PostMessageW", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PostMessage([Optional] nint hWnd, uint Msg, nuint wParam, nint lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnableWindow(nint hWnd, [MarshalAs(UnmanagedType.Bool)] bool bEnable);

    [LibraryImport("user32.dll")]
    public static partial nint SetActiveWindow(nint hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(
        nint hWnd,
        [Optional] nint hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowTextW", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowText(nint hWnd, [MarshalAs(UnmanagedType.LPWStr), Optional] string lpString);

    [LibraryImport("user32.dll")]
    [SuppressGCTransition]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetClientRect(nint hWnd, out RECT lpRect);

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

    public const uint WM_DESTROY = 0x0002,
                      WM_MOVE = 0x0003,
                      WM_SIZE = 0x0005,
                      WM_ACTIVATE = 0x0006,
                      WM_CLOSE = 0x0010,
                      WM_SHOWWINDOW = 0x0018,
                      WM_WINDOWPOSCHANGING = 0x0046,
                      WM_WINDOWPOSCHANGED = 0x0047,
                      WM_CANCELMODE = 0x001F,
                      WM_GETMINMAXINFO = 0x0024,
                      WM_NCCALCSIZE = 0x0083,
                      WM_NCLBUTTONDBLCLK = 0x00A3,
                      WM_SYSCOMMAND = 0x0112,
                      WM_DPICHANGED = 0x02E0;

    public const uint WA_INACTIVE = 0;

    public const uint SIZE_MINIMIZED = 1,
                      SIZE_MAXSHOW = 3;

    public const uint SWP_NOSIZE = 0x0001,
                      SWP_NOMOVE = 0x0002,
                      SWP_NOZORDER = 0x0004,
                      SWP_NOACTIVATE = 0x0010,
                      SWP_FRAMECHANGED = 0x0020,
                      SWP_SHOWWINDOW = 0x0040;

    public const uint SC_MOVE = 0xF010,
                      SC_CLOSE = 0xF060;

    public const uint MF_STRING = 0x00000000,
                      MF_BYCOMMAND = 0x00000000,
                      MF_SEPARATOR = 0x00000800;
}
