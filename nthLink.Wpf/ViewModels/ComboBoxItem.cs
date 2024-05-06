using nthLink.SDK.Model;

namespace nthLink.Wpf.ViewModels
{
    internal class ComboBoxItem : NotifyPropertyChangedBase
    {
        private string display = string.Empty;

        public string Display
        {
            get { return this.display; }
            set { SetProperty(ref this.display, value); }
        }

        private object? value;

        public object? Value
        {
            get { return this.value; }
            set { SetProperty(ref this.value, value); }
        }
    }
}
