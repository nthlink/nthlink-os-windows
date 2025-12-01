using nthLink.Header.Interface;
using System.Globalization;

namespace nthLink.Wpf.Application.Services
{
    /// <summary>
    /// 本地化服務介面 - 應用層
    /// </summary>
    public interface ILocalizationService : ILanguageService
    {
        /// <summary>
        /// 目前文化資訊
        /// </summary>
        CultureInfo CurrentCulture { get; }

        /// <summary>
        /// 獲取本地化字串
        /// </summary>
        /// <param name="key">字串鍵</param>
        /// <returns>本地化字串</returns>
        string GetString(string key);

        /// <summary>
        /// 獲取預設文化的字串
        /// </summary>
        /// <param name="key">字串鍵</param>
        /// <returns>預設文化字串</returns>
        string GetStringWithDefaultCulture(string key);

        /// <summary>
        /// 檢查是否支援指定文化
        /// </summary>
        /// <param name="culture">文化資訊</param>
        /// <returns>是否支援</returns>
        bool IsCultureSupported(CultureInfo culture);
    }
}