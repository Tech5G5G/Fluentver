using System.Runtime.InteropServices;

namespace Fluver.System.Interop;

partial class PInvoke
{
    [LibraryImport("uxtheme.dll", EntryPoint = "#135")]
    public static partial int SetPreferredAppMode(PreferredAppMode preferredAppMode);

    [LibraryImport("uxtheme.dll", EntryPoint = "#136")]
    public static partial void FlushMenuThemes();
}
