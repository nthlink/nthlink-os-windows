using nthLink.Header.Interface;

namespace nthLink.Wpf.ViewModels
{
    internal class NotifyItemViewModel : WebItemViewModel
    {
        private string? notifyString;

        public NotifyItemViewModel(IWebBrowser webBrowser, IMainThreadSyncContext mainThreadSyncContext, IEventSource eventSource)
            : base(webBrowser, mainThreadSyncContext, eventSource)
        {
        }

        public string? NotifyString
        {
            get { return this.notifyString; }
            set { SetProperty(ref this.notifyString, value); }
        }
    }
}
