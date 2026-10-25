using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Fluver.Options;
using Fluver.Strings;
using Fluver.ViewModels;
using Fluver.UI.Controls;

namespace Fluver.Views
{
    public sealed partial class SettingsPage : ViewModelPage
    {
        public SettingsPageViewModel ViewModel { get; } = App.Services.GetRequiredService<SettingsPageViewModel>();

        protected override PageViewModel PageViewModel => ViewModel;

        public SettingsPage()
        {
            InitializeComponent();
            TranslatorLink.NavigateUri = new(Text.TranslationAuthorURL);
        }

        private void OnLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var culture = ViewModel.EvaluatedRequestedCulture;

            TranslatorLink.Content = Text.GetString("TranslatorLink/Content", culture);
            TranslatorLink.NavigateUri = new(Text.GetString(nameof(Text.TranslationAuthorURL), culture));

            RestartBar.Title = Text.GetString("RestartBar/Title", culture);
            RestartBar.Message = Text.GetString("RestartBar/Message", culture);
            RestartBar.ActionButton.Content = Text.GetString("RestartButton/Content", culture);
        }

        private int FluverPageToIndex(FluverPage page)
        {
            return (int)page + 1;
        }

        private void SetStartupPageSetting(int index)
        {
            ViewModel.StartupPageSetting.Value = (FluverPage)(index - 1);
        }

        private int BackdropTypeToIndex(BackdropType backdrop)
        {
            return (int)backdrop;
        }

        private void SetBackdropSetting(int index)
        {
            ViewModel.BackdropSetting.Value = (BackdropType)index;
        }
    }
}
