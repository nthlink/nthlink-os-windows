using nthLink.Header;
using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Extension;
using nthLink.Wpf.Interface;
using nthLink.Wpf.Struct;
using nthLink.Wpf.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;

namespace nthLink.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IContainerProvider containerProvider;
        private readonly IWebBrowser webBrowser;
        private readonly IMainThreadSyncContext mainThreadSyncContext;
        private bool isDeactivated;
        private StateEnum vpnState;

        public MainWindow(IContainerProvider containerProvider,
            IWebBrowser webBrowser,
            ILanguageService languageService,
            IMainThreadSyncContext mainThreadSyncContext,
            IDialogBox dialogBox)
        {
            InitializeComponent();
            this.containerProvider = containerProvider;
            this.webBrowser = webBrowser;
            this.mainThreadSyncContext = mainThreadSyncContext;
            if (containerProvider.Resolve<IEventBus<VpnServiceStateArgs>>()
                   is IEventBus<VpnServiceStateArgs> eventBus)
            {
                eventBus.Subscribe(Const.Channel.VpnService, OnVpnServiceStateChanged);
            }

            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;
        }

        private void MainWindow_Deactivated(object? sender, EventArgs e)
        {
            this.isDeactivated = true;
        }

        private void MainWindow_Activated(object? sender, EventArgs e)
        {
            if (this.isDeactivated)
            {
                if (this.containerProvider.Resolve<IEventBus<AppEventArgs>>()
                               is IEventBus<AppEventArgs> appEvent)
                {
                    appEvent.Publish(AppEventArgs.AppEventArgsMessage.AppEvent,
                        new AppEventArgs(AppEventArgs.AppEventArgsMessage.WindowActivated));
                }

                this.isDeactivated = false;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource? source = PresentationSource.FromVisual(this) as HwndSource;
            if (source != null)
            {
                source.AddHook(WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_CLOSE = 0x0010;

            if (msg == WM_CLOSE)
            {
                IEventBus<VpnServiceFunctionArgs>? functionEventBus =
           this.containerProvider.Resolve<IEventBus<VpnServiceFunctionArgs>>();
                if (functionEventBus != null)
                {
                    functionEventBus.Publish(Const.Channel.VpnService,
                        new VpnServiceFunctionArgs(FunctionEnum.Stop));
                }
            }

            return IntPtr.Zero;
        }
        private void OnVpnServiceStateChanged(string s, VpnServiceStateArgs args)
        {
            this.vpnState = args.State;

            this.mainThreadSyncContext.Post(() =>
            {
                if (args.State == StateEnum.Started)
                {
                    PART_WebPageGrid.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                }
                else
                {
                    PART_WebPageGrid.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                }
            });
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

            System.Windows.Application.Current.Shutdown();
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

        private void PrivacyPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            this.webBrowser.OpenUrlWithoutReport("https://www.nthlink.com/policies/");
        }

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            CloseMenu();
        }

        private void CloseMenu()
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, false);
        }

        private void FollowUsButton_Click(object sender, RoutedEventArgs e)
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, false);
            PART_FollowUsPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
        }

        private void FollowUsPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PART_FollowUsPage is FrameworkElement frameworkElement)
            {
                frameworkElement.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Collapsed);
            }
        }

        private void DiagnosticButton_Click(object sender, RoutedEventArgs e)
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, false);
            PART_DiagnosticPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
        }

        private void BypassButton_Click(object sender, RoutedEventArgs e)
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, false);

            if (this.vpnState == StateEnum.Started)
            {
                if (this.containerProvider.Resolve<IDialogBox>() is IDialogBox dialogBox)
                {
                    dialogBox.ShowDialog(string.Empty, "Please disconnect the VPN before modifying the bypass settings.", "Ok");
                }
            }
            else
            {
                PART_BypassPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
            }
        }

        private void PhoneConnect_Click(object sender, RoutedEventArgs e)
        {
            PART_MenuPopup.SetCurrentValue(Popup.IsOpenProperty, false);

            if (this.vpnState == StateEnum.Started)
            {
                if (this.containerProvider.Resolve<IDialogBox>() is IDialogBox dialogBox)
                {
                    dialogBox.ShowDialog(string.Empty, "Please disconnect your current VPN or network connection before starting Phone Connect.", "Ok");
                }
            }
            else
            {
                PART_PhoneConnectPage.SetCurrentValue(FrameworkElement.VisibilityProperty, Visibility.Visible);
            }
        }
    }
}
