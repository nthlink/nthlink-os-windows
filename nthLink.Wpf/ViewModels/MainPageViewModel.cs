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
    internal class MainPageViewModel : NotifyPropertyChangedBase
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
        public MainPageViewModel(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
            this.mainThreadSyncContext = containerProvider.Resolve<IMainThreadSyncContext>().Unwrap();
            this.directoryServerConfigProvider = containerProvider.Resolve<IDirectoryServerConfigProvider>().Unwrap();
            ConnectionCommand = new RelayCommand(OnConnectionCommandExecute, CanConnectionCommandExecute);
            NewsCommand = new RelayCommand(OnNewsCommandExecute, CanNewsCommandExecute);

            if (this.containerProvider.Resolve<IEventBus<VpnServiceStateArgs>>()
                   is IEventBus<VpnServiceStateArgs> eventBus)
            {
                eventBus.Subscribe(Const.Channel.VpnService, OnVpnServiceStateChanged);
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

                        if (this.directoryServerConfigProvider.DirectoryServerConfig != null &&
                            !string.IsNullOrEmpty(this.directoryServerConfigProvider.DirectoryServerConfig.redirectUrl))
                        {
                            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(task =>
                            {
                                this.mainThreadSyncContext.Post(() =>
                                {
                                    this.containerProvider.Resolve<IWebBrowser>().Unwrap()
                                    .OpenUrl(this.directoryServerConfigProvider.DirectoryServerConfig.redirectUrl, EventSourceTypeEnum.Loading);
                                });
                            });
                        }

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
            IEventBus<VpnServiceFunctionArgs>? functionEventBus =
           this.containerProvider.Resolve<IEventBus<VpnServiceFunctionArgs>>();
            if (functionEventBus != null)
            {
                if (State == StateEnum.Waiting ||
                State == StateEnum.Stopped ||
                State == StateEnum.Terminating)
                {
                    ClearErrorMessage();

                    functionEventBus.Publish(Const.Channel.VpnService,
                        new VpnServiceFunctionArgs(FunctionEnum.Start));

                }
                else
                {
                    functionEventBus.Publish(Const.Channel.VpnService,
                        new VpnServiceFunctionArgs(FunctionEnum.Stop));
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
