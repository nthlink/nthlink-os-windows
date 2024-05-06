using nthLink.Header;
using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.Wpf.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IWebBrowser webBrowser;
        private readonly ILanguageService languageService;
        private readonly IMainThreadSyncContext mainThreadSyncContext;

        public MainWindow(IVpnService vpnService,
            IWebBrowser webBrowser,
            ILanguageService languageService,
            IMainThreadSyncContext mainThreadSyncContext)
        {
            InitializeComponent();

            vpnService.StateChanged += VpsService_StateChanged;
            this.webBrowser = webBrowser;
            this.languageService = languageService;
            this.mainThreadSyncContext = mainThreadSyncContext;
        }

        private void VpsService_StateChanged(object? sender, System.EventArgs e)
        {
            if (sender is IVpnService vpnService)
            {
                this.mainThreadSyncContext.Post(() =>
                {
                    if (vpnService.State == StateEnum.Started)
                    {
                        PART_WebPageGrid.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                    }
                    else
                    {
                        PART_WebPageGrid.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                    }
                });
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        protected void OpenWebPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.IsExpand = true;
            }
        }

        protected void CloseWebPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.IsExpand = false;
            }
        }
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                if (this.notifyIcon == null)
                {
                    this.notifyIcon = InitialNotifyIcon();
                }

                this.notifyIcon.Visible = true;

                ShowInTaskbar = false;
                Hide();
            }
        }

        #region NotifyIcon
        private System.Windows.Forms.NotifyIcon? notifyIcon;

        private System.Windows.Forms.NotifyIcon InitialNotifyIcon()
        {
            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon(System.Windows.Application.GetResourceStream(new Uri("Resource/Icon.ico", UriKind.Relative)).Stream);

            notifyIcon.Text = Const.String.ProductName;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            notifyIcon.ContextMenuStrip = InitialContextMenu();
            return notifyIcon;
        }

        private void NotifyIcon_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }

            ShowWindow(this, new EventArgs());
        }

        private System.Windows.Forms.ContextMenuStrip InitialContextMenu()
        {
            System.Windows.Forms.ContextMenuStrip contextMenu = new System.Windows.Forms.ContextMenuStrip();

            contextMenu.Items.Add(Const.String.ProductName).Click += ShowWindow;
            //contextMenu.Items.Add(this.languageService.GetString("about_page_title")).Click += ToWebsite;

            return contextMenu;
        }

        private void ShowWindow(object? sender, EventArgs e)
        {
            ShowWindow();
        }

        public void ShowWindow()
        {
            ReleaseNotifyIcon();

            if (!IsVisible)
            {
                ShowInTaskbar = true;
                Show();
                WindowState = WindowState.Normal;
            }
        }

        private void ToWebsite(object? sender, EventArgs e)
        {

        }

        private void OnExit(object? sender, EventArgs e)
        {
            Hide();

            ReleaseNotifyIcon();

            Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ReleaseNotifyIcon();

            base.OnClosing(e);
        }

        private void ReleaseNotifyIcon()
        {
            if (this.notifyIcon != null)
            {
                this.notifyIcon.Visible = false;
                this.notifyIcon.Dispose();
                this.notifyIcon = null;
            }
        }
        #endregion NotifyIcon

        protected void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, !PART_MenuPopup.IsOpen);
        }

        protected void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, false);
            PART_AboutPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
        }

        protected void AboutPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                frameworkElement.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Collapsed);
            }
        }

        protected void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            this.webBrowser.OpenUrlWithoutReport("https://s3.us-west-1.amazonaws.com/dwo-jar-kmf-883/help.html");
        }

        protected void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, false);
            PART_FeedbackPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
        }

        protected void MainPage_Feedback(object sender, RoutedEventArgs e)
        {
            PART_FeedbackPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
        }

        protected void PolicyPage_About(object sender, RoutedEventArgs e)
        {
            PART_AboutPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
        }
    }
}
