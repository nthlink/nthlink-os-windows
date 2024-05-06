using nthLink.Header.Enum;
using nthLink.Header.Interface;
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

        private bool isPolicyAggred;

        public bool IsPolicyAggred
        {
            get { return this.isPolicyAggred; }
            set { SetProperty(ref this.isPolicyAggred, value); }
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

        public WebPageViewModel WebPageViewModel { get; }

        private WebViewModel httpConfuseViewModel;

        public WebViewModel HttpConfuseViewModel
        {
            get { return this.httpConfuseViewModel; }
            set { SetProperty(ref this.httpConfuseViewModel, value); }
        }


        private readonly SimpleTimer simpleTimer = new SimpleTimer();
        private readonly IContainerProvider containerProvider;
        private readonly IVpnService vpnService;
        private readonly INetwork network;
        private readonly IDataPersistence dataPersistence;
        private readonly DelayAction delayAction;
        public MainWindowViewModel(IContainerProvider containerProvider,
            IVpnService vpnService,
            INetwork network,
            IDataPersistence dataPersistence,
            IWindowsRegister windowsRegister)
        {
            WebPageViewModel = containerProvider.Resolve<WebPageViewModel>().Unwrap();

            this.httpConfuseViewModel = containerProvider.Resolve<WebViewModel>().Unwrap();

            vpnService.StateChanged += VpnService_StateChanged;

            this.simpleTimer.Interval = 30 * 60 * 1000;

            this.simpleTimer.Ticks += SimpleTimer_Ticks;

            this.containerProvider = containerProvider;
            this.vpnService = vpnService;
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
                IsPolicyAggred = true;
            }
            else
            {
                windowsRegister.RegisterChanged += WindowsRegister_RegisterChanged;
            }
        }

        private void WindowsRegister_RegisterChanged(string arg1, string arg2)
        {
            if (arg1 == PolicyPageViewModel.POLICY_AGREED &&
                arg2 == PolicyPageViewModel.AGREED)
            {
                IsPolicyAggred = true;
            }
        }

        private void CheckNotifyFunction()
        {
            if (this.isNotifyEnabled &&
                this.vpnService.State == StateEnum.Started)
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

        private void VpnService_StateChanged(object? sender, EventArgs e)
        {
            if (this.vpnService.State == StateEnum.Started)
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
