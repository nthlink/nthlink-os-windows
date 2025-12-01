using System.Windows;
using System.Windows.Controls;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// Interaction logic for DiagnosticPage.xaml
    /// </summary>
    public partial class DiagnosticPage : UserControl
    {
        public DiagnosticPage()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Collapsed);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Collapsed);
        }
    }
}
