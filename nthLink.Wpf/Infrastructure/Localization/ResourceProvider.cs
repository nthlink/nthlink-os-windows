using nthLink.Wpf.Model;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace nthLink.Wpf.Infrastructure.Localization
{
    /// <summary>
    /// 資源提供者 - 基礎設施層
    /// </summary>
    public class ResourceProvider
    {
        private readonly Assembly assembly;

        public ResourceProvider()
        {
            assembly = typeof(App).Assembly;
        }

        /// <summary>
        /// 獲取指定文化的語言資源
        /// </summary>
        public LanguageResources? GetLanguageResources(CultureInfo cultureInfo)
        {
            foreach (var item in assembly.GetManifestResourceNames())
            {
                if (item.Contains(cultureInfo.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream? stream = assembly.GetManifestResourceStream(item))
                    {
                        if (stream != null)
                        {
                            return ParseLanguageResource(stream);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 獲取預設語言資源（英語）
        /// </summary>
        public LanguageResources? GetDefaultLanguageResources()
        {
            return GetLanguageResources(CultureInfo.GetCultureInfo("en-US"));
        }

        /// <summary>
        /// 獲取支援的文化清單
        /// </summary>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var supportedCultures = new List<CultureInfo>();

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.Contains("_strings.xml"))
                {
                    // 從資源名稱中提取文化代碼
                    // 例如: d_082c3a741v2_zh-CN_strings.xml -> zh-CN
                    var culturePart = ExtractCultureFromResourceName(resourceName);
                    if (!string.IsNullOrEmpty(culturePart))
                    {
                        try
                        {
                            supportedCultures.Add(CultureInfo.GetCultureInfo(culturePart));
                        }
                        catch
                        {
                            // 忽略無效的文化代碼
                        }
                    }
                }
            }

            return supportedCultures;
        }

        private LanguageResources? ParseLanguageResource(Stream stream)
        {
            try
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
            catch
            {
                // 忽略解析錯誤
            }

            return null;
        }

        private string ExtractCultureFromResourceName(string resourceName)
        {
            // 從形如 "d_082c3a741v2_zh-CN_strings.xml" 的名稱中提取 "zh-CN"
            var parts = resourceName.Split('_');
            if (parts.Length >= 3)
            {
                var culturePart = parts[parts.Length - 2]; // 倒數第二部分
                if (culturePart != "strings") // 排除英語預設檔案
                {
                    return culturePart;
                }
            }

            return string.Empty;
        }
    }
} 