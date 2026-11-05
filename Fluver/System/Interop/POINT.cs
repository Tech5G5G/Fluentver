using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int x;
    public int y;
}
