using nthLink.Header.Interface;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace nthLink.Wpf.ViewModels
{
    internal class NewsItemViewModel : WebItemViewModel
    {
        private static readonly BitmapImage? defaultImageSource;

        static NewsItemViewModel()
        {
            using (Stream? stream = typeof(App).Assembly.GetManifestResourceStream($"{nameof(nthLink)}.{nameof(Wpf)}.Resource.Image.News.png"))
            {
                if (stream != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();

                    defaultImageSource = bitmapImage;
                }
            }
        }


        private string? preview;

        public string? Preview
        {
            get { return this.preview; }
            set { SetProperty(ref this.preview, value); }
        }

        private ImageSource? imageSource = defaultImageSource;

        public ImageSource? ImageSource
        {
            get { return this.imageSource; }
            set { SetProperty(ref this.imageSource, value); }
        }
        public int Rate { get; set; }
        public string[]? Categories { get; set; }
        public NewsItemViewModel(IWebBrowser webBrowser,
            IMainThreadSyncContext mainThreadSyncContext)
            : base(webBrowser, mainThreadSyncContext)
        {

        }
    }
}
