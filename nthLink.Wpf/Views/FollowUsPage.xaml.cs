using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.Wpf.Interface;
using System.Windows;
using System.Windows.Controls;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// FollowUsPage.xaml 的互動邏輯
    /// </summary>
    public partial class FollowUsPage : UserControl
    {
        public FollowUsPage()
        {
            InitializeComponent();
        }

        private async void CopyTelegramIdButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("nthLinkVPN");

            if (App.ContainerProvider.Resolve<IDialogBox>() is IDialogBox dialogBox &&
              App.ContainerProvider.Resolve<ILanguageService>() is ILanguageService languageService)
            {
                await dialogBox.ShowDialog(languageService.GetString("copied"),
                    languageService.GetString("paste_id_telegram"), "OK");
            }
        }
    }
}
