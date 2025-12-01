using nthLink.Header;
using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Model;
using nthLink.SDK.Extension;
using System;
using System.Threading.Tasks;
using nthLink.Wpf.Interface;
using System.Diagnostics;
using nthLink.Header.Enum;

namespace nthLink.Wpf.ViewModels
{
    internal class DiagnosticPageViewModel : NotifyPropertyChangedBase
    {
        private readonly IContainerProvider containerProvider;
        private readonly IMainThreadSyncContext mainThreadSyncContext;
        private readonly ILanguageService languageService;
        private StateEnum vpnState = StateEnum.Waiting;
        public string ContentString { get; }

        public IRelayCommand StartDiagnosticCommand { get; }

        private bool isOnDiagnostics;

        public bool IsOnDiagnostics
        {
            get { return this.isOnDiagnostics; }
            set { SetProperty(ref this.isOnDiagnostics, value); }
        }


        public DiagnosticPageViewModel(IContainerProvider containerProvider, IMainThreadSyncContext mainThreadSyncContext)
        {
            this.containerProvider = containerProvider;
            this.mainThreadSyncContext = mainThreadSyncContext;
            this.languageService = containerProvider.Resolve<ILanguageService>().Unwrap();
            // ContentString = "At nthLink, we occasionally seek your assistance in enhancing our service by providing diagnostic data.\r\n\r\nBy clicking the button, the diagnostics process begins, collecting information and transmitting it to our servers. Rest assured, all information gathered is completely anonymous.";
            ContentString = this.languageService.GetString("diagnostic_info")
                .Replace("\\r", "\r")
                .Replace("\\n", "\n");
            StartDiagnosticCommand = new RelayCommand(OnStartDiagnosticCommandExecute);

            if (this.containerProvider.Resolve<IEventBus<VpnServiceStateArgs>>()
                   is IEventBus<VpnServiceStateArgs> eventBus)
            {
                eventBus.Subscribe(Const.Channel.VpnService, OnVpnServiceStateChanged);
            }
        }

        private void OnVpnServiceStateChanged(string s, VpnServiceStateArgs args)
        {
            this.vpnState = args.State;

            if (args.State == StateEnum.StartDiagnostics)
            {
                IsOnDiagnostics = true;
            }
            else if (args.State == StateEnum.StopDiagnostics)
            {
                IsOnDiagnostics = false;

                if (this.containerProvider.Resolve<IDialogBox>() is IDialogBox dialogBox)
                {
                    dialogBox.ShowDialog(string.Empty, 
                        this.languageService.GetString("diagnostic_thanks"), "OK");
                }
            }
        }

        private void OnStartDiagnosticCommandExecute()
        {
            if (this.vpnState == StateEnum.Started)
            {
                if (this.containerProvider.Resolve<IDialogBox>() is IDialogBox dialogBox)
                {
                    dialogBox.ShowDialog(string.Empty, 
                        this.languageService.GetString("diagnostic_is_connected"), "OK");
                }

                return;
            }

            if (IsOnDiagnostics)
            {
                return;
            }

            Task.Run(() =>
            {
                if (this.containerProvider.Resolve<IEventBus<VpnServiceFunctionArgs>>()
                     is IEventBus<VpnServiceFunctionArgs> eventBus)
                {
                    eventBus.Publish(Const.Channel.VpnService,
                        new VpnServiceFunctionArgs(Header.Enum.FunctionEnum.Diagnostics));
                }
            });
        }
    }
}
