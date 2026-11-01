using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Fluver.Helpers;
using Fluver.ViewModels;
using Fluver.UI.Controls;

namespace Fluver.Windows
{
    public sealed partial class RenamerWindow : FluverWindow
    {
        public RenamerWindowViewModel ViewModel { get; }

        public RenamerWindow(RenamerWindowViewModel viewModel)
        {
            InitializeComponent();

            Title = viewModel.Resources.GetString("RenamePC/Text");
            NameBox.Header = viewModel.Resources.CurrentName(SystemHelper.SystemName);

            Header.OverlapsContent = true;
            Header.DragElement = TitleBar;

            ViewModel = viewModel;
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            NextButton.IsEnabled = SystemHelper.CheckNetBiosName(NameBox.Text, out var result);
            ErrorText.Text = result switch
            {
                NetBiosNameCheckResult.ExceedsMaxLength => ViewModel.Resources.NameTooLong,
                NetBiosNameCheckResult.InvalidCharacter => ViewModel.Resources.NameInvaildCharacters,
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

            NextButton.Content = ViewModel.Resources.Finish;
            NextButton.IsEnabled = CancelButton.IsEnabled = false;

            bool renamed = await SystemHelper.RenameSystem(NameBox.Text);

            LoadingRing.Visibility = Visibility.Collapsed;
            ClosingText.Visibility = Visibility.Visible;

            CancelButton.IsEnabled = !(NextButton.IsEnabled = renamed);
            if (!renamed)
            {
                ClosingText.Text = ViewModel.Resources.ErrorPowerShell;
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
