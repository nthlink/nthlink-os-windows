using nthLink.Header.Interface;
using nthLink.SDK.Model;
using nthLink.Wpf.Interface;

namespace nthLink.Wpf.ViewModels
{
    class PolicyPageViewModel : NotifyPropertyChangedBase
    {
        public const string POLICY_AGREED = "PolicyAgreed";
        public const string AGREED = "true";

        private readonly IWindowsRegister windowsRegister;

        public IRelayCommand AgreeCommand { get; }

        public PolicyPageViewModel(IWindowsRegister windowsRegister)
        {
            AgreeCommand = new RelayCommand(OnAgreeCommandExecute);
            this.windowsRegister = windowsRegister;
        }

        private void OnAgreeCommandExecute()
        {
            this.windowsRegister.SetRegister(POLICY_AGREED, AGREED);
        }
    }
}
