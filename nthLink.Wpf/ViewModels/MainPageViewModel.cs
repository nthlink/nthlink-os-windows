using nthLink.Header;
using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace nthLink.Wpf.ViewModels
{
    internal class MainPageViewModel_Old : NotifyPropertyChangedBase
    {
        public IRelayCommand ConnectionCommand { get; }
        public IRelayCommand NewsCommand { get; }

        private StateEnum state = StateEnum.Waiting;

        public StateEnum State
        {
            get { return this.state; }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get { return this.isConnected; }
            set { SetProperty(ref this.isConnected, value); }
        }


        private bool isConnectingOrConnected;

        public bool IsConnectingOrConnected
        {
            get { return this.isConnectingOrConnected; }
            private set { SetProperty(ref this.isConnectingOrConnected, value); }
        }

        private string connectButtonString = "connection_label_connect";

        public string ConnectButtonString
        {
            get { return this.connectButtonString; }
            private set { SetProperty(ref this.connectButtonString, value); }
        }

        private string hitToConnectString = "connection_hint_connect";

        public string HitToConnectString
        {
            get { return this.hitToConnectString; }
            private set { SetProperty(ref this.hitToConnectString, value); }
        }

        private string serverStateString = string.Empty;

        public string ServerStateString
        {
            get { return this.serverStateString; }
            private set
            {
                this.readerWriterLock.EnterWriteLock();
                if (SetProperty(ref this.serverStateString, value))
                {
                    RaisePropertyChanged(nameof(ShowServerState));
                }
                this.readerWriterLock.ExitWriteLock();
            }
        }

        private string serverErrorMessage = string.Empty;

        public string ServerErrorMessage
        {
            get { return this.serverErrorMessage; }
            private set { SetProperty(ref this.serverErrorMessage, value); }
        }

        private string stateString = "connection_hint_connect";

        public string StateString
        {
            get { return this.stateString; }
            private set { SetProperty(ref this.stateString, value); }
        }

        public bool ShowServerState => !string.IsNullOrEmpty(ServerStateString);

        private readonly IContainerProvider containerProvider;
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        private readonly IMainThreadSyncContext mainThreadSyncContext;
        private readonly IDirectoryServerConfigProvider directoryServerConfigProvider;
        private readonly ISystemReportLog? systemReportLog;
        private readonly IPublicPrivateKeyEncryption? publicPrivateKeyEncryption;
        private readonly IJsonConverter? jsonConverter;
        public MainPageViewModel_Old(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
            this.mainThreadSyncContext = containerProvider.Resolve<IMainThreadSyncContext>().Unwrap();
            this.directoryServerConfigProvider = containerProvider.Resolve<IDirectoryServerConfigProvider>().Unwrap();
            this.systemReportLog = containerProvider.Resolve<ISystemReportLog>();
            this.publicPrivateKeyEncryption = containerProvider.Resolve<IPublicPrivateKeyEncryption>();
            this.jsonConverter = containerProvider.Resolve<JsonConverter>();
            ConnectionCommand = new RelayCommand(OnConnectionCommandExecute, CanConnectionCommandExecute);
            NewsCommand = new RelayCommand(OnNewsCommandExecute, CanNewsCommandExecute);

            if (this.containerProvider.Resolve<IEventBus<VpnServiceStateArgs>>()
                   is IEventBus<VpnServiceStateArgs> eventBus)
            {
                eventBus.Subscribe(Const.Channel.VpnService, OnVpnServiceStateChanged);
            }

            if (this.containerProvider.Resolve<IEventBus<RequestErrorArgs>>()
                  is IEventBus<RequestErrorArgs> requestErrorEventBus)
            {
                requestErrorEventBus.Subscribe(Const.Channel.VpnService, OnRequestError);
            }
        }

        private async void OnRequestError(string s, RequestErrorArgs args)
        {
            if (args.RequestError != null)
            {
                await this.mainThreadSyncContext.Post(() =>
                  {
                      ServerErrorMessage = args.RequestError.message;
                  });

                if (this.systemReportLog != null &&
                    this.jsonConverter != null &&
                    this.publicPrivateKeyEncryption != null)
                {
                    string json = this.jsonConverter.Serialize(args.RequestError);
                    EncodePackage encodePackage = this.publicPrivateKeyEncryption.EncodingString(json);
                    string dataFileFullPath = await this.systemReportLog.LogToFile(json);
                    await this.systemReportLog.LogToFile($"Getting the config from the server failed, AesKey = {encodePackage.Key}, AesIV = {encodePackage.IV}, AesDataFile = {dataFileFullPath}");
                }
            }
        }

        private void OnVpnServiceStateChanged(string s, VpnServiceStateArgs args)
        {
            StateChanged(args.State, args.Message);
        }

        private bool CanNewsCommandExecute()
        {
            return State == StateEnum.Started;
        }

        private void OnNewsCommandExecute()
        {
            if (this.directoryServerConfigProvider.DirectoryServerConfig != null &&
                !string.IsNullOrEmpty(this.directoryServerConfigProvider.DirectoryServerConfig.redirectUrl))
            {
                this.containerProvider.Resolve<IWebBrowser>().Unwrap().OpenUrl(this.directoryServerConfigProvider.DirectoryServerConfig.redirectUrl, EventSourceTypeEnum.User);
            }
        }

        private void StateChanged(StateEnum state, string message)
        {
            if (!SetProperty(ref this.state, state))
            {
                return;
            }

            Debug.Print(state.ToString());

            ClearErrorMessage();

            switch (state)
            {
                case StateEnum.Waiting:
                    {
                        ConnectButtonString = "connection_label_connect";
                        HitToConnectString = "connection_hint_connect";
                        StateString = "connection_hint_connect";
                        ServerStateString = string.Empty;
                        IsConnectingOrConnected = false;
                        IsConnected = false;
                    }
                    break;
                case StateEnum.Starting:
                    {
                        ConnectButtonString = "connection_server_state_connecting";
                        ServerStateString = "connection_server_state_connecting";
                        StateString = "connection_server_state_connecting";
                        IsConnectingOrConnected = true;
                        IsConnected = false;
                    }
                    break;
                case StateEnum.Started:
                    {
                        ConnectButtonString = "connection_label_disconnect";
                        HitToConnectString = "connection_hint_disconnect";
                        StateString = "connection_hint_disconnect";
                        ServerStateString = "connection_server_state_connected";

                        IsConnectingOrConnected = true;
                        IsConnected = true;
                    }
                    break;
                case StateEnum.Stopping:
                    {
                        ConnectButtonString = "connection_server_state_disconnecting";
                        ServerStateString = "connection_server_state_disconnecting";
                        StateString = "connection_server_state_disconnecting";
                        IsConnectingOrConnected = false;
                        IsConnected = false;
                    }
                    break;
                case StateEnum.Stopped:
                    {
                        ServerStateString = string.Empty;

                        ConnectButtonString = "connection_label_connect";
                        HitToConnectString = "connection_hint_connect";
                        StateString = "connection_hint_connect";
                        IsConnectingOrConnected = false;
                        IsConnected = false;
                    }
                    break;
                case StateEnum.Terminating:
                    {
                        ServerStateString = "connection_error";
                        ConnectButtonString = "connection_label_connect";
                        StateString = "connection_hint_connect";
                        HitToConnectString = string.Empty;
                        IsConnectingOrConnected = false;
                        IsConnected = false;
                    }
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(message))
            {
                ServerErrorMessage = message;
            }

            this.mainThreadSyncContext.Post(() =>
            {
                RaisePropertyChanged(nameof(State));
                ConnectionCommand.RaiseCanExecuteChanged();
                NewsCommand.RaiseCanExecuteChanged();
            });
        }

        private void OnConnectionCommandExecute()
        {
            if (this.systemReportLog != null)
            {
                this.systemReportLog.Log(LogLevelEnum.Info, "Connect button clicked");
            }

            IEventBus<VpnServiceFunctionArgs>? functionEventBus =
           this.containerProvider.Resolve<IEventBus<VpnServiceFunctionArgs>>();
            if (functionEventBus != null)
            {
                if (State == StateEnum.Waiting ||
                State == StateEnum.Stopped ||
                State == StateEnum.Terminating)
                {
                    ClearErrorMessage();

                    if (this.systemReportLog != null)
                    {
                        this.systemReportLog.Log(LogLevelEnum.Info, "Sending event to VPN service");
                    }

                    Task.Run(() =>
                    {
                        functionEventBus.Publish(Const.Channel.VpnService,
                            new VpnServiceFunctionArgs(FunctionEnum.Start));
                    });

                }
                else
                {
                    if (this.systemReportLog != null)
                    {
                        this.systemReportLog.Log(LogLevelEnum.Info, $"Disconnect button clicked, current state: {State}");
                    }
                    
                    Task.Run(() =>
                    {
                        if (this.systemReportLog != null)
                        {
                            this.systemReportLog.Log(LogLevelEnum.Info, "Sending stop event to VPN service");
                        }
                        
                        functionEventBus.Publish(Const.Channel.VpnService,
                        new VpnServiceFunctionArgs(FunctionEnum.Stop));
                    });
                }
            };
        }

        private bool CanConnectionCommandExecute()
        {
            return State == StateEnum.Waiting ||
                State == StateEnum.Started ||
                State == StateEnum.Stopped ||
                State == StateEnum.Terminating;
        }

        public void ClearErrorMessage()
        {
            ServerErrorMessage = string.Empty;
        }
    }
}
