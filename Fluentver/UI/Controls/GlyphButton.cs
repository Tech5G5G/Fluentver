using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Fluver.UI.Controls
{
    public sealed partial class GlyphButton : ButtonBase
    {
        public GlyphButton()
        {
            DefaultStyleKey = typeof(GlyphButton);
        }

        public static DependencyProperty GlyphSourceProperty { get; } =
            DependencyProperty.Register(nameof(GlyphSource), typeof(IconSource), typeof(GlyphButton), new PropertyMetadata(defaultValue: null));

        public IconSource GlyphSource
        {
            get => (IconSource)GetValue(GlyphSourceProperty);
            set => SetValue(GlyphSourceProperty, value);
        }
    }
}