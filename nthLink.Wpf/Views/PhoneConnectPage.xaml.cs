using System.Windows;
using System.Windows.Controls;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// PhoneConnectPage.xaml 的互動邏輯
    /// </summary>
    public partial class PhoneConnectPage : UserControl
    {
        public PhoneConnectPage()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Collapsed);
        }
    }
}
