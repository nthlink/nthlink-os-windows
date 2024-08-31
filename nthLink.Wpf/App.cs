using nthLink.Header;
using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.Wpf.Converter;
using nthLink.Wpf.Interface;
using nthLink.Wpf.MarkupExtension;
using nthLink.Wpf.Model;
using nthLink.Wpf.ViewModels;
using nthLink.Wpf.Views;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace nthLink.Wpf
{
    public partial class App : Application
    {
        public static IContainerProvider ContainerProvider { get; } = RegisterType(SDK.Entry.CreateContainerRegistry());

        public App()
        {
            using (System.IO.Stream? stream = typeof(App).Assembly.GetManifestResourceStream(
                $"{nameof(nthLink)}.{nameof(Wpf)}.Theme.Generic.xaml"))
            {
                if (stream != null)
                {
                    Resources = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                }
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LanguageStringConverter.LanguageService = ContainerProvider.Resolve<ILanguageService>();

            SplashWindow? splashWindow = ContainerProvider.Resolve<SplashWindow>();

            if (splashWindow != null)
            {
                splashWindow.Show();
            }

            MainWindow = ContainerProvider.Resolve<Views.MainWindow>();

            if (splashWindow != null)
            {
                splashWindow.Hide();
                splashWindow.Close();
            }

            if (MainWindow != null)
            {
                MainWindow.Title = Const.String.ProductName;
                MainWindow.Show();
            }
        }

        private static IContainerProvider RegisterType(IContainerRegistry containerProvider)
        {
            MainThreadSyncContext mainThreadSyncContext = new MainThreadSyncContext();
            return containerProvider
                .Register<NotifyItemViewModel, NotifyItemViewModel>()
                .Register<UpdateViewModel, UpdateViewModel>()
                .Register<NewsItemViewModel, NewsItemViewModel>()
                .RegisterSingleton<IDialogBox, DialogPageViewModel>()
                .RegisterSingleton<IWindowsRegister, RegisterImp>()
                .RegisterInstance<IMainThreadSyncContext>(mainThreadSyncContext)
                .RegisterSingleton<ILanguageService, LanguageService>()
                .RegisterSingleton<TranslationSource, TranslationSource>()
                .RegisterSingleton<nthLink.Header.Interface.IWebBrowser, WebBrowserImp>()
                .RegisterSingleton<IUpdater, UpdaterImp>()
                .RegisterInstance<Encoding>(Encoding.UTF8)
                .LoadModule()
                .InitializeModuleAndCreateContainerProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ContainerProvider.Resolve<IDataPersistence>().Unwrap().ExecuteCache();
            ContainerProvider.Resolve<ISystemReportLog>()?.Save();
        }
    }
}
