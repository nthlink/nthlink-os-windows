using nthLink.Header.Enum;
using nthLink.Header.Interface;
using nthLink.SDK.Model;

namespace nthLink.Wpf.ViewModels
{
    internal class WebViewModel : NotifyPropertyChangedBase
    {
        private string? url;
        protected readonly IEventSource eventSource;

        public virtual string? Url
        {
            get { return this.url; }
            set { SetProperty(ref this.url, value); }
        }
        public WebViewModel(IEventSource eventSource)
        {
            this.eventSource = eventSource;
        }

        public virtual void RaiseLoadedEvent()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                this.eventSource.AddSource(EventTypeEnum.OpenUrl, Url, EventSourceTypeEnum.MiniBrowser);
            }
        }
    }
}
