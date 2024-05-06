using Microsoft.Win32;
using nthLink.Header.Enum;
using nthLink.Header.Interface;
using System.Diagnostics;

namespace nthLink.Wpf.Model
{
    internal class WebBrowserImp : IWebBrowser
    {
        public WebBrowserImp()
        {
        }
        public void OpenUrl(string url, EventSourceTypeEnum eventSourceType)
        {
            if (!string.IsNullOrEmpty(url))
            {
                OpenBrowserProcess(url);
            }
        }

        public void OpenUrlWithoutReport(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                OpenBrowserProcess(url);
            }
        }

        private static void OpenBrowserProcess(string url)
        {
            url = url.Replace("&", "^&");

            try
            {
                string progid = GetProgid(GetUrlProtocol(url));

                if (progid == string.Empty)
                {
                    progid = "HTTP";
                }

                RegistryKey? browserKey = Registry.ClassesRoot
                    .OpenSubKey($@"{progid}\shell\open\command", false);

                if (browserKey != null)
                {
                    if (browserKey.GetValue(null) is string browserPath)
                    {
                        browserPath = browserPath.ToLower().Replace("\"", string.Empty);

                        string privateModeParam = string.Empty;

                        if (browserPath.Contains("chrome"))
                        {
                            privateModeParam = $"-incognito {url}";
                        }
                        else
                        if (browserPath.Contains("edge"))
                        {
                            privateModeParam = $"-inprivate {url}";
                        }
                        else
                        if (browserPath.Contains("firefox"))
                        {
                            privateModeParam = $"-private-window {url}";
                        }
                        else
                        if (browserPath.Contains("iexplore") ||
                            browserPath.Contains("opera"))
                        {
                            privateModeParam = $"-private {url}";
                        }
                        else
                        {
                            privateModeParam = url;
                        }

                        string processPath = browserPath.Substring(0, browserPath.LastIndexOf(".exe") + 4);

                        Process.Start(processPath, browserPath.Replace(processPath, string.Empty).Replace("%1", privateModeParam));
                    }

                    browserKey.Close();
                }
            }
            catch
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private static string GetUrlProtocol(string url)
        {
            if (url.Contains("https"))
            {
                return "https";
            }
            else
            {
                return "http";
            }
        }

        private static string GetProgid(string protocol)
        {
            using (RegistryKey? userChoiceKey = Registry.CurrentUser
                .OpenSubKey($@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\{protocol}\UserChoice", false))
            {
                if (userChoiceKey != null &&
                    userChoiceKey.GetValue("Progid") is string progid)
                {
                    return progid;
                }
            }

            return string.Empty;
        }
    }
}
