using nthLink.SDK.Model;
using nthLink.Wpf.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace nthLink.Wpf.ViewModels
{
    internal class PhoneConnectPageViewModel : NotifyPropertyChangedBase
    {
        private string listenAddress = string.Empty;

        public string ListenAddress
        {
            get { return this.listenAddress; }
            set { SetProperty(ref this.listenAddress, value); }
        }

        //private LeafConnectService? _vpnService;
        //private DispatcherTimer? _statusTimer;
        public PhoneConnectPageViewModel()
        {
            //InitializeVpnService();
            //StartStatusTimer();
            //LoadInitialState();
        }

        //private void InitializeVpnService()
        //{
        //    try
        //    {
        //        _vpnService = new LeafConnectService();
        //        _vpnService.VpnStartRequested += OnVpnStartRequested;

        //        if (!_vpnService.Initialize())
        //        {
        //            MessageBox.Show("Failed to initialize VPN service. Please check if the native library is available.",
        //                          "Initialization Error",
        //                          MessageBoxButton.OK,
        //                          MessageBoxImage.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Failed to initialize VPN service: {ex.Message}",
        //                      "Initialization Error",
        //                      MessageBoxButton.OK,
        //                      MessageBoxImage.Error);
        //    }
        //}

        //private void StartStatusTimer()
        //{
        //    _statusTimer = new DispatcherTimer
        //    {
        //        Interval = TimeSpan.FromSeconds(1)
        //    };
        //    _statusTimer.Tick += (s, e) => UpdateUI();
        //    _statusTimer.Start();
        //}

        //private async void LoadInitialState()
        //{
        //    await Task.Delay(500);
        //    UpdateUI();
        //}

        //private void UpdateUI()
        //{
        //    if (_vpnService == null) return;

        //    try
        //    {
        //        bool isRunning = _vpnService.IsVpnRunning();

        //        if (isRunning)
        //        {
        //            ShowConnectedState();
        //        }
        //        else
        //        {
        //            ShowDisconnectedState();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error updating UI: {ex.Message}");
        //    }
        //}

        //private void ShowConnectedState()
        //{
        //}

        //private void ShowDisconnectedState()
        //{
        //    GenerateQrCode();
        //}

        //private void GenerateQrCode()
        //{
        //    if (_vpnService == null) return;

        //    try
        //    {
        //        string? listenAddress = _vpnService.GetListenAddress();

        //        if (string.IsNullOrEmpty(listenAddress))
        //        {
        //            Debug.WriteLine("Listen address is not available yet");
        //        }
        //        else
        //        {
        //            ListenAddress = listenAddress;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error generating QR code: {ex.Message}");
        //    }
        //}

        //private void OnVpnStartRequested()
        //{
        //    if (_vpnService == null) return;

        //    try
        //    {
        //        string? ip = _vpnService.GetRemoteSocksIp();
        //        ushort port = _vpnService.GetRemoteSocksPort();

        //        if (!string.IsNullOrEmpty(ip) && port > 0)
        //        {
        //            bool success = _vpnService.StartVpn(ip, port);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error handling VPN start request: {ex.Message}");
        //    }
        //}
    }
}
