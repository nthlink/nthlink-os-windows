using nthLink.Header;
using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Extension;
using nthLink.Wpf.Domain.Models;
using System;
using System.Threading.Tasks;

namespace nthLink.Wpf.Application.Services
{
    /// <summary>
    /// VPN連接服務實作 - 應用層
    /// 職責：處理VPN連接的業務邏輯，不涉及UI狀態管理
    /// </summary>
    public class ConnectionService : IConnectionService
    {
        private readonly IContainerProvider containerProvider;
        private readonly ISystemReportLog? systemReportLog;
        private readonly IMainThreadSyncContext mainThreadSyncContext;
        private readonly IDirectoryServerConfigProvider directoryServerConfigProvider;
        private readonly ConnectionState connectionState;

        public StateEnum CurrentState => connectionState.State;
        public bool IsConnected => connectionState.IsConnected;
        public string ServerStateMessage => connectionState.ServerMessage;
        public string ErrorMessage => connectionState.ErrorMessage;

        public event Action<StateEnum, bool>? ConnectionStateChanged;
        public event Action<string>? ErrorMessageChanged;

        public ConnectionService(
            IContainerProvider containerProvider,
            ISystemReportLog? systemReportLog,
            IMainThreadSyncContext mainThreadSyncContext,
            IDirectoryServerConfigProvider directoryServerConfigProvider)
        {
            this.containerProvider = containerProvider;
            this.systemReportLog = systemReportLog;
            this.mainThreadSyncContext = mainThreadSyncContext;
            this.directoryServerConfigProvider = directoryServerConfigProvider;
            this.connectionState = new ConnectionState(StateEnum.Waiting);

            // 訂閱VPN服務狀態變化事件
            SubscribeToVpnEvents();
        }

        public async Task<bool> ConnectAsync()
        {
            if (!connectionState.CanConnect())
            {
                return false;
            }

            try
            {
                await LogAsync(LogLevelEnum.Info, "Starting VPN connection");

                var functionEventBus = containerProvider.Resolve<IEventBus<VpnServiceFunctionArgs>>();
                if (functionEventBus != null)
                {
                    connectionState.ClearError();
                    NotifyErrorMessageChanged();

                    await LogAsync(LogLevelEnum.Info, "Publishing start command to VPN service");

                    await Task.Run(() =>
                    {
                        functionEventBus.Publish(Const.Channel.VpnService,
                            new VpnServiceFunctionArgs(FunctionEnum.Start));
                    });

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await LogAsync(LogLevelEnum.Error, $"Failed to start connection: {ex.Message}");
                connectionState.SetError($"Connection failed: {ex.Message}");
                NotifyErrorMessageChanged();
                return false;
            }
        }

        public async Task<bool> DisconnectAsync()
        {
            if (!connectionState.CanDisconnect())
            {
                return false;
            }

            try
            {
                await LogAsync(LogLevelEnum.Info, "Stopping VPN connection");

                var functionEventBus = containerProvider.Resolve<IEventBus<VpnServiceFunctionArgs>>();
                if (functionEventBus != null)
                {
                    await LogAsync(LogLevelEnum.Info, "Publishing stop command to VPN service");
                    
                    await Task.Run(() =>
                    {
                        functionEventBus.Publish(Const.Channel.VpnService,
                            new VpnServiceFunctionArgs(FunctionEnum.Stop));
                    });

                    await LogAsync(LogLevelEnum.Info, "VPN stop command published successfully");
                    return true;
                }

                await LogAsync(LogLevelEnum.Error, "Failed to resolve VPN function event bus for stop command");
                return false;
            }
            catch (Exception ex)
            {
                await LogAsync(LogLevelEnum.Error, $"Failed to stop connection: {ex.Message}");
                connectionState.SetError($"Disconnection failed: {ex.Message}");
                NotifyErrorMessageChanged();
                return false;
            }
        }

        public void ClearErrorMessage()
        {
            connectionState.ClearError();
            NotifyErrorMessageChanged();
        }

        /// <summary>
        /// 處理VPN服務狀態變化
        /// </summary>
        private void OnVpnServiceStateChanged(string channel, VpnServiceStateArgs args)
        {
            LogAsync(LogLevelEnum.Info, $"VPN service state changed: {args.State}, Message: {args.Message ?? "N/A"}");
            
            connectionState.UpdateState(args.State, args.Message);

            // 處理連接成功後的URL打開邏輯
            if (args.State == StateEnum.Started)
            {
                LogAsync(LogLevelEnum.Info, "VPN connection established successfully, handling post-connection actions");
                HandleConnectionSuccessActions();
            }
            else if (args.State == StateEnum.Stopped)
            {
                LogAsync(LogLevelEnum.Info, "VPN connection stopped");
            }
            else if (args.State == StateEnum.Terminating)
            {
                LogAsync(LogLevelEnum.Warning, $"VPN connection terminating: {args.Message ?? "Unknown reason"}");
            }

            // 在主執行緒中通知狀態變化
            mainThreadSyncContext.Post(() =>
            {
                ConnectionStateChanged?.Invoke(connectionState.State, connectionState.IsConnected);
            });
        }

        /// <summary>
        /// 處理連接成功後的動作
        /// </summary>
        private void HandleConnectionSuccessActions()
        {
            // 連接成功後的自動瀏覽器開啟邏輯
            if (directoryServerConfigProvider.DirectoryServerConfig != null &&
                !string.IsNullOrEmpty(directoryServerConfigProvider.DirectoryServerConfig.redirectUrl))
            {
                LogAsync(LogLevelEnum.Info, $"Scheduling redirect URL opening: {directoryServerConfigProvider.DirectoryServerConfig.redirectUrl}");
                
                Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(task =>
                {
                    mainThreadSyncContext.Post(() =>
                    {
                        try
                        {
                            LogAsync(LogLevelEnum.Info, "Opening redirect URL after connection success");
                            containerProvider.Resolve<IWebBrowser>().Unwrap()
                                .OpenUrl(directoryServerConfigProvider.DirectoryServerConfig.redirectUrl, EventSourceTypeEnum.Loading);
                        }
                        catch (Exception ex)
                        {
                            LogAsync(LogLevelEnum.Error, $"Failed to open redirect URL: {ex.Message}");
                        }
                    });
                });
            }
            else
            {
                LogAsync(LogLevelEnum.Info, "No redirect URL configured, skipping browser opening");
            }
        }



        /// <summary>
        /// 處理VPN服務錯誤
        /// </summary>
        private void OnRequestError(string channel, RequestErrorArgs args)
        {
            if (args.RequestError != null)
            {
                LogAsync(LogLevelEnum.Error, $"VPN service error received: {args.RequestError.message}");
                
                connectionState.SetError(args.RequestError.message);

                mainThreadSyncContext.Post(() =>
                {
                    NotifyErrorMessageChanged();
                });
            }
            else
            {
                LogAsync(LogLevelEnum.Warning, "Received empty request error from VPN service");
            }
        }

        /// <summary>
        /// 訂閱VPN相關事件
        /// </summary>
        private void SubscribeToVpnEvents()
        {
            LogAsync(LogLevelEnum.Info, "Subscribing to VPN service events");
            
            // 訂閱VPN狀態變化事件
            if (containerProvider.Resolve<IEventBus<VpnServiceStateArgs>>() is IEventBus<VpnServiceStateArgs> stateEventBus)
            {
                stateEventBus.Subscribe(Const.Channel.VpnService, OnVpnServiceStateChanged);
                LogAsync(LogLevelEnum.Info, "Successfully subscribed to VPN state change events");
            }
            else
            {
                LogAsync(LogLevelEnum.Error, "Failed to resolve VPN state event bus");
            }

            // 訂閱VPN錯誤事件
            if (containerProvider.Resolve<IEventBus<RequestErrorArgs>>() is IEventBus<RequestErrorArgs> errorEventBus)
            {
                errorEventBus.Subscribe(Const.Channel.VpnService, OnRequestError);
                LogAsync(LogLevelEnum.Info, "Successfully subscribed to VPN error events");
            }
            else
            {
                LogAsync(LogLevelEnum.Error, "Failed to resolve VPN error event bus");
            }
        }

        /// <summary>
        /// 記錄日誌
        /// </summary>
        private async Task LogAsync(LogLevelEnum level, string message)
        {
            if (systemReportLog != null)
            {
                await systemReportLog.Log(level, message);
            }
        }

        /// <summary>
        /// 通知錯誤訊息變化
        /// </summary>
        private void NotifyErrorMessageChanged()
        {
            ErrorMessageChanged?.Invoke(connectionState.ErrorMessage);
        }
    }
}