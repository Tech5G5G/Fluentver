using Windows.Globalization;
using Microsoft.Windows.AppLifecycle;

namespace Fluentver.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private readonly string currentLanguage = ApplicationLanguages.PrimaryLanguageOverride;

        private readonly static Dictionary<string, int> languages = new()
        {
            { string.Empty, 0 },
            { "en-US", 1 },
            { "el-GR", 2 },
            { "pl", 3 }
        };

        public SettingsPage()
        {
            this.InitializeComponent();

            InitializeComboBox(startupPage, SettingValues.StartupPage);
            InitializeComboBox(backdrop, SettingValues.Backdrop);

            InitializeLanguage();
            DetermineWIPItemsVisibility();
        }

        private void InitializeLanguage()
        {
            language.SelectedIndex = languages.TryGetValue(currentLanguage, out int index) ? index : 0;
            language.SelectionChanged += (s, e) =>
            {
                string language = ApplicationLanguages.PrimaryLanguageOverride = languages.FirstOrDefault(x => x.Value == this.language.SelectedIndex).Key;
                bool isRestartRequired = language != currentLanguage;

                language = language == string.Empty ? ApplicationLanguages.Languages[0] : language;
                restartAlert.IsOpen = isRestartRequired;
                languageExpander.Margin = isRestartRequired ? new() : new(0, 0, 0, -4);

                if (isRestartRequired)
                {
                    restartAlert.Title = StringsHelper.GetString("RestartAlert.Title", language);
                    restartAlert.Message = StringsHelper.GetString("RestartAlert.Message", language);
                    restartAlert.ActionButton.Content = StringsHelper.GetString("RestartButton.Content", language);
                }

                translatorButton.Content = StringsHelper.GetString("TranslationAuthor.Content", language);
                translatorButton.NavigateUri = new(StringsHelper.GetString("TranslationAuthorURL", language));
            };

            translatorButton.NavigateUri = new(StringsHelper.GetString("TranslationAuthorURL"));
        }

        private void DetermineWIPItemsVisibility()
        {
            if (VersionHelper.IsWindowsInsider)
                wipItem.Visibility = Visibility.Visible;
            else if (SettingValues.StartupPage == Pages.Insider)
                startupPage.SelectedIndex = 0;
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

        private void RestartButton_Click(object sender, RoutedEventArgs e) => AppInstance.Restart(string.Empty);
    }
}
