#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。using System.Collections.Generic;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace nthLink.Wpf.Model
{
    [XmlRoot(ElementName = "string")]
    public class StringItem
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name;

        [XmlAttribute(AttributeName = "translatable")]
        public string Translatable { get; set; }

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "string-array")]
    public class StringArrayItem
    {
        [XmlElement(ElementName = "item")]
        public List<string> Item;

        [XmlAttribute(AttributeName = "name")]
        public string Name;

        [XmlText]
        public string Text;
    }

    [XmlRoot(ElementName = "resources")]
    public class LanguageResources
    {
        [XmlElement(ElementName = "string")]
        public List<StringItem> String;

        [XmlElement(ElementName = "string-array")]
        public List<StringArrayItem> StringArray;
    }
}
#pragma warning restore CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。