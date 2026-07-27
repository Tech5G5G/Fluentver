using Windows.System;
using Microsoft.UI.Xaml;

namespace Fluver.UI
{
    public static class Accelerator
    {
        #region Key Property

        public static VirtualKey GetKey(UIElement target)
        {
            return (VirtualKey)target.GetValue(KeyProperty);
        }

        public static void SetKey(UIElement target, VirtualKey value)
        {
            target.SetValue(KeyProperty, value);
        }

        public static DependencyProperty KeyProperty { get; } =
            DependencyProperty.RegisterAttached("Key", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, KeyPropertyChanged));

        private static void KeyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as UIElement).KeyboardAccelerators.Add(new() { Key = (VirtualKey)e.NewValue });
        }

        #endregion

        #region Ctrl Property

        public static VirtualKey GetCtrl(UIElement target)
        {
            return (VirtualKey)target.GetValue(CtrlProperty);
        }

        public static void SetCtrl(UIElement target, VirtualKey value)
        {
            target.SetValue(CtrlProperty, value);
        }

        public static DependencyProperty CtrlProperty { get; } =
            DependencyProperty.RegisterAttached("Ctrl", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, CtrlPropertyChanged));

        private static void CtrlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as UIElement).KeyboardAccelerators.Add(new()
            {
                Key = (VirtualKey)e.NewValue,
                Modifiers = VirtualKeyModifiers.Control
            });
        }

        #endregion

        #region Alt Property

        public static VirtualKey GetAlt(UIElement target)
        {
            return (VirtualKey)target.GetValue(AltProperty);
        }

        public static void SetAlt(UIElement target, VirtualKey value)
        {
            target.SetValue(AltProperty, value);
        }

        public static DependencyProperty AltProperty { get; } =
            DependencyProperty.RegisterAttached("Alt", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, AltPropertyChanged));

        private static void AltPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as UIElement).KeyboardAccelerators.Add(new()
            {
                Key = (VirtualKey)e.NewValue,
                Modifiers = VirtualKeyModifiers.Menu
            });
        }

        #endregion

        #region Shift Property

        public static VirtualKey GetShift(UIElement target)
        {
            return (VirtualKey)target.GetValue(ShiftProperty);
        }

        public static void SetShift(UIElement target, VirtualKey value)
        {
            target.SetValue(ShiftProperty, value);
        }

        public static DependencyProperty ShiftProperty { get; } =
            DependencyProperty.RegisterAttached("Shift", typeof(VirtualKey), typeof(Accelerator), new PropertyMetadata(VirtualKey.None, ShiftPropertyChanged));

        private static void ShiftPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as UIElement).KeyboardAccelerators.Add(new()
            {
                Key = (VirtualKey)e.NewValue,
                Modifiers = VirtualKeyModifiers.Shift
            });
        }

        #endregion
    }
}
