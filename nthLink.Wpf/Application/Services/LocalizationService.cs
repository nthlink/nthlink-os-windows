using nthLink.Header.Interface;
using nthLink.Wpf.Infrastructure.Localization;
using nthLink.Wpf.Model;
using System.Collections.Generic;
using System.Globalization;

namespace nthLink.Wpf.Application.Services
{
    /// <summary>
    /// 本地化服務實作 - 應用層
    /// 職責：處理本地化業務邏輯，不直接處理資源檔案讀取
    /// </summary>
    public class LocalizationService : ILocalizationService, ILanguageService
    {
        private readonly ResourceProvider resourceProvider;
        private readonly Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
        private readonly Dictionary<string, string> defaultStringDictionary = new Dictionary<string, string>();

        public CultureInfo CurrentCulture { get; }

        public LocalizationService(ResourceProvider resourceProvider)
        {
            this.resourceProvider = resourceProvider;

            // 初始化預設語言（英語）
            var defaultLanguageResources = this.resourceProvider.GetDefaultLanguageResources();
            InitializeStringDictionary(this.defaultStringDictionary, defaultLanguageResources);

#if DEBUG
            CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var languageResources = this.resourceProvider.GetLanguageResources(CurrentCulture);
#else
            CurrentCulture = CultureInfo.CurrentUICulture;
            var languageResources = this.resourceProvider.GetLanguageResources(CurrentCulture);
#endif

            InitializeStringDictionary(this.stringDictionary, languageResources);
        }

        public string GetString(string stringKey)
        {
            if (this.stringDictionary != null && this.stringDictionary.ContainsKey(stringKey))
            {
                if (this.stringDictionary[stringKey] is string result)
                {
                    // 處理 @string/ 引用
                    if (result.StartsWith("@string/"))
                    {
                        return GetString(result.Substring("@string/".Length));
                    }

                    // 處理轉義字符
                    return result.Replace("\\'", "'");
                }
            }

            // 回退到預設語言
            return GetStringWithDefaultCulture(stringKey);
        }

        public string GetStringWithDefaultCulture(string stringKey)
        {
            if (this.defaultStringDictionary != null && this.defaultStringDictionary.ContainsKey(stringKey))
            {
                if (this.defaultStringDictionary[stringKey] is string result)
                {
                    // 處理 @string/ 引用
                    if (result.StartsWith("@string/"))
                    {
                        return GetString(result.Substring("@string/".Length));
                    }

                    // 處理轉義字符
                    return result.Replace("\\'", "'");
                }
            }

            // 如果都找不到，返回鍵本身
            return stringKey;
        }

        public bool IsCultureSupported(CultureInfo culture)
        {
            var supportedCultures = resourceProvider.GetSupportedCultures();
            foreach (var supportedCulture in supportedCultures)
            {
                if (supportedCulture.Name.Equals(culture.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 初始化字串字典
        /// </summary>
        private void InitializeStringDictionary(Dictionary<string, string> dictionary, LanguageResources? languageResources)
        {
            dictionary.Clear();

            if (languageResources == null)
            {
                // 提供硬編碼的預設值作為最後的回退
                AddDefaultStrings(dictionary);
                return;
            }

            if (languageResources.String != null)
            {
                foreach (var stringItem in languageResources.String)
                {
                    if (!string.IsNullOrEmpty(stringItem.Name) && !string.IsNullOrEmpty(stringItem.Text))
                    {
                        dictionary[stringItem.Name] = stringItem.Text;
                    }
                }
            }

            // 處理 StringArray - 與原始 LanguageService 保持一致
            if (languageResources.StringArray != null)
            {
                foreach (var stringArrayItem in languageResources.StringArray)
                {
                    if (!string.IsNullOrEmpty(stringArrayItem.Name) && stringArrayItem.Item != null)
                    {
                        for (int j = 0; j < stringArrayItem.Item.Count; j++)
                        {
                            dictionary[$"{stringArrayItem.Name}_{j}"] = stringArrayItem.Item[j];
                        }
                    }
                }
            }

            // 如果字典為空，添加預設字串
            if (dictionary.Count == 0)
            {
                AddDefaultStrings(dictionary);
            }
        }

        /// <summary>
        /// 添加預設字串 - 作為最後的回退機制，與原始LanguageService保持一致
        /// </summary>
        private void AddDefaultStrings(Dictionary<string, string> dictionary)
        {
            dictionary.Add("privacy_notice_title", "Your privacy comes first");
            dictionary.Add("privacy_notice_text", "nthLink DOES NOT collect personally identifiable information or websites / online services that you visit. nthLink uses your IP address, device's default language, cookies, and basic device information ( e.g. operating system information ) to customize the services that you receive such as locating the nearest servers that can best support your device and displaying the proper web page upon connecting to the nthLink server.");
            dictionary.Add("privacy_notice_submit", "Accept and continue");
            dictionary.Add("connection_hint_connect", "Tap To Connect");
            dictionary.Add("connection_hint_disconnect", "Tap To Disconnect");
            dictionary.Add("connection_label_connect", "Connect");
            dictionary.Add("connection_label_disconnect", "Disconnect");
            dictionary.Add("connection_server_state_connected", "Connected");
            dictionary.Add("connection_server_state_connecting", "Connecting…");
            dictionary.Add("connection_server_state_disconnecting", "Disconnecting…");
            dictionary.Add("connection_error", "Connection error");
            dictionary.Add("feedback_page_title", "Feedback");
            dictionary.Add("feedback_issue_category_title", "Issue category");
            dictionary.Add("feedback_description_title", "Description");
            dictionary.Add("feedback_note_text", "Your description, email address (if provided), and additional information will be sent to the nthLink team. Please refer to the nthLink website\'s Privacy Policy for our data collection policy.");
            dictionary.Add("feedback_submit", "Send");
            dictionary.Add("feedback_submit_failed_message", "Can\'t connect to nthlink server.");
            dictionary.Add("feedback_submit_success_message", "Thanks for helping us improve! We love hearing from you.");
            dictionary.Add("feedback_init_failed_message", "Don\'t have a server");
            dictionary.Add("about_page_title", "About");
            dictionary.Add("about_version", "Version %s");
            dictionary.Add("about_text", "We are a group of experienced software and information security engineers who started nthLink in 2016 to support human rights lawyers who needed to obtain restricted information and to express their perspectives. Since then we've made the service available to the wider public. Our development team are experts in both network security technology and service reliability.\n\nOur service is free and will remain free thanks to our sponsors and partners. They are:\n• Open Technology Fund\n• Google Jigsaw\n• Cure53\n• Include Security\n• Plaintext Design\n\nOur development team excels in both the sophistication of censorship circumvention technology and the reliability of the service. With years of experience in this specialty area, we provide the users in targeted geographies simple, safe, and reliable access to otherwise censored information.");
            dictionary.Add("menu_drawer_item_1", "Home");
            dictionary.Add("menu_drawer_item_2", "@string/feedback_page_title");
            dictionary.Add("menu_drawer_item_3", "@string/about_page_title");
            dictionary.Add("menu_drawer_item_4", "Help");
            dictionary.Add("menu_drawer_item_5", "Rate App");
            dictionary.Add("menu_drawer_item_6", "Privacy Policy");
            dictionary.Add("menu_web_item_1", "Copy link");
            dictionary.Add("menu_web_item_2", "Open in browser");
            dictionary.Add("menu_web_item_3", "Share link");
            dictionary.Add("menu_connection_item_1", "Landing Page");
            dictionary.Add("word_loading", "Loading");
            dictionary.Add("word_coped_link", "Coped link");

            dictionary.Add("issue_categories_0", "General feedback");
            dictionary.Add("issue_categories_1", "Cannot connect");
            dictionary.Add("issue_categories_2", "Connection speed is slow");
            dictionary.Add("issue_categories_3", "Suggestions");
            dictionary.Add("issue_categories_4", "Other");
            dictionary.Add("copied", "Copied");
            dictionary.Add("paste_id_telegram", "Please paste ID on Telegram.");
            dictionary.Add("update", "Update");
            dictionary.Add("download_new_version", "Do you want to download the new version of nthLink?");
            dictionary.Add("last_version", "Your nthLink is the last version.");
            dictionary.Add("visit", "Visit");
            dictionary.Add("follow_us", "Follow us");

            dictionary.Add("menu_drawer_item_11", "Diagnostic");
            dictionary.Add("diagnostic_button", "Start Diagnostics");
            dictionary.Add("diagnostic_info", "At nthLink, we occasionally seek your assistance in enhancing our service by providing diagnostic data.\r\n\r\nBy clicking the button, the diagnostics process begins, collecting information and transmitting it to our servers. Rest assured, all information gathered is completely anonymous.");
            dictionary.Add("diagnostic_thanks", "Thanks for helping us improve! We love hearing from you");
            dictionary.Add("diagnostic_is_connected", "The server is connected. Please disconnect it before running diagnostics");
        }
    }
} 