using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.Wpf.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// BrowserPage.xaml 的互動邏輯
    /// </summary>
    public partial class WebPage : UserControl
    {
        private readonly IWebBrowser webBrowser;

        public WebPage()
        {
            InitializeComponent();
            this.webBrowser = App.ContainerProvider.Resolve<IWebBrowser>().Unwrap();
        }

        protected void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
               button.DataContext is WebItemViewModel viewModel &&
               !string.IsNullOrEmpty(viewModel.Url))
            {
                this.webBrowser.OpenUrl(viewModel.Url, EventSourceTypeEnum.User);
            }
        }
    }
}
