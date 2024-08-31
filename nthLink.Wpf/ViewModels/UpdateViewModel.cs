using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.SDK.Interface;
using nthLink.SDK.Model;
using nthLink.Wpf.Interface;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace nthLink.Wpf.ViewModels
{
    internal class UpdateViewModel : NotifyPropertyChangedBase
    {
        private readonly IContainerProvider containerProvider;

        public IRelayCommand CheckUpdateCommand { get; }

        public UpdateViewModel(IContainerProvider containerProvider)
        {
            CheckUpdateCommand = new RelayCommand(OnCheckUpdateCommandExecute);
            this.containerProvider = containerProvider;
        }

        private async void OnCheckUpdateCommandExecute()
        {
            if (this.containerProvider.Resolve<IUpdater>() is IUpdater updater &&
                this.containerProvider.Resolve<IDialogBox>() is IDialogBox dialogBox &&
                this.containerProvider.Resolve<ILanguageService>() is ILanguageService languageService)
            {
                bool needUpdate = await updater.NeedUpdate();

                if (needUpdate)
                {
                    bool download = await dialogBox.ShowDialog(string.Empty,
                        languageService.GetString("download_new_version"), "YES", "NO");

                    if (download)
                    {
                        string installExe = await updater.Download();

                        if (!string.IsNullOrEmpty(installExe) &&
                            File.Exists(installExe))
                        {
                            Process.Start(installExe);
                        }
                    }
                }
                else
                {
                    await dialogBox.ShowDialog(string.Empty, languageService.GetString("last_version"), "Close");
                }
            }
        }
    }
}
