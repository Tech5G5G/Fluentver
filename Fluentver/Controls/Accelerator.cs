using Windows.System;

namespace Fluentver.Controls
{
    public sealed partial class Accelerator : NavigationViewItem
    {
        public static readonly DependencyProperty CtrlProperty =
            DependencyProperty.RegisterAttached("Ctrl",
                typeof(VirtualKey),
                typeof(Accelerator),
                new PropertyMetadata(false, UpdateCtrl));

        public static VirtualKey GetCtrl(NavigationViewItem target) => (VirtualKey)target.GetValue(CtrlProperty);

        public static void SetCtrl(NavigationViewItem target, VirtualKey value) => target.SetValue(CtrlProperty, value);

        private static void UpdateCtrl(DependencyObject sender, DependencyPropertyChangedEventArgs args) => (sender as NavigationViewItem).KeyboardAccelerators.Add(new()
        {
            Key = (VirtualKey)args.NewValue,
            Modifiers = VirtualKeyModifiers.Control
        });


        public static readonly DependencyProperty AltProperty =
            DependencyProperty.RegisterAttached("Alt",
                typeof(VirtualKey),
                typeof(Accelerator),
                new PropertyMetadata(false, UpdateAlt));

        public static VirtualKey GetAlt(NavigationViewItem target) => (VirtualKey)target.GetValue(AltProperty);

        public static void SetAlt(NavigationViewItem target, VirtualKey value) => target.SetValue(AltProperty, value);

        private static void UpdateAlt(DependencyObject sender, DependencyPropertyChangedEventArgs args) => (sender as NavigationViewItem).KeyboardAccelerators.Add(new()
        {
            Key = (VirtualKey)args.NewValue,
            Modifiers = VirtualKeyModifiers.Menu
        });


        public static readonly DependencyProperty ShiftProperty =
            DependencyProperty.RegisterAttached("Shift",
                typeof(VirtualKey),
                typeof(Accelerator),
                new PropertyMetadata(false, UpdateShift));

        public static VirtualKey GetShift(NavigationViewItem target) => (VirtualKey)target.GetValue(AltProperty);

        public static void SetShift(NavigationViewItem target, VirtualKey value) => target.SetValue(AltProperty, value);

        private static void UpdateShift(DependencyObject sender, DependencyPropertyChangedEventArgs args) => (sender as NavigationViewItem).KeyboardAccelerators.Add(new()
        {
            Key = (VirtualKey)args.NewValue,
            Modifiers = VirtualKeyModifiers.Shift
        });
    }
}
