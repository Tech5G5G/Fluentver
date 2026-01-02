using Windows.System;
using Windows.UI.Core;

namespace Fluentver.Controls
{
    public static class Accelerator
    {
        #region Key Property

        public static VirtualKey GetKey(UIElement target) => (VirtualKey)target.GetValue(KeyProperty);

        public static void SetKey(UIElement target, VirtualKey value) => target.SetValue(KeyProperty, value);

        public static DependencyProperty KeyProperty { get; } =
            DependencyProperty.RegisterAttached("Key", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, KeyPropertyChanged));

        private static void KeyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            (sender as UIElement).KeyboardAccelerators.Add(new() { Key = (VirtualKey)args.NewValue });

        #endregion

        #region Ctrl Property

        public static VirtualKey GetCtrl(UIElement target) => (VirtualKey)target.GetValue(CtrlProperty);

        public static void SetCtrl(UIElement target, VirtualKey value) => target.SetValue(CtrlProperty, value);

        public static DependencyProperty CtrlProperty { get; } =
            DependencyProperty.RegisterAttached("Ctrl", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, CtrlPropertyChanged));

        private static void CtrlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            (sender as UIElement).KeyboardAccelerators.Add(new()
            {
                Key = (VirtualKey)args.NewValue,
                Modifiers = VirtualKeyModifiers.Control
            });

        #endregion

        #region Alt Property

        public static VirtualKey GetAlt(UIElement target) => (VirtualKey)target.GetValue(AltProperty);

        public static void SetAlt(UIElement target, VirtualKey value) => target.SetValue(AltProperty, value);

        public static DependencyProperty AltProperty { get; } =
            DependencyProperty.RegisterAttached("Alt", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, AltPropertyChanged));

        private static void AltPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            (sender as UIElement).KeyboardAccelerators.Add(new()
            {
                Key = (VirtualKey)args.NewValue,
                Modifiers = VirtualKeyModifiers.Menu
            });

        #endregion

        #region Shift Property

        public static VirtualKey GetShift(UIElement target) => (VirtualKey)target.GetValue(ShiftProperty);

        public static void SetShift(UIElement target, VirtualKey value) => target.SetValue(ShiftProperty, value);

        public static DependencyProperty ShiftProperty { get; } =
            DependencyProperty.RegisterAttached("Shift", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, ShiftPropertyChanged));

        private static void ShiftPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            (sender as UIElement).KeyboardAccelerators.Add(new()
            {
                Key = (VirtualKey)args.NewValue,
                Modifiers = VirtualKeyModifiers.Shift
            });

        #endregion

        #region OEM

        /// <summary>Creates keyboard accelerators that use OEM <see cref="VirtualKey"/> codes.</summary>
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

        #endregion
    }
}
