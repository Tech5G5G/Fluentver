using Windows.UI;
using Microsoft.Win32;
using Microsoft.UI.Xaml;
using Fluver.Extensions;
using static Fluver.System.Interop.PInvoke;

namespace Fluver.UI.Controls;

partial class FluverWindow
{
    public static WindowAppearance Appearance { get; } = new();

    private static unsafe void UpdateAnimations()
    {
        int clientAnimation;
        var animations =
            SystemParametersInfo(uiAction: SPI_GETCLIENTAREAANIMATION, uiParam: 0, (nint)(&clientAnimation), fWinIni: 0) &&
            clientAnimation != 0;

        Appearance.SetValue(WindowAppearance.AreAnimationsEnabledProperty, animations);
    }

    private static void UpdateAccentColor()
    {
        Registry.TryGetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\DWM", "ColorPrevalence", out int colorPrevalence);

        // AccentColorMenu is more reliable than DWM\AccentColor fsr
        Registry.TryGetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentColorMenu", out int accentColor);
        var (accentR, accentG, accentB) = Rgb(unchecked((uint)accentColor));

        var appearance = Appearance;
        appearance.SetValue(WindowAppearance.IsAccentedProperty, colorPrevalence != 0);

        var accentTheme = IsLight(accentR, accentG, accentB) ? ElementTheme.Light : ElementTheme.Dark;
        if (accentTheme != appearance.AccentTheme)
        {
            appearance.SetValue(WindowAppearance.AccentThemeProperty, accentTheme);
        }

        var color = Color.FromArgb(0xFF, accentR, accentG, accentB);
        if (color != appearance.AccentColor)
        {
            appearance.SetValue(WindowAppearance.AccentColorProperty, color);
        }

        static (byte R, byte G, byte B) Rgb(uint bgr)
        {
            return (
                R: (byte)(bgr & 0xFF),
                G: (byte)((bgr >> 8) & 0xFF),
                B: (byte)((bgr >> 16) & 0xFF));
        }

        static bool IsLight(byte r, byte g, byte b)
        {
            return ((5 * g) + (2 * r) + b) > (8 * 128);
        }
    }
}

public sealed class WindowAppearance : DependencyObject
{
    public bool AreAnimationsEnabled => (bool)GetValue(AreAnimationsEnabledProperty);

    public static DependencyProperty AreAnimationsEnabledProperty { get; } =
        DependencyProperty.Register(nameof(AreAnimationsEnabled), typeof(bool), typeof(WindowAppearance), new(defaultValue: false));

    public bool IsAccented => (bool)GetValue(IsAccentedProperty);

    public static DependencyProperty IsAccentedProperty { get; } =
        DependencyProperty.Register(nameof(IsAccented), typeof(bool), typeof(WindowAppearance), new(defaultValue: false));

    public Color AccentColor => (Color)GetValue(AccentColorProperty);

    public static DependencyProperty AccentColorProperty { get; } =
        DependencyProperty.Register(nameof(AccentColor), typeof(Color), typeof(WindowAppearance), new(defaultValue: default(Color)));

    public ElementTheme AccentTheme => (ElementTheme)GetValue(AccentThemeProperty);

    public static DependencyProperty AccentThemeProperty { get; } =
        DependencyProperty.Register(nameof(AccentTheme), typeof(ElementTheme), typeof(WindowAppearance), new(defaultValue: ElementTheme.Light));
}
