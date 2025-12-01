using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.SDK.Model;
using System;

namespace nthLink.Wpf.ViewModels
{
    internal class WebItemViewModel : WebViewModel, ICanLoad
    {
        private string? url;
        private readonly IWebBrowser webBrowser;
        private readonly IMainThreadSyncContext mainThreadSyncContext;

        public event Action<ICanLoad>? Loaded;

        public override string? Url
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
            IMainThreadSyncContext mainThreadSyncContext,
            IEventSource eventSource) : base(eventSource)
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

        public override void RaiseLoadedEvent()
        {
            base.RaiseLoadedEvent();

            if (Loaded != null)
            {
                Loaded.Invoke(this);
            }
        }
    }
}
