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
    /// PolicyPage.xaml 的互動邏輯
    /// </summary>
    public partial class PolicyPage : UserControl
    {

        public event RoutedEventHandler About
        {
            add { AddHandler(AboutEvent, value); }
            remove { RemoveHandler(AboutEvent, value); }
        }

        public static readonly RoutedEvent AboutEvent = EventManager.RegisterRoutedEvent(
    name: nameof(About),
    routingStrategy: RoutingStrategy.Bubble,
    handlerType: typeof(RoutedEventHandler),
    ownerType: typeof(MainPage));
        public PolicyPage()
        {
            InitializeComponent();
        }

        protected void Open_About_Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(AboutEvent));
        }
    }
}
