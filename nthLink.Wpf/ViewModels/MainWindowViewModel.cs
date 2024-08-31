using nthLink.Header;
using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using nthLink.Wpf.Interface;
using System.Threading.Tasks;

namespace nthLink.Wpf.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase
    {
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

        private bool isStatic;
        public bool IsStatic
        {
            get { return this.isStatic; }
            private set { SetProperty(ref this.isStatic, value); }
        }

        public WebPageViewModel WebPageViewModel { get; }
        public UpdateViewModel UpdateViewModel { get; }
        public DialogPageViewModel? DialogPageViewModel { get; }
        public string AppVersion { get; }

        private readonly IContainerProvider containerProvider;
        private readonly INetwork network;
        private readonly IDataPersistence dataPersistence;

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

            this.containerProvider = containerProvider;
            this.network = network;
            this.dataPersistence = dataPersistence;

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

                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        }

        private void WindowsRegister_RegisterChanged(string arg1, string arg2)
        {
            if (arg1 == PolicyPageViewModel.POLICY_AGREED &&
                arg2 == PolicyPageViewModel.AGREED)
            {
                IsPolicyAgreed = true;
            }
        }

        private void HttpObfuscator_ObfuscatorUrl(object? sender, HttpObfuscatorUrlEventArgs e)
        {
            e.IsRequestGet = false;
        }
    }
}
