using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.Wpf.Application.Services;
using nthLink.Wpf.Infrastructure.Localization;
using nthLink.Wpf.Interface;
using nthLink.Wpf.Model;
using nthLink.Wpf.ViewModels;

namespace nthLink.Wpf.Infrastructure.DependencyInjection
{
    /// <summary>
    /// 服務註冊器 - 基礎設施層
    /// 職責：統一管理所有服務的相依性注入設定
    /// </summary>
    public static class ServiceRegistry
    {
        /// <summary>
        /// 註冊應用層服務
        /// </summary>
        public static IContainerRegistry RegisterApplicationServices(this IContainerRegistry containerRegistry)
        {
            // 連接服務
            containerRegistry.RegisterSingleton<IConnectionService, ConnectionService>();
            LocalizationService localizationService = new LocalizationService(new ResourceProvider());
            // 本地化服務 - 使用新的分層實作
            containerRegistry.RegisterInstance<ILocalizationService>(localizationService);
            containerRegistry.RegisterInstance<ILanguageService>(localizationService);

            return containerRegistry;
        }

        /// <summary>
        /// 註冊基礎設施服務
        /// </summary>
        public static IContainerRegistry RegisterInfrastructureServices(this IContainerRegistry containerRegistry)
        {
            // 資源提供者
            containerRegistry.RegisterSingleton<ResourceProvider>();

            // 主執行緒同步上下文
            var mainThreadSyncContext = new MainThreadSyncContext();
            containerRegistry.RegisterInstance<IMainThreadSyncContext>(mainThreadSyncContext);

            // 其他基礎設施服務
            containerRegistry.RegisterSingleton<ISystemReportLog, SystemReportLog>();
            containerRegistry.RegisterSingleton<IBypassSetProvider, BypassSetProvider>();

            return containerRegistry;
        }

        /// <summary>
        /// 註冊表現層服務（ViewModels等）
        /// </summary>
        public static IContainerRegistry RegisterPresentationServices(this IContainerRegistry containerRegistry)
        {
            // ViewModels
            containerRegistry.Register<NotifyItemViewModel, NotifyItemViewModel>();
            containerRegistry.Register<WebViewModel, WebViewModel>();
            containerRegistry.Register<UpdateViewModel, UpdateViewModel>();
            containerRegistry.Register<WebItemViewModel, WebItemViewModel>();
            containerRegistry.Register<NewsItemViewModel, NewsItemViewModel>();

            //忙碌動畫
            containerRegistry.RegisterSingleton<ILoadAnimation, LoadAnimationViewModel>();
            // 對話方塊和Toast服務
            containerRegistry.RegisterSingleton<IDialogBox, DialogPageViewModel>();

            MainThreadSyncContext mainThreadSyncContext = new MainThreadSyncContext();
            containerRegistry.RegisterInstance<IMainThreadSyncContext>(mainThreadSyncContext);
            containerRegistry.RegisterInstance<IToastWindow>(new ToastWindowImp(mainThreadSyncContext));

            return containerRegistry;
        }

        /// <summary>
        /// 註冊領域服務
        /// </summary>
        public static IContainerRegistry RegisterDomainServices(this IContainerRegistry containerRegistry)
        {
            // 目前領域層主要是模型，服務較少
            // 未來可以在這裡添加領域服務的註冊
            return containerRegistry;
        }

        /// <summary>
        /// 註冊所有服務 - 統一入口
        /// </summary>
        public static IContainerRegistry RegisterAllServices(this IContainerRegistry containerRegistry)
        {
            return containerRegistry
                .RegisterInfrastructureServices()  // 基礎設施層
                .RegisterDomainServices()          // 領域層
                .RegisterApplicationServices()     // 應用層
                .RegisterPresentationServices();   // 表現層
        }
    }
}