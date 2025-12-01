using nthLink.Header.Enum;

namespace nthLink.Wpf.Domain.Models
{
    /// <summary>
    /// VPN連接狀態領域模型
    /// </summary>
    public class ConnectionState
    {
        public StateEnum State { get; private set; }
        public bool IsConnected => State == StateEnum.Started;
        public bool IsConnecting => State == StateEnum.Starting;
        public bool IsDisconnecting => State == StateEnum.Stopping;
        public string ErrorMessage { get; private set; } = string.Empty;
        public string ServerMessage { get; private set; } = string.Empty;

        public ConnectionState(StateEnum state)
        {
            State = state;
        }

        /// <summary>
        /// 更新連接狀態
        /// </summary>
        public void UpdateState(StateEnum newState, string message = "")
        {
            State = newState;
            ServerMessage = message;
            
            // 狀態變更時清除錯誤訊息
            if (newState == StateEnum.Started || newState == StateEnum.Starting)
            {
                ErrorMessage = string.Empty;
            }
        }

        /// <summary>
        /// 設置錯誤訊息
        /// </summary>
        public void SetError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 清除錯誤訊息
        /// </summary>
        public void ClearError()
        {
            ErrorMessage = string.Empty;
        }

        /// <summary>
        /// 是否可以開始連接
        /// </summary>
        public bool CanConnect()
        {
            return State == StateEnum.Waiting || State == StateEnum.Stopped;
        }

        /// <summary>
        /// 是否可以中斷連接
        /// </summary>
        public bool CanDisconnect()
        {
            return State == StateEnum.Started || State == StateEnum.Starting;
        }
    }
} 