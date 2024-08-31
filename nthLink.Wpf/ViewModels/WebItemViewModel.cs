using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.SDK.Model;
using System;

namespace nthLink.Wpf.ViewModels
{
    internal class WebItemViewModel : NotifyPropertyChangedBase, ICanLoad
    {
        private string? url;
        private readonly IWebBrowser webBrowser;
        private readonly IMainThreadSyncContext mainThreadSyncContext;

        public event Action<ICanLoad>? Loaded;

        public string? Url
        {
            get { return this.url; }
            set
            {
                if (SetProperty(ref this.url, value))
                {
                    this.mainThreadSyncContext.Post(OpenUrlCommand.RaiseCanExecuteChanged);
                }
            }
        }

        public IRelayCommand OpenUrlCommand { get; }
        public WebItemViewModel(IWebBrowser webBrowser,
            IMainThreadSyncContext mainThreadSyncContext) : base()
        {
            OpenUrlCommand = new RelayCommand(OnOpenUrlCommandExecute, CanOpenUrlCommandExecute);
            this.webBrowser = webBrowser;
            this.mainThreadSyncContext = mainThreadSyncContext;
        }

        protected virtual void OnOpenUrlCommandExecute()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                this.webBrowser.OpenUrl(Url, EventSourceTypeEnum.User);
            }
        }

        private bool CanOpenUrlCommandExecute()
        {
            return !string.IsNullOrEmpty(Url);
        }

        public void RaiseLoadedEvent()
        {
            if (Loaded != null)
            {
                Loaded.Invoke(this);
            }
        }
    }
}
