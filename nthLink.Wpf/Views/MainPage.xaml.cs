using nthLink.Wpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// MainPage.xaml 的互動邏輯
    /// </summary>
    public partial class MainPage : UserControl
    {
        public event RoutedEventHandler Feedback
        {
            add { AddHandler(FeedbackEvent, value); }
            remove { RemoveHandler(FeedbackEvent, value); }
        }

        public static readonly RoutedEvent FeedbackEvent = EventManager.RegisterRoutedEvent(
    name: nameof(Feedback),
    routingStrategy: RoutingStrategy.Bubble,
    handlerType: typeof(RoutedEventHandler),
    ownerType: typeof(MainPage));

        public MainPage()
        {
            InitializeComponent();
        }

        protected void FeedbackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(FeedbackEvent));
        }

        protected void Ellipse_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
#if DEBUG
            System.Windows.Window window = new System.Windows.Window();
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.Width = 360;
            window.MinHeight = 520;
            window.SizeToContent = System.Windows.SizeToContent.Height;
            window.Content = new ThemeEditorPage();

            window.Show();
#endif
        }

        protected void Error_Message_Close_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is MainPageViewModel mainPageViewModel)
            {
                mainPageViewModel.ClearErrorMessage();
            }
        }

        protected void Run_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(FeedbackEvent));

            if (DataContext is MainPageViewModel mainPageViewModel)
            {
                mainPageViewModel.ClearErrorMessage();
            }
        }
    }
}
