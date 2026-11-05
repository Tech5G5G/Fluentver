using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;

namespace Fluver.UI.Controls
{
    public sealed partial class UserEntry : UserControl
    {
        public UserEntry()
        {
            InitializeComponent();
        }

        public ImageSource ProfilePicture
        {
            get => (ImageSource)GetValue(ProfilePictureProperty);
            set => SetValue(ProfilePictureProperty, value);
        }

        public static DependencyProperty ProfilePictureProperty { get; } =
            DependencyProperty.Register("ProfilePicture", typeof(ImageSource), typeof(UserEntry), new PropertyMetadata(defaultValue: null));

        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }

        public static DependencyProperty DisplayNameProperty { get; } =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(UserEntry), new PropertyMetadata(defaultValue: string.Empty));

        public string AccountName
        {
            get => (string)GetValue(AccountNameProperty);
            set => SetValue(AccountNameProperty, value);
        }

        public static DependencyProperty AccountNameProperty { get; } =
            DependencyProperty.Register("AccountName", typeof(string), typeof(UserEntry), new PropertyMetadata(defaultValue: string.Empty));
    }
}
