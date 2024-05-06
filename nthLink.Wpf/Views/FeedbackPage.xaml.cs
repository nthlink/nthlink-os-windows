using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FeedbackPage : UserControl
    {
        public FeedbackPage()
        {
            InitializeComponent();
        }

        protected void Minimum_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is Window window)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        protected void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(VisibilityProperty, Visibility.Collapsed);

            if (DataContext is FeedbackPageViewModel feedbackPageViewModel)
            {
                feedbackPageViewModel.RestoreDefault();
            }
        }

        protected void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.ContainerProvider.Resolve<IWebBrowser>().Unwrap().OpenUrlWithoutReport("https://www.nthlink.com/privacy");
        }

        protected void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is FeedbackPageViewModel feedbackPageViewModel)
            {
                feedbackPageViewModel.RestoreDefault();
            }
        }
    }
}
