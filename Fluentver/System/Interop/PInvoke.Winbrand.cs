using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

partial class PInvoke
{
    [LibraryImport("winbrand.dll", StringMarshalling = StringMarshalling.Utf16)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool BrandingLoadString(
        [MarshalAs(UnmanagedType.LPWStr)] string lpNameSpace,
        uint uID,
        Span<char> lpBuffer,
        int cchBufferMax);

    public const int WINDOWS_EDITION_PATTERN_LENGTH = 128;
}
