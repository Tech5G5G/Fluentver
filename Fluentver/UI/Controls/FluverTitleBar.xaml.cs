using System.Windows.Input;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;

namespace Fluver.UI.Controls
{
    public sealed partial class FluverTitleBar : UserControl
    {
        public event RoutedEventHandler SettingsButtonClick;

        public FluverTitleBar()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static DependencyProperty TitleProperty { get; } =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(FluverTitleBar), new(defaultValue: string.Empty));

        public bool IsSettingsButtonActive
        {
            get => (bool)GetValue(IsSettingsButtonActiveProperty);
            set => SetValue(IsSettingsButtonActiveProperty, value);
        }

        public static DependencyProperty IsSettingsButtonActiveProperty { get; } =
            DependencyProperty.Register(nameof(IsSettingsButtonActive), typeof(bool), typeof(FluverTitleBar), new(defaultValue: true));

        public bool IsBackButtonVisible
        {
            get => (bool)GetValue(IsBackButtonVisibleProperty);
            set => SetValue(IsBackButtonVisibleProperty, value);
        }

        public static DependencyProperty IsBackButtonVisibleProperty { get; } =
            DependencyProperty.Register(
                nameof(IsBackButtonVisible),
                typeof(bool),
                typeof(FluverTitleBar),
                new(defaultValue: false, OnIsBackButtonVisibleChanged));

        private static void OnIsBackButtonVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FluverTitleBar { SettingsIcon: { } icon } || AnimatedIcon.GetState(icon) == "Pressed")
            {
                return;
            }

            AnimatedIcon.SetState(icon, "Pressed");
            icon.UpdateLayout();
            AnimatedIcon.SetState(
                icon,
                // AnimatedSettingsVisualSource looks better transitioning to PointerOver fsr
                icon.Source is AnimatedSettingsVisualSource ? "PointerOver" : "Normal");
        }

        public ICommand SettingsCommand
        {
            get => (ICommand)GetValue(SettingsCommandProperty);
            set => SetValue(SettingsCommandProperty, value);
        }

        public static DependencyProperty SettingsCommandProperty { get; } =
            DependencyProperty.Register(nameof(SettingsCommand), typeof(ICommand), typeof(FluverTitleBar), new(defaultValue: null));

        public object SettingsCommandParameter
        {
            get => GetValue(SettingsCommandParameterProperty);
            set => SetValue(SettingsCommandParameterProperty, value);
        }

        public static DependencyProperty SettingsCommandParameterProperty { get; } =
            DependencyProperty.Register(nameof(SettingsCommandParameter), typeof(object), typeof(FluverTitleBar), new(defaultValue: null));

        private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsButtonClick?.Invoke(sender: this, e);

            if (SettingsCommand is not { } command)
            {
                return;
            }

            var parameter = SettingsCommandParameter;

            if (command.CanExecute(parameter))
            {
                SettingsCommand.Execute(parameter);
            }
        }

        private void OnSettingsButtonLoaded(object sender, RoutedEventArgs e)
        {
            SettingsButton.KeyboardAccelerators.Add(new()
            {
                Modifiers = VirtualKeyModifiers.Control,
                Key = (VirtualKey)188 // VK_OEM_COMMA
            });
        }

        private void OnSettingsButtonUnloaded(object sender, RoutedEventArgs e)
        {
            SettingsButton.KeyboardAccelerators.Clear();
        }

        private bool InvertBool(bool value)
        {
            return !value;
        }
    }
}
