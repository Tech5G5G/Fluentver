using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int left;
    public int top;
    public int right;
    public int bottom;

    public readonly int Width => right - left;
    public readonly int Height => bottom - top;

    public readonly bool IsEmpty => left == 0 && top == 0 && right == 0 && bottom == 0;
}
