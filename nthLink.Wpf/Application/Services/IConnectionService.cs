using nthLink.Header.Enum;
using System.Threading.Tasks;

namespace nthLink.Wpf.Application.Services
{
    /// <summary>
    /// VPN連接服務介面 - 應用層
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// 目前連接狀態
        /// </summary>
        StateEnum CurrentState { get; }

        /// <summary>
        /// 是否已連接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 伺服器狀態訊息
        /// </summary>
        string ServerStateMessage { get; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// 開始連接
        /// </summary>
        Task<bool> ConnectAsync();

        /// <summary>
        /// 中斷連接
        /// </summary>
        Task<bool> DisconnectAsync();

        /// <summary>
        /// 清除錯誤訊息
        /// </summary>
        void ClearErrorMessage();

        /// <summary>
        /// 連接狀態變化事件
        /// </summary>
        event System.Action<StateEnum, bool> ConnectionStateChanged;

        /// <summary>
        /// 錯誤訊息變化事件
        /// </summary>
        event System.Action<string> ErrorMessageChanged;
    }
} 