using System.Runtime.InteropServices;

namespace Fluver.System.Interop.Com;

[ComImport]
[Guid("56FDF342-FD6D-11D0-958A-006097C9A090")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal partial interface ITaskbarList
{
    [PreserveSig] int HrInit();
    [PreserveSig] int AddTab(nint hwnd);
    [PreserveSig] int DeleteTab(nint hwnd);
    [PreserveSig] int ActivateTab(nint hwnd);
    [PreserveSig] int SetActiveAlt(nint hwnd);
}
