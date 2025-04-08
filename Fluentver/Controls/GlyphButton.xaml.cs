namespace Fluentver.Controls
{
    public sealed partial class GlyphButton : UserControl
    {
        public GlyphButton()
        {
            this.InitializeComponent();
            this.DefaultStyleKey = typeof(GlyphButton);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessName is not null)
                Process.Start(new ProcessStartInfo(ProcessName) { UseShellExecute = true });

            Click?.Invoke(sender, e);
        }

        public event RoutedEventHandler Click;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GlyphButton), new PropertyMetadata(string.Empty));

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(GlyphButton), new PropertyMetadata(string.Empty));

        public string ProcessName { get; set; }
    }
}
