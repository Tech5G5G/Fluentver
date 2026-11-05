using System.Runtime.InteropServices;

namespace Fluver.Platform.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int x;
    public int y;
}
