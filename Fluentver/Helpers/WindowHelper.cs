using System.Runtime.InteropServices;

namespace Fluentver.Helpers
{
    public static class WindowHelper
    {
        #region PInvoke

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetPreferredAppMode(ElementTheme preferredAppMode);

        #endregion

        public static void SetAppTheme(ElementTheme theme) => _ = SetPreferredAppMode(theme);
    }
}
