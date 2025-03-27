namespace Fluentver.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            startupPage.SelectedIndex = (int)SettingValues.StartupPage.Value;
        }

        private void StartupPage_SelectionChanged(object sender, SelectionChangedEventArgs e) => SettingValues.StartupPage.Value = (Pages)startupPage.SelectedIndex;
    }
}
