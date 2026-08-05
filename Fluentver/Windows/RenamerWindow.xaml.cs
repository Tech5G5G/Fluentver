using Windows.System;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;
using Fluver.Helpers;
using Fluver.ViewModels;
using Fluver.System.Interop;

namespace Fluver.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RenamerWindow : UI.Controls.WindowEx
    {
        public RenamerWindowViewModel ViewModel { get; }

        public RenamerWindow(RenamerWindowViewModel viewModel, Window parentWindow)
        {
            InitializeComponent();

            SetTitleBar(TitleBar);
            ExtendsContentIntoTitleBar = true;
            Title = Text.GetString("RenamePC.Text");

            SystemBackdrop = SettingValues.Backdrop.Value.ToSystemBackdrop();
            PInvoke.SetWindowLong(this.GetWindowHandle(), PInvoke.GWL_HWNDPARENT, parentWindow.GetWindowHandle());

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsModal = true;
            }

            NameBox.Header = string.Format(Text.CurrentName, SystemHelper.SystemName);

            (ViewModel = viewModel).AddToWindowManager(renamerWindow: this);
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            NextButton.IsEnabled = SystemHelper.CheckNetBiosName(NameBox.Text, out var result);
            ErrorText.Text = result switch
            {
                NetBiosNameCheckResult.ExceedsMaxLength => Text.NameTooLong,
                NetBiosNameCheckResult.InvalidCharacter => Text.NameInvaildCharacters,
                _ => string.Empty
            };
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (FinishingScreen.Visibility == Visibility.Visible)
            {
                Close();
                return;
            }

            FinishingScreen.Visibility = Visibility.Visible;
            RenamingScreen.Visibility = Visibility.Collapsed;

            NextButton.Content = Text.Finish;
            NextButton.IsEnabled = CancelButton.IsEnabled = false;

            bool renamed = await SystemHelper.RenameSystem(NameBox.Text);

            LoadingRing.Visibility = Visibility.Collapsed;
            ClosingText.Visibility = Visibility.Visible;

            CancelButton.IsEnabled = !(NextButton.IsEnabled = renamed);
            if (!renamed)
            {
                ClosingText.Text = Text.ErrorPowerShell;
            }
        }

        private void Name_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (NextButton.IsEnabled && e.Key == VirtualKey.Enter)
            {
                NextButton_Click(sender, e);
            }
        }
    }
}
