using nthLink.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// Interaction logic for BypassPage.xaml
    /// </summary>
    public partial class BypassPage : UserControl
    {
        public BypassPage()
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
        }
    }
}
