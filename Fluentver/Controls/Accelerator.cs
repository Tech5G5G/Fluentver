using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Windows.System;
using Windows.UI.Core;

namespace Fluentver.Controls
{
    public sealed partial class Accelerator : FrameworkElement
    {
        public static readonly DependencyProperty CtrlProperty =
            DependencyProperty.RegisterAttached("Ctrl",
                typeof(VirtualKey),
                typeof(Accelerator),
                new PropertyMetadata(false, UpdateCtrl));

        public static VirtualKey GetCtrl(FrameworkElement target) => (VirtualKey)target.GetValue(CtrlProperty);

        public static void SetCtrl(FrameworkElement target, VirtualKey value) => target.SetValue(CtrlProperty, value);

        private static void UpdateCtrl(DependencyObject sender, DependencyPropertyChangedEventArgs args) => (sender as FrameworkElement).KeyboardAccelerators.Add(new()
        {
            Key = (VirtualKey)args.NewValue,
            Modifiers = VirtualKeyModifiers.Control
        });


        public static readonly DependencyProperty AltProperty =
            DependencyProperty.RegisterAttached("Alt",
                typeof(VirtualKey),
                typeof(Accelerator),
                new PropertyMetadata(false, UpdateAlt));

        public static VirtualKey GetAlt(FrameworkElement target) => (VirtualKey)target.GetValue(AltProperty);

        public static void SetAlt(FrameworkElement target, VirtualKey value) => target.SetValue(AltProperty, value);

        private static void UpdateAlt(DependencyObject sender, DependencyPropertyChangedEventArgs args) => (sender as FrameworkElement).KeyboardAccelerators.Add(new()
        {
            Key = (VirtualKey)args.NewValue,
            Modifiers = VirtualKeyModifiers.Menu
        });


        public static readonly DependencyProperty ShiftProperty =
            DependencyProperty.RegisterAttached("Shift",
                typeof(VirtualKey),
                typeof(Accelerator),
                new PropertyMetadata(false, UpdateShift));

        public static VirtualKey GetShift(FrameworkElement target) => (VirtualKey)target.GetValue(AltProperty);

        public static void SetShift(FrameworkElement target, VirtualKey value) => target.SetValue(AltProperty, value);

        private static void UpdateShift(DependencyObject sender, DependencyPropertyChangedEventArgs args) => (sender as FrameworkElement).KeyboardAccelerators.Add(new()
        {
            Key = (VirtualKey)args.NewValue,
            Modifiers = VirtualKeyModifiers.Shift
        });


        /// <summary>Allows for setting element accelerators to OEM <see cref="VirtualKey"/> codes.</summary>
        /// <param name="element">The <see cref="UIElement"/> to listen to keyboards events from.</param>
        /// <param name="virtualKey">The OEM <see cref="VirtualKey"/> code to check for.</param>
        /// <param name="modifier">The optional modifier to check for.</param>
        /// <param name="invoked">The <see cref="Action"/> to invoke when <paramref name="modifier"/> and <paramref name="virtualKey"/> are pressed.</param>
        public static void SetOEMAccelerator(UIElement element, int virtualKey, VirtualKey modifier, Action invoked)
        {
            element.KeyDown += (s, e) =>
            {
                if (e.Key == (VirtualKey)virtualKey &&
                InputKeyboardSource.GetKeyStateForCurrentThread(modifier).HasFlag(CoreVirtualKeyStates.Down))
                    invoked();
            };
        }
    }
}
