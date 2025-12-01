using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using nthLink.Wpf.Interface;
using nthLink.Wpf.Struct;
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
                this.containerProvider.Resolve<ILanguageService>() is ILanguageService languageService &&
                this.containerProvider.Resolve<IClientInfo>() is IClientInfo clientInfo)
            {
                ILoadAnimation? loadAnimation = this.containerProvider.Resolve<ILoadAnimation>();

                loadAnimation?.Show();

                bool needUpdate = await Task.Run(() => updater.NeedUpdate(clientInfo.AppVersion));

                loadAnimation?.Hide();

                if (needUpdate)
                {
                    bool download = await dialogBox.ShowDialog(string.Empty,
                        languageService.GetString("download_new_version"), "YES", "NO");

                    if (download)
                    {
                        loadAnimation?.Show();

                        string installExe = await Task.Run(() => updater.Download());

                        loadAnimation?.Hide();

                        bool installNow = await dialogBox.ShowDialog(string.Empty,
                        languageService.GetString("install_new_version_now"), "YES", "NO");

                        if (installNow)
                        {
                            if (!string.IsNullOrEmpty(installExe) &&
                            File.Exists(installExe))
                            {
                                try
                                {
                                    Process? installerProcess = Process.Start(installExe);

                                    if (installerProcess != null)
                                    {
                                        TriggerApplicationShutdown();
                                    }
                                    else
                                    {
                                        await dialogBox.ShowDialog(string.Empty,
                                            languageService.GetString("failed_to_start_installer") ?? "Failed to start installer",
                                            "OK");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    await dialogBox.ShowDialog(string.Empty,
                                        $"{languageService.GetString("failed_to_start_installer") ?? "Failed to start installer"}: {ex.Message}",
                                        "OK");
                                }
                            }
                        }
                    }
                }
                else
                {
                    await dialogBox.ShowDialog(string.Empty, languageService.GetString("last_version"), "Close");
                }
            }
        }

        private void TriggerApplicationShutdown()
        {
            if (this.containerProvider.Resolve<IEventBus<AppEventArgs>>() is IEventBus<AppEventArgs> appEventBus)
            {
                appEventBus.Publish(AppEventArgs.AppEventArgsMessage.AppEvent,
                    new AppEventArgs(AppEventArgs.AppEventArgsMessage.ApplicationShutdown));
            }
        }
    }
}
