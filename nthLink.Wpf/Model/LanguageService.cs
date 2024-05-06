using nthLink.Header.Interface;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace nthLink.Wpf.Model
{
    public class LanguageService : ILanguageService
    {
        private readonly Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
        private readonly Dictionary<string, string> defaultStringDictionary = new Dictionary<string, string>();

        public LanguageService()
        {
            LanguageResources? defaultLanguageResources = GetLanguageResources(CultureInfo.GetCultureInfo("en-US"));

            InitializeStringDictionary(this.defaultStringDictionary, defaultLanguageResources);

            LanguageResources? languageResources = GetLanguageResources(CultureInfo.CurrentUICulture);

            InitializeStringDictionary(this.stringDictionary, languageResources);
        }

        private void InitializeStringDictionary(Dictionary<string, string> dictionary, LanguageResources? languageResources)
        {
            dictionary.Clear();

            if (languageResources == null)
            {
                dictionary.Add("privacy_notice_title", "Your privacy comes first");
                dictionary.Add("privacy_notice_text", "nthLink DOES NOT collect personally identifiable information or websites / online services that you visit. nthLink uses your IP address, device’s default language, cookies, and basic device information ( e.g. operating system information ) to customize the services that you receive such as locating the nearest servers that can best support your device and displaying the proper web page upon connecting to the nthLink server.");
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
                dictionary.Add("about_text", "nthLink was started in 2016 as a project by a group of experienced software and information security engineers to support human rights lawyers to obtain censored information and to express their perspectives to the outside world.\n\nOur development team excels in both the sophistication of censorship circumvention technology and the reliability of the service. With years of experience in this specialty area, we provide the users in targeted geographies simple, safe, and reliable access to otherwise censored information.");
                dictionary.Add("menu_drawer_item_1", "Home");
                dictionary.Add("menu_drawer_item_2", "@string/feedback_page_title");
                dictionary.Add("menu_drawer_item_3", "@string/about_page_title");
                dictionary.Add("menu_drawer_item_4", "Help");
                dictionary.Add("menu_drawer_item_5", "Rate App");
                dictionary.Add("menu_web_item_1", "Copy link");
                dictionary.Add("menu_web_item_2", "Open in browser");
                dictionary.Add("menu_web_item_3", "Share link");
                dictionary.Add("menu_connection_item_1", "Landing Page");
                dictionary.Add("word_loading", "Loading");
                dictionary.Add("word_coped_link", "Coped link");
                //dictionary.Add("word_play_preview_1", "打破障礙釋放聲音");
                //dictionary.Add("word_play_preview_2", "強加密保護信息");
                //dictionary.Add("word_play_preview_3", "安全可靠地訪問 Internet");
                //dictionary.Add("word_play_preview_4", "安全地查看世界各地的新聞");
                dictionary.Add("issue_categories_0", "General feedback");
                dictionary.Add("issue_categories_1", "Cannot connect");
                dictionary.Add("issue_categories_2", "Connection speed is slow");
                dictionary.Add("issue_categories_3", "Suggestions");
                dictionary.Add("issue_categories_4", "Other");
            }
            else
            {
                for (int i = 0; i < languageResources.String.Count; i++)
                {
                    dictionary.Add(languageResources.String[i].Name, languageResources.String[i].Text);
                }

                for (int i = 0; i < languageResources.StringArray.Count; i++)
                {
                    for (int j = 0; j < languageResources.StringArray[i].Item.Count; j++)
                    {
                        dictionary.Add($"{languageResources.StringArray[i].Name}_{j}", languageResources.StringArray[i].Item[j]);
                    }
                }
            }
        }

        private LanguageResources? GetLanguageResources(CultureInfo cultureInfo)
        {
            Assembly assembly = typeof(App).Assembly;

            foreach (var item in assembly.GetManifestResourceNames())
            {
                if (item.Contains(cultureInfo.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream? stream = assembly.GetManifestResourceStream(item))
                    {
                        if (stream != null)
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(stream);

                            foreach (XmlNode node in xmlDocument.ChildNodes)
                            {
                                if (node.Name == "resources")
                                {
                                    string xml = "<resources>" + node.InnerXml + "</resources>";
                                    XmlSerializer serializer = new XmlSerializer(typeof(LanguageResources));
                                    using (StringReader reader = new StringReader(xml))
                                    {
                                        return serializer.Deserialize(reader) as LanguageResources;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
        public string GetString(string stringKey)
        {
            if (this.stringDictionary != null)
            {
                if (this.stringDictionary.ContainsKey(stringKey))
                {
                    if (this.stringDictionary[stringKey] is string result)
                    {
                        if (result.StartsWith("@string/"))
                        {
                            return GetString(result.Substring("@string/".Length));
                        }

                        return result.Replace("\\'", "'");
                    }
                }
            }
            return stringKey;
        }

        public string GetStringWithDefaultCulture(string stringKey)
        {
            if (this.defaultStringDictionary != null)
            {
                if (this.defaultStringDictionary.ContainsKey(stringKey))
                {
                    if (this.defaultStringDictionary[stringKey] is string result)
                    {
                        if (result.StartsWith("@string/"))
                        {
                            return GetString(result.Substring("@string/".Length));
                        }

                        return result.Replace("\\'", "'");
                    }
                }
            }
            return stringKey;
        }
    }
}
