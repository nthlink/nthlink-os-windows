using nthLink.SDK.Model;

namespace nthLink.Wpf.ViewModels
{
    internal class WebViewModel : NotifyPropertyChangedBase
    {
        private string? url;

        public virtual string? Url
        {
            get { return this.url; }
            set { SetProperty(ref this.url, value); }
        }
        public WebViewModel()
        {
        }

        public virtual void RaiseLoadedEvent()
        {
        }
    }
}
