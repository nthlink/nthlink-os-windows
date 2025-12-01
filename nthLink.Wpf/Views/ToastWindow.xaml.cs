using System.Windows;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ToastWindow.xaml
    /// </summary>
    public partial class ToastWindow : Window
    {
        public object? ToastContent
        {
            get { return (object?)GetValue(ToastContentProperty); }
            set { SetValue(ToastContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToastContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToastContentProperty =
            DependencyProperty.Register(nameof(ToastContent), typeof(object), typeof(ToastWindow));

        public ToastWindow()
        {
            InitializeComponent();
        }

        protected void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
