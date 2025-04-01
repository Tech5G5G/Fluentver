namespace Fluentver.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private readonly Array backdrops = Enum.GetValues<BackdropType>();

        public SettingsPage()
        {
            this.InitializeComponent();

            InitializeComboBox(startupPage, SettingValues.StartupPage);
            InitializeComboBox(backdrop, SettingValues.Backdrop);
        }

        private static void InitializeComboBox<T>(ComboBox box, EnumSetting<T> setting) where T : Enum
        {
            box.SelectedIndex = (int)(object)setting.Value;
            box.SelectionChanged += (s, e) =>
            {
                var value = (T)(object)box.SelectedIndex;
                if (setting.Value.CompareTo(value) != 0)
                    setting.Value = value;
            };
        }
    }
}
