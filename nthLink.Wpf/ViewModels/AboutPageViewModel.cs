using nthLink.Header.Interface;
using nthLink.SDK.Model;

namespace nthLink.Wpf.ViewModels
{
    internal class AboutPageViewModel : NotifyPropertyChangedBase
    {
        public string AppVersion { get; }
        public string AboutString { get; }

        public AboutPageViewModel(ILanguageService languageService, IClientInfo clientInfo)
        {
            AboutString = languageService.GetString("about_text")
                .Replace("\\r", "\r")
                .Replace("\\n", "\n");

            //about_version
            string str = languageService.GetString("about_version");

            if (string.IsNullOrEmpty(str))
            {
                str = "V %s";
            }

            AppVersion = str.Replace("%s", clientInfo.AppVersion.ToString());
        }
    }
}
