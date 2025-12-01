using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using nthLink.Wpf.Application.Services;
using System;
using System.Threading;

namespace nthLink.Wpf.ViewModels
{
    /// <summary>
    /// 重構後的MainPageViewModel - 職責更清晰，依賴服務處理業務邏輯
    /// </summary>
    internal class MainPageViewModel : NotifyPropertyChangedBase
    {
        #region Properties

        public IRelayCommand ConnectionCommand { get; }
        public IRelayCommand NewsCommand { get; }

        // UI狀態屬性 - 從服務獲取，不直接處理業務邏輯
        public StateEnum State => connectionService.CurrentState;
        public bool IsConnected => connectionService.IsConnected;
        public bool IsConnectingOrConnected =>
            State == StateEnum.Starting || State == StateEnum.Started;

        private string connectButtonString = "connection_label_connect";
        public string ConnectButtonString
        {
            get => connectButtonString;
            private set => SetProperty(ref connectButtonString, value);
        }

        private string hitToConnectString = "connection_hint_connect";
        public string HitToConnectString
        {
            get => hitToConnectString;
            private set => SetProperty(ref hitToConnectString, value);
        }

        private string stateString = "connection_hint_connect";
        public string StateString
        {
            get => stateString;
            private set => SetProperty(ref stateString, value);
        }

        public string ServerStateString => connectionService.ServerStateMessage;
        public string ServerErrorMessage => connectionService.ErrorMessage;
        public bool ShowServerState => !string.IsNullOrEmpty(ServerStateString);

        #endregion

        #region Private Fields

        private readonly IConnectionService connectionService;
        private readonly ILocalizationService localizationService;
        private readonly IContainerProvider containerProvider;
        private readonly IDirectoryServerConfigProvider directoryServerConfigProvider;

        #endregion

        #region Constructor

        public MainPageViewModel(
            IConnectionService connectionService,
            ILocalizationService localizationService,
            IContainerProvider containerProvider)
        {
            this.connectionService = connectionService;
            this.localizationService = localizationService;
            this.containerProvider = containerProvider;
            this.directoryServerConfigProvider =
                containerProvider.Resolve<IDirectoryServerConfigProvider>().Unwrap();

            // 建立命令
            ConnectionCommand = new RelayCommand(OnConnectionCommandExecute, CanConnectionCommandExecute);
            NewsCommand = new RelayCommand(OnNewsCommandExecute, CanNewsCommandExecute);

            // 訂閱服務事件
            SubscribeToServiceEvents();

            // 初始化UI狀態
            UpdateUIState(connectionService.CurrentState);
        }

        #endregion

        #region Event Handlers

        private async void OnConnectionCommandExecute()
        {
            bool success;

            if (connectionService.IsConnected || State == StateEnum.Starting)
            {
                success = await connectionService.DisconnectAsync();
            }
            else
            {
                success = await connectionService.ConnectAsync();
            }

            if (!success)
            {
                // 處理連接失敗 - 這裡可以顯示錯誤訊息
                // 錯誤訊息已經透過事件傳遞給UI
            }
        }

        private bool CanConnectionCommandExecute()
        {
            return State == StateEnum.Waiting ||
                   State == StateEnum.Started ||
                   State == StateEnum.Stopped ||
                   State == StateEnum.Terminating;
        }

        private bool CanNewsCommandExecute()
        {
            return State == StateEnum.Started;
        }

        private void OnNewsCommandExecute()
        {
            if (directoryServerConfigProvider.DirectoryServerConfig != null &&
                !string.IsNullOrEmpty(directoryServerConfigProvider.DirectoryServerConfig.redirectUrl))
            {
                containerProvider.Resolve<IWebBrowser>().Unwrap()
                    .OpenUrl(directoryServerConfigProvider.DirectoryServerConfig.redirectUrl, EventSourceTypeEnum.User);
            }
        }

        #endregion

        #region Service Event Handlers

        private void OnConnectionStateChanged(StateEnum state, bool isConnected)
        {
            UpdateUIState(state);

            // 通知UI屬性變化
            RaisePropertyChanged(nameof(State));
            RaisePropertyChanged(nameof(IsConnected));
            RaisePropertyChanged(nameof(IsConnectingOrConnected));
            RaisePropertyChanged(nameof(ServerStateString));
            RaisePropertyChanged(nameof(ShowServerState));

            // 更新命令狀態
            ConnectionCommand.RaiseCanExecuteChanged();
            NewsCommand.RaiseCanExecuteChanged();
        }

        private void OnErrorMessageChanged(string errorMessage)
        {
            RaisePropertyChanged(nameof(ServerErrorMessage));
        }

        #endregion

        #region Private Methods

        private void SubscribeToServiceEvents()
        {
            connectionService.ConnectionStateChanged += OnConnectionStateChanged;
            connectionService.ErrorMessageChanged += OnErrorMessageChanged;
        }

        /// <summary>
        /// 更新UI狀態字串 - 使用本地化服務
        /// </summary>
        private void UpdateUIState(StateEnum state)
        {
            switch (state)
            {
                case StateEnum.Waiting:
                    ConnectButtonString = localizationService.GetString("connection_label_connect");
                    HitToConnectString = localizationService.GetString("connection_hint_connect");
                    StateString = localizationService.GetString("connection_hint_connect");
                    break;

                case StateEnum.Starting:
                    ConnectButtonString = localizationService.GetString("connection_server_state_connecting");
                    StateString = localizationService.GetString("connection_server_state_connecting");
                    break;

                case StateEnum.Started:
                    ConnectButtonString = localizationService.GetString("connection_label_disconnect");
                    HitToConnectString = localizationService.GetString("connection_hint_disconnect");
                    StateString = localizationService.GetString("connection_hint_disconnect");
                    break;

                case StateEnum.Stopping:
                    ConnectButtonString = localizationService.GetString("connection_server_state_disconnecting");
                    StateString = localizationService.GetString("connection_server_state_disconnecting");
                    break;

                case StateEnum.Stopped:
                    ConnectButtonString = localizationService.GetString("connection_label_connect");
                    HitToConnectString = localizationService.GetString("connection_hint_connect");
                    StateString = localizationService.GetString("connection_hint_connect");
                    break;

                case StateEnum.Terminating:
                    ConnectButtonString = localizationService.GetString("connection_label_connect");
                    StateString = localizationService.GetString("connection_hint_connect");
                    HitToConnectString = string.Empty;
                    break;
            }
        }



        public void ClearErrorMessage()
        {
            connectionService.ClearErrorMessage();
        }

        #endregion
    }
}