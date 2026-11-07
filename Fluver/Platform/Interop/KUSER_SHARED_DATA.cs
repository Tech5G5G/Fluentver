using System.Runtime.CompilerServices;

namespace Windows.Wdk.System.SystemServices;

partial struct KUSER_SHARED_DATA
{
    private const nint KUSER_SHARED_DATA_ADDRESS = 0x7FFE0000;

    public static unsafe ref readonly KUSER_SHARED_DATA GetReference()
    {
        return ref Unsafe.AsRef<KUSER_SHARED_DATA>((void*)KUSER_SHARED_DATA_ADDRESS);
    }
}
