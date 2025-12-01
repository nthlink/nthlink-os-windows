using nthLink.Header;
using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using nthLink.Wpf.Model;
using nthLink.Wpf.Struct;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace nthLink.Wpf.ViewModels
{
    internal class WebPageViewModel : NotifyPropertyChangedBase
    {
        private static readonly Random random1 = new Random(DateTime.Now.Millisecond % 8);

        private WebItemViewModel[]? notifyMessageItemsSource;

        public WebItemViewModel[]? NotifyMessageItemsSource
        {
            get { return this.notifyMessageItemsSource; }
            set { SetProperty(ref this.notifyMessageItemsSource, value); }
        }

        private WebItemViewModel[]? webItemsSource;

        public WebItemViewModel[]? WebItemsSource
        {
            get { return this.webItemsSource; }
            set { SetProperty(ref this.webItemsSource, value); }
        }

        private NewsItemViewModel[]? newsItemsSource;

        public NewsItemViewModel[]? NewsItemsSource
        {
            get { return this.newsItemsSource; }
            set
            {
                if (SetProperty(ref this.newsItemsSource, value))
                {
                    this.webIndex = 0;
                }
            }
        }

        private int webIndex;
        private readonly IContainerProvider containerProvider;
        private readonly IMainThreadSyncContext mainThreadSyncContext;

        private readonly SimpleTimer timer = new SimpleTimer();

        public WebPageViewModel(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;

            IDirectoryServerConfigProvider directoryServerConfigProvider = containerProvider.Resolve<IDirectoryServerConfigProvider>().Unwrap();
            if (directoryServerConfigProvider.DirectoryServerConfig != null)
            {
                MakeItemsSource(directoryServerConfigProvider.DirectoryServerConfig);
            }
            directoryServerConfigProvider.PropertyChanged += DirectoryServerConfigProvider_PropertyChanged;

            this.mainThreadSyncContext = containerProvider.Resolve<IMainThreadSyncContext>().Unwrap();

            this.timer.Interval = 3 * 60 * 1000;
            this.timer.Ticks += Timer_Ticks;

            if (containerProvider.Resolve<IEventBus<VpnServiceStateArgs>>()
                               is IEventBus<VpnServiceStateArgs> eventBus)
            {
                eventBus.Subscribe(Const.Channel.VpnService, OnVpnServiceStateChanged);
            }

            if (containerProvider.Resolve<IEventBus<AppEventArgs>>()
                              is IEventBus<AppEventArgs> appEvent)
            {
                appEvent.Subscribe(AppEventArgs.AppEventArgsMessage.AppEvent, OnAppEventReceived);
            }
        }

        private void OnAppEventReceived(string s, AppEventArgs args)
        {
            if (args.Message == AppEventArgs.AppEventArgsMessage.WindowActivated)
            {
                this.mainThreadSyncContext.Post(ReloadWebItem);
            }
        }

        private void Timer_Ticks()
        {
            this.mainThreadSyncContext.Post(ReloadWebItem);
        }

        private void OnVpnServiceStateChanged(string s, VpnServiceStateArgs args)
        {
            if (args.State == Header.Enum.StateEnum.Started)
            {
                this.timer.Start();
            }
            else if (args.State == Header.Enum.StateEnum.Stopped ||
                args.State == Header.Enum.StateEnum.Terminating)
            {
                this.timer.Stop();
            }
        }

        private void ReloadWebItem()
        {
            WebItemViewModel[]? itemsSource = WebItemsSource;
            WebItemsSource = null;
            WebItemsSource = itemsSource;
        }

        private void DirectoryServerConfigProvider_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is IDirectoryServerConfigProvider directoryServerConfigProvider)
            {
                if (directoryServerConfigProvider.DirectoryServerConfig != null)
                {
                    MakeItemsSource(directoryServerConfigProvider.DirectoryServerConfig);
                }
            }
        }

        private void MakeItemsSource(DirectoryServerConfig directoryServerConfig)
        {
            this.mainThreadSyncContext.Post(async () =>
            {
                NotifyItemViewModel[] notifyMessageItemsSource = new NotifyItemViewModel[directoryServerConfig.notifications.Count];

                for (int i = 0; i < directoryServerConfig.notifications.Count; i++)
                {
                    NotifyItemViewModel item = this.containerProvider.Resolve<NotifyItemViewModel>().Unwrap();
                    item.NotifyString = directoryServerConfig.notifications[i].title;
                    item.Url = directoryServerConfig.notifications[i].url;
                    notifyMessageItemsSource[i] = item;
                }

                NotifyMessageItemsSource = notifyMessageItemsSource;

                const int maximumNewsCount = 4;

                int headlineNewsCount = directoryServerConfig.headlineNews.Count > maximumNewsCount ?
                maximumNewsCount : directoryServerConfig.headlineNews.Count;

                WebItemViewModel[] webItemsSource = new WebItemViewModel[headlineNewsCount];

                List<HeadlineNews> headlineNewsList = new List<HeadlineNews>(directoryServerConfig.headlineNews);
                bool isPinned = false;
                for (int i = 0; i < headlineNewsList.Count; i++)
                {
                    if (headlineNewsList[i].pinToTop)
                    {
                        isPinned = true;
                        webItemsSource[0] = containerProvider.Resolve<WebItemViewModel>().Unwrap();
                        webItemsSource[0].Url = headlineNewsList[i].url;
                        headlineNewsList.RemoveAt(i);
                        break;
                    }
                }

                for (int i = isPinned ? 1 : 0; i < headlineNewsCount; i++)
                {
                    int index = random1.Next(0, headlineNewsList.Count);
                    webItemsSource[i] = containerProvider.Resolve<WebItemViewModel>().Unwrap();
                    webItemsSource[i].Url = headlineNewsList[index].url;
                    headlineNewsList.RemoveAt(index);
                }

                WebItemsSource = webItemsSource;

                List<NewsItemViewModel> newsItemViewModels =
                new List<NewsItemViewModel>(directoryServerConfig.headlineNews.Count);

                ICategoriesRater? categoriesRater =
                this.containerProvider.Resolve<ICategoriesRater>();

                IClientInfo? clientInfo =
              this.containerProvider.Resolve<IClientInfo>();

                for (int i = 0; i < directoryServerConfig.headlineNews.Count; i++)
                {
                    NewsItemViewModel item = this.containerProvider.Resolve<NewsItemViewModel>().Unwrap();
                    item.Preview = directoryServerConfig.headlineNews[i].title;
                    item.Url = directoryServerConfig.headlineNews[i].url;
                    if (!string.IsNullOrEmpty(directoryServerConfig.headlineNews[i].image))
                    {
                        item.ImageSource = await LoadImage(directoryServerConfig.headlineNews[i].image);
                    }

                    if (categoriesRater != null &&
                    directoryServerConfig.headlineNews[i].categories != null &&
                    clientInfo != null)
                    {
                        item.Rate = categoriesRater.GetRate(clientInfo.FavoriteCategories,
                            directoryServerConfig.headlineNews[i].categories);

                        item.Categories = directoryServerConfig.headlineNews[i].categories.ToArray();
                    }

                    newsItemViewModels.Add(item);
                }

                //Order by rate
                //newsItemViewModels.Sort((x, y) => x.Rate > y.Rate ? 1 : -1);

                NewsItemsSource = newsItemViewModels.ToArray();
            });
        }
        private async Task<BitmapImage> LoadImage(string url)
        {
            HttpClient client = new HttpClient();
            BitmapImage img = new BitmapImage();

            try
            {
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.BeginInit();
                img.StreamSource = await client.GetStreamAsync(url);
                img.EndInit();
                return img;
            }
            catch (HttpRequestException)
            {
                // the download failed, log error
                return img;
            }
        }
        public WebItemViewModel? GetWebItemViewModel()
        {
            if (this.webItemsSource != null && this.webItemsSource.Length > 0)
            {
                if (this.webIndex >= this.webItemsSource.Length)
                {
                    this.webIndex = 0;
                }

                WebItemViewModel webItemViewModel = this.webItemsSource[this.webIndex];

                this.webIndex++;

                return webItemViewModel;
            }
            else
            {
                return null;
            }
        }
    }
}
