using nthLink.Header.Interface;
using nthLink.SDK.Model;
using nthLink.Wpf.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace nthLink.Wpf.ViewModels
{
    internal class DialogPageViewModel : NotifyPropertyChangedBase, IDialogBox
    {
        private bool result = false;

        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        private string title = string.Empty;

        public string Title
        {
            get { return this.title; }
            set { SetProperty(ref this.title, value); }
        }

        private string message = string.Empty;

        public string Message
        {
            get { return this.message; }
            set { SetProperty(ref this.message, value); }
        }

        private string trueOption = string.Empty;

        public string TrueOption
        {
            get { return this.trueOption; }
            set { SetProperty(ref this.trueOption, value); }
        }

        private string falseOption = string.Empty;

        public string FalseOption
        {
            get { return this.falseOption; }
            set { SetProperty(ref this.falseOption, value); }
        }

        private bool isVisible;

        public bool IsVisible
        {
            get { return this.isVisible; }
            set { SetProperty(ref this.isVisible, value); }
        }

        public IRelayCommand TrueCommand { get; }
        public IRelayCommand FalseCommand { get; }

        public DialogPageViewModel()
        {
            TrueCommand = new RelayCommand(OnTrueCommandExecute);
            FalseCommand = new RelayCommand(OnFalseCommandExecute);
        }

        private void OnFalseCommandExecute()
        {
            this.result = false;
            this.autoResetEvent.Set();
        }

        private void OnTrueCommandExecute()
        {
            this.result = true;
            this.autoResetEvent.Set();
        }

        public async Task<bool> ShowDialog(string title, string message, string trueOption, string falseOption)
        {
            Title = title;

            Message = message;

            TrueOption = trueOption;

            FalseOption = falseOption;

            IsVisible = true;

            await Task.Run(this.autoResetEvent.WaitOne);

            bool r = this.result;

            CloseDialog();

            return r;
        }

        private void CloseDialog()
        {
            IsVisible = false;

            this.result = false;

            Title = string.Empty;

            Message = string.Empty;

            TrueOption = string.Empty;

            FalseOption = string.Empty;
        }

        public async Task ShowDialog(string title, string message, string trueOption)
        {
            Title = title;

            Message = message;

            TrueOption = trueOption;

            IsVisible = true;

            await Task.Run(this.autoResetEvent.WaitOne);

            CloseDialog();
        }
    }
}
