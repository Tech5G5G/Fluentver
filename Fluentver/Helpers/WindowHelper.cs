using System.Runtime.InteropServices;

namespace Fluver.Helpers
{
    public static class WindowHelper
    {
        #region PInvoke

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetPreferredAppMode(ElementTheme preferredAppMode);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        public static void SetAppTheme(ElementTheme theme) => _ = SetPreferredAppMode(theme);

        public static void ActivateWindow(IntPtr hWnd) => SetForegroundWindow(hWnd);
    }
}
