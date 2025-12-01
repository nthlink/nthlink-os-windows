using nthLink.SDK.Model;
using nthLink.Wpf.Interface;

namespace nthLink.Wpf.ViewModels
{
    internal class LoadAnimationViewModel : NotifyPropertyChangedBase, ILoadAnimation
    {
        private bool isVisible;

        public bool IsVisible
        {
            get { return this.isVisible; }
            private set { SetProperty(ref this.isVisible, value); }
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }
    }
}


