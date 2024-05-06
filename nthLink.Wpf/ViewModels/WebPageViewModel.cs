using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace nthLink.Wpf.ViewModels
{
    internal class WebPageViewModel : NotifyPropertyChangedBase
    {
        private static readonly Random random1 = new Random(DateTime.Now.Millisecond % 8);
        private static readonly Random random2 = new Random(DateTime.Now.Millisecond % 88);

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

                for (int i = 0; i < headlineNewsCount; i++)
                {
                    int index = random1.Next(0, headlineNewsList.Count);
                    webItemsSource[i] = containerProvider.Resolve<WebItemViewModel>().Unwrap();
                    webItemsSource[i].Url = headlineNewsList[index].url;
                    headlineNewsList.RemoveAt(index);
                }

                WebItemsSource = webItemsSource;

                NewsItemViewModel[] newsItemViewModels = new NewsItemViewModel[directoryServerConfig.headlineNews.Count];

                for (int i = 0; i < directoryServerConfig.headlineNews.Count; i++)
                {
                    NewsItemViewModel item = this.containerProvider.Resolve<NewsItemViewModel>().Unwrap();
                    item.Preview = directoryServerConfig.headlineNews[i].title;
                    item.Url = directoryServerConfig.headlineNews[i].url;
                    if (!string.IsNullOrEmpty(directoryServerConfig.headlineNews[i].image))
                    {
                        item.ImageSource = await LoadImage(directoryServerConfig.headlineNews[i].image);
                    }
                    newsItemViewModels[i] = item;
                }

                NewsItemsSource = newsItemViewModels;
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
