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

            Theme = ActualTheme;
            ActualThemeChanged += OnActualThemeChanged;
        }

        private ElementTheme Theme
        {
            get => (ElementTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        private static DependencyProperty ThemeProperty { get; } =
            DependencyProperty.Register(nameof(Theme), typeof(ElementTheme), typeof(FluverTitleBar), new(defaultValue: ElementTheme.Default));

        private bool AnimateIcons
        {
            get => (bool)GetValue(AnimateIconsProperty);
            set => SetValue(AnimateIconsProperty, value);
        }

        private static DependencyProperty AnimateIconsProperty { get; } =
            DependencyProperty.Register(nameof(AnimateIcons), typeof(bool), typeof(FluverTitleBar), new(defaultValue: true));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static DependencyProperty TitleProperty { get; } =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(FluverTitleBar), new(defaultValue: string.Empty));

        public FluverWindow Window
        {
            get => (FluverWindow)GetValue(WindowProperty);
            set => SetValue(WindowProperty, value);
        }

        public static DependencyProperty WindowProperty { get; } =
            DependencyProperty.Register(nameof(Window), typeof(FluverWindow), typeof(FluverTitleBar), new(defaultValue: null, OnWindowChanged));

        private static void OnWindowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FluverTitleBar titleBar)
            {
                return;
            }

            if (e.OldValue is FluverWindow oldWindow && oldWindow.Header.DragElement == titleBar.DragRegion)
        {
                oldWindow.Header.DragElement = null;
        }

            if (e.NewValue is FluverWindow window)
            {
                window.Header.DragElement = titleBar.DragRegion;
            }
        }

        public bool IsSettingsOpen
        {
            get => (bool)GetValue(IsSettingsOpenProperty);
            set => SetValue(IsSettingsOpenProperty, value);
        }

        public static DependencyProperty IsSettingsOpenProperty { get; } =
            DependencyProperty.Register(
                nameof(IsSettingsOpen),
                typeof(bool),
                typeof(FluverTitleBar),
                new(defaultValue: false, OnIsSettingsOpenChanged));

        private static void OnIsSettingsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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

        private void OnActualThemeChanged(FrameworkElement sender, object e)
        {
            Theme = ActualTheme;
        }

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
                Key = (VirtualKey)0xBC // VK_OEM_COMMA
            });
        }

        private void OnSettingsButtonUnloaded(object sender, RoutedEventArgs e)
        {
            SettingsButton.KeyboardAccelerators.Clear();
        }

        private void OnSettingsIconLoaded(object sender, RoutedEventArgs e)
        {
            SettingsIcon.Loaded -= OnSettingsIconLoaded;

            // Detect whether the settings AnimatedIcon displayed its visual source
            // If not, use the same fallback icon we use for high contrast mode
            if (VisualTreeHelper.GetChildrenCount(SettingsIcon) == 0 ||
                VisualTreeHelper.GetChild(SettingsIcon, childIndex: 0) is not Panel rootPanel ||
                ElementCompositionPreview.GetElementChildVisual(rootPanel) is null)
        {
                AnimateIcons = false;
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Window?.Close();
        }
    }
}
