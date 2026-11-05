using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

partial class PInvoke
{
    [LibraryImport("comctl32.dll")]
    public static partial nint DefSubclassProc(nint hWnd, uint uMsg, nuint wParam, nint lParam);
}
