using CefSharp;
using CefSharp.Wpf;
using nthLink.Header;
using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.Wpf.Application.Services;
using nthLink.Wpf.Converter;
using nthLink.Wpf.Infrastructure.DependencyInjection;
using nthLink.Wpf.Interface;
using nthLink.Wpf.MarkupExtension;
using nthLink.Wpf.Model;
using nthLink.Wpf.Struct;
using nthLink.Wpf.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace nthLink.Wpf
{
    /// <summary>
    /// 重構後的App類 - 使用新的分層架構和ServiceRegistry
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IContainerProvider ContainerProvider { get; } = RegisterServices(SDK.Entry.CreateContainerRegistry());

        public App()
        {
            LoadThemeResources();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeApplication();
        }

        /// <summary>
        /// 初始化應用程式
        /// </summary>
        private void InitializeApplication()
        {
#if DEBUG
            // 開發階段驗證服務註冊
            ValidateServicesInDebugMode();
#endif

            // 設置語言轉換器 - 使用新的本地化服務
            var localizationService = ContainerProvider.Resolve<ILocalizationService>();
            LanguageStringConverter.LanguageService = localizationService as ILanguageService;

            // 訂閱應用程序關閉事件
            SubscribeToApplicationEvents();

            // 顯示啟動畫面
            var splashWindow = ContainerProvider.Resolve<SplashWindow>();
            splashWindow?.Show();

            // 初始化CEF
            const int milliseconds = 3 * 1000;
            var stopwatch = Stopwatch.StartNew();
            InitializeCef();
            stopwatch.Stop();

            // 確保啟動畫面顯示足夠時間
            int remainingTime = milliseconds - (int)stopwatch.ElapsedMilliseconds;
            if (remainingTime > 0)
            {
                Task.Delay(remainingTime).Wait();
            }

            // 建立並顯示主視窗
            ShowMainWindow(splashWindow);
        }

        /// <summary>
        /// 初始化CEF瀏覽器
        /// </summary>
        private void InitializeCef()
        {
            CefSettings settings = new CefSettings();
            settings.RootCachePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{Const.String.ProductName}\\{nameof(CefSettings)}\\Cache");

            Cef.Initialize(settings);
        }

        /// <summary>
        /// 顯示主視窗
        /// </summary>
        private void ShowMainWindow(SplashWindow? splashWindow)
        {
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

        /// <summary>
        /// 載入主題資源
        /// </summary>
        private void LoadThemeResources()
        {
            using (System.IO.Stream? stream = typeof(App).Assembly.GetManifestResourceStream(
                $"{nameof(nthLink)}.{nameof(Wpf)}.Theme.Generic.xaml"))
            {
                if (stream != null)
                {
                    Resources = System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
                }
            }
        }

        /// <summary>
        /// 訂閱應用程序事件
        /// </summary>
        private void SubscribeToApplicationEvents()
        {
            if (ContainerProvider.Resolve<IEventBus<AppEventArgs>>() is IEventBus<AppEventArgs> appEventBus)
            {
                appEventBus.Subscribe(AppEventArgs.AppEventArgsMessage.AppEvent, OnApplicationEventReceived);
            }
        }

        /// <summary>
        /// 處理應用程序事件
        /// </summary>
        private async void OnApplicationEventReceived(string channel, AppEventArgs args)
        {
            if (args.Message == AppEventArgs.AppEventArgsMessage.ApplicationShutdown)
            {
                await HandleApplicationShutdownAsync();
            }
        }

        /// <summary>
        /// 處理應用程序關閉事件
        /// </summary>
        private async Task HandleApplicationShutdownAsync()
        {
            try
            {
                // 獲取連接服務
                var connectionService = ContainerProvider.Resolve<IConnectionService>();

                if (connectionService != null && connectionService.IsConnected)
                {
                    // 斷開VPN連接
                    await connectionService.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                // 記錄斷開連接時的異常，但不阻止應用程序關閉
                Debug.WriteLine($"Failed to disconnect VPN before shutdown: {ex.Message}");
            }
            finally
            {
                // 在主線程中關閉應用程序
                Dispatcher.Invoke(() =>
                {
                    Shutdown();
                });
            }
        }

        /// <summary>
        /// 使用新的ServiceRegistry註冊服務 - 分層明確
        /// </summary>
        private static IContainerProvider RegisterServices(IContainerRegistry containerRegistry)
        {
            return RegisterLegacyServices(// 註冊遺留服務以保持相容性
                containerRegistry.RegisterAllServices()) // 使用ServiceRegistry註冊所有分層服務
                .LoadModule()                    // 載入SDK模組
                .InitializeModuleAndCreateContainerProvider();
        }

        /// <summary>
        /// 註冊遺留服務以保持向後相容性
        /// </summary>
        private static IContainerRegistry RegisterLegacyServices(IContainerRegistry containerRegistry)
        {
            // 註冊遺留的服務實作以保持相容性
            containerRegistry.RegisterSingleton<IWindowsRegister, RegisterImp>();
            containerRegistry.RegisterSingleton<TranslationSource, TranslationSource>();
            containerRegistry.RegisterInstance<Encoding>(Encoding.UTF8);

            return containerRegistry;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // 清理資源
            CleanupResources();
        }

        /// <summary>
        /// 清理應用程式資源
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                // 儲存資料
                ContainerProvider.Resolve<IDataPersistence>()?.Unwrap()?.ExecuteCache();

                // 清理日誌
                if (ContainerProvider.Resolve<ISystemReportLog>() is ISystemReportLog systemReportLog)
                {
                    systemReportLog.ClearLog(new TimeSpan(30, 0, 0, 0));
                    systemReportLog.Save();
                }
            }
            catch (Exception ex)
            {
                // 記錄清理過程中的錯誤，但不阻止應用程式退出
                Debug.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

#if DEBUG
        /// <summary>
        /// 開發階段驗證服務註冊 - 僅在DEBUG模式下執行
        /// </summary>
        private static void ValidateServicesInDebugMode()
        {
            try
            {
                var report = ServiceValidator.GenerateServiceReport(ContainerProvider);
                Debug.WriteLine(report);

                var result = ServiceValidator.ValidateServices(ContainerProvider);
                if (!result.IsValid)
                {
                    var errorMessage = $"服務註冊驗證失敗！發現 {result.Errors.Count} 個錯誤。請檢查偵錯輸出獲取詳細資訊。";
                    Debug.WriteLine($"❌ {errorMessage}");

                    // 在DEBUG模式下，如果服務註冊有問題，可以選擇拋出異常來立即發現問題
                    // throw new InvalidOperationException(errorMessage);
                }
                else
                {
                    Debug.WriteLine("✅ 所有服務註冊驗證通過！");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"服務驗證過程中發生異常: {ex.Message}");
            }
        }
#endif
    }
}