using nthLink.Header.Interface;
using nthLink.SDK.Model;

namespace nthLink.Wpf.ViewModels
{
    internal class FollowUsPageViewModel : NotifyPropertyChangedBase
    {
        public VisitItemViewModel[] FacebookItems { get; }
        public VisitItemViewModel[] InstagramItems { get; }

        public VisitItemViewModel YoutubeItem { get; }

        public string Telegram { get; } = "Telegram ID: @nthLinkVPN";

        public FollowUsPageViewModel(IWebBrowser webBrowser)
        {
            FacebookItems = new VisitItemViewModel[]
            {
                new VisitItemViewModel(webBrowser)
                {
                    Context = "English: nthLink VPN",
                    Url = "https://www.facebook.com/profile.php?id=61560873763629"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = "中文: CNnthLink",
                    Url = "https://www.facebook.com/CNnthLink"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = " فارسی: NthLinkIR",
                    Url = "https://www.facebook.com/NthLinkIR/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = "Русский: NthLinkRU",
                    Url = "https://www.facebook.com/NthLinkRU/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = " မြန်မာ: NthLinkMM",
                    Url = "https://www.facebook.com/NthLinkMM/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = "Español: NthlinkES",
                    Url = "https://www.facebook.com/NthlinkES/"
                },
            };
            InstagramItems = new VisitItemViewModel[]
            {
                new VisitItemViewModel(webBrowser)
                {
                    Context = "English: nthlink_vpn",
                    Url = "https://www.instagram.com/nthlink_vpn/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = "中文: cn_nthlink",
                    Url = "https://www.instagram.com/cn_nthlink/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = " فارسی: ir_nthlink",
                    Url = "https://www.instagram.com/ir_nthlink/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = "Русский: ru_nthlink",
                    Url = "https://www.instagram.com/ru_nthlink/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = " မြန်မာ: mm_nthlink",
                    Url = "https://www.instagram.com/mm_nthlink/"
                },
                new VisitItemViewModel(webBrowser)
                {
                    Context = "Español: es_nthlink",
                    Url = "https://www.instagram.com/es_nthlink/"
                },
            };

            YoutubeItem = new VisitItemViewModel(webBrowser)
            {
                Context = "Youtube: @nthLinkApp",
                Url = "https://www.youtube.com/@nthLinkApp"
            };
        }
    }
}
