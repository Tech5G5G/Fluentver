using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fluver.UI.Controls
{
    public sealed partial class IconButton : Button
    {
        public IconButton()
        {
            DefaultStyleKey = typeof(IconButton);
        }

        public IconElement Icon
        {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static DependencyProperty IconProperty { get; } =
            DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(IconButton), new PropertyMetadata(defaultValue: null));
    }
}