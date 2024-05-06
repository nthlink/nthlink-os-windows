using CefSharp;
using CefSharp.Wpf;
using nthLink.Header;
using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
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

            const int milliseconds = 3 * 1000;

            Stopwatch stopwatch = Stopwatch.StartNew();

            Cef.Initialize(new CefSettings());

            stopwatch.Stop();

            int sub = milliseconds - (int)stopwatch.ElapsedMilliseconds;

            if (sub > 0)
            {
                await Task.Delay(sub);
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
                .Register<WebViewModel, WebViewModel>()
                .Register<WebItemViewModel, WebItemViewModel>()
                .Register<NewsItemViewModel, NewsItemViewModel>()
                //.RegisterSingleton<IHttpObfuscator, HttpObfuscatorImp>()
                .RegisterSingleton<IWindowsRegister, RegisterImp>()
                .RegisterInstance<IToastWindow>(new ToastWindowImp(mainThreadSyncContext))
                .RegisterInstance<IMainThreadSyncContext>(mainThreadSyncContext)
                .RegisterSingleton<ILanguageService, LanguageService>()
                .RegisterSingleton<TranslationSource, TranslationSource>()
                .RegisterSingleton<nthLink.Header.Interface.IWebBrowser, WebBrowserImp>()
                .RegisterInstance<Encoding>(Encoding.UTF8)
                .LoadModule()
                .InitializeModuleAndCreateContainerProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ContainerProvider.Resolve<IDataPersistence>().Unwrap().ExecuteCache();
        }
    }
}
