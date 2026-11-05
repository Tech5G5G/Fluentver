using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Fluver.Platform.Interop;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct KUSER_SHARED_DATA
{
    private const nint KUSER_SHARED_DATA_ADDRESS = 0x7FFE0000;

    [FieldOffset(712)] public readonly long SystemExpirationDate;

    public static unsafe ref readonly KUSER_SHARED_DATA GetReference()
    {
        return ref Unsafe.AsRef<KUSER_SHARED_DATA>((void*)KUSER_SHARED_DATA_ADDRESS);
    }
}
