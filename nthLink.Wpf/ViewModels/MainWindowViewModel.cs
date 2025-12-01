using nthLink.Header;
using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using nthLink.Wpf.Interface;
using nthLink.Wpf.Model;
using System;
using System.Threading.Tasks;

namespace nthLink.Wpf.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private const string IsNotifyEnabledKey = nameof(IsNotifyEnabled);

        private bool isPolicyAgreed;

        public bool IsPolicyAgreed
        {
            get { return this.isPolicyAgreed; }
            set { SetProperty(ref this.isPolicyAgreed, value); }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get { return this.isConnected; }
            set { SetProperty(ref this.isConnected, value); }
        }

        private bool isExpand;

        public bool IsExpand
        {
            get { return this.isExpand; }
            set { SetProperty(ref this.isExpand, value); }
        }

        private bool canNotify;

        public bool CanNotify
        {
            get { return this.canNotify; }
            set { SetProperty(ref this.canNotify, value); }
        }

        private bool isNotifyEnabled;

        public bool IsNotifyEnabled
        {
            get { return this.isNotifyEnabled; }
            set
            {
                if (SetProperty(ref this.isNotifyEnabled, value))
                {
                    this.delayAction.DoAction();

                    this.dataPersistence.Cache(IsNotifyEnabledKey, value ? "true" : "false");
                }
            }
        }

        private bool isStatic;
        public bool IsStatic
        {
            get { return this.isStatic; }
            private set { SetProperty(ref this.isStatic, value); }
        }

        public WebPageViewModel WebPageViewModel { get; }

        private WebViewModel httpConfuseViewModel;

        public WebViewModel HttpConfuseViewModel
        {
            get { return this.httpConfuseViewModel; }
            set { SetProperty(ref this.httpConfuseViewModel, value); }
        }
        public UpdateViewModel UpdateViewModel { get; }
        public DialogPageViewModel? DialogPageViewModel { get; }
        public LoadAnimationViewModel? LoadAnimationViewModel { get; }
        public string AppVersion { get; }

        private readonly SimpleTimer simpleTimer = new SimpleTimer();
        private readonly IContainerProvider containerProvider;
        private readonly INetwork network;
        private readonly IDataPersistence dataPersistence;
        private readonly DelayAction delayAction;

        private StateEnum vpnServiceState = StateEnum.Waiting;

        public MainWindowViewModel(IContainerProvider containerProvider,
            INetwork network,
            IDataPersistence dataPersistence,
            IWindowsRegister windowsRegister,
            ILanguageService languageService,
            IClientInfo clientInfo)
        {
            WebPageViewModel = containerProvider.Resolve<WebPageViewModel>().Unwrap();
            UpdateViewModel = containerProvider.Resolve<UpdateViewModel>().Unwrap();
            DialogPageViewModel = containerProvider.Resolve<IDialogBox>() as DialogPageViewModel;
            LoadAnimationViewModel = containerProvider.Resolve<ILoadAnimation>() as LoadAnimationViewModel;

            this.httpConfuseViewModel = containerProvider.Resolve<WebViewModel>().Unwrap();

            this.simpleTimer.Interval = 30 * 60 * 1000;

            this.simpleTimer.Ticks += SimpleTimer_Ticks;

            this.containerProvider = containerProvider;
            this.network = network;
            this.dataPersistence = dataPersistence;

            if (dataPersistence.Load(IsNotifyEnabledKey).ToLower() == "true")
            {
                this.isNotifyEnabled = true;
            }

            this.delayAction = new DelayAction(CheckNotifyFunction);

            IHttpObfuscator? httpObfuscator = containerProvider.Resolve<IHttpObfuscator>();

            if (httpObfuscator != null)
            {
                httpObfuscator.ObfuscatorUrl += HttpObfuscator_ObfuscatorUrl;
            }

            if (windowsRegister.GetRegister(PolicyPageViewModel.POLICY_AGREED) ==
                PolicyPageViewModel.AGREED)
            {
                IsPolicyAgreed = true;
            }
            else
            {
                windowsRegister.RegisterChanged += WindowsRegister_RegisterChanged;
            }

            if (this.containerProvider.Resolve<IEventBus<VpnServiceStateArgs>>()
                   is IEventBus<VpnServiceStateArgs> eventBus)
            {
                eventBus.Subscribe(Const.Channel.VpnService, OnVpnServiceStateChanged);
            }

            if (this.containerProvider.Resolve<IDirectoryServerConfigProvider>()
                is IDirectoryServerConfigProvider directoryServerConfigProvider)
            {
                directoryServerConfigProvider.PropertyChanged += DirectoryServerConfigProvider_PropertyChanged;
            }

            //about_version
            string str = languageService.GetString("about_version");

            if (string.IsNullOrEmpty(str))
            {
                str = "V %s";
            }

            AppVersion = str.Replace("%s", clientInfo.AppVersion.ToString());
        }

        private void DirectoryServerConfigProvider_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDirectoryServerConfigProvider.DirectoryServerConfig))
            {
                if (sender is IDirectoryServerConfigProvider directoryServerConfigProvider &&
                    directoryServerConfigProvider.DirectoryServerConfig != null)
                {
                    IsStatic = directoryServerConfigProvider.DirectoryServerConfig.isStatic;
                }
            }
        }

        private void OnVpnServiceStateChanged(string s, VpnServiceStateArgs args)
        {
            this.vpnServiceState = args.State;

            if (args.State == StateEnum.Started)
            {
                IsExpand = true;
                CanNotify = true;

                IsConnected = true;
            }
            else
            {
                CanNotify = false;

                IsConnected = false;
            }

            this.delayAction.DoAction();
        }

        private void WindowsRegister_RegisterChanged(string arg1, string arg2)
        {
            if (arg1 == PolicyPageViewModel.POLICY_AGREED &&
                arg2 == PolicyPageViewModel.AGREED)
            {
                IsPolicyAgreed = true;
            }
        }

        private void CheckNotifyFunction()
        {
            if (this.isNotifyEnabled &&
                this.vpnServiceState == StateEnum.Started)
            {
                this.simpleTimer.StartWithTick();
            }
            else
            {
                this.simpleTimer.Stop();
                this.containerProvider.Resolve<IToastWindow>().Unwrap().Cancel();
            }
        }

        private void HttpObfuscator_ObfuscatorUrl(object? sender, HttpObfuscatorUrlEventArgs e)
        {
            HttpConfuseViewModel.Url = e.Url;

            e.IsRequestGet = false;
        }

        private void SimpleTimer_Ticks()
        {
            if (this.network.IsNetworkAvailable &&
                WebPageViewModel.GetWebItemViewModel() is WebItemViewModel webItemViewModel)
            {
                this.containerProvider.Resolve<IToastWindow>().Unwrap().Show(webItemViewModel, TimeSpan.FromSeconds(5));
            }
        }
    }
}
