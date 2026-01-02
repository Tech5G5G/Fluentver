namespace Fluver.Controls
{
    public sealed partial class UserEntry : UserControl
    {
        public UserEntry()
        {
            this.InitializeComponent();
            this.DefaultStyleKey = typeof(UserEntry);
        }

        public ImageSource ProfilePicture
        {
            get { return (ImageSource)GetValue(ProfilePictureProperty); }
            set { SetValue(ProfilePictureProperty, value); }
        }

        public static readonly DependencyProperty ProfilePictureProperty = DependencyProperty.Register("ProfilePicture", typeof(ImageSource), typeof(UserEntry), new PropertyMetadata(null));

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(UserEntry), new PropertyMetadata(string.Empty));

        public string AccountName
        {
            get { return (string)GetValue(AccountNameProperty); }
            set { SetValue(AccountNameProperty, value); }
        }

        public static readonly DependencyProperty AccountNameProperty = DependencyProperty.Register("AccountName", typeof(string), typeof(UserEntry), new PropertyMetadata(string.Empty));
    }
}
