using nthLink.Header.Interface;
using nthLink.SDK.Model;

namespace nthLink.Wpf.ViewModels
{
    internal class VisitItemViewModel : NotifyPropertyChangedBase
    {
        private readonly IWebBrowser webBrowser;
        private string context = string.Empty;

        public string Context
        {
            get { return this.context; }
            set { SetProperty(ref this.context, value); }
        }

        private string url = string.Empty;

        public string Url
        {
            get { return this.url; }
            set { SetProperty(ref this.url, value); }
        }

        public IRelayCommand VisitCommand { get; }

        public VisitItemViewModel(IWebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;

            VisitCommand = new RelayCommand(OnVisitCommandExecute);
        }

        private void OnVisitCommandExecute()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                this.webBrowser.OpenUrl(Url, Header.Enum.EventSourceTypeEnum.User);
            }
        }
    }
}
