using Windows.Globalization;
using Microsoft.Windows.AppLifecycle;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Fluver.Helpers;
using Fluver.Options;

namespace Fluver.Pages
{
    public sealed partial class SettingsPage : Microsoft.UI.Xaml.Controls.Page
    {
        private static readonly string s_currentLanguage = ApplicationLanguages.PrimaryLanguageOverride;

        private static readonly List<string> s_languages =
        [
            string.Empty,
            "en-US",
            "de",
            "el",
            "pl",
            "ru",
            "zh-Hans-CN"
        ];

        public SettingsPage()
        {
            InitializeComponent();

            InitializeComboBox(StartupPageBox, SettingValues.StartupPage);
            InitializeComboBox(BackdropBox, SettingValues.Backdrop);

            InitializeLanguage();
            DetermineWIPItemsVisibility();
        }

        private void InitializeLanguage()
        {
            LanguageBox.SelectedIndex = Math.Max(s_languages.IndexOf(s_currentLanguage), 0);
            LanguageBox.SelectionChanged += (s, e) =>
            {
                string language = ApplicationLanguages.PrimaryLanguageOverride = s_languages[LanguageBox.SelectedIndex];
                bool isRestartRequired = language != s_currentLanguage;

                language = language == string.Empty ? ApplicationLanguages.Languages[0] : language;
                RestartBar.IsOpen = isRestartRequired;
                LanguageExpander.Margin = isRestartRequired ? new() : new(0, 0, 0, -4);

                if (isRestartRequired)
                {
                    RestartBar.Title = StringsHelper.GetString("RestartAlert.Title", language);
                    RestartBar.Message = StringsHelper.GetString("RestartAlert.Message", language);
                    RestartBar.ActionButton.Content = StringsHelper.GetString("RestartButton.Content", language);
                }

                TranslatorLink.Content = StringsHelper.GetString("TranslationAuthor.Content", language);
                TranslatorLink.NavigateUri = new(StringsHelper.GetString("TranslationAuthorURL", language));
            };

           TranslatorLink.NavigateUri = new(StringsHelper.GetString("TranslationAuthorURL"));
        }

        private void DetermineWIPItemsVisibility()
        {
            if (VersionHelper.IsWindowsInsider)
            {
                WipBoxItem.Visibility = Visibility.Visible;
            }
            else if (SettingValues.StartupPage.Value == Options.Page.Insider)
            {
                StartupPageBox.SelectedIndex = 0;
            }
        }

        private static void InitializeComboBox<T>(ComboBox box, EnumSetting<T> setting) where T : Enum
        {
            box.SelectedIndex = (int)(object)setting.Value;
            box.SelectionChanged += (s, e) =>
            {
                var value = (T)(object)box.SelectedIndex;
                if (setting.Value.CompareTo(value) != 0)
                {
                    setting.Value = value;
                }
            };
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            AppInstance.Restart(string.Empty);
        }
    }
}
